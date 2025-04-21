using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] BountyCoin coinPrefab;
    [SerializeField] Health health;

    [Header("Settings")]
    [SerializeField] int bountyCoinCount = 10;
    [SerializeField] int minBountyCoinValue = 5;
    [SerializeField] float coinSpread = 3f;
    [SerializeField] float bountyPercentage = 50f;
    [SerializeField] LayerMask layerMask;

    Collider2D[] coinBuffer = new Collider2D[1];
    float coinRadius;

    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += (health) => HandleDie(health);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        health.OnDie -= (health) => HandleDie(health);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<Coin>(out Coin coin))
        {
            int coinValue = coin.Collect();

            if (IsServer)
            {
                TotalCoins.Value += coinValue;
            }
        }
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * bountyPercentage / 100);
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue) { return; }

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin coinInstance =
                Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    public void SpendCoins(int numCoins)
    {
        TotalCoins.Value -= numCoins;
    }

    Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
