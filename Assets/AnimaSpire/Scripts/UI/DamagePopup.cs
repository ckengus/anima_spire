using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    private const float Lifetime = 0.8f;
    private static readonly Vector3 SpawnOffset = new Vector3(0f, 1.05f, 0f);
    private static readonly Vector3 MoveOffset = new Vector3(0f, 0.45f, 0f);

    private Text damageText;
    private Vector3 startPosition;
    private float elapsedTime;

    public static void ShowDamage(Vector3 targetPosition, float damage)
    {
        if (damage <= 0f)
        {
            return;
        }

        GameObject popupObject = new GameObject("DamagePopup", typeof(RectTransform), typeof(Canvas), typeof(DamagePopup));
        popupObject.transform.position = targetPosition + SpawnOffset;

        Canvas canvas = popupObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 40;

        RectTransform canvasRect = popupObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(120f, 48f);
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        GameObject textObject = new GameObject("DamageText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(popupObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 36;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = Mathf.RoundToInt(damage).ToString();

        DamagePopup popup = popupObject.GetComponent<DamagePopup>();
        popup.Initialize(text);
    }

    private void Initialize(Text text)
    {
        damageText = text;
        startPosition = transform.position;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / Lifetime);

        transform.position = Vector3.Lerp(startPosition, startPosition + MoveOffset, progress);

        if (damageText != null)
        {
            Color color = damageText.color;
            color.a = 1f - progress;
            damageText.color = color;
        }

        if (elapsedTime >= Lifetime)
        {
            Destroy(gameObject);
        }
    }
}
