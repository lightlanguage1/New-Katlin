using System;
using UnityEngine;

//网格中的每一个格子
public class Node : IComparable<Node>
{
    public Vector2Int gridPos;  //网格坐标
    public int gCost = 0;
    public int hCost = 0;
    public int fCost => gCost + hCost;
    public bool isObstacle = false;
    public Node parentNode;

    public Node(Vector2Int pos)
    {
        gridPos = pos;
        parentNode = null;
    }

    public int CompareTo(Node other)
    {
        int res = fCost.CompareTo(other.fCost);
        if (res == 0)
        {
            res = hCost.CompareTo(other.hCost);
        }
        return res;
    }
}