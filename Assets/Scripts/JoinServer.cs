using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }
}
