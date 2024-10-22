using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePoint
{
    public string areaName; //场景内的区域名
    public Vector2Int gridMapPos;

    public MovePoint(string areaName_, Vector2Int gridMapPos_)
    {
        areaName = areaName_;
        gridMapPos = gridMapPos_;
    }
}

public class AStar : MonoBehaviour
{
    private GridNodes grid; //网格地图
    private Node startNode; //起点
    private Node endNode;   //终点
    private int gridWidth;  //宽
    private int gridHeight; //高
    private int originX;    //原点x
    private int originY;    //原点y

    private List<Node> openList;    //开放列表
    private HashSet<Node> closeList;   //关闭列表 HashSet存储的是唯一元素的集合
    private Node headNode;
    private Stack<MovePoint> points;

    [Header("地图库")]
    public MapData_SO mapData_so;

    private bool isFoundPath;

    [Header("AStar测试")]
    public Tilemap displayMap;

    public TileBase displayTile;
    public bool isStartTest;
    public bool displayPath;
    public bool clearPath;

    public Vector3Int startPos;

    public Vector3Int endPos;

    private void Update()
    {
        if (isStartTest)
            ShowPathOnGridMap((Vector2Int)startPos, (Vector2Int)endPos);
    }

    /// <summary>
    /// 设置起点和终点
    /// </summary>
    /// <param name="startPos_"></param>
    /// <param name="endPos_"></param>
    public void setStartAndEnd(Vector3Int startPos_, Vector3Int endPos_)
    {
        startPos = startPos_;
        endPos = endPos_;
    }

    public void ShowPathOnGridMap(Vector2Int startPos_, Vector2Int endPos_)
    {
        if (displayMap != null && displayTile != null)
        {
            //绘制起点终点瓦片地图
            //startPos = new Vector3Int(-20, -9, 0);
            //endPos = new Vector3Int(-10, -9, 0);
            if (displayPath)
            {
                displayPath = false;
                points = buildPath("Area1", (Vector2Int)startPos_, (Vector2Int)endPos_);
                if (points != null)
                {
                    foreach (var point in points)
                    {
                        displayMap.SetTile((Vector3Int)point.gridMapPos, displayTile);
                    }
                }
            }
            if (clearPath)
            {
                if (points != null && points.Count > 0)
                {
                    foreach (var point in points)
                    {
                        displayMap.SetTile((Vector3Int)point.gridMapPos, null);
                    }
                    points.Clear();
                    clearPath = false;
                }
            }
        }
    }

    private bool generateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
    {
        MapData mapData = mapData_so.getMapData(sceneName);
        if (mapData != null)
        {
            //初始化
            gridWidth = mapData.gridWidth;
            gridHeight = mapData.gridHeight;
            originX = mapData.originX;
            originY = mapData.originY;
            openList = new List<Node>();
            closeList = new HashSet<Node>();
            points = new Stack<MovePoint>();
            //初始化映射网格的二维数组
            grid = new GridNodes(gridWidth, gridHeight, originX, originY);
            //得到当前地图区域对应的网格地图中的节点
            startNode = grid.getGridNode(Mathf.Abs(startPos.x - originX), Mathf.Abs(startPos.y - originY));
            endNode = grid.getGridNode(Mathf.Abs(endPos.x - originX), Mathf.Abs(endPos.y - originY));

            return true;
        }
        Debug.Log("生成地图失败!");
        return false;
    }

    /// <summary>
    /// 通过A*算法构建路径
    /// </summary>
    /// <param name="sceneName">地图当区域对应的场景名</param>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <returns></returns>
    public Stack<MovePoint> buildPath(string sceneName, Vector2Int start, Vector2Int end)
    {
        isFoundPath = false;
        if (generateGridNodes(sceneName, start, end))
        {
            //查找最短路径
            if (findShortPath())
            {
                MapData mapData = mapData_so.getMapData(sceneName);
                Stack<MovePoint> stack = new Stack<MovePoint>();
                //回溯算法 逆向链表
                while (headNode.parentNode != null)
                {
                    MovePoint point = new MovePoint(mapData.childSceneName, new Vector2Int(originX + headNode.gridPos.x, originY - headNode.gridPos.y));
                    stack.Push(point);
                    headNode = headNode.parentNode;
                }
                return stack;
            }
            else
            {
                Debug.LogWarning("AStar找不到近似最短路径!");
                return null;
            }
        }
        return null;
    }

    private bool findShortPath()
    {
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            openList.Sort();
            Node node = openList[0];    //获得F值 或 H值 最小的节点
            openList.RemoveAt(0);
            closeList.Add(node);
            if (node.gridPos.x == endNode.gridPos.x && node.gridPos.y == endNode.gridPos.y)   //找到了终点
            {
                isFoundPath = true;
                headNode = node;    //拿到头节点
                break;
            }
            evalueateNeighbourNodes(node);
        }
        return isFoundPath;
    }

    //评估周围8个点
    private void evalueateNeighbourNodes(Node curNode)
    {
        Vector2Int pos = curNode.gridPos;
        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; //exclude currentNode
                else if ((i == 1 || i == -1) && (j == -1 || j == 1)) continue;   //不让斜着走
                validNeighbourNode = getValidNode(pos.x + i, pos.y + j);
                if (validNeighbourNode != null)
                {
                    validNeighbourNode.gCost = curNode.gCost + getNodesDistance(curNode, validNeighbourNode);
                    validNeighbourNode.hCost = getNodesDistance(curNode, endNode);
                    //链到父节点上
                    validNeighbourNode.parentNode = curNode;
                    openList.Add(validNeighbourNode);
                }
            }
        }
    }

    //得到有效的点
    private Node getValidNode(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight)
        {
            return null;
        }

        Node node = grid.getGridNode(x, y);
        if (closeList.Contains(node) || node.isObstacle)
        {
            return null;
        }
        else
        {
            return node;
        }
    }

    //注意这里没有斜着的4个点
    private int getNodesDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int distanceY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if (distanceX == 0) return distanceX * 10;    //10为cost 消耗值
        else if (distanceY == 0) return distanceY * 10;
        //斜着的用勾股定理计算直线距离
        int res = Mathf.RoundToInt(Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY));
        return res * 10;
    }
}