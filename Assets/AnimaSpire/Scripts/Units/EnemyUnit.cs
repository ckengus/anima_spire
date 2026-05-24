using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public string enemyName = "Enemy";
    public float maxHp = 50f;
    public float currentHp = 50f;
    public float attackPower = 5f;
    public float attackInterval = 2.0f;
    public bool isRanged = false;
    public bool isBoss = false;

    private void Start()
    {
        Debug.Log($"EnemyUnit stats - Name: {enemyName}, HP: {currentHp}/{maxHp}, Attack Power: {attackPower}, Attack Interval: {attackInterval}, Ranged: {isRanged}, Boss: {isBoss}");
    }
}
