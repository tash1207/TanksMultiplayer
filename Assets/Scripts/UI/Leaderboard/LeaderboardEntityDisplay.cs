using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text displayText;
    [SerializeField] Color myColor;

    private FixedString32Bytes playerName;

    public ulong ClientId { get; private set; }
    public int Coins { get; private set; }

    public void Initialize(ulong clientId, FixedString32Bytes name, int numCoins)
    {
        ClientId = clientId;
        playerName = name;

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            displayText.color = myColor;
        }
        
        UpdateCoins(numCoins);
    }

    public void UpdateCoins(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    public void UpdateText()
    {
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} ({Coins})";
    }
}
