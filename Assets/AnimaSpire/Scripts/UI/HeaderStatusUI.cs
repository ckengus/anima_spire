using UnityEngine;
using UnityEngine.UI;

public class HeaderStatusUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Text stageText;
    [SerializeField] private Text goldText;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
        }

        stageText = EnsureText(stageText, "StageText", TextAnchor.MiddleLeft, new Vector2(0.04f, 0f), new Vector2(0.52f, 1f));
        goldText = EnsureText(goldText, "GoldText", TextAnchor.MiddleLeft, new Vector2(0.52f, 0f), new Vector2(0.98f, 1f));
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (stageText != null && stageManager != null)
        {
            stageText.text = $"Stage: {stageManager.GetCurrentStageLogLabel()}";
        }

        if (goldText != null && gameManager != null)
        {
            goldText.text = $"Gold: {gameManager.gold}";
        }
    }

    private Text EnsureText(Text text, string objectName, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        if (text != null)
        {
            ApplyTextStyle(text, alignment);
            return text;
        }

        Transform existingText = transform.Find(objectName);
        if (existingText != null && existingText.TryGetComponent(out Text existing))
        {
            RectTransform existingRect = existing.GetComponent<RectTransform>();
            existingRect.anchorMin = anchorMin;
            existingRect.anchorMax = anchorMax;
            existingRect.anchoredPosition = Vector2.zero;
            existingRect.sizeDelta = Vector2.zero;
            ApplyTextStyle(existing, alignment);
            return existing;
        }

        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Text createdText = textObject.GetComponent<Text>();
        ApplyTextStyle(createdText, alignment);

        return createdText;
    }

    private void ApplyTextStyle(Text text, TextAnchor alignment)
    {
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 34;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 20;
        text.resizeTextMaxSize = 34;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
    }
}
