using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager networkManager;

    private Dictionary<ulong, string> clientIdToAuthId = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        this.networkManager.ConnectionApprovalCallback += ApprovalCheck;
        this.networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuthId[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuthId.TryGetValue(clientId, out string authId))
        {
            clientIdToAuthId.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }
}
