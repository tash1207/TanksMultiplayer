using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

    private void LateUpdate()
    {
        if (!IsOwner) { return; }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(inputReader.AimPosition);
        turretTransform.up = mousePosition - (Vector2)turretTransform.position;
    }
}
