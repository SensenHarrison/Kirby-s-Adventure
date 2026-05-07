using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform player;
    public Tilemap groundTilemap;
    public Tilemap destructibleTilemap;
    public Tilemap indestructibleTilemap;

    [Header("Spawn Settings")]
    public int enemyCount = 10;
    public float minDistanceFromPlayer = 5f;
    
    public int minX = 10;
    public int maxX = 10;
    public int minY = -11;
    public int maxY = 9;

    void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (enemyPrefab == null || player == null || groundTilemap == null ||
            destructibleTilemap == null || indestructibleTilemap == null)
        {
            return;
        }

        List<Vector3Int> validCells = new List<Vector3Int>();
        
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                
                if (destructibleTilemap.HasTile(cellPos))
                    continue;
                
                if (indestructibleTilemap.HasTile(cellPos))
                    continue;
                
                Vector3 worldPos = groundTilemap.GetCellCenterWorld(cellPos);

                if (Vector2.Distance(worldPos, player.position) < minDistanceFromPlayer)
                    continue;

                validCells.Add(cellPos);
            }
        }
        
        if (validCells.Count < enemyCount)
        {
            Debug.LogWarning($"Not enough");
        }

        int spawnAmount = Mathf.Min(enemyCount, validCells.Count);
        
        for (int i = 0; i < spawnAmount; i++)
        {
            int randomIndex = Random.Range(0, validCells.Count);
            Vector3Int chosenCell = validCells[randomIndex];
            validCells.RemoveAt(randomIndex);

            Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(chosenCell);

            Instantiate(enemyPrefab, spawnWorldPos, Quaternion.identity);
        }
    }
}