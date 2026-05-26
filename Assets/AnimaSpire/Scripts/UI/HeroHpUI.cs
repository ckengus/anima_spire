using UnityEngine;
using UnityEngine.UI;

public class HeroHpUI : MonoBehaviour
{
    [SerializeField] private HeroUnit hero;
    [SerializeField] private Text hpText;

    private void Awake()
    {
        if (hero == null)
        {
            hero = FindAnyObjectByType<HeroUnit>();
        }

        hpText = EnsureText(hpText);
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (hpText == null || hero == null)
        {
            return;
        }

        int currentHp = Mathf.RoundToInt(hero.currentHp);
        int maxHp = Mathf.RoundToInt(hero.maxHp);
        hpText.text = $"Hero HP: {currentHp}/{maxHp}";
    }

    private Text EnsureText(Text text)
    {
        if (text != null)
        {
            ApplyTextStyle(text);
            return text;
        }

        Transform existingText = transform.Find("HeroHpText");
        if (existingText != null && existingText.TryGetComponent(out Text existing))
        {
            RectTransform existingRect = existing.GetComponent<RectTransform>();
            existingRect.anchorMin = new Vector2(0.06f, 0.5f);
            existingRect.anchorMax = new Vector2(0.94f, 1f);
            existingRect.anchoredPosition = Vector2.zero;
            existingRect.sizeDelta = Vector2.zero;
            ApplyTextStyle(existing);
            return existing;
        }

        GameObject textObject = new GameObject("HeroHpText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.06f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.94f, 1f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Text createdText = textObject.GetComponent<Text>();
        ApplyTextStyle(createdText);

        return createdText;
    }

    private void ApplyTextStyle(Text text)
    {
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 42;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 28;
        text.resizeTextMaxSize = 42;
        text.alignment = TextAnchor.UpperLeft;
        text.color = Color.white;
        text.raycastTarget = false;
    }
}
