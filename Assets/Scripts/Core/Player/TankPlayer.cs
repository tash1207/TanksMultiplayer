using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour {

    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] int ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            
            PlayerName.Value = userData.userName;
        }

        if (IsOwner)
        {
            followCamera.Priority = ownerPriority;
        }
    }
}