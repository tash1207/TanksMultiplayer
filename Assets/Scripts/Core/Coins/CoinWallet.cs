using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

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

    public void SpendCoins(int numCoins)
    {
        TotalCoins.Value -= numCoins;
    }
}
