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

        stageText = EnsureText(stageText, "StageText", TextAnchor.MiddleLeft, new Vector2(0f, 0f), new Vector2(0.55f, 1f), new Vector2(32f, 0f));
        goldText = EnsureText(goldText, "GoldText", TextAnchor.MiddleRight, new Vector2(0.55f, 0f), new Vector2(1f, 1f), new Vector2(-32f, 0f));
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

    private Text EnsureText(Text text, string objectName, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
    {
        if (text != null)
        {
            return text;
        }

        Transform existingText = transform.Find(objectName);
        if (existingText != null && existingText.TryGetComponent(out Text existing))
        {
            return existing;
        }

        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = Vector2.zero;

        Text createdText = textObject.GetComponent<Text>();
        createdText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        createdText.fontSize = 40;
        createdText.alignment = alignment;
        createdText.color = Color.white;
        createdText.raycastTarget = false;

        return createdText;
    }
}
