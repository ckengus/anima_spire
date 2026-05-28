using UnityEngine;
using UnityEngine.UI;

public sealed class EquipmentSynthesisPanelUI : MonoBehaviour
{
    private bool hasBuiltUi;

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        EnsureUi();
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    private void EnsureUi()
    {
        if (hasBuiltUi)
        {
            return;
        }

        GameObject backgroundObject = EnsurePanel(transform, "EquipmentSynthesisBackground", new Color(0.06f, 0.075f, 0.1f, 0.98f));
        StretchToParent(backgroundObject.GetComponent<RectTransform>());

        GameObject contentObject = new GameObject("EquipmentSynthesisContent", typeof(RectTransform));
        contentObject.transform.SetParent(transform, false);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(28f, 28f);
        contentRect.offsetMax = new Vector2(-28f, -28f);

        VerticalLayoutGroup contentLayout = contentObject.AddComponent<VerticalLayoutGroup>();
        contentLayout.padding = new RectOffset(0, 0, 24, 24);
        contentLayout.spacing = 18f;
        contentLayout.childAlignment = TextAnchor.UpperCenter;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;

        Text titleText = EnsureText(contentObject.transform, "EquipmentSynthesisTitleText", "\uC5F0\uC131\uC2E4", 34, TextAnchor.MiddleCenter);
        LayoutElement titleLayout = titleText.gameObject.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 56f;

        Text descriptionText = EnsureText(contentObject.transform, "EquipmentSynthesisDescriptionText", "Gold\uB97C \uC0AC\uC6A9\uD574 \uC7A5\uBE44\uB97C \uC5F0\uC131\uD569\uB2C8\uB2E4.", 22, TextAnchor.MiddleCenter);
        descriptionText.color = new Color(0.8f, 0.86f, 0.94f, 1f);
        LayoutElement descriptionLayout = descriptionText.gameObject.AddComponent<LayoutElement>();
        descriptionLayout.preferredHeight = 54f;

        GameObject buttonObject = EnsurePanel(contentObject.transform, "EquipmentSynthesisButton", new Color(0.2f, 0.38f, 0.72f, 0.98f));
        LayoutElement buttonLayout = buttonObject.AddComponent<LayoutElement>();
        buttonLayout.minWidth = 220f;
        buttonLayout.preferredWidth = 260f;
        buttonLayout.minHeight = 64f;
        buttonLayout.preferredHeight = 72f;
        buttonLayout.flexibleWidth = 0f;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.AddListener(() => Debug.Log("Equipment synthesis button clicked."));

        Text buttonText = EnsureText(buttonObject.transform, "Text", "\uC7A5\uBE44 \uC5F0\uC131", 24, TextAnchor.MiddleCenter);
        StretchToParent(buttonText.GetComponent<RectTransform>());

        GameObject resultAreaObject = EnsurePanel(contentObject.transform, "EquipmentSynthesisResultArea", new Color(0.1f, 0.13f, 0.18f, 0.98f));
        LayoutElement resultLayout = resultAreaObject.AddComponent<LayoutElement>();
        resultLayout.minHeight = 120f;
        resultLayout.preferredHeight = 140f;
        resultLayout.flexibleWidth = 1f;

        Text resultText = EnsureText(resultAreaObject.transform, "ResultText", "\uC544\uC9C1 \uC5F0\uC131 \uACB0\uACFC \uC5C6\uC74C", 22, TextAnchor.MiddleCenter);
        resultText.color = new Color(0.78f, 0.84f, 0.92f, 1f);
        StretchToParent(resultText.GetComponent<RectTransform>());

        hasBuiltUi = true;
    }

    private GameObject EnsurePanel(Transform parent, string objectName, Color color)
    {
        GameObject panelObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = true;

        return panelObject;
    }

    private Text EnsureText(Transform parent, string objectName, string textValue, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = textValue;

        return text;
    }

    private void StretchToParent(RectTransform rectTransform)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }
}
