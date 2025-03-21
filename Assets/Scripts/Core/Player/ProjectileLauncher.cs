using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject clientProjectilePrefab;
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] float projectileSpeed;
    [SerializeField] float fireRate;
    [SerializeField] float muzzleFlashDuration;

    bool shouldFire;
    float previousFireTime;
    float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if (muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return; }
        if (!shouldFire) { return; }

        if (Time.time < (1 / fireRate) + previousFireTime) { return; }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        previousFireTime = Time.time;
    }

    void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    void PrimaryFireServerRpc(Vector2 spawnPos, Vector2 direction)
    {
        SpawnProjectilePrefabInstance(serverProjectilePrefab, spawnPos, direction);
        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    void SpawnDummyProjectileClientRpc(Vector2 spawnPos, Vector2 direction)
    {
        if (IsOwner) { return; }
        
        SpawnDummyProjectile(spawnPos, direction);
    }

    void SpawnDummyProjectile(Vector2 spawnPos, Vector2 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        SpawnProjectilePrefabInstance(clientProjectilePrefab, spawnPos, direction);
    }

    void SpawnProjectilePrefabInstance(GameObject projectilePrefab, Vector2 spawnPos, Vector2 direction)
    {
        GameObject projectileInstance =
            Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        if (projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }
    }
}
