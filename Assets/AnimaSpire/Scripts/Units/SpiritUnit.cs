using UnityEngine;

public class SpiritUnit : MonoBehaviour
{
    public string spiritName = "Spirit";
    public float spiritAttack = 8f;
    public float castInterval = 3.0f;
    public string roleType = "SupportAttack";

    private void Start()
    {
        Debug.Log($"SpiritUnit stats - Name: {spiritName}, Attack: {spiritAttack}, Cast Interval: {castInterval}, Role: {roleType}");
    }
}
