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
    public int goldReward = 10;

    private bool defeatLogged;

    public bool IsAlive => currentHp > 0f;

    private void Start()
    {
        Debug.Log($"EnemyUnit stats - Name: {enemyName}, HP: {currentHp}/{maxHp}, Attack Power: {attackPower}, Attack Interval: {attackInterval}, Ranged: {isRanged}, Boss: {isBoss}");
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive)
        {
            return;
        }

        currentHp = Mathf.Max(currentHp - amount, 0f);
        DamagePopup.ShowDamage(transform.position, amount);

        if (!IsAlive && !defeatLogged)
        {
            defeatLogged = true;
            Debug.Log("Enemy defeated.");
        }
    }

    public void ResetHp()
    {
        currentHp = maxHp;
        defeatLogged = false;
    }

    public void ApplyStats(float newMaxHp, float newAttackPower)
    {
        maxHp = Mathf.Max(newMaxHp, 1f);
        attackPower = Mathf.Max(newAttackPower, 0f);
        ResetHp();
    }

    public void DealDamage(HeroUnit target)
    {
        if (!IsAlive || target == null || !target.IsAlive)
        {
            return;
        }

        target.TakeDamage(attackPower);
        Debug.Log($"Enemy dealt {attackPower} damage. Hero HP: {target.currentHp}/{target.maxHp}");
    }
}
