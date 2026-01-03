using UnityEngine;

public class CurrencyManager
{
    public int Coins { get; private set; }

    public CurrencyManager()
    {
        Coins = PlayerPrefs.GetInt(GameKeys.PLAYER_COINS, 0);
    }

    public bool CanAfford(int amount) => Coins >= amount;

    public void Spend(int amount)
    {
        Coins -= amount;
        Save();
    }

    public void Earn(int amount)
    {
        Coins += amount;
        Save();
    }

    public void Save()
    {
        PlayerPrefs.SetInt(GameKeys.PLAYER_COINS, Coins);
        PlayerPrefs.Save();
    }
}