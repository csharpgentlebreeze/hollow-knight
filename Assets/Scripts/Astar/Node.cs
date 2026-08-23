using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
   public Vector2 position; //在世界坐标的位置
   public bool isWalkable; //能否行走
   public int gCost;       //已产生代价
   public int hCost;       //预估代价
   public Node parent;   //父节点
   public int gridX;     //网格X坐标
   public int gridY;     //网格Y坐标

   public Node(Vector2 position, bool isWalkable, int gridX, int gridY)
   {
      this.position = position;
      this.isWalkable = isWalkable;
      this.gridX = gridX;
      this.gridY = gridY;
   }
   
   public int fCost => gCost + hCost;
   
}
