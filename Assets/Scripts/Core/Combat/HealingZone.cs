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

    float remainingCooldown;
    float tickTimer;

    List<TankPlayer> playersInZone = new List<TankPlayer>();
    NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, HealPower.Value);
        }

        if (IsServer)
        {
            HealPower.Value = maxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    void Update()
    {
        if (!IsServer) { return; }

        if (remainingCooldown > 0f)
        {
            remainingCooldown -= Time.deltaTime;

            if (remainingCooldown <= 0f)
            {
                HealPower.Value = maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= 1 / healTickRate)
        {
            foreach (TankPlayer player in playersInZone)
            {
                if (HealPower.Value == 0) { break; }

                if (player.Health.CurrentHealth.Value == player.Health.MaxHealth) { continue; }

                if (player.Wallet.TotalCoins.Value < coinsPerTick) { continue; }

                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healthPerTick);

                HealPower.Value -= 1;

                if (HealPower.Value == 0)
                {
                    remainingCooldown = healCooldown;
                }
            }
            tickTimer = tickTimer % (1 / healTickRate);
        }
    }

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

    void HandleHealPowerChanged(int oldHealPower, int newHealPower)
    {
        healPowerBar.fillAmount = (float) newHealPower / maxHealPower;
    }
}
