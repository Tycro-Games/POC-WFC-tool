using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RecognizeIsland : MonoBehaviour
{
    private Tilemap map;

    //private void Start()
    //{
    //    Debug.Log("Tiles of island:" + name);
    //    map = GetComponent<Tilemap>();

    //    var tiles = map.GetTilesBlock(map.cellBounds);
    //    var cellPosition = map.WorldToCell(transform.position);

    //    for (var x = 0; x < map.cellBounds.size.x; x++)
    //    for (var y = 0; y < map.cellBounds.size.y; y++)
    //    for (var z = 0; z < map.cellBounds.size.z; z++)
    //    {
    //        var tile = tiles[map.cellBounds.size.x * map.cellBounds.size.y * z + map.cellBounds.size.x * y + x];
    //        if (tile)
    //        {
    //            var tilePos = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z + z);
    //            Debug.Log("Tile at: " + tilePos);
    //        }
    //    }
    //}
}