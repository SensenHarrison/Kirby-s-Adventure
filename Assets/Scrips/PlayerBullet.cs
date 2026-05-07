using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 15f;
    public float maxLifeTime = 3f;
    public int damage = 2;

    [Header("Tilemap Settings")]
    public Tilemap destructibleTilemap;

    [Header("Sound Effect")]
    public AudioClip hitClip;

    private Rigidbody2D rb;
    private float lifeTimer;

    public bool IsFlying { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsFlying) return;

        lifeTimer += Time.deltaTime;

        if (lifeTimer >= maxLifeTime)
        {
            HideBullet();
        }
    }

    public void Fire(Vector2 startPosition, Vector2 direction)
    {
        transform.position = startPosition;

        if (direction != Vector2.zero)
        {
            transform.right = direction;
        }

        gameObject.SetActive(true);
        IsFlying = true;
        lifeTimer = 0f;

        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    private void HideBullet()
    {
        IsFlying = false;
        lifeTimer = 0f;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        gameObject.SetActive(false);
    }

    private void PlayHitSound()
    {
        if (hitClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hitClip);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsFlying) return;
        if (other.CompareTag("Player")) return;
        
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
            }
            PlayHitSound();
            HideBullet();
            return;
        }
        
        Tilemap hitTilemap = other.GetComponent<Tilemap>();
        if (hitTilemap == null)
        {
            hitTilemap = other.GetComponentInParent<Tilemap>();
        }

        if (hitTilemap != null && hitTilemap == destructibleTilemap)
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);
            Vector3Int targetCell = FindNearestOccupiedCell(hitPoint);

            if (targetCell.x != int.MinValue)
            {
                destructibleTilemap.SetTile(targetCell, null);
            }
            PlayHitSound();
            HideBullet();
            return;
        }
        
        if (other.CompareTag("AirWall") || other.CompareTag("IndestructibleWall"))
        {
            PlayHitSound();
            HideBullet();
            return;
        }
    }

    private Vector3Int FindNearestOccupiedCell(Vector2 hitPoint)
    {
        Vector3Int centerCell = destructibleTilemap.WorldToCell(hitPoint);

        float minDistance = float.MaxValue;
        Vector3Int bestCell = new Vector3Int(int.MinValue, int.MinValue, 0);

        for (int x = centerCell.x - 1; x <= centerCell.x + 1; x++)
        {
            for (int y = centerCell.y - 1; y <= centerCell.y + 1; y++)
            {
                Vector3Int checkCell = new Vector3Int(x, y, 0);

                if (!destructibleTilemap.HasTile(checkCell))
                    continue;

                Vector3 cellCenter = destructibleTilemap.GetCellCenterWorld(checkCell);
                float distance = Vector2.Distance(hitPoint, cellCenter);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestCell = checkCell;
                }
            }
        }

        return bestCell;
    }
    
    public void DoubleDamage()
    {
        damage *= 2;
    }
}