using UnityEngine;

public class HeroUnit : MonoBehaviour
{
    public string unitName = "Hero";
    public float maxHp = 100f;
    public float currentHp = 100f;
    public float magicAttack = 10f;
    public float castInterval = 2.5f;

    private void Start()
    {
        Debug.Log($"HeroUnit stats - Name: {unitName}, HP: {currentHp}/{maxHp}, Magic Attack: {magicAttack}, Cast Interval: {castInterval}");
    }
}
