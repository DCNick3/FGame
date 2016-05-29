using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{

    class GamePole
    {
        //TODO: split gamePole to chunks!
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
                if (point.X < 0 || point.X >= Tiles.GetLength(0))
                    continue;
                if (point.Y < 0 || point.Y >= Tiles.GetLength(1))
                    continue;
                if (Tiles[point.X, point.Y].IsObstacle && wallWeight == -1)
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
            return ((Tiles[one.X, one.Y].IsObstacle) ? wallWeight : nullWeight) + ((Tiles[two.X, two.Y].IsObstacle) ? wallWeight : nullWeight);
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
                AStarNode currentNode = null; //= openList.OrderBy(node =>
                  //node.EstimateFullPathLength).First();
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
                    //if (closedList.Count(node => node.Position == neighbourNode.Position) > 0) //TODO: optimize!!!
                    //  continue;
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
                    //var openNode = openList.FirstOrDefault(node =>
                    //  node.Position == neighbourNode.Position);

                    //if (openNode == null)
                    //    openList.Add(neighbourNode);

                    if (f && openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                    }
                }
            }
            return null;
        }

        public static GamePole Generate(int poleWidth, int poleHeight, int roomCount, int wallCost, Random rnd, Game1 game)
        {
            List<Room> rooms = new List<Room>();
            GamePole gp = new GamePole(poleWidth, poleHeight, game);
            int floorType = rnd.Next(35, 39);

            for (int i = 0; i < roomCount; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int w = rnd.Next(2, 10);
                    int h = rnd.Next(2, 10);

                    Rectangle roomRect = new Rectangle();

                    roomRect.X = rnd.Next(1, poleWidth - w - 1);
                    roomRect.Y = rnd.Next(1, poleHeight - h - 1);
                    roomRect.Width = w;
                    roomRect.Height = h;

                    List<Room> inter = rooms.FindAll(
                        (Room room_) =>
                        {
                            //return ((room.X < room_.X && room.X > room_.X + room_.Width) && (room.X + room.Width < room_.X && room.X + room.Width > room_.X + room_.Width) && (room.Y < room_.Y && room.Y > room_.Y + room_.Height) && (room.Y + room.Height < room_.Y && room.Y + room.Height > room_.Y + room_.Height));
                            return roomRect.Intersects(room_.Position);
                        });
                    if (inter.Count == 0)
                    {
                        var room = new Room(roomRect);
                        rooms.Add(room);
                        gp.Rooms = rooms.ToArray();
                        for (int x = 0; x < room.Position.Width; x++)
                            for (int y = 0; y < room.Position.Height; y++)
                                gp.Tiles[room.Position.X + x, room.Position.Y + y] = new Tile(floorType, false);

                        //Connect
                        int cn = 2 + rnd.Next(2);
                        for (int i_ = 0; i_ < cn; i_++)
                        {
                            Room t = gp.GetRndRoom(rnd);
                            for (int x = 0; x < 10; x++)
                            {
                                if (t == room) continue;
                                t = gp.GetRndRoom(rnd);
                                break;
                            }
                            Point[] pp = gp.AStarGetWay(1, wallCost, room.Center, t.Center);
                            for (int x = 0; x < pp.Length; x++)
                            {
                                gp.Tiles[pp[x].X, pp[x].Y] = new Tile(floorType, false);
                            }
                            if (t != room)
                                room.ConnectsTo.Add(t);
                        }

                        break;
                    }
                }
            }

            gp.Rooms = rooms.ToArray();

            foreach (var room in gp.Rooms)
            {
                if (room.ConnectsTo.Count == 0)
                {
                    Room t = gp.GetRndRoom(rnd);
                    for (int x = 0; x < 10; x++)
                    {
                        if (t == room) continue;
                        t = gp.GetRndRoom(rnd);
                        break;
                    }
                    Point[] pp = gp.AStarGetWay(1, wallCost, room.Center, t.Center);
                    for (int x = 0; x < pp.Length; x++)
                    {
                        gp.Tiles[pp[x].X, pp[x].Y] = new Tile(floorType, false);
                    }
                    room.ConnectsTo.Add(t);
                }
            }

            for (int i = 0; i < roomCount / 10; i++)
            {
                Room room = gp.GetRndRoom(rnd);
                Room t = gp.GetRndRoom(rnd);
                Point[] pp = gp.AStarGetWay(1, wallCost, room.Center, t.Center);
                for (int x = 0; x < pp.Length; x++)
                {
                    gp.Tiles[pp[x].X, pp[x].Y] = new Tile(floorType, false);
                }
                if (t != room)
                    room.ConnectsTo.Add(t);
            }

            //54

            int chestCount = roomCount / 3;
            List<Chest> chsts = new List<Chest>();

            for (int i = 0; i < chestCount; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    Point p = gp.GetRandomFreePole(rnd);
                    Room r = gp.GetRoomAt(p);
                    if (r == null || r.Chest != null) continue;
                    Chest c = new Chest(gp.game);
                    c.Position = p;
                    c.Type = rnd.Next(4);
                    chsts.Add(c);
                    r.Chest = c;
                    break;
                }
            }

            chsts.Sort((Chest a, Chest b) => a.Position.Y.CompareTo(b.Position.Y));
            gp.Chests = chsts.ToArray();
            

            for (int x = 0; x < poleWidth; x++)
                for (int y = 0; y < poleHeight; y++)
                    if (gp.Tiles[x, y].IsObstacle && gp.GetNeighbours(new Point(x, y)).Where((Point p) => !gp.Tiles[p.X, p.Y].IsObstacle).Count() != 0)
                    {
                        gp.Tiles[x, y].Id = 53;
                    }
            return gp;
        }

        //public GameObject[] GameObjects { get; private set; }
        public Chest[] Chests { get; private set; }
        public Room[] Rooms { get; private set; }
        public Tile[,] Tiles { get; private set; }
        public int PoleWidth { get; private set; }
        public int PoleHeight { get; private set; }
        protected Game1 game;

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
                if (point_.X < 0 || point_.X >= PoleWidth)
                    continue;
                if (point_.Y < 0 || point_.Y >= PoleHeight)
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
                res = new Point(rnd.Next(0, PoleWidth), rnd.Next(0, PoleHeight));
                isFree = !Tiles[res.X, res.Y].IsObstacle;
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

        public GamePole(int poleWidth, int poleHeight, Game1 game)
        {
            this.PoleWidth = poleWidth;
            this.PoleHeight = poleHeight;
            Tiles = new Tile[poleWidth, poleHeight];
            for (int x = 0; x < poleWidth; x++)
                for (int y = 0; y < poleHeight; y++)
                    Tiles[x, y] = new Tile(0, true);
            this.game = game;
        }
    }
}
