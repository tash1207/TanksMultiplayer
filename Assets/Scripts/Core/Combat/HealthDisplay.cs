using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Health health;
    [SerializeField] Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) { return; }

        if (health == null || health.CurrentHealth == null) { return; }

        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }

        if (health == null || health.CurrentHealth == null) { return; }

        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        health.CurrentHealth.Value = newHealth;
        healthBarImage.fillAmount = (float) newHealth / health.MaxHealth;
    }
}
