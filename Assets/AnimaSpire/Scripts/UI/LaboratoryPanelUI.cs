using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class LaboratoryPanelUI : MonoBehaviour
{
    private bool hasBuiltUi;
    private Button wardrobeButton;
    private Button synthesisRoomButton;
    private UnityAction onWardrobeClicked;
    private UnityAction onSynthesisRoomClicked;

    public void SetCallbacks(UnityAction wardrobeClicked, UnityAction synthesisRoomClicked)
    {
        onWardrobeClicked = wardrobeClicked;
        onSynthesisRoomClicked = synthesisRoomClicked;
        RefreshButtonCallbacks();
    }

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

        GameObject backgroundObject = EnsurePanel(transform, "LaboratoryBackground", new Color(0.06f, 0.075f, 0.1f, 0.98f));
        StretchToParent(backgroundObject.GetComponent<RectTransform>());

        GameObject contentObject = new GameObject("LaboratoryContent", typeof(RectTransform));
        contentObject.transform.SetParent(transform, false);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(28f, 28f);
        contentRect.offsetMax = new Vector2(-28f, -28f);

        VerticalLayoutGroup contentLayout = contentObject.AddComponent<VerticalLayoutGroup>();
        contentLayout.padding = new RectOffset(0, 0, 24, 24);
        contentLayout.spacing = 20f;
        contentLayout.childAlignment = TextAnchor.UpperCenter;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = true;
        contentLayout.childForceExpandWidth = true;
        contentLayout.childForceExpandHeight = false;

        Text titleText = EnsureText(contentObject.transform, "LaboratoryTitleText", "\uC5F0\uAD6C\uC2E4", 34, TextAnchor.MiddleCenter);
        LayoutElement titleLayout = titleText.gameObject.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 56f;

        GameObject cardRowObject = new GameObject("LaboratoryCardRow", typeof(RectTransform));
        cardRowObject.transform.SetParent(contentObject.transform, false);
        HorizontalLayoutGroup rowLayout = cardRowObject.AddComponent<HorizontalLayoutGroup>();
        rowLayout.padding = new RectOffset(0, 0, 0, 0);
        rowLayout.spacing = 18f;
        rowLayout.childAlignment = TextAnchor.UpperCenter;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = true;
        rowLayout.childForceExpandHeight = false;

        LayoutElement rowLayoutElement = cardRowObject.AddComponent<LayoutElement>();
        rowLayoutElement.preferredHeight = 160f;

        EnsureLaboratoryCard(
            cardRowObject.transform,
            "LaboratoryCard_Wardrobe",
            "\uC637\uC7A5",
            "\uC7A5\uBE44\uB97C \uD655\uC778\uD569\uB2C8\uB2E4.",
            out wardrobeButton);
        EnsureLaboratoryCard(
            cardRowObject.transform,
            "LaboratoryCard_SynthesisRoom",
            "\uC5F0\uC131\uC2E4",
            "\uC7A5\uBE44\uB97C \uC5F0\uC131\uD569\uB2C8\uB2E4.",
            out synthesisRoomButton);

        hasBuiltUi = true;
        RefreshButtonCallbacks();
    }

    private void EnsureLaboratoryCard(Transform parent, string objectName, string title, string description, out Button button)
    {
        GameObject cardObject = EnsurePanel(parent, objectName, new Color(0.13f, 0.16f, 0.22f, 0.98f));
        LayoutElement cardLayout = cardObject.AddComponent<LayoutElement>();
        cardLayout.minHeight = 140f;
        cardLayout.preferredHeight = 150f;
        cardLayout.flexibleWidth = 1f;

        button = cardObject.GetComponent<Button>();
        if (button == null)
        {
            button = cardObject.AddComponent<Button>();
        }

        button.targetGraphic = cardObject.GetComponent<Image>();

        VerticalLayoutGroup layoutGroup = cardObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(14, 14, 18, 18);
        layoutGroup.spacing = 10f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        Text titleText = EnsureText(cardObject.transform, "Label", title, 26, TextAnchor.MiddleCenter);
        LayoutElement titleLayout = titleText.gameObject.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 44f;

        Text descriptionText = EnsureText(cardObject.transform, "Description", description, 18, TextAnchor.MiddleCenter);
        LayoutElement descriptionLayout = descriptionText.gameObject.AddComponent<LayoutElement>();
        descriptionLayout.preferredHeight = 54f;
        descriptionText.color = new Color(0.78f, 0.84f, 0.92f, 1f);
    }

    private void RefreshButtonCallbacks()
    {
        if (wardrobeButton != null)
        {
            wardrobeButton.onClick.RemoveAllListeners();
            wardrobeButton.onClick.AddListener(onWardrobeClicked ?? LogWardrobeClicked);
        }

        if (synthesisRoomButton != null)
        {
            synthesisRoomButton.onClick.RemoveAllListeners();
            synthesisRoomButton.onClick.AddListener(onSynthesisRoomClicked ?? LogSynthesisRoomClicked);
        }
    }

    private void LogWardrobeClicked()
    {
        Debug.Log("Laboratory wardrobe clicked.");
    }

    private void LogSynthesisRoomClicked()
    {
        Debug.Log("Laboratory synthesis room clicked.");
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
