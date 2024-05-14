using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap mainMap;
    [SerializeField] private Tilemap[] islands;

    [SerializeField] private GameObject toBeLand;
    [SerializeField] private GameObject water;


    private List<int> grid;
    private List<Vector3Int> positionsGrid;
    private int height = 0;
    private int width = 0;

    private void Awake()
    {
        mainMap.CompressBounds();
        grid = new List<int>(mainMap.size.x * mainMap.size.y);
        positionsGrid = new List<Vector3Int>(mainMap.size.x * mainMap.size.y);
        width = mainMap.size.x;
        height = mainMap.size.y;
        grid.AddRange(Enumerable.Repeat(0, mainMap.size.x * mainMap.size.y));
        foreach (var island in islands)
        {
            Debug.Log("Before: " + island.cellBounds);
            island.origin = mainMap.origin;
            island.size = mainMap.size;
            island.ResizeBounds();
            Debug.Log("After: " + island.cellBounds);
        }
    }

    public void GenerateWFC()
    {
        GetDataFromTilemaps();
    }

    private void GetDataFromTilemaps()
    {
        var first = true;
        //do something with tiles
        foreach (var map in islands)
        {
            var tiles = map.GetTilesBlock(map.cellBounds);
            var cellPosition = mainMap.WorldToCell(transform.position);

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var tile = tiles[width * y + x];
                var tilePos = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z);
                if (first) positionsGrid.Add(tilePos);

                if (tile) grid[x + y * width] = 1;
            }

            first = false;


            map.ClearAllTiles();
        }


        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var index = x + y * width;
            var value = grid[index];

            var tilePos = positionsGrid[index];

            if (value == 0)
            {
                var tileObj = Instantiate(water);
                tileObj.transform.parent = transform;

                tileObj.transform.position = mainMap.CellToWorld(tilePos + mainMap.origin) + mainMap.tileAnchor;


                tileObj.name = ((Vector2Int)tilePos).ToString();
            }
            else
            {
                var tileObj = Instantiate(toBeLand);
                tileObj.transform.parent = transform;

                tileObj.transform.position = mainMap.CellToWorld(tilePos + mainMap.origin) + mainMap.tileAnchor;


                tileObj.name = ((Vector2Int)tilePos).ToString();
            }
        }

        mainMap.ClearAllTiles();
    }
}