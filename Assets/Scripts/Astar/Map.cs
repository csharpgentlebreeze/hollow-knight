using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour  //地图创建工厂
{
    public Vector2 gridWorldSize;  //网格世界坐标大小
    public LayerMask unwalkableMask; //不能行走的图层
    
    public float nodeDiameter;  //单元格直径
    public float nodeRadius;   //单元格半径
    
    public float detectRadius;  //检测大小
    
    public int gridSizeX;   //横向单元格数量
    public int gridSizeY;   //纵向单元格数量
    
    public Transform mapSource;  //地图原点
    

    public Node[,] CreateGrid(AStarPathFinding pathFinding)
    {
        Node[,] grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = new Vector2(mapSource.position.x,mapSource.position.y) - Vector2.right * gridWorldSize.x/2 - Vector2.up * gridWorldSize.y/2;
        
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                bool isPass = true;
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                for (int i = 0; i < 4; i++)
                {
                    float currentAngle = 90 * i - 180;
                    float radians = currentAngle * Mathf.Deg2Rad;
                    Vector2 direction = new Vector2(Vector2.right.x * Mathf.Cos(radians) - Vector2.right.y * Mathf.Sin(radians), Vector2.right.x * Mathf.Sin(radians) + Vector2.right.y * Mathf.Cos(radians));
                    if (i % 2 == 0)
                    {
                        if (Physics2D.Raycast(worldPoint, direction, pathFinding.bounds.extents.x - .1f, LayerMask.GetMask("ground")))
                        {
                            isPass = false;
                        }
                    }
                    else
                    {
                        if (Physics2D.Raycast(worldPoint, direction, pathFinding.bounds.extents.y - .1f, LayerMask.GetMask("ground")))
                        {
                            isPass = false;
                        }
                    }
                }
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, detectRadius, unwalkableMask)) && isPass;
                grid[x, y] = new Node(worldPoint,walkable , x, y);
            }
        }
        return grid;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        nodeRadius = nodeDiameter / 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
}
