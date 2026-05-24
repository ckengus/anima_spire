using UnityEngine;

public class HeroUnit : MonoBehaviour
{
    public string unitName = "Hero";
    public float maxHp = 100f;
    public float currentHp = 100f;
    public float magicAttack = 10f;
    public float castInterval = 2.5f;

    private bool defeatLogged;

    public bool IsAlive => currentHp > 0f;

    private void Start()
    {
        Debug.Log($"HeroUnit stats - Name: {unitName}, HP: {currentHp}/{maxHp}, Magic Attack: {magicAttack}, Cast Interval: {castInterval}");
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive)
        {
            return;
        }

        currentHp = Mathf.Max(currentHp - amount, 0f);

        if (!IsAlive && !defeatLogged)
        {
            defeatLogged = true;
            Debug.Log("Hero defeated.");
        }
    }

    public void ResetHp()
    {
        currentHp = maxHp;
        defeatLogged = false;
    }

    public void DealDamage(EnemyUnit target)
    {
        if (!IsAlive || target == null || !target.IsAlive)
        {
            return;
        }

        target.TakeDamage(magicAttack);
        Debug.Log($"Hero dealt {magicAttack} damage. Enemy HP: {target.currentHp}/{target.maxHp}");
    }
}
