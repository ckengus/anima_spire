using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int gold = 0;

    public event Action<int> OnGoldChanged;

    private void Start()
    {
        Debug.Log("GameManager initialized.");
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Gold: {gold}");
        OnGoldChanged?.Invoke(gold);
    }

    public void SetGoldForLoad(int loadedGold)
    {
        gold = Mathf.Max(loadedGold, 0);
        Debug.Log($"Gold loaded: {gold}");
    }

    public int GetGold()
    {
        return gold;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (gold < amount)
        {
            return false;
        }

        gold -= amount;
        Debug.Log($"Gold: {gold}");
        OnGoldChanged?.Invoke(gold);
        return true;
    }
}
