using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding : MonoBehaviour
{
    public Map map;
    public Node[,] grid;
    public List<Node> path;
    
    private BoxCollider2D _capsuleCollider;
    public Bounds bounds;
    
    // Start is called before the first frame update
    void Start()
    {
        _capsuleCollider = GetComponent<BoxCollider2D>();
        bounds = _capsuleCollider.bounds;
    }

    void Update()
    {
        grid = map.CreateGrid(this);
    }

    /*private IEnumerator MoveTo(float speed)
    {
        for(int i = 1; i < path.Count; i++) {
            while(Vector2.Distance(transform.position, path[i].position) > 0.1f) {
                transform.position = Vector2.MoveTowards(transform.position, path[i].position, speed * Time.fixedDeltaTime);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void MoveTo(Vector2 targetPos,float speed)
    { 
        path = FindPath(transform.position, targetPos);
        if (path == null || path.Count == 1) return;
        StartCoroutine(MoveTo(speed));
    }*/
    
    public List<Node> FindPath(Vector2 startPos, Vector2 targetPos) {
        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);
        if (targetNode == null) return null;//目标在网格之外

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if ((openSet[i].fCost < currentNode.fCost) || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // 找到路径
            if (currentNode == targetNode) {
                return RetracePath(startNode, targetNode);
            }

            // 检查相邻节点
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour) ) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null; // 没有找到路径
    }
    
    List<Node> RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }
    
    List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < map.gridSizeX && checkY >= 0 && checkY < map.gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }
    
    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14*dstY + 10* (dstX-dstY);
        return 14*dstX + 10 * (dstY-dstX);
    }
    
    public Node NodeFromWorldPoint(Vector2 worldPosition) {
        float percentX = (worldPosition.x - map.mapSource.position.x + map.gridWorldSize.x/2) / map.gridWorldSize.x;
        float percentY = (worldPosition.y - map.mapSource.position.y + map.gridWorldSize.y/2) / map.gridWorldSize.y;
        if ((percentX >= 0 && percentX <= 1 && percentY >= 0 && percentY <= 1) == false) return null;

        int x = Mathf.RoundToInt((map.gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((map.gridSizeY - 1) * percentY);
        return grid[x, y];
    }
    
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(map.mapSource.position, new Vector2(map.gridWorldSize.x, map.gridWorldSize.y));
        
        
        if (grid != null) {
            foreach (Node n in grid) {
                Gizmos.color = (n.isWalkable)?Color.white:Color.red;
                Gizmos.DrawWireCube(n.position, Vector2.one * map.nodeDiameter);
            }
        }
    }
    
}
