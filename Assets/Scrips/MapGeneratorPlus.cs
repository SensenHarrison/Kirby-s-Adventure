using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneratorPlus : MonoBehaviour
{
    [Header("Map Range")]
    public int minCellX = -10;
    public int maxCellX = 10;
    public int minCellY = -11;
    public int maxCellY = 9;

    [Header("Tilemaps")]
    public Tilemap destructibleTilemap;
    public Tilemap indestructibleTilemap;

    [Header("Tiles")]
    public TileBase destructibleTile;
    public TileBase indestructibleTile;

    [Header("Obstacle Chance")]
    [Range(0f, 1f)] public float destructibleChance = 0.25f;
    [Range(0f, 1f)] public float indestructibleChance = 0.15f;

    [Header("Player Safe Area")]
    public Transform playerTransform;
    public int safeRadius = 3;

    [Header("Pickups")]
    public GameObject cakePrefab;
    public GameObject damagePickupPrefab;

    [Range(0f, 1f)] public float cakeChance = 0.05f;
    [Range(0f, 1f)] public float damagePickupChance = 0.05f;

    public float enemyCheckRadius = 0.5f;
    public LayerMask enemyLayer;

    private void Start()
    {
        GenerateMapWithoutOverwriting();
        GeneratePickups();
    }

    public void GenerateMapWithoutOverwriting()
    {
        if (playerTransform == null) return;

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

    private void GeneratePickups()
    {
        if (playerTransform == null) return;

        Vector3Int playerCell = destructibleTilemap.WorldToCell(playerTransform.position);

        List<Vector3> availablePositions = new List<Vector3>();
        
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

                Vector3 worldPos = destructibleTilemap.GetCellCenterWorld(cellPos);
                
                Collider2D enemy = Physics2D.OverlapCircle(worldPos, enemyCheckRadius, enemyLayer);
                if (enemy != null)
                {
                    continue;
                }

                availablePositions.Add(worldPos);
            }
        }
        
        if (cakePrefab != null && availablePositions.Count > 0 && Random.value < cakeChance)
        {
            int index = Random.Range(0, availablePositions.Count);
            Vector3 spawnPos = availablePositions[index];
            Instantiate(cakePrefab, spawnPos, Quaternion.identity);
            
            availablePositions.RemoveAt(index);
        }
        
        if (damagePickupPrefab != null && availablePositions.Count > 0 && Random.value < damagePickupChance)
        {
            int index = Random.Range(0, availablePositions.Count);
            Vector3 spawnPos = availablePositions[index];
            Instantiate(damagePickupPrefab, spawnPos, Quaternion.identity);
        }
    }
}