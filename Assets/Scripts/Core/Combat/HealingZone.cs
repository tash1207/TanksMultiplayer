using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Image healPowerBar;

    [Header("Settings")]
    [SerializeField] int maxHealPower = 30;
    [SerializeField] float healCooldown = 30f;
    [SerializeField] float healTickRate = 1f;
    [SerializeField] int coinsPerTick = 10;
    [SerializeField] int healthPerTick = 10;

    List<TankPlayer> playersInZone = new List<TankPlayer>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) { return; }

        if (col.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            playersInZone.Add(player);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!IsServer) { return; }

        if (col.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            playersInZone.Remove(player);
        }
    }
}
