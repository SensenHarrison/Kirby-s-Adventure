using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawnerPlus : MonoBehaviour
{
    [Header("References")]
    public GameObject[] enemyPrefabs;   
    public Transform player;
    public Tilemap groundTilemap;
    public Tilemap destructibleTilemap;
    public Tilemap indestructibleTilemap;

    [Header("Spawn Settings")]
    public int maxEnemiesOnField = 10;   
    public int totalEnemyCount = 20;    
    public float minDistanceFromPlayer = 5f;
    public float refillInterval = 2f;  

    [Header("Spawn Range")]
    public int minX = -10;
    public int maxX = 10;
    public int minY = -11;
    public int maxY = 9;

    private List<Vector3Int> validCells = new List<Vector3Int>();

    private int currentAliveEnemies = 0;
    private int totalSpawnedEnemies = 0;

    private bool isRefilling = false;

    void Start()
    {
        BuildValidCellList();
        SpawnInitialEnemies();
    }

    void BuildValidCellList()
    {
        validCells.Clear();

        if (player == null || groundTilemap == null ||
            destructibleTilemap == null || indestructibleTilemap == null)
        {
            Debug.LogWarning("False reference");
            return;
        }

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
    }
    
    public void SpawnEnemies()
    {
        BuildValidCellList();
        SpawnInitialEnemies();
    }

    void SpawnInitialEnemies()
    {
        int amount = Mathf.Min(maxEnemiesOnField, totalEnemyCount);

        for (int i = 0; i < amount; i++)
        {
            bool success = SpawnOneEnemy();
            if (!success)
            {
                Debug.LogWarning("Not enough");
                break;
            }
        }
    }

    bool SpawnOneEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return false;

        if (totalSpawnedEnemies >= totalEnemyCount)
            return false;

        List<Vector3Int> availableCells = new List<Vector3Int>();

        
        for (int i = 0; i < validCells.Count; i++)
        {
            Vector3 worldPos = groundTilemap.GetCellCenterWorld(validCells[i]);
            
            if (Vector2.Distance(worldPos, player.position) < minDistanceFromPlayer)
                continue;
            
            Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.2f);
            if (hit != null && hit.CompareTag("Enemy"))
                continue;

            availableCells.Add(validCells[i]);
        }

        if (availableCells.Count == 0)
            return false;

        int cellIndex = Random.Range(0, availableCells.Count);
        Vector3Int chosenCell = availableCells[cellIndex];
        Vector3 spawnWorldPos = groundTilemap.GetCellCenterWorld(chosenCell);

        int prefabIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[prefabIndex];

        GameObject enemy = Instantiate(selectedPrefab, spawnWorldPos, Quaternion.identity);

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.spawner = this;
        }

        currentAliveEnemies++;
        totalSpawnedEnemies++;

        return true;
    }

    public void OnEnemyRemoved()
    {
        currentAliveEnemies--;

        if (currentAliveEnemies < 0)
            currentAliveEnemies = 0;

        TryStartRefill();
    }

    void TryStartRefill()
    {
        if (isRefilling) return;
        if (totalSpawnedEnemies >= totalEnemyCount) return;
        if (currentAliveEnemies >= maxEnemiesOnField) return;

        StartCoroutine(RefillEnemiesCoroutine());
    }

    IEnumerator RefillEnemiesCoroutine()
    {
        isRefilling = true;

        while (currentAliveEnemies < maxEnemiesOnField &&
               totalSpawnedEnemies < totalEnemyCount)
        {
            yield return new WaitForSeconds(2f);

            bool success = SpawnOneEnemy();
            if (!success)
            {
                break;
            }
        }

        isRefilling = false;
    }
}
