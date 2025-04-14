using Unity.Netcode;
using UnityEngine;

public class Leaderboard : NetworkBehaviour 
{
    [SerializeField] Transform leaderboardEntityHolder;
    [SerializeField] LeaderboardEntityDisplay leaderboardEntityPrefab;

    NetworkList<LeaderboardEntityState> leaderboardEntities;

    void Awake()
    {
        leaderboardEntities = new NetworkList<LeaderboardEntityState>();
    }
}
