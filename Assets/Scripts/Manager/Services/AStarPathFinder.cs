using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class Grid
    {
        public NodeTest[,] nodes;
        public int width;
        public int height;
        public float cellSize;
        public Vector3 originPosition;

        // 修改后的构造方法
        public Grid(int widthLocal, int heightLocal, Texture2D obstacleMap, float cellSize = 1f, Vector3 origin = default)
        {
            this.width = (int)(widthLocal / cellSize);
            this.height = (int)(heightLocal/ cellSize);
            this.cellSize = cellSize;
            this.originPosition = origin;
            nodes = new NodeTest[width+1, height+1];

            // 根据障碍物贴图初始化网格
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    bool walkable = true;
                    
                    int pixelX = x * (int)this.cellSize;
                    int pixelY = obstacleMap.height - y * (int)this.cellSize;
                    // 检查障碍物贴图（非白色像素视为障碍）
                    if (obstacleMap != null && x < obstacleMap.width && y < obstacleMap.height)
                    {
                        Color pixel = obstacleMap.GetPixel(pixelX, pixelY);
                        
                        //Color pixel = obstacleMap.GetPixel(x, y);
                        walkable = pixel.a > 0.5f;
                        
                        // Debug.Log($"x:{x} y:{y} walkable:{walkable}"); 
                        // walkable = pixel.grayscale > 0.1f;
                    }
                    
                    // Vector3 worldPos = origin + new Vector3(x * cellSize, 0, y * cellSize);
                    // Vector2 worldPos = origin + new Vector3(x * cellSize, y * cellSize, 0);
                    // nodes[x, y] = new NodeTest(x, y, walkable, worldPos);
                    // Vector2 worldPos = origin + new Vector3(pixelX * cellSize, pixelY * cellSize, 0);
                    Vector2 worldPos = new Vector2(x, y);
                    nodes[x, y] = new NodeTest(x, y, walkable, worldPos);
                }
            }
        }

        public NodeTest NodeFromWorldPoint(Vector2 worldPosition)
        {
            // worldPosition.y = height - worldPosition.y;
            
            int x = Mathf.FloorToInt((worldPosition.x - originPosition.x) / cellSize);
            int y = Mathf.FloorToInt((worldPosition.y - originPosition.y) / cellSize);
            x = Mathf.Clamp(x, 0, width - 1);
            y = Mathf.Clamp(y, 0, height - 1);
            return nodes[x, y];
        }

        public List<NodeTest> GetNeighbours(NodeTest NodeTest)
        {
            List<NodeTest> neighbours = new List<NodeTest>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = NodeTest.gridX + x;
                    int checkY = NodeTest.gridY + y;

                    if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                    {
                        neighbours.Add(nodes[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
    }

    public class NodeTest
    {
        public int gridX; //网格 x
        public int gridY; //网格 y
        public bool walkable;
        public Vector3 worldPosition;
        /// <summary>
        /// 起点到当前节点的实际距离
        /// </summary>
        public int gCost;  
        /// <summary>
        /// 当前节点到终点的启发式估算距离
        /// </summary>
        public int hCost;
        public NodeTest parent;

        /// <summary>
        /// 总预估代价  返回总的预估
        /// </summary>
        public int fCost => gCost + hCost;

        public NodeTest(int x, int y, bool walkable, Vector3 worldPos)
        {
            gridX = x;
            gridY = y;
            this.walkable = walkable;
            worldPosition = worldPos;
        }
    }
    
    public class AStarPathFinder //: TSingleton<AStarPathFinder>
    {
        
        private Grid grid;
    
        public AStarPathFinder(int width, int height, Texture2D obstacleMap, float cellSize)
        {
            grid = new Grid(width, height, obstacleMap, cellSize);
        }

        public List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
        {
            NodeTest startNode = grid.NodeFromWorldPoint(startPos);
            NodeTest targetNode = grid.NodeFromWorldPoint(targetPos);

            List<NodeTest> openSet = new List<NodeTest>();
            HashSet<NodeTest> closedSet = new HashSet<NodeTest>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                NodeTest currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (NodeTest neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        private List<Vector2> RetracePath(NodeTest startNode, NodeTest endNode)
        {
            List<Vector2> path = new List<Vector2>();
            NodeTest currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.worldPosition);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        private int GetDistance(NodeTest nodeA, NodeTest nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
        
        public bool isCanWalk(Vector2 pos)
        {
            NodeTest startNode = grid.NodeFromWorldPoint(pos);
            return startNode.walkable;
        }
    }
}