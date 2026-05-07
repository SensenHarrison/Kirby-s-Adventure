using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int minCellX = -10;
    public int maxCellX = 10;
    public int minCellY = -11;
    public int maxCellY = 9;

    public Tilemap destructibleTilemap;
    public Tilemap indestructibleTilemap;

    public TileBase destructibleTile;
    public TileBase indestructibleTile;

    [Range(0f, 1f)] public float destructibleChance = 0.25f;
    [Range(0f, 1f)] public float indestructibleChance = 0.15f;

    [Header("Player Safe Area")]
    public Transform playerTransform;
    public int safeRadius = 3;

    private void Start()
    {
        GenerateMapWithoutOverwriting();
    }

    public void GenerateMapWithoutOverwriting()
    {
        Vector3Int playerCell = destructibleTilemap.WorldToCell(playerTransform.position);

        for (int x = minCellX; x <= maxCellX; x++)
        {
            for (int y = minCellY; y <= maxCellY; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                
                if (Mathf.Abs(x - playerCell.x) <= safeRadius &&
                    Mathf.Abs(y - playerCell.y) <= safeRadius)
                {
                    continue;
                }
                
                if (destructibleTilemap.HasTile(cellPos) || indestructibleTilemap.HasTile(cellPos))
                {
                    continue;
                }

                float randomValue = Random.value;

                if (randomValue < indestructibleChance)
                {
                    indestructibleTilemap.SetTile(cellPos, indestructibleTile);
                }
                else if (randomValue < indestructibleChance + destructibleChance)
                {
                    destructibleTilemap.SetTile(cellPos, destructibleTile);
                }
            }
        }
    }
}