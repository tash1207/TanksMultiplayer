using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour {

    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] int ownerPriority = 15;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            followCamera.Priority = ownerPriority;
        }
    }
}