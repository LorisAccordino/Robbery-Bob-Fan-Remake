using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinsUIController : MonoBehaviour
{
    [Header("PlayerPrefs Key")]
    [SerializeField] private string coinsKey = "PLAYER_COINS";

    [Header("DEBUG (Inspector Only)")]
    [SerializeField] private int debugCoinsValue = 100;
    [SerializeField] private int debugAddAmount = 10;

    private TextMeshProUGUI coinsLabel;

    private void Awake()
    {
        coinsLabel = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        RefreshCoins();
    }

    // =======================
    // PUBLIC API (RUNTIME)
    // =======================

    public void RefreshCoins()
    {
        int coins = PlayerPrefs.GetInt(coinsKey, 0);
        coinsLabel.text = coins.ToString();
    }

    public void SetCoins(int value)
    {
        PlayerPrefs.SetInt(coinsKey, value);
        RefreshCoins();
    }

    public void AddCoins(int amount)
    {
        int coins = PlayerPrefs.GetInt(coinsKey, 0);
        coins += amount;
        PlayerPrefs.SetInt(coinsKey, coins);
        RefreshCoins();
    }

    // =======================
    // DEBUG BUTTONS (INSPECTOR)
    // =======================

    [ContextMenu("DEBUG / Set Coins")]
    private void DebugSetCoins()
    {
        SetCoins(debugCoinsValue);
    }

    [ContextMenu("DEBUG / Add Coins")]
    private void DebugAddCoins()
    {
        AddCoins(debugAddAmount);
    }

    [ContextMenu("DEBUG / Refresh Coins")]
    private void DebugRefreshCoins()
    {
        RefreshCoins();
    }
}