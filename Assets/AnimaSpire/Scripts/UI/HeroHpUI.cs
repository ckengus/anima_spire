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
        hpText.text = $"Hero HP: {currentHp} / {maxHp}";
    }

    private Text EnsureText(Text text)
    {
        if (text != null)
        {
            return text;
        }

        Transform existingText = transform.Find("HeroHpText");
        if (existingText != null && existingText.TryGetComponent(out Text existing))
        {
            return existing;
        }

        GameObject textObject = new GameObject("HeroHpText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0.5f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(32f, -24f);
        rectTransform.sizeDelta = new Vector2(-64f, -48f);

        Text createdText = textObject.GetComponent<Text>();
        createdText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        createdText.fontSize = 36;
        createdText.alignment = TextAnchor.UpperLeft;
        createdText.color = Color.white;
        createdText.raycastTarget = false;

        return createdText;
    }
}
