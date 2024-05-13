using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RecognizeIsland : MonoBehaviour
{
    private Tilemap map;
    private TileBase[] tiles;
    [SerializeField] private GameObject obj;

    private void Start()
    {
        Debug.Log("Tiles of island:" + name);
        map = GetComponent<Tilemap>();
        //map.CompressBounds();
        tiles = map.GetTilesBlock(map.cellBounds);
        var cellPosition = map.WorldToCell(transform.position);

        for (var x = 0; x < map.cellBounds.size.x; x++)
        for (var y = 0; y < map.cellBounds.size.y; y++)
        for (var z = 0; z < map.cellBounds.size.z; z++)
        {
            var tile = tiles[map.cellBounds.size.x * map.cellBounds.size.y * z + map.cellBounds.size.x * y + x];
            if (tile)
            {
                var tilePos = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z + z);
                Debug.Log("Tile at: " + tilePos);
            }
            else
            {
                //Debug.Log("Tile empty: " + new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z + z));
            }
        }
    }

    public void GenerateWFC()
    {
        //do something with tiles

        var cellPosition = map.WorldToCell(transform.position);

        for (var x = 0; x < map.cellBounds.size.x; x++)
        for (var y = 0; y < map.cellBounds.size.y; y++)
        for (var z = 0; z < map.cellBounds.size.z; z++)
        {
            var tile = tiles[map.cellBounds.size.x * map.cellBounds.size.y * z + map.cellBounds.size.x * y + x];
            if (tile)
            {
                var tilePos = new Vector3Int(cellPosition.x + x, cellPosition.y + y, cellPosition.z + z);
                var data = new TileData();
                tile.GetTileData(tilePos, map, ref data);

                var tileObj = Instantiate(obj);
                tileObj.transform.parent = map.transform;

                tileObj.transform.position = map.CellToWorld(tilePos + map.origin) + map.tileAnchor;


                tileObj.name = tilePos.ToString();
            }
        }

        map.ClearAllTiles();
    }
}