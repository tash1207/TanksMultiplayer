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

    [Header("Settings")]
    [SerializeField] float projectileSpeed;

    bool shouldFire;

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
        if (!IsOwner) { return; }
        if (!shouldFire) { return; }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
    }

    void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    void PrimaryFireServerRpc(Vector2 spawnPos, Vector2 direction)
    {
        GameObject projectileInstance =
            Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        
        projectileInstance.transform.up = direction;

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
        GameObject projectileInstance =
            Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);
        
        projectileInstance.transform.up = direction;
    }
}
