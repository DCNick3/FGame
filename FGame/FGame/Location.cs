using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGame
{

    public class Location
    {
        #region astar
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
        #endregion
        private class ObjectCache
        {
            const int zoneSize = 32;

            Dictionary<Point, HashSet<GamePoleObject>> p2o;
            Dictionary<GamePoleObject, HashSet<Point>> o2p;

            public ObjectCache()
            {
                p2o = new Dictionary<Point, HashSet<GamePoleObject>>();
                o2p = new Dictionary<GamePoleObject, HashSet<Point>>();
            }
            
            public GamePoleObject[] GetObjectsInZone(Point p)
            {
                return GetObjectsInZone(p.ToVector2());
            }

            public GamePoleObject[] GetObjectsInZone(Vector2 p)
            {
                Point x = new Point((int)Math.Floor(p.X / zoneSize), (int)Math.Floor(p.Y / zoneSize));
                HashSet<GamePoleObject> res;
                if (p2o.TryGetValue(x, out res))
                    return res.ToArray();
                else
                    return (p2o[x] = new HashSet<GamePoleObject>()).ToArray();
            }

            private Point[] GetPoses(FloatRectangle rect)
            {
                List<Point> result = new List<Point>();
                Point firstZone = new Point((int)Math.Floor(rect.X / zoneSize), (int)Math.Floor(rect.Y / zoneSize));
                int width = 0;
                int height = 0;
                FloatRectangle zone = new FloatRectangle(firstZone.X * 32, firstZone.Y * 32, zoneSize, zoneSize);
                while (rect.Intersects(zone))
                {
                    zone.X += zoneSize;
                    width++;
                }
                zone = new FloatRectangle(firstZone.X * 32, firstZone.Y * 32, zoneSize, zoneSize);
                while (rect.Intersects(zone))
                {
                    zone.Y += zoneSize;
                    height++;
                }
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        result.Add(new Point(firstZone.X + x, firstZone.Y + y));
                    }
                }
                return result.ToArray();
            }

            private Point[] GetPoses(GamePoleObject obj)
            {
                return GetPoses(obj.Rectangle);
            }

            private void AppendObjectPoses(GamePoleObject obj, Point[] poses)
            {
                for (int i = 0; i < poses.Length; i++)
                {
                    HashSet<GamePoleObject> o;
                    if (!p2o.TryGetValue(poses[i], out o))
                    {
                        o = new HashSet<GamePoleObject>();
                        p2o[poses[i]] = o;
                    }
                    o.Add(obj);
                }
                o2p[obj] = new HashSet<Point>(poses);
            }

            public void ObjectMove(GamePoleObject obj)
            {
                Point[] oldPoses = o2p[obj].ToArray();
                Point[] newPoses = GetPoses(obj);
                for (int i = 0; i < oldPoses.Length; i++)
                {
                    p2o[oldPoses[i]].Remove(obj);
                }
                AppendObjectPoses(obj, newPoses);
            }

            public void AddObject(GamePoleObject obj)
            {
                Point[] zones = GetPoses(obj);
                AppendObjectPoses(obj, zones);
            }

            public void RemoveObject(GamePoleObject obj)
            {
                Point[] oldPoses = o2p[obj].ToArray();
                for (int i = 0; i < oldPoses.Length; i++)
                {
                    p2o[oldPoses[i]].Remove(obj);
                }
                o2p[obj] = null;
            }

            public GamePoleObject[] GetObjectsIntersectsRect(FloatRectangle rect)
            {
                Point[] zones = GetPoses(rect);
                HashSet<GamePoleObject> objs = new HashSet<GamePoleObject>();
                for (int i = 0; i < zones.Length; i++)
                {
                    HashSet<GamePoleObject> obj;
                    if (!p2o.TryGetValue(zones[i], out obj))
                    {
                        obj = new HashSet<GamePoleObject>();
                        p2o[zones[i]] = obj;
                    }
                    for (int j = 0; j < obj.Count; j++)
                    {
                        objs.Add(obj.ElementAt(j));
                    }
                }
                return objs.Where((GamePoleObject o) => o.Rectangle.Intersects(rect)).ToArray();
            }
        }

        protected Game1 game;
        private List<GamePoleObject> objects;
        private ObjectCache cache;

        public Location(Game1 game)
        {
            this.game = game;
            objects = new List<GamePoleObject>();
            cache = new ObjectCache();
            for (int i = 0; i < 128; i++)
            {
                for (int j = 0; j < 128; j++)
                    AddObject(new GamePoleObjectTile(new Vector2(i * 32f + 64f, j * 32f), i, false, 0));
            }
        }
        
        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(a.X - b.X), 2) + Math.Pow(Math.Abs(a.Y - b.Y), 2));
        }
        
        public bool IsFree(Point p)
        {
            return GetObjectsIntersectsPoint(p.ToVector2()).Where((GamePoleObject x) => x.IsObstacle).Count() == 0;
        }

        public bool IsFree(Vector2 p)
        {
            return GetObjectsIntersectsPoint(p).Where((GamePoleObject x) => x.IsObstacle).Count() == 0;
        }

        public bool IsFree(Rectangle r)
        {
            return GetObjectsIntersectsRect(r).Where((GamePoleObject x) => x.IsObstacle).Count() == 0;
        }

        public void AddObject(GamePoleObject obj)
        {
            objects.Add(obj);
            cache.AddObject(obj);
        }

        public GamePoleObject[] GetObjectsIntersectsPoint(Vector2 point)
        {
            throw new NotImplementedException();
        }

        public GamePoleObject[] GetObjectsInRect(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public GamePoleObject[] GetObjectsIntersectsRect(Rectangle rect)
        {
            return GetObjectsIntersectsRect(rect.ToFloatRect());
        }

        public GamePoleObject[] GetObjectsIntersectsRect(FloatRectangle rect)
        {
            return cache.GetObjectsIntersectsRect(rect);
        }

        public void RemoveObject(GamePoleObject obj)
        {
            objects.Remove(obj);
            cache.RemoveObject(obj);
        }

        public void RemoveObjects(GamePoleObject[] obj)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                RemoveObject(obj[i]);
            }
        }

        //public void DeleteObjectsAtPoint(Vector2 point)
        //{
        //   RemoveObjects(GetObjectsIntersectsPoint(point));
        //}

        public void Draw(GameRegistry registry, SpriteBatch spriteBatch, Vector2 screenPos, Vector2 screenSize)
        {
            GamePoleObject[] o2d = GetObjectsIntersectsRect(new FloatRectangle(screenPos.X, screenPos.Y, screenSize.X, screenSize.Y));
            for (int i = 0; i < o2d.Length; i++)
            {
                o2d[i].Draw(registry, spriteBatch, screenPos - new Vector2(32,32)); //TODO: КОСТЫЛЬ!
            }
        }

        public virtual void Update(GameRegistry registry, GameTime gameTime)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                obj.Update(registry, gameTime);
                if (obj.Moved)
                    cache.ObjectMove(obj);
            }
        }
    }
}
