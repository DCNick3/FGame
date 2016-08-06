using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FGame
{
    public class Chunk
    {
        public Chunk(GamePole gp, Game1 gm)
        {
            gamePole = gp;
            game = gm;
            //Tiles = new Tile[width, height];
            //for (int x = 0; x < width; x++)
            //    for (int y = 0; y < height; y++)
            //        Tiles[x, y] = new Tile(0, true);
            objects = new List<GamePoleStaticObject>();
        }
        
        private class AStarNode
        {
            public Point Position { get; set; }
            public int PathLengthFromStart { get; set; }
            public AStarNode CameFrom { get; set; }
            public int HeuristicEstimatePathLength { get; set; }
            public int EstimateFullPathLength
            {
                get
                {
                    return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
                }
            }
        }

        private List<AStarNode> GetNeighbours(AStarNode pathNode, Point goal, int nullWeight, int wallWeight)
        {
            var result = new List<AStarNode>();

            Point[] neighbourPoints = new Point[4];
            neighbourPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);

            foreach (var point in neighbourPoints)
            {
                //if (point.X < 0 || point.X >= Tiles.GetLength(0))
                //    continue;
                //if (point.Y < 0 || point.Y >= Tiles.GetLength(1))
                //    continue;
                if (!IsFree(point) && wallWeight == -1)
                    continue;


                var neighbourNode = new AStarNode()
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart +
                    GetDistanceBetweenNeighbours(pathNode.Position, point, nullWeight, wallWeight),
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };
                result.Add(neighbourNode);
            }
            return result;
        }

        private static int GetHeuristicPathLength(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        private int GetDistanceBetweenNeighbours(Point one, Point two, int nullWeight, int wallWeight)
        {
            return ((!IsFree(one)) ? wallWeight : nullWeight) + ((!IsFree(two)) ? wallWeight : nullWeight);
        }

        private static List<Point> GetPathForNode(AStarNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }

        public Point[] AStarGetWay(int nullWeight, int wallWeight, Point start, Point end)
        {
            List<AStarNode> openList = new List<AStarNode>();
            List<AStarNode> closedList = new List<AStarNode>();

            AStarNode startNode = new AStarNode()
            {
                Position = start,
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(start, end)
            };
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                AStarNode currentNode = null; 
                int smallestPL = int.MaxValue;

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].EstimateFullPathLength < smallestPL)
                    {
                        smallestPL = openList[i].EstimateFullPathLength;
                        currentNode = openList[i];
                    }
                }

                if (currentNode.Position == end)
                    return GetPathForNode(currentNode).ToArray();

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (var neighbourNode in GetNeighbours(currentNode, end, nullWeight, wallWeight))
                {
                    for (int i = 0; i < closedList.Count; i++)
                    {
                        if (neighbourNode.Position == closedList[i].Position)
                            goto cont;
                    }
                    goto nc;//Spagetti
                    cont:
                    continue;
                    nc:
                    bool f = false;
                    AStarNode openNode = null;
                    for (int i = 0; i < openList.Count; i++)
                    {
                        if (neighbourNode.Position == openList[i].Position)
                        {
                            openNode = openList[i];
                            f = true;
                        }
                    }
                    if (!f)
                        openList.Add(neighbourNode);

                    if (f && openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            return null;
        }

        public const int width = 16;
        public const int height = 16;
        //public Chest[] Chests { get; private set; }
        public Room[] Rooms { get; private set; }
        //public Tile[,] Tiles { get; private set; }
        List<GamePoleStaticObject> objects;
        public int FloorType { get; private set; }

        Game1 game;
        GamePole gamePole;
        public List<GamePoleStaticObject> Objects { get { return objects; } }

        public static Chunk Generate(GamePole gp, Game1 game, Random rnd)
        {
            List<Room> rooms = new List<Room>();
            Chunk ck = new Chunk(gp, game);
            ck.FloorType = rnd.Next(35, 39);

            Vector2 tileSize = new Vector2(32, 32);

            int roomCount = 4;
            int wallCost = 30;

            for (int i = 0; i < roomCount; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int w = rnd.Next(2, 10);
                    int h = rnd.Next(2, 10);

                    Rectangle roomRect = new Rectangle();

                    roomRect.X = rnd.Next(1, width - w - 1);
                    roomRect.Y = rnd.Next(1, height - h - 1);
                    roomRect.Width = w;
                    roomRect.Height = h;

                    List<Room> inter = rooms.FindAll(
                        (Room room_) =>
                        {
                            return roomRect.Intersects(room_.Position);
                        });
                    if (inter.Count == 0)
                    {
                        var room = new Room(roomRect);
                        rooms.Add(room);
                        ck.Rooms = rooms.ToArray();
                        for (int x = 0; x < room.Position.Width; x++)
                            for (int y = 0; y < room.Position.Height; y++)
                                ck.AddTile(new Vector2(room.Position.X + x, room.Position.Y + y) * tileSize , ck.FloorType, false, 0);

                        //Connect
                        int cn = 2 + rnd.Next(2);
                        for (int i_ = 0; i_ < cn; i_++)
                        {
                            Room t = ck.GetRndRoom(rnd);
                            for (int x = 0; x < 10; x++)
                            {
                                if (t == room) continue;
                                t = ck.GetRndRoom(rnd);
                                break;
                            }
                            Point[] pp = ck.AStarGetWay(1, wallCost, room.Center, t.Center);
                            for (int x = 0; x < pp.Length; x++)
                            {
                                ck.AddTile(pp[x].ToVector2() * tileSize, ck.FloorType, false, 0);
                            }
                            if (t != room)
                                room.ConnectsTo.Add(t);
                        }

                        break;
                    }
                }
            }

            ck.Rooms = rooms.ToArray();

            foreach (var room in ck.Rooms)
            {
                if (room.ConnectsTo.Count == 0)
                {
                    Room t = ck.GetRndRoom(rnd);
                    for (int x = 0; x < 10; x++)
                    {
                        if (t == room) continue;
                        t = ck.GetRndRoom(rnd);
                        break;
                    }
                    Point[] pp = ck.AStarGetWay(1, wallCost, room.Center, t.Center);
                    for (int x = 0; x < pp.Length; x++)
                    {
                        ck.AddTile(pp[x].ToVector2() * tileSize, ck.FloorType, false, 0);
                    }
                    room.ConnectsTo.Add(t);
                }
            }

            for (int i = 0; i < roomCount / 10; i++)
            {
                Room room = ck.GetRndRoom(rnd);
                Room t = ck.GetRndRoom(rnd);
                Point[] pp = ck.AStarGetWay(1, wallCost, room.Center, t.Center);
                for (int x = 0; x < pp.Length; x++)
                {
                    ck.AddTile(pp[x].ToVector2() * tileSize, ck.FloorType, false, 0);
                }
                if (t != room)
                    room.ConnectsTo.Add(t);
            }

            //54

            int chestCount = roomCount / 3;
            List<GamePoleObjectChest> chsts = new List<GamePoleObjectChest>();

            for (int i = 0; i < chestCount; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Point p = ck.GetRandomFreePole(rnd);
                    Room r = ck.GetRoomAt(p);
                    if (r == null || r.Chest != null) continue;
                    GamePoleObjectChest c = new GamePoleObjectChest(p.ToVector2() * tileSize, 11, rnd.Next(4));
                    chsts.Add(c);
                    break;
                }
            }

            chsts.Sort((GamePoleObjectChest a, GamePoleObjectChest b) => a.Position.Y.CompareTo(b.Position.Y));
            ck.objects.AddRange(chsts);


            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (!ck.IsFree(new Point(x,y)) && ck.GetNeighbours(new Point(x, y)).Where((Point p) => ck.IsFree(p)).Count() != 0)
                    {
                        foreach (var xx in ck.GetTileIntersectsPoint(new Vector2(x, y))) xx.Type = 53;
                    }
            return ck;
        }

        public Point[] GetNeighbours(Point point)
        {
            List<Point> result = new List<Point>();

            Point[] points = new Point[8];
            points[0] = new Point(point.X - 1, point.Y);
            points[1] = new Point(point.X + 1, point.Y);
            points[2] = new Point(point.X, point.Y - 1);
            points[3] = new Point(point.X, point.Y + 1);
            points[4] = new Point(point.X - 1, point.Y - 1);
            points[5] = new Point(point.X - 1, point.Y + 1);
            points[6] = new Point(point.X + 1, point.Y - 1);
            points[7] = new Point(point.X + 1, point.Y + 1);


            foreach (var point_ in points)
            {
                if (point_.X < 0 || point_.X >= width)
                    continue;
                if (point_.Y < 0 || point_.Y >= height)
                    continue;

                result.Add(point_);
            }

            return result.ToArray();
        }

        public Point GetRandomFreePole(Random rnd)
        {
            bool isFree = false;
            Point res;
            do
            {
                res = new Point(rnd.Next(0, width), rnd.Next(0, height));
                isFree = IsFree(res);// !Tiles[res.X, res.Y].IsObstacle;
            }
            while (!isFree);
            return res;
        }

        public Room GetRoomAt(Point pos)
        {
            return Rooms.Where((Room s) => s.Position.Contains(pos)).FirstOrDefault();
        }

        public Room GetRndRoom(Random rnd)
        {
            return Rooms[rnd.Next(Rooms.Length)];
        }

        public bool IsFree(Point p)
        {
            return true;
            return GetTileIntersectsPoint(p.ToVector2()).Where((GamePoleObjectTile x) => x.IsObstacle).Count() == 0;
        }

        public bool IsFree(Vector2 p)
        {
            return GetTileIntersectsPoint(p).Where((GamePoleObjectTile x) => x.IsObstacle).Count() == 0;
        }

        public void AddStaticObject(GamePoleStaticObject obj)
        {
            objects.Add(obj);
        }

        public void AddTile(Vector2 position, int type, bool isObstacle, int layer)
        {
            AddStaticObject(new GamePoleObjectTile(position, type, isObstacle, layer));
        }

        public GamePoleStaticObject[] GetObjectsIntersectsPoint(Vector2 point)
        {
            return objects.Where((GamePoleStaticObject x) => x.Rectangle.Intersects(point)).ToArray();
        }

        public GamePoleStaticObject[] GetObjectsInRect(FloatRectangle rect)
        {
            return objects.Where((GamePoleStaticObject x) => x.Rectangle.IsInside(rect)).ToArray();
        }

        public GamePoleStaticObject[] GetObjectsIntersectsRect(FloatRectangle rect)
        {
            return objects.Where((GamePoleStaticObject x) => x.Rectangle.Intersects(rect)).ToArray();
        }

        public GamePoleObjectTile[] GetTileIntersectsPoint(Vector2 point)
        {
            return (from x in GetObjectsIntersectsPoint(point) select x as GamePoleObjectTile).Where((GamePoleObjectTile x) => x != null).ToArray();
        }

        public void DeleteObject(GamePoleStaticObject obj)
        {
            objects.Remove(obj);
        }

        public void DeleteObjects(GamePoleStaticObject[] obj)
        {
            objects.RemoveAll((GamePoleStaticObject x) => obj.Contains(x));
        }

        public void DeleteObjectsAtPoint(Vector2 point)
        {
            DeleteObjects(GetObjectsIntersectsPoint(point));
        }
    }
}
