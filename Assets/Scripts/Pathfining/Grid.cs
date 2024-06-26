﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private Node[,] grid;

    private float nodeDiameter;
    public int gridSizeX, gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
    }

    public void CreateGrid()
    {
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        grid = new Node[gridSizeX, gridSizeY];
        var worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 -
                              Vector3.up * gridWorldSize.y / 2;

        for (var x = 0; x < gridSizeX; x++)
        for (var y = 0; y < gridSizeY; y++)
        {
            var worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) +
                             Vector3.up * (y * nodeDiameter + nodeRadius);
            var walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
            grid[x, y] = new Node(walkable, worldPoint, x, y);
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        var neighbours = new List<Node>();

        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
        {
            if ((x == 0 && y == 0) || (x != 0 && y != 0))
                continue;

            var checkX = node.gridX + x;
            var checkY = node.gridY + y;

            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                neighbours.Add(grid[checkX, checkY]);
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        worldPosition -= transform.position;
        var percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        var percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> path = new();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
            foreach (var n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
    }
}