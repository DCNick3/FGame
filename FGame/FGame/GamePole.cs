using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{

    public class GamePole
    {
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
                //if (GetTileAt(point) == null)
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

        public static GamePole Generate(Random rnd, Game1 game)
        {
            GamePole gp = new GamePole(game);
            gp.chunks = new Chunk[1, 1];
            gp.chunks[0, 0] = Chunk.Generate(gp, game, rnd);
            gp.AllocChunk(1, 0, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(-1, 0, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(0, 1, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(0, -1, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(-1, 1, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(1, -1, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(1, 1, Chunk.Generate(gp, game, rnd));
            gp.AllocChunk(-1, -1, Chunk.Generate(gp, game, rnd));
            return gp;
        }
        

        internal Chunk[,] chunks;

        /*public Chest[] Chests
        {
            get
            {
                List<Chest> chsts = new List<Chest>();
                for (int x = TileCoordinateOffsetX; x < chunks.GetLength(0) + TileCoordinateOffsetX; x++)
                    for (int y = TileCoordinateOffsetY; y < chunks.GetLength(1) + TileCoordinateOffsetY; y++)
                    {
                        var cnk = chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY];
                        if (cnk != null)
                        {
                            foreach (var c in cnk.Chests)
                            {
                                c.ChunkPosition = new Point(x, y);
                            }
                            chsts.AddRange(cnk.Chests);
                        }
                    }
                return chsts.ToArray();
            }
        }*/
        public Room[] Rooms
        {
            get
            {
                List<Room> rms = new List<Room>();
                foreach (var cnk in chunks)
                {
                    if (cnk != null)
                    rms.AddRange(cnk.Rooms);
                }
                return rms.ToArray();
            }
        }
        /// <summary>
        /// This is a stub!
        /// U better to use GetChunkAt(pos)
        /// </summary>
        /*public Tile[,] Tiles
        {
            get
            {
                Tile[,] res = new Tile[chunks.GetLength(0) * Chunk.width, chunks.GetLength(1) * Chunk.height];
                for (int x = 0; x < chunks.GetLength(0); x++)
                    for (int y = 0;y < chunks.GetLength(1); y++)
                    {
                        Chunk chunk = chunks[x, y];
                        if (chunk != null)
                        for (int xx = 0; xx < chunk.Tiles.GetLength(0); xx++)
                            for (int yy = 0; yy < chunk.Tiles.GetLength(1); yy++)
                            {
                                res[x * Chunk.width + xx, y * Chunk.height + yy] = chunk.Tiles[xx,yy];
                            }
                    }
                return res;
            }
        }*/

        /*public Tile GetTileAt(Point pos)
        {
            int xx = pos.X % Chunk.width;
            int x = (int)Math.Floor((float)pos.X / Chunk.width);
            int yy = pos.Y % Chunk.height;
            int y = (int)Math.Floor((float)pos.Y / Chunk.height);
            if (xx < 0) xx = Chunk.width + xx;
            if (yy < 0) yy = Chunk.height + yy;
            if (x - TileCoordinateOffsetX >= 0 && x - TileCoordinateOffsetX < chunks.GetLength(0) && y - TileCoordinateOffsetY >= 0 && y - TileCoordinateOffsetY < chunks.GetLength(1) && chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY] != null)
                return chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY].Tiles[xx, yy];
            else
                return null;
        }*/

        /*public bool SetTileAt(Point pos, Tile tile)
        {
            int xx = pos.X % Chunk.width;
            int x = (int)Math.Floor((float)pos.X / Chunk.width);
            int yy = pos.Y % Chunk.height;
            int y = (int)Math.Floor((float)pos.Y / Chunk.height);
            if (xx < 0) xx = Chunk.width + xx;
            if (yy < 0) yy = Chunk.height + yy;
            if (x - TileCoordinateOffsetX >= 0 && x - TileCoordinateOffsetX < chunks.GetLength(0) && y - TileCoordinateOffsetY >= 0 && y - TileCoordinateOffsetY < chunks.GetLength(1))
            {
                chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY].Tiles[xx, yy] = tile;
                return true;
            }
            return false;
        }*/

        //public GameObject[] GameObjects { get; private set; }

        public int PoleWidth
        {
            get
            {
                return chunks.GetLength(0) * Chunk.width;
            }
        }
        public int PoleHeight
        {
            get
            {
                return chunks.GetLength(1) * Chunk.height;
            }
        }
        public int MinX
        {
            get
            {
                return TileCoordinateOffsetX * Chunk.width;
            }
        }
        public int MaxX
        {
            get
            {
                return PoleWidth + TileCoordinateOffsetX * Chunk.width;
            }
        }
        public int MinY
        {
            get
            {
                return TileCoordinateOffsetY * Chunk.height;
            }
        }
        public int MaxY
        {
            get
            {
                return PoleHeight + TileCoordinateOffsetY * Chunk.height;
            }
        }
        protected Game1 game;
        public int TileCoordinateOffsetX = 0;
        public int TileCoordinateOffsetY = 0;

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
                //if (GetTileAt(point_) == null)
                //    continue;

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
                res = new Point(rnd.Next(MinX, MaxX), rnd.Next(MinY, MaxY));
                //Tile t = //GetTileAt(new Point(res.X, res.Y));
                isFree = IsFree(res);
            }
            while (!isFree);
            return res;
        }

        //public Room GetRoomAt(Point pos)
        //{
        //    return Rooms.Where((Room s) => s.Position.Contains(pos)).FirstOrDefault();
        //}

        //public Room GetRndRoom(Random rnd)
        //{
        //    return Rooms[rnd.Next(Rooms.Length)];
        //}

        public GamePole(Game1 game)
        {
            this.game = game;
        }

        public void AllocChunk(int x, int y, Chunk chunk)
        {
            int absX = x - TileCoordinateOffsetX;
            int absY = y - TileCoordinateOffsetY;
            if (PoleWidth + TileCoordinateOffsetX < x && TileCoordinateOffsetX <= x && PoleHeight + TileCoordinateOffsetY < x && TileCoordinateOffsetY <= y)
            {
                chunks[absX, absY] = chunk;
            }
            else
            {
                int addX;
                int addY;

                if (x < 0)
                {
                    //Adding to left
                    if (absX < 0)
                        addX = absX;
                    //if (absX >= chunks.GetLength(0))
                    //    addX = chunks.GetLength(0) - absX;
                    else
                        addX = 0;
                }
                else
                {
                    //Adding to right
                    if (absX >= chunks.GetLength(0))
                        addX = chunks.GetLength(0) - absX + 1;
                    else
                        addX = 0;
                }

                if (y < 0)
                {
                    //Adding to left
                    if (absY < 0)
                        addY = absY;
                    //if (absY >= chunks.GetLength(1))
                    //    addY = chunks.GetLength(1) - absY;
                    else
                        addY = 0;
                }
                else
                {
                    //Adding to right
                    if (absY >= chunks.GetLength(1))
                        addY = chunks.GetLength(1) - absY + 1;
                    else
                        addY = 0;
                }

                        Chunk[,] newChunks = new Chunk[chunks.GetLength(0) + Math.Abs(addX), chunks.GetLength(1) + Math.Abs(addY)];
                for (int xx = 0; xx < chunks.GetLength(0); xx++)
                    for (int yy = 0; yy < chunks.GetLength(1); yy++)
                    {
                        newChunks[xx - (addX > 0 ? 0 : addX), yy - (addY > 0 ? 0 : addY)] = chunks[xx, yy];
                    }
                newChunks[(addX >= 0) ? addX : (0), (addY >= 0) ? addY : (0)] = chunk;
                chunks = newChunks;
                TileCoordinateOffsetX += (addX <= 0) ? addX : (0);
                TileCoordinateOffsetY += (addY <= 0) ? addY : (0);

                chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY] = chunk;

                List<Point> neibourgs = new List<Point>();
                neibourgs.Add(new Point(x + 1, y));
                neibourgs.Add(new Point(x - 1, y));
                neibourgs.Add(new Point(x, y + 1));
                neibourgs.Add(new Point(x, y - 1));
                foreach (var neibourg in neibourgs)
                {
                    if (neibourg.X < TileCoordinateOffsetX || neibourg.X >= chunks.GetLength(0) + TileCoordinateOffsetX)
                        continue;
                    if (neibourg.Y < TileCoordinateOffsetY || neibourg.Y >= chunks.GetLength(1) + TileCoordinateOffsetY)
                        continue;
                    ConnectChunks(x, y, neibourg.X, neibourg.Y);
                }
            }
        }
        
        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(a.X - b.X), 2) + Math.Pow(Math.Abs(a.Y - b.Y), 2));
        }

        private void ConnectChunks(int x1, int y1, int x2, int y2)
        {
            if (Math.Abs(x1 - x2) + Math.Abs(y1 - y2) > 1)
                throw new Exception("Rooms must be neiborgs");
            Chunk c1 = chunks[x1 - TileCoordinateOffsetX, y1 - TileCoordinateOffsetY];
            Chunk c2 = chunks[x2 - TileCoordinateOffsetX, y2 - TileCoordinateOffsetY];
            if (c1 == null || c2 == null)
                return;
                //throw new Exception("Chunkc must exists!");
           
            Room r1 = null;
            Room r2 = null;
            st:
            if (x1 - x2 > 0)
            {
                r1 = c1.Rooms.OrderBy((Room r) => r.Center.X).First();
                r2 = c2.Rooms.OrderByDescending((Room r) => r.Center.X).First();

            }
            else if (x1 - x2 < 0)
            {
                Chunk cc = c1;
                c1 = c2;
                c2 = cc;
                int t = x1;
                x1 = x2;
                x2 = t;
                t = y1;
                y1 = y2;
                y2 = t;//Exchange
                goto st;
            }
            else if (y1 - y2 > 0)
            {
                r1 = c1.Rooms.OrderBy((Room r) => r.Center.Y).First();
                r2 = c2.Rooms.OrderByDescending((Room r) => r.Center.Y).First();
            }
            else if (y1 - y2 < 0)
            {
                Chunk cc = c1;
                c1 = c2;
                c2 = cc;
                int t = x1;
                x1 = x2;
                x2 = x1;
                t = y1;
                y1 = y2;
                y2 = t;//Exchange
                goto st;
            }
            Point gp1 = new Point(r1.Center.X + x1 * Chunk.width, r1.Center.Y + y1 * Chunk.height);
            Point gp2 = new Point(r2.Center.X + x2 * Chunk.width, r2.Center.Y + y2 * Chunk.height);

            Point[] pp = AStarGetWay(1, 30, gp1, gp2);


            foreach (var p in pp)
            {
                if (!IsFree(p))
                {
                    int ft;
                    if (GetDistance(p, gp1) > GetDistance(p, gp2))
                    {
                        ft = c2.FloorType;// GetTileAt(gp2).Id;
                    }
                    else
                    {
                        ft = c1.FloorType;// GetTileAt(gp1).Id;
                    }
                    DeleteObjectsAtPoint(p.ToVector2());
                    AddTile(p.ToVector2(), ft, false, 0);
                }
            }

            for (int x = MinX; x < MaxX; x++)
                for (int y = MinY; y < MaxY; y++)
                {
                    GamePoleObjectTile[] t = GetTileIntersectsPoint(new Vector2(x, y));
                    
                    if (!IsFree(new Point(x,y)) && GetNeighbours(new Point(x, y)).Where((Point p) => IsFree(p)).Count() != 0)
                    {
                        DeleteObjectsAtPoint(new Point(x,y).ToVector2());
                        AddTile(new Point(x, y).ToVector2(), 53, false, 0);
                    }
                }
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
            throw new NotImplementedException();
        }

        public void AddTile(Vector2 position, int type, bool isObstacle, int layer)
        {
            AddStaticObject(new GamePoleObjectTile(position, type, isObstacle, layer));
        }

        public GamePoleStaticObject[] GetObjectsIntersectsPoint(Vector2 point)
        {
            //float xx = point.X % Chunk.width;
            int x = (int)Math.Floor((float)point.X / Chunk.width);
            //float yy = point.Y % Chunk.height;
            int y = (int)Math.Floor((float)point.Y / Chunk.height);
            //if (xx < 0) xx = Chunk.width + xx;
            //if (yy < 0) yy = Chunk.height + yy;
            return chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY] != null ? chunks[x - TileCoordinateOffsetX, y - TileCoordinateOffsetY].GetObjectsIntersectsPoint(point) : new GamePoleStaticObject[] { };
        }

        public GamePoleStaticObject[] GetObjectsInRect(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public GamePoleStaticObject[] GetObjectsIntersectsRect(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public GamePoleObjectTile[] GetTileIntersectsPoint(Vector2 point)
        {
            return (from x in GetObjectsIntersectsPoint(point) select x as GamePoleObjectTile).Where((GamePoleObjectTile x) => x != null).ToArray();
        }

        public void DeleteObject(GamePoleStaticObject obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteObjects(GamePoleStaticObject[] obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteObjectsAtPoint(Vector2 point)
        {
            DeleteObjects(GetObjectsIntersectsPoint(point));
        }

        public void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos)
        {
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    var oo = chunks[x, y].Objects;
                    for (int i = 0; i < oo.Count; i++)
                    {
                        oo[i].Draw(registry, spriteBatch, screenPos);
                    }
                }
            }
        }
        public virtual void Update(GameRegistry registry, GameTime gameTime)
        {
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    var oo = chunks[x, y].Objects;
                    for (int i = 0; i < oo.Count; i++)
                    {
                        oo[i].Update(registry, gameTime);
                    }
                }
            }
        }
    }
}
