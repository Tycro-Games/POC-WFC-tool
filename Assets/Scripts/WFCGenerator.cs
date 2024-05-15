using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap mainMap;
    [SerializeField] private Tilemap[] islands;

    [SerializeField] private GameObject water;


    private List<int> grid;
    private List<int> indexLandTiles = new();
    private List<Vector3Int> positionsGrid;
    private int height = 0;
    private int width = 0;
    private Vector3 offset;

    [Header("Tiles")] [SerializeField] private Transform tilesParent;
    [SerializeField] private ScriptableTiles[] tiles;
    private Dictionary<int, List<ScriptableTiles>> tilesEntropy;
    private List<KeyValuePair<int, List<ScriptableTiles>>> sortedTiles;

    public void CleanTiles()
    {
        foreach (Transform child in tilesParent) Destroy(child.gameObject);
    }


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

        offset = mainMap.origin + mainMap.tileAnchor;
    }

    public void GenerateWFC()
    {
        //all the same chances
        var tempLandTiles = new List<int>(indexLandTiles);
        tilesEntropy = new Dictionary<int, List<ScriptableTiles>>();
        foreach (var indexLandTile in indexLandTiles)
        {
            var neightbours = GetNeightbours(positionsGrid[indexLandTile]);
            var values = new List<int>();
            foreach (var neightbour in neightbours) values.Add(grid[neightbour]);

            var possibleTiles = tiles.Where(x => values.All(v => v == -1 || x.acceptedTiles.Contains(v))).ToList();
            tilesEntropy.Add(indexLandTile, possibleTiles);
        }

        while (tilesEntropy.Count > 0) Iterate();


        //update each one with the chances

        //foreach (var indexLandTile in tempLandTiles)
        //{
        //    var neightbours = GetNeightbours(positionsGrid[indexLandTile]);
        //    foreach (var neightbour in neightbours)
        //        if (grid[neightbour] == 0)
        //        {
        //            CreateTile(positionsGrid[indexLandTile], tiles[0].prefab);
        //            indexLandTiles.Remove(indexLandTile);
        //            break;
        //        }
        //}

        ////use entropy
        ////spawn the rest with WFC
        //while (indexLandTiles.Count > 0)
        //{
        //    var randomLandTile = indexLandTiles[Random.Range(0, 1)];

        //    var neightbours = GetNeightbours(positionsGrid[randomLandTile]);
        //    var values = new List<int>();
        //    foreach (var neightbour in neightbours) values.Add(grid[neightbour]);
        //    var possibleTiles = tiles.Where(x => values.All(x.acceptedTiles.Contains)).ToList();

        //    CreateTile(positionsGrid[randomLandTile], possibleTiles[Random.Range(0, possibleTiles.Count)].prefab);
        //    indexLandTiles.Remove(randomLandTile);
        //}
    }

    public void Iterate()
    {
        var firstElement = CreateSmallestTIle();
        //propagate the changes
        PropagateChanges(firstElement.Key);
    }

    private KeyValuePair<int, List<ScriptableTiles>> CreateSmallestTIle()
    {
        //sort them by entropy
        sortedTiles = tilesEntropy.OrderBy(x => x.Value.Count).ToList();


        // Find the smallest count
        var toDeleteIndex = 0;
        var firstElement = RandomSmallestValue(sortedTiles, ref toDeleteIndex);

        tilesEntropy.Remove(firstElement.Key);
        var randomTileIndex = Random.Range(0, firstElement.Value.Count);
        CreateTile(positionsGrid[firstElement.Key],
            firstElement.Value[randomTileIndex].prefab);
        grid[firstElement.Key] = firstElement.Value[randomTileIndex].index;
        return firstElement;
    }

    private void PropagateChanges(int changedTile)
    {
        var neigh = GetNeightBoursValues(changedTile, out var values);
        for (var i = 0; i < values.Count; i++)
            if (values[i] == -1)
            {
                if (!tilesEntropy.ContainsKey(neigh[i]))
                    continue;
                Debug.Log(positionsGrid[neigh[i]] + " " + tilesEntropy[neigh[i]].Count);
                var currentPossibleTiles = tilesEntropy[neigh[i]];

                if (currentPossibleTiles.Count > 1)
                {
                    var possibleTiles = currentPossibleTiles
                        .Where(x => x.acceptedTiles.Contains(grid[changedTile]))
                        .ToList();
                    Debug.Log(positionsGrid[neigh[i]] + " " + possibleTiles.Count);
                    tilesEntropy[neigh[i]] = possibleTiles;
                }
                //var neighNeight = GetNeightBoursValues(neigh[i], out var neightValues);
                //for (var j = 0; j < neightValues.Count; j++)
                //    if (values[j] == -1)
                //    {
                //        var possibleTiles = tiles
                //            .Where(x => neightValues.All(v => v == -1 || x.acceptedTiles.Contains(v)))
                //            .ToList();
                //        Debug.Log(positionsGrid[neigh[i]] + " " + possibleTiles.Count);
                //    }
            }
    }

    private List<int> GetNeightBoursValues(int changedTile, out List<int> values)
    {
        var neigh = GetNeightbours(positionsGrid[changedTile]);
        values = new List<int>();
        foreach (var neightbour in neigh)
            values.Add(grid[neightbour]);
        return neigh;
    }

    private static KeyValuePair<int, List<ScriptableTiles>> RandomSmallestValue(
        List<KeyValuePair<int, List<ScriptableTiles>>> sortedTiles, ref int index)
    {
        var smallestCount = sortedTiles.First().Value.Count;

        // Filter out all elements with the smallest count
        var smallestElements = sortedTiles
            .Where(x => x.Value.Count == smallestCount)
            .ToList();

        // Select one of the smallest elements randomly
        index = Random.Range(0, smallestElements.Count);

        var firstElement = smallestElements[index];
        return firstElement;
    }

    private GameObject ChooseRandomTile()
    {
        return null;
    }

    private List<int> GetNeightbours(Vector3Int position)
    {
        var neightbours = new List<int>();
        var x = position.x;
        var y = position.y;
        if (x > 0) neightbours.Add(x - 1 + y * width);
        if (x < width - 1) neightbours.Add(x + 1 + y * width);
        if (y > 0) neightbours.Add(x + (y - 1) * width);
        if (y < height - 1) neightbours.Add(x + (y + 1) * width);
        return neightbours;
    }

    public void GetDataFromTilemaps()
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

                if (tile)
                {
                    grid[x + y * width] = -1;
                    indexLandTiles.Add(x + y * width);
                }
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
            }
        }
    }

    private void CreateTile(Vector3Int tilePos, GameObject toBeLand)
    {
        var tileObj = Instantiate(toBeLand);
        tileObj.transform.parent = tilesParent;

        tileObj.transform.position = mainMap.CellToWorld(tilePos) + offset;


        tileObj.name = ((Vector2Int)tilePos).ToString();
    }
}