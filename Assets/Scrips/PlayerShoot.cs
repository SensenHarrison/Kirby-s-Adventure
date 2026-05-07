using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    public PlayerBullet playerBullet;
    public Transform firePoint;
    public Text cdText;

    [Header("Shoot Settings")]
    public float reloadTime = 2f;

    [Header("Sound Effect")]
    public AudioClip shootClip;
    
    private float cooldownTimer;

    private void Start()
    {
        cooldownTimer = reloadTime;

        if (playerBullet != null)
        {
            playerBullet.gameObject.SetActive(false);
        }

        UpdateCDText();
    }

    private void Update()
    {
        if (cooldownTimer < reloadTime)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > reloadTime)
            {
                cooldownTimer = reloadTime;
            }

            UpdateCDText();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (playerBullet == null || firePoint == null) return;
        
        if (cooldownTimer < reloadTime) return;
        
        if (playerBullet.IsFlying) return;
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 shootDirection = mouseWorldPos - firePoint.position;

        playerBullet.Fire(firePoint.position, shootDirection);

        cooldownTimer = 0f;
        UpdateCDText();

        if (shootClip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(shootClip);
        }
    }

    private void UpdateCDText()
    {
        if (cdText == null) return;

        int current = Mathf.FloorToInt(cooldownTimer);

        if (cooldownTimer >= reloadTime)
        {
            current = Mathf.RoundToInt(reloadTime);
        }

        cdText.text = $"CD:  {current}/{Mathf.RoundToInt(reloadTime)}";
    }
}