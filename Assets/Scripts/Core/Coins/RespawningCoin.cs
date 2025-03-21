using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    Vector3 previousPosition;

    void Update() {
        if (IsServer) { return; }
        if (transform.position != previousPosition)
        {
            Show(true);
        }
        previousPosition = transform.position;
    }

    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected)
        {
            return 0;
        }
        else
        {
            alreadyCollected = true;
            OnCollected?.Invoke(this);
            return coinValue;
        }
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
