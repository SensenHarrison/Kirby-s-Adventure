using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 1;
    
    [Header("Sound Effect")]
    public AudioClip hitClip;
    
    private Rigidbody2D rb;
    private float timer;
    private bool isFlying;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        timer = lifeTime;
        isFlying = true;
    }

    private void Update()
    {
        if (!isFlying) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            HideBullet(); 
        }
    }

    public void Fire(Vector2 startPos, Vector2 direction)
    {
        transform.position = startPos;
        transform.right = direction;

        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }

        timer = lifeTime;
        isFlying = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFlying) return;

        if (other.CompareTag("Enemy")) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);
            }
            PlayHitSound();
            HideBullet();
            return;
        }
        PlayHitSound();
        HideBullet();
    }

    public void HideBullet()
    {
        isFlying = false;

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
}