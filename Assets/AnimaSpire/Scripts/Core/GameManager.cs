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
}
