using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Prefab of the projectile
    [SerializeField] private Transform shootPoint; // Point from where projectiles are shot
    [SerializeField] private float shootInterval = 2.0f; // Time between shots
    [SerializeField] private float projectileSpeed = 10.0f; // Speed of the projectile
    [SerializeField] private AudioClip shootClip; // Audio clip to play when shooting

    private Transform player;
    private bool isShooting;
    private AudioSource audioSource; // Reference to the AudioSource component
    private float lastShootTime;

    void Start()
    {
        player = PlayerController.Instance.transform;
        isShooting = false;
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component if it doesn't exist
        }
        lastShootTime = -shootInterval; // Initialize last shoot time to allow immediate shooting at start
    }

    void Update()
    {
        if (isShooting)
        {
            TryShoot();
        }
    }

    public void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    void TryShoot()
    {
        if (Time.time - lastShootTime >= shootInterval)
        {
            ShootProjectile();
            lastShootTime = Time.time;
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null && shootPoint != null && player != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController != null)
            {
                projectileController.SetTarget(player, projectileSpeed);
            }
            PlayShootSound(); // Play the shooting sound
        }
    }

    void PlayShootSound()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && shootClip != null)
        {
            audioSource.PlayOneShot(shootClip); // Play the shooting sound clip
        }
    }
}
