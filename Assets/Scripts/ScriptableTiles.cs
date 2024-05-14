using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Tile")]
public class ScriptableTiles : ScriptableObject
{
    public GameObject prefab;
    public int[] acceptedTiles;
}