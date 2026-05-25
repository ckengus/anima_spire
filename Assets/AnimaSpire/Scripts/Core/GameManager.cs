using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int gold = 0;

    private void Start()
    {
        Debug.Log("GameManager initialized.");
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"Gold: {gold}");
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
        return true;
    }
}
