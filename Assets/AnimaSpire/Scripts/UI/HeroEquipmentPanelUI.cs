using UnityEngine;
using UnityEngine.UI;

public sealed class HeroEquipmentPanelUI : MonoBehaviour
{
    private static readonly string[] OffensiveSlotNames =
    {
        "\uBB34\uAE30",
        "\uBAA9\uAC78\uC774",
        "\uADC0\uACE0\uB9AC",
        "\uBC18\uC9C0"
    };

    private static readonly string[] DefensiveSlotNames =
    {
        "\uBAA8\uC790",
        "\uC637",
        "\uC7A5\uAC11",
        "\uC2E0\uBC1C"
    };

    private Text selectedSlotText;
    private Text equipmentNameText;
    private Text effectText;
    private string selectedSlotName;

    private void Awake()
    {
        EnsureUi();
        SelectSlot(OffensiveSlotNames[0]);
    }

    private void OnEnable()
    {
        EnsureUi();

        if (string.IsNullOrEmpty(selectedSlotName))
        {
            SelectSlot(OffensiveSlotNames[0]);
        }
    }

    private void EnsureUi()
    {
        Transform root = EnsureRoot();
        EnsureTitle(root);
        EnsureEquipmentLayout(root);
        EnsureSummaryCard(root);
        EnsureActionButtons(root);
    }

    private Transform EnsureRoot()
    {
        const string objectName = "HeroEquipmentContent";
        Transform existing = transform.Find(objectName);
        GameObject rootObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform));
        rootObject.transform.SetParent(transform, false);

        RectTransform rectTransform = rootObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.04f, 0.04f);
        rectTransform.anchorMax = new Vector2(0.96f, 0.96f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        VerticalLayoutGroup layoutGroup = rootObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = rootObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(8, 8, 6, 6);
        layoutGroup.spacing = 12f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        return rootObject.transform;
    }

    private void EnsureTitle(Transform parent)
    {
        Text titleText = EnsureText(parent, "HeroEquipmentTitleText", "\uC601\uC6C5 \uC7A5\uBE44", 42, TextAnchor.MiddleCenter, 72f);
        titleText.color = new Color(0.93f, 0.96f, 1f, 1f);
    }

    private void EnsureEquipmentLayout(Transform parent)
    {
        Transform row = EnsureHorizontalRow(parent, "HeroEquipmentSlotLayout", 520f);
        EnsureSlotColumn(row, "OffensiveEquipmentSlots", "\uACF5\uACA9 \uC7A5\uBE44", OffensiveSlotNames);
        EnsureHeroPlaceholder(row);
        EnsureSlotColumn(row, "DefensiveEquipmentSlots", "\uBC29\uC5B4 \uC7A5\uBE44", DefensiveSlotNames);
    }

    private void EnsureSlotColumn(Transform parent, string objectName, string title, string[] slotNames)
    {
        Transform column = EnsureVerticalGroup(parent, objectName, 0f);
        LayoutElement layoutElement = EnsureLayoutElement(column.gameObject);
        layoutElement.flexibleWidth = 1f;
        layoutElement.minWidth = 250f;

        Text titleText = EnsureText(column, objectName + "Title", title, 28, TextAnchor.MiddleCenter, 42f);
        titleText.color = new Color(0.77f, 0.86f, 1f, 1f);

        for (int i = 0; i < slotNames.Length; i++)
        {
            EnsureSlotButton(column, slotNames[i]);
        }
    }

    private void EnsureHeroPlaceholder(Transform parent)
    {
        GameObject heroObject = EnsurePanel(parent, "HeroEquipmentHeroPlaceholder", new Color(0.08f, 0.11f, 0.16f, 0.92f));
        LayoutElement layoutElement = EnsureLayoutElement(heroObject);
        layoutElement.minWidth = 250f;
        layoutElement.preferredWidth = 280f;
        layoutElement.minHeight = 500f;
        layoutElement.flexibleWidth = 1.15f;

        VerticalLayoutGroup heroLayoutGroup = heroObject.GetComponent<VerticalLayoutGroup>();
        if (heroLayoutGroup == null)
        {
            heroLayoutGroup = heroObject.AddComponent<VerticalLayoutGroup>();
        }

        heroLayoutGroup.padding = new RectOffset(18, 18, 22, 18);
        heroLayoutGroup.spacing = 14f;
        heroLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        heroLayoutGroup.childControlWidth = true;
        heroLayoutGroup.childControlHeight = true;
        heroLayoutGroup.childForceExpandWidth = true;
        heroLayoutGroup.childForceExpandHeight = false;

        Transform heroTransform = heroObject.transform;
        EnsureText(heroTransform, "HeroPlaceholderTitle", "Hero", 30, TextAnchor.MiddleCenter, 58f);

        GameObject iconObject = EnsurePanel(heroTransform, "HeroPlaceholderIcon", new Color(0.18f, 0.42f, 0.74f, 0.95f));
        LayoutElement iconLayoutElement = EnsureLayoutElement(iconObject);
        iconLayoutElement.minHeight = 300f;
        iconLayoutElement.preferredHeight = 330f;
        iconLayoutElement.flexibleWidth = 1f;

        Text iconText = EnsureText(iconObject.transform, "HeroPlaceholderIconText", "\uC601\uC6C5", 34, TextAnchor.MiddleCenter, 0f);
        iconText.color = Color.white;

        Text captionText = EnsureText(heroTransform, "HeroPlaceholderCaption", "\uC7A5\uBE44 UI placeholder", 24, TextAnchor.MiddleCenter, 52f);
        captionText.color = new Color(0.78f, 0.83f, 0.9f, 1f);
    }

    private void EnsureSlotButton(Transform parent, string slotName)
    {
        GameObject buttonObject = EnsurePanel(parent, "EquipmentSlotButton_" + slotName, new Color(0.14f, 0.17f, 0.23f, 0.96f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 92f;
        layoutElement.preferredHeight = 96f;
        layoutElement.flexibleWidth = 1f;

        HorizontalLayoutGroup rowLayout = buttonObject.GetComponent<HorizontalLayoutGroup>();
        if (rowLayout == null)
        {
            rowLayout = buttonObject.AddComponent<HorizontalLayoutGroup>();
        }

        rowLayout.padding = new RectOffset(12, 12, 10, 10);
        rowLayout.spacing = 10f;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = false;
        rowLayout.childForceExpandHeight = true;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        Image background = buttonObject.GetComponent<Image>();
        button.targetGraphic = background;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectSlot(slotName));

        GameObject iconObject = EnsurePanel(buttonObject.transform, "IconPlaceholder", new Color(0.28f, 0.33f, 0.43f, 1f));
        LayoutElement iconLayout = EnsureLayoutElement(iconObject);
        iconLayout.minWidth = 58f;
        iconLayout.preferredWidth = 64f;
        iconLayout.minHeight = 58f;
        iconLayout.preferredHeight = 64f;

        Text iconText = EnsureText(iconObject.transform, "IconText", "?", 34, TextAnchor.MiddleCenter, 0f);
        iconText.color = new Color(0.9f, 0.94f, 1f, 1f);

        Text slotText = EnsureText(buttonObject.transform, "SlotNameText", slotName, 28, TextAnchor.MiddleLeft, 0f);
        LayoutElement slotTextLayout = EnsureLayoutElement(slotText.gameObject);
        slotTextLayout.flexibleWidth = 1f;
    }

    private void EnsureSummaryCard(Transform parent)
    {
        GameObject cardObject = EnsurePanel(parent, "SelectedEquipmentSummaryCard", new Color(0.08f, 0.1f, 0.14f, 0.96f));
        LayoutElement layoutElement = EnsureLayoutElement(cardObject);
        layoutElement.minHeight = 190f;
        layoutElement.preferredHeight = 210f;
        layoutElement.flexibleWidth = 1f;

        VerticalLayoutGroup layoutGroup = cardObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = cardObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(22, 22, 16, 16);
        layoutGroup.spacing = 8f;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        selectedSlotText = EnsureText(cardObject.transform, "SelectedSlotSummaryText", string.Empty, 30, TextAnchor.MiddleLeft, 46f);
        equipmentNameText = EnsureText(cardObject.transform, "EquipmentNameSummaryText", string.Empty, 28, TextAnchor.MiddleLeft, 42f);
        effectText = EnsureText(cardObject.transform, "EquipmentEffectSummaryText", string.Empty, 26, TextAnchor.MiddleLeft, 52f);
    }

    private void EnsureActionButtons(Transform parent)
    {
        Transform row = EnsureHorizontalRow(parent, "HeroEquipmentActionButtons", 88f);
        EnsureActionButton(row, "DetailPlaceholderButton", "\uC0C1\uC138\uBCF4\uAE30");
        EnsureActionButton(row, "ChangePlaceholderButton", "\uC7A5\uBE44 \uAD50\uCCB4");
        EnsureActionButton(row, "UnequipPlaceholderButton", "\uC7A5\uBE44 \uD574\uC81C");
    }

    private void EnsureActionButton(Transform parent, string objectName, string label)
    {
        GameObject buttonObject = EnsurePanel(parent, objectName, new Color(0.18f, 0.22f, 0.3f, 0.98f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 76f;
        layoutElement.preferredHeight = 86f;
        layoutElement.flexibleWidth = 1f;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Debug.Log(label + " placeholder clicked."));

        EnsureText(buttonObject.transform, "Text", label, 28, TextAnchor.MiddleCenter, 0f);
    }

    private Transform EnsureHorizontalRow(Transform parent, string objectName, float preferredHeight)
    {
        GameObject rowObject = EnsureGroup(parent, objectName, true);
        LayoutElement layoutElement = EnsureLayoutElement(rowObject);
        layoutElement.minHeight = preferredHeight;
        layoutElement.preferredHeight = preferredHeight;
        layoutElement.flexibleWidth = 1f;

        HorizontalLayoutGroup layoutGroup = rowObject.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.padding = new RectOffset(0, 0, 0, 0);
        layoutGroup.spacing = 14f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;

        return rowObject.transform;
    }

    private Transform EnsureVerticalGroup(Transform parent, string objectName, float preferredHeight)
    {
        GameObject groupObject = EnsureGroup(parent, objectName, false);
        LayoutElement layoutElement = EnsureLayoutElement(groupObject);
        if (preferredHeight > 0f)
        {
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
        }

        VerticalLayoutGroup layoutGroup = groupObject.GetComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(0, 0, 0, 0);
        layoutGroup.spacing = 10f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        return groupObject.transform;
    }

    private GameObject EnsureGroup(Transform parent, string objectName, bool horizontal)
    {
        Transform existing = parent.Find(objectName);
        GameObject groupObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform));
        groupObject.transform.SetParent(parent, false);

        RectTransform rectTransform = groupObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        if (horizontal)
        {
            if (groupObject.GetComponent<HorizontalLayoutGroup>() == null)
            {
                groupObject.AddComponent<HorizontalLayoutGroup>();
            }
        }
        else if (groupObject.GetComponent<VerticalLayoutGroup>() == null)
        {
            groupObject.AddComponent<VerticalLayoutGroup>();
        }

        return groupObject;
    }

    private GameObject EnsurePanel(Transform parent, string objectName, Color color)
    {
        Transform existing = parent.Find(objectName);
        GameObject panelObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = true;

        return panelObject;
    }

    private Text EnsureText(Transform parent, string objectName, string textValue, int fontSize, TextAnchor alignment, float preferredHeight)
    {
        Transform existing = parent.Find(objectName);
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        if (preferredHeight > 0f)
        {
            LayoutElement layoutElement = EnsureLayoutElement(textObject);
            layoutElement.minHeight = preferredHeight;
            layoutElement.preferredHeight = preferredHeight;
        }

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 16;
        text.resizeTextMaxSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = textValue;

        return text;
    }

    private LayoutElement EnsureLayoutElement(GameObject target)
    {
        LayoutElement layoutElement = target.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = target.AddComponent<LayoutElement>();
        }

        return layoutElement;
    }

    private void SelectSlot(string slotName)
    {
        selectedSlotName = slotName;

        if (selectedSlotText != null)
        {
            selectedSlotText.text = "\uC120\uD0DD \uC2AC\uB86F: " + slotName;
        }

        if (equipmentNameText != null)
        {
            equipmentNameText.text = "\uC7A5\uBE44\uBA85: \uC7A5\uCC29 \uC7A5\uBE44 \uC5C6\uC74C";
        }

        if (effectText != null)
        {
            effectText.text = "\uD6A8\uACFC: \uB2E4\uC74C \uB2E8\uACC4\uC5D0\uC11C \uD45C\uC2DC \uC608\uC815";
        }
    }
}
