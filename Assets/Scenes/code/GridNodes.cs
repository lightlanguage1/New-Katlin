using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;
    private Node[,] grid;

    //初始化自定义网格地图
    public GridNodes(int width, int height,int originX,int originY)
    {
        grid = new Node[height,width];
        this.width = width;
        this.height = height;   
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = new Node(new Vector2Int(j, i));
            }
        }
    }

    public Node getGridNode(int xPos, int yPos)
    {
        if (xPos < width && yPos < height)
        {
            return grid[yPos, xPos];
        }
        Debug.Log("没有这样的节点!");
        return null;
    }
}