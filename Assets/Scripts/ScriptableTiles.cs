using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Tile")]
public class ScriptableTiles : ScriptableObject
{
    public int index = 1;
    public GameObject prefab;
    public int[] acceptedTiles;
}