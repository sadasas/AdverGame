// references :
// - http://www.jgallant.com/nodal-pathfinding-in-unity-2d-with-a-in-non-grid-based-games/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdverGame.PathFinding
{

    public class Node
    {
        public float FCost { get { return GCost + HCost; } }
        public Node Parent;
        public float GCost;
        public float HCost;
        public bool IsValid = true;
        public Vector2 Position;
        public readonly Node[] Connections;
        public Node(Vector2 pos)
        {
            Position = pos;
            Connections = new Node[8];
        }

        public bool AnyConnectionsBad()
        {
            for (int i = 0; i < Connections.Length; i++)
            {
                if (Connections[i] == null || !Connections[i].IsValid)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class NavMeshGenerator
    {
        enum Directions { Right, DownRight, Down, DownLeft, Left, UpLeft, Up, UpRight }

        List<Node> m_grid;
        Rect m_size;
        float m_pointDistributionSize;
        LayerMask m_environmentMask;
        public NavMeshGenerator(Rect size, float pointDistributionSize, LayerMask environmentMask)
        {
            m_size = size;
            m_pointDistributionSize = pointDistributionSize;
            m_environmentMask = environmentMask;

            FillOutGrid();
            DestroyBadNodes();
        }

        void FillOutGrid()
        {
            m_grid = new();

            //start point from left to right
            var currentPoint = new Vector2((m_size.x - m_size.width / 2) + m_pointDistributionSize, (m_size.y + m_size.height / 2) - m_pointDistributionSize);
            var iteration = 0;
            var length = -1;
            var yLength = 0;
            var isCacheIteration = false;
            var isAlternate = false;

            while (true)
            {
                iteration++;
                Node newNode = new(currentPoint);
                m_grid.Add(newNode);

                //move to next point 
                currentPoint += new Vector2(m_pointDistributionSize * 2, 0);

                // change to new row if x coordinate of currentPoint greater than rect width
                if (currentPoint.x > m_size.x + m_size.width / 2)
                {
                    if (length != -1)
                    {
                        while (iteration < length)
                        {
                            Node extraNode = new(currentPoint);
                            m_grid.Add(extraNode);
                            iteration++;
                        }
                    }
                    else
                    {
                        Node extraNode = new(currentPoint);
                        m_grid.Add(extraNode);
                    }

                    // change coordinate y currentPoint to new row 
                    currentPoint = new Vector2((m_size.x - m_size.width / 2) + (isAlternate ? m_pointDistributionSize : 0), currentPoint.y - m_pointDistributionSize);
                    isAlternate = !isAlternate;
                    yLength++;
                    isCacheIteration = true;

                }

                // stop create point when current y coordinat of currentPoint is greater than rect height
                if (currentPoint.y < m_size.y - m_size.height / 2)
                {
                    break;
                }

                if (isCacheIteration)
                {
                    if (length == -1)
                    {
                        length = iteration + 1;
                    }
                    iteration = 0;
                    isCacheIteration = false;
                }

                // search for connections in every node
                for (int i = 0; i < m_grid.Count; i++)
                {
                    for (int direction = 0; direction < m_grid[i].Connections.Length; direction++)
                    {
                        m_grid[i].Connections[direction] = GetNodeFromDirection(i, (Directions)direction, length);
                    }
                }

            }
        }

        void DestroyBadNodes()
        {
            for (int i = m_grid.Count - 1; i >= 0; i--)
            {
                Collider2D hit = Physics2D.OverlapCircle(m_grid[i].Position, 0.01f, m_environmentMask);
                if (hit != null)
                {
                    for (int j = 0; j < m_grid[i].Connections.Length; j++)
                    {
                        // goto all conection to this node
                        if (m_grid[index: i].Connections[j] != null)
                        {
                            // delete all connections reference to this node  in child node
                            for (int k = 0; k < m_grid[i].Connections[j].Connections.Length; k++)
                            {
                                if (m_grid[i].Connections[j].Connections[k] != null)
                                {
                                    if (m_grid[i].Connections[j].Connections[k] == m_grid[i])
                                    {
                                        m_grid[i].Connections[j].Connections[k] = null;
                                    }
                                }

                            }
                        }
                    }
                    m_grid.RemoveAt(i);
                }
            }


            for (int i = m_grid.Count - 1; i >= 0; i--)
            {
                int a = 0;
                for (int c = 0; c < m_grid[index: i].Connections.Length; c++)
                {
                    if (m_grid[i].Connections[c] == null) a++;
                }
                if (a == m_grid[index: i].Connections.Length) m_grid.RemoveAt(i);
            }

        }
        Node GetNodeFromDirection(int nodeIndex, Directions direction, int length)
        {
            var index = -1;
            var isStartOfRow = (nodeIndex + 1) % length == 1;
            var isEndOfRow = (nodeIndex + 1) % length == 0;
            var isOddRow = (((nodeIndex + 1) - Mathf.FloorToInt((nodeIndex) % length)) / length) % 2 == 0;

            switch (direction)
            {
                case Directions.Right:
                    if (isEndOfRow) return null;
                    index = nodeIndex + 1;
                    break;
                case Directions.DownRight:
                    if (isEndOfRow && isOddRow) return null;
                    index = nodeIndex + length + (isOddRow ? 1 : 0);
                    break;
                case Directions.Down:
                    index = nodeIndex + length * 2;
                    break;
                case Directions.DownLeft:
                    if (isStartOfRow && !isOddRow) return null;
                    index = nodeIndex + (length - (isOddRow ? 0 : 1));
                    break;
                case Directions.Left:
                    if (isStartOfRow) return null;
                    index = nodeIndex - 1;
                    break;
                case Directions.UpLeft:
                    if (isStartOfRow && !isOddRow) return null;
                    index = nodeIndex - (length + (isOddRow ? 0 : 1));
                    break;
                case Directions.Up:
                    index = nodeIndex - length * 2;
                    break;
                case Directions.UpRight:
                    if (isEndOfRow && isOddRow) return null;
                    index = nodeIndex - (length - (isOddRow ? 1 : 0));
                    break;
            }

            if (index >= 0 && index < m_grid.Count)
            {
                return m_grid[index];
            }
            else
            {
                return null;
            }
        }


        public Node FindClosestNode(Vector2 position)
        {
            Node closest = null;
            float current = float.MaxValue;

            for (int i = 0; i < m_grid.Count; i++)
            {
                float distance = Vector2.Distance(m_grid[i].Position, position);
                if (distance < current)
                {
                    current = distance;
                    closest = m_grid[i];
                }
            }

            return closest;
        }
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var item in m_grid)
            {
                var pos = item.Position;
                Gizmos.DrawSphere(pos, 0.1f);

            }
        }
    }

    public class PathFinder
    {
        NavMeshGenerator m_generator;

        List<Node> m_openSet = new();
        List<Node> m_closedSet = new();
        List<Node> m_path = new();
        public PathFinder(NavMeshGenerator generator)
        {
            m_generator = generator;
        }

        public Vector2[] FindPath(Vector2 currentPosition, Vector2 destination)
        {
            Node[] points = GetAStar(currentPosition, destination);

            if (points == null)
            {

                return null;
            }

            return ShortenPoinByVisibility(points).ToArray();
        }

        List<Vector2> ShortenPoinByVisibility(Node[] points)
        {
            List<Vector2> p = new();

            for (int i = 0; i < points.Length; i++)
            {
                p.Add(points[i].Position);
            }

            return p;
        }

        float Heuristic(Node one, Node two)
        {
            return Vector2.Distance(one.Position, two.Position);
        }
        private Node[] GetAStar(Vector2 currentPosition, Vector2 destination)
        {
            m_openSet.Clear();
            m_closedSet.Clear();

            Node start = m_generator.FindClosestNode(currentPosition);
            Node end = m_generator.FindClosestNode(destination);

            if (start == null || end == null)
            {

                return null;
            }

            m_openSet.Add(start);
            while (m_openSet.Count > 0)
            {

                Node current = m_openSet[0];

                // Evaluate costs
                for (int i = 1; i < m_openSet.Count; i++)
                {
                    if (m_openSet[i].FCost < current.FCost || m_openSet[i].FCost == current.FCost)
                    {
                        if (m_openSet[i].HCost < current.HCost) current = m_openSet[i];
                    }
                }
               
                m_openSet.Remove(current);
                m_closedSet.Add(current);
                m_openSet.Clear();
                if (current.Equals(end)) break;

                foreach (var neighbor in current.Connections.Where(x => x != null))
                {
                    float newCost = current.GCost + Heuristic(current, neighbor);

                    if (newCost < neighbor.GCost || !m_openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newCost;
                        neighbor.HCost = Heuristic(neighbor, end);
                        neighbor.Parent = current;

                        if (!m_openSet.Contains(neighbor)) m_openSet.Add(neighbor);
                    }
                }


            }

            if (end.Parent == null) return null;

            return m_closedSet.ToArray();
        }
    }

}
