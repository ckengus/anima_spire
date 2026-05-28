using UnityEngine;
using UnityEngine.UI;

public sealed class HeroEquipmentPanelUI : MonoBehaviour
{
    private Transform equipmentRootTarget;
    private EquipmentManager equipmentManager;

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
    private Text weaponSlotUpgradeLevelText;
    private Text weaponSlotUpgradeCostText;
    private Text weaponSlotUpgradeMessageText;
    private GameObject equipmentContentRoot;
    private GameObject weaponSlotUpgradePopupRoot;
    private string selectedSlotName;
    private bool hasReceivedRootTarget;
    private bool isPanelVisible;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        if (!hasReceivedRootTarget)
        {
            return;
        }

        if (isPanelVisible && EnsureUi() && string.IsNullOrEmpty(selectedSlotName))
        {
            SelectSlot(OffensiveSlotNames[0], false);
        }
    }

    public void SetEquipmentRootTarget(Transform target)
    {
        hasReceivedRootTarget = true;
        equipmentRootTarget = target;
        equipmentContentRoot = GetExistingRootObject();
        weaponSlotUpgradePopupRoot = GetExistingWeaponSlotUpgradePopupObject();

        if (equipmentContentRoot != null)
        {
            equipmentContentRoot.SetActive(isPanelVisible);
        }

        HideWeaponSlotUpgradePopup();
    }

    public void SetEquipmentManager(EquipmentManager manager)
    {
        equipmentManager = manager;

        if (weaponSlotUpgradePopupRoot != null)
        {
            RefreshWeaponSlotUpgradePopup();
        }
    }

    public void ShowPanel()
    {
        SetPanelVisible(true);
    }

    public void HidePanel()
    {
        SetPanelVisible(false);
    }

    public void SetPanelVisible(bool visible)
    {
        isPanelVisible = visible;

        if (!visible)
        {
            equipmentContentRoot = GetExistingRootObject();
            if (equipmentContentRoot != null)
            {
                equipmentContentRoot.SetActive(false);
            }

            HideWeaponSlotUpgradePopup();
            return;
        }

        if (!EnsureUi())
        {
            return;
        }

        equipmentContentRoot.SetActive(true);
        HideWeaponSlotUpgradePopup();

        if (string.IsNullOrEmpty(selectedSlotName))
        {
            SelectSlot(OffensiveSlotNames[0], false);
        }
    }

    private bool EnsureUi()
    {
        Transform root = EnsureRoot();
        if (root == null)
        {
            return false;
        }

        Transform equippedEquipmentArea = EnsureEquipmentAreas(root);
        EnsureTitle(equippedEquipmentArea);
        EnsureEquipmentLayout(equippedEquipmentArea);
        EnsureSummaryCard(equippedEquipmentArea);
        EnsureActionButtons(equippedEquipmentArea);

        return true;
    }

    private Transform EnsureRoot()
    {
        if (equipmentRootTarget == null)
        {
            Debug.LogError("HeroEquipmentPanelUI equipmentRootTarget is missing. Assign UI_OverlayCanvas > SafeAreaUIRoot > MainContentArea to MainTabController Equipment Root Target in the Inspector.");
            return null;
        }

        const string objectName = "HeroEquipmentContent";
        Transform existing = GetDirectChild(equipmentRootTarget, objectName);
        GameObject rootObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform));
        rootObject.transform.SetParent(equipmentRootTarget, false);
        equipmentContentRoot = rootObject;

        RectTransform rectTransform = rootObject.GetComponent<RectTransform>();
        StretchToParent(rectTransform);

        VerticalLayoutGroup layoutGroup = rootObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = rootObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(8, 8, 6, 6);
        layoutGroup.spacing = 10f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;
        rootObject.SetActive(isPanelVisible);

        return rootObject.transform;
    }

    private Transform EnsureEquipmentAreas(Transform root)
    {
        Transform equippedEquipmentArea = EnsureEquippedEquipmentArea(root);
        EnsureEquipmentFilterArea(root);
        EnsureOwnedEquipmentArea(root);
        return equippedEquipmentArea;
    }

    private Transform EnsureEquippedEquipmentArea(Transform root)
    {
        GameObject areaObject = EnsureAreaPanel(root, "EquippedEquipmentArea", new Color(0.06f, 0.08f, 0.12f, 0.92f), 4f);

        VerticalLayoutGroup layoutGroup = areaObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = areaObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(10, 10, 8, 8);
        layoutGroup.spacing = 6f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        return areaObject.transform;
    }

    private Transform EnsureEquipmentFilterArea(Transform root)
    {
        GameObject areaObject = EnsureAreaPanel(root, "EquipmentFilterArea", new Color(0.08f, 0.1f, 0.15f, 0.94f), 1f);
        EnsurePlaceholderAreaText(areaObject.transform, "EquipmentFilterPlaceholderText", "\uC7A5\uBE44 \uD544\uD130 \uC601\uC5ED - 031N-2\uC5D0\uC11C \uAD6C\uD604 \uC608\uC815");
        return areaObject.transform;
    }

    private Transform EnsureOwnedEquipmentArea(Transform root)
    {
        GameObject areaObject = EnsureAreaPanel(root, "OwnedEquipmentArea", new Color(0.07f, 0.09f, 0.13f, 0.94f), 5f);
        EnsurePlaceholderAreaText(areaObject.transform, "OwnedEquipmentPlaceholderText", "\uBCF4\uC720 \uC7A5\uBE44 \uC601\uC5ED - 031N-3\uC5D0\uC11C \uAD6C\uD604 \uC608\uC815");
        return areaObject.transform;
    }

    private GameObject EnsureAreaPanel(Transform parent, string objectName, Color color, float flexibleHeight)
    {
        GameObject areaObject = EnsurePanel(parent, objectName, color);
        LayoutElement layoutElement = EnsureLayoutElement(areaObject);
        layoutElement.minHeight = 0f;
        layoutElement.preferredHeight = 0f;
        layoutElement.flexibleWidth = 1f;
        layoutElement.flexibleHeight = flexibleHeight;

        return areaObject;
    }

    private void EnsurePlaceholderAreaText(Transform parent, string objectName, string textValue)
    {
        Text placeholderText = EnsureText(parent, objectName, textValue, 24, TextAnchor.MiddleCenter, 0f);
        placeholderText.color = new Color(0.74f, 0.8f, 0.88f, 1f);
    }

    private GameObject GetExistingRootObject()
    {
        if (equipmentRootTarget == null)
        {
            return null;
        }

        const string objectName = "HeroEquipmentContent";
        Transform existing = GetDirectChild(equipmentRootTarget, objectName);
        return existing != null ? existing.gameObject : null;
    }

    private GameObject GetExistingWeaponSlotUpgradePopupObject()
    {
        if (equipmentRootTarget == null)
        {
            return null;
        }

        const string objectName = "WeaponSlotUpgradePopup";
        Transform existing = GetDirectChild(equipmentRootTarget, objectName);
        return existing != null ? existing.gameObject : null;
    }

    private void EnsureTitle(Transform parent)
    {
        Text titleText = EnsureText(parent, "HeroEquipmentTitleText", "\uC601\uC6C5 \uC7A5\uBE44", 32, TextAnchor.MiddleCenter, 42f);
        titleText.color = new Color(0.93f, 0.96f, 1f, 1f);
    }

    private void EnsureEquipmentLayout(Transform parent)
    {
        Transform row = EnsureHorizontalRow(parent, "HeroEquipmentSlotLayout", 0f);
        LayoutElement layoutElement = EnsureLayoutElement(row.gameObject);
        layoutElement.minHeight = 0f;
        layoutElement.preferredHeight = 0f;
        layoutElement.flexibleHeight = 1f;

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

        Text titleText = EnsureText(column, objectName + "Title", title, 22, TextAnchor.MiddleCenter, 28f);
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
        layoutElement.minWidth = 180f;
        layoutElement.preferredWidth = 220f;
        layoutElement.minHeight = 0f;
        layoutElement.flexibleWidth = 1.15f;

        VerticalLayoutGroup heroLayoutGroup = heroObject.GetComponent<VerticalLayoutGroup>();
        if (heroLayoutGroup == null)
        {
            heroLayoutGroup = heroObject.AddComponent<VerticalLayoutGroup>();
        }

        heroLayoutGroup.padding = new RectOffset(12, 12, 10, 10);
        heroLayoutGroup.spacing = 6f;
        heroLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        heroLayoutGroup.childControlWidth = true;
        heroLayoutGroup.childControlHeight = true;
        heroLayoutGroup.childForceExpandWidth = true;
        heroLayoutGroup.childForceExpandHeight = false;

        Transform heroTransform = heroObject.transform;
        EnsureText(heroTransform, "HeroPlaceholderTitle", "Hero", 24, TextAnchor.MiddleCenter, 34f);

        GameObject iconObject = EnsurePanel(heroTransform, "HeroPlaceholderIcon", new Color(0.18f, 0.42f, 0.74f, 0.95f));
        LayoutElement iconLayoutElement = EnsureLayoutElement(iconObject);
        iconLayoutElement.minHeight = 0f;
        iconLayoutElement.preferredHeight = 0f;
        iconLayoutElement.flexibleWidth = 1f;
        iconLayoutElement.flexibleHeight = 1f;

        Text iconText = EnsureText(iconObject.transform, "HeroPlaceholderIconText", "\uC601\uC6C5", 28, TextAnchor.MiddleCenter, 0f);
        iconText.color = Color.white;

        Text captionText = EnsureText(heroTransform, "HeroPlaceholderCaption", "\uC7A5\uBE44 UI placeholder", 18, TextAnchor.MiddleCenter, 30f);
        captionText.color = new Color(0.78f, 0.83f, 0.9f, 1f);
    }

    private void EnsureSlotButton(Transform parent, string slotName)
    {
        GameObject buttonObject = EnsurePanel(parent, "EquipmentSlotButton_" + slotName, new Color(0.14f, 0.17f, 0.23f, 0.96f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 40f;
        layoutElement.preferredHeight = 46f;
        layoutElement.flexibleWidth = 1f;

        HorizontalLayoutGroup rowLayout = buttonObject.GetComponent<HorizontalLayoutGroup>();
        if (rowLayout == null)
        {
            rowLayout = buttonObject.AddComponent<HorizontalLayoutGroup>();
        }

        rowLayout.padding = new RectOffset(8, 8, 6, 6);
        rowLayout.spacing = 8f;
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
        iconLayout.minWidth = 36f;
        iconLayout.preferredWidth = 40f;
        iconLayout.minHeight = 32f;
        iconLayout.preferredHeight = 36f;

        Text iconText = EnsureText(iconObject.transform, "IconText", "?", 22, TextAnchor.MiddleCenter, 0f);
        iconText.color = new Color(0.9f, 0.94f, 1f, 1f);

        Text slotText = EnsureText(buttonObject.transform, "SlotNameText", slotName, 22, TextAnchor.MiddleLeft, 0f);
        LayoutElement slotTextLayout = EnsureLayoutElement(slotText.gameObject);
        slotTextLayout.flexibleWidth = 1f;
    }

    private void EnsureSummaryCard(Transform parent)
    {
        GameObject cardObject = EnsurePanel(parent, "SelectedEquipmentSummaryCard", new Color(0.08f, 0.1f, 0.14f, 0.96f));
        LayoutElement layoutElement = EnsureLayoutElement(cardObject);
        layoutElement.minHeight = 76f;
        layoutElement.preferredHeight = 84f;
        layoutElement.flexibleWidth = 1f;

        VerticalLayoutGroup layoutGroup = cardObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = cardObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(14, 14, 8, 8);
        layoutGroup.spacing = 3f;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        selectedSlotText = EnsureText(cardObject.transform, "SelectedSlotSummaryText", string.Empty, 22, TextAnchor.MiddleLeft, 24f);
        equipmentNameText = EnsureText(cardObject.transform, "EquipmentNameSummaryText", string.Empty, 20, TextAnchor.MiddleLeft, 22f);
        effectText = EnsureText(cardObject.transform, "EquipmentEffectSummaryText", string.Empty, 20, TextAnchor.MiddleLeft, 24f);
    }

    private void EnsureActionButtons(Transform parent)
    {
        Transform row = EnsureHorizontalRow(parent, "HeroEquipmentActionButtons", 54f);
        EnsureActionButton(row, "DetailPlaceholderButton", "\uC0C1\uC138\uBCF4\uAE30");
        EnsureActionButton(row, "ChangePlaceholderButton", "\uC7A5\uBE44 \uAD50\uCCB4");
        EnsureActionButton(row, "UnequipPlaceholderButton", "\uC7A5\uBE44 \uD574\uC81C");
    }

    private void EnsureActionButton(Transform parent, string objectName, string label)
    {
        GameObject buttonObject = EnsurePanel(parent, objectName, new Color(0.18f, 0.22f, 0.3f, 0.98f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 44f;
        layoutElement.preferredHeight = 52f;
        layoutElement.flexibleWidth = 1f;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Debug.Log(label + " placeholder clicked."));

        EnsureText(buttonObject.transform, "Text", label, 22, TextAnchor.MiddleCenter, 0f);
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
        Transform existing = GetDirectChild(parent, objectName);
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
        Transform existing = GetDirectChild(parent, objectName);
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
        Transform existing = GetDirectChild(parent, objectName);
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

    private bool EnsureWeaponSlotUpgradePopup()
    {
        if (equipmentRootTarget == null)
        {
            Debug.LogError("HeroEquipmentPanelUI equipmentRootTarget is missing. WeaponSlotUpgradePopup cannot be created.");
            return false;
        }

        const string objectName = "WeaponSlotUpgradePopup";
        Transform existing = GetDirectChild(equipmentRootTarget, objectName);
        bool wasActive = existing != null && existing.gameObject.activeSelf;
        weaponSlotUpgradePopupRoot = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        weaponSlotUpgradePopupRoot.transform.SetParent(equipmentRootTarget, false);

        RectTransform popupRect = weaponSlotUpgradePopupRoot.GetComponent<RectTransform>();
        StretchToParent(popupRect);

        Image dimImage = weaponSlotUpgradePopupRoot.GetComponent<Image>();
        if (dimImage == null)
        {
            dimImage = weaponSlotUpgradePopupRoot.AddComponent<Image>();
        }

        dimImage.color = new Color(0f, 0f, 0f, 0.58f);
        dimImage.raycastTarget = true;

        GameObject cardObject = EnsurePanel(weaponSlotUpgradePopupRoot.transform, "WeaponSlotUpgradeCard", new Color(0.09f, 0.12f, 0.17f, 0.98f));
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        cardRect.localScale = Vector3.one;
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = new Vector2(720f, 520f);
        cardRect.offsetMin = new Vector2(-360f, -260f);
        cardRect.offsetMax = new Vector2(360f, 260f);

        VerticalLayoutGroup cardLayout = cardObject.GetComponent<VerticalLayoutGroup>();
        if (cardLayout == null)
        {
            cardLayout = cardObject.AddComponent<VerticalLayoutGroup>();
        }

        cardLayout.padding = new RectOffset(34, 34, 30, 28);
        cardLayout.spacing = 14f;
        cardLayout.childAlignment = TextAnchor.UpperCenter;
        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = true;
        cardLayout.childForceExpandHeight = false;

        Text titleText = EnsureText(cardObject.transform, "WeaponSlotUpgradeTitleText", "\uBB34\uAE30 \uC2AC\uB86F \uAC15\uD654", 34, TextAnchor.MiddleCenter, 62f);
        titleText.color = new Color(0.94f, 0.97f, 1f, 1f);

        weaponSlotUpgradeLevelText = EnsureText(cardObject.transform, "WeaponSlotUpgradeLevelText", "\uD604\uC7AC \uB808\uBCA8: Lv. 0", 28, TextAnchor.MiddleCenter, 50f);
        weaponSlotUpgradeLevelText.color = new Color(0.84f, 0.9f, 1f, 1f);

        weaponSlotUpgradeCostText = EnsureText(cardObject.transform, "WeaponSlotUpgradeCostText", "\uAC15\uD654 \uBE44\uC6A9: 10 Gold", 28, TextAnchor.MiddleCenter, 50f);
        weaponSlotUpgradeCostText.color = new Color(1f, 0.91f, 0.62f, 1f);

        Text descriptionText = EnsureText(cardObject.transform, "WeaponSlotUpgradeDescriptionText", "\uC2E4\uC81C \uAC15\uD654 \uB85C\uC9C1\uC740 031M\uC5D0\uC11C \uC5F0\uACB0 \uC608\uC815", 24, TextAnchor.MiddleCenter, 72f);
        descriptionText.color = new Color(0.75f, 0.8f, 0.88f, 1f);

        weaponSlotUpgradeMessageText = EnsureText(cardObject.transform, "WeaponSlotUpgradeMessageText", string.Empty, 24, TextAnchor.MiddleCenter, 48f);
        weaponSlotUpgradeMessageText.color = new Color(0.8f, 0.88f, 1f, 1f);

        Transform buttonRow = EnsureHorizontalRow(cardObject.transform, "WeaponSlotUpgradeButtonRow", 86f);
        EnsureWeaponSlotUpgradeButton(buttonRow);
        EnsureWeaponSlotUpgradeCloseButton(buttonRow);

        if (weaponSlotUpgradeLevelText == null || weaponSlotUpgradeCostText == null || weaponSlotUpgradeMessageText == null)
        {
            Debug.LogError("HeroEquipmentPanelUI failed to cache WeaponSlotUpgradePopup text components.");
            return false;
        }

        weaponSlotUpgradePopupRoot.SetActive(wasActive);
        return true;
    }

    private void EnsureWeaponSlotUpgradeButton(Transform parent)
    {
        GameObject buttonObject = EnsurePanel(parent, "WeaponSlotUpgradeButton", new Color(0.25f, 0.48f, 0.84f, 0.98f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 76f;
        layoutElement.preferredHeight = 82f;
        layoutElement.flexibleWidth = 1f;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HandleWeaponSlotUpgradeButtonClicked);

        EnsureText(buttonObject.transform, "Text", "\uAC15\uD654", 28, TextAnchor.MiddleCenter, 0f);
    }

    private void EnsureWeaponSlotUpgradeCloseButton(Transform parent)
    {
        GameObject buttonObject = EnsurePanel(parent, "WeaponSlotUpgradeCloseButton", new Color(0.18f, 0.21f, 0.28f, 0.98f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 76f;
        layoutElement.preferredHeight = 82f;
        layoutElement.flexibleWidth = 1f;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HideWeaponSlotUpgradePopup);

        EnsureText(buttonObject.transform, "Text", "\uB2EB\uAE30", 28, TextAnchor.MiddleCenter, 0f);
    }

    private void ShowWeaponSlotUpgradePopup()
    {
        if (!EnsureWeaponSlotUpgradePopup())
        {
            return;
        }

        ClearWeaponSlotUpgradeMessage();
        RefreshWeaponSlotUpgradePopup();
        weaponSlotUpgradePopupRoot.SetActive(true);
        weaponSlotUpgradePopupRoot.transform.SetAsLastSibling();
    }

    private void HideWeaponSlotUpgradePopup()
    {
        if (weaponSlotUpgradePopupRoot == null)
        {
            weaponSlotUpgradePopupRoot = GetExistingWeaponSlotUpgradePopupObject();
        }

        if (weaponSlotUpgradePopupRoot != null)
        {
            weaponSlotUpgradePopupRoot.SetActive(false);
        }

        ClearWeaponSlotUpgradeMessage();
    }

    private bool IsWeaponSlot(string slotName)
    {
        return slotName == OffensiveSlotNames[0];
    }

    private void RefreshWeaponSlotUpgradePopup()
    {
        if (weaponSlotUpgradeLevelText == null || weaponSlotUpgradeCostText == null || weaponSlotUpgradeMessageText == null)
        {
            if (!EnsureWeaponSlotUpgradePopup())
            {
                return;
            }
        }

        if (equipmentManager == null)
        {
            weaponSlotUpgradeLevelText.text = "\uD604\uC7AC \uB808\uBCA8: -";
            weaponSlotUpgradeCostText.text = "\uAC15\uD654 \uBE44\uC6A9: -";
            weaponSlotUpgradeMessageText.text = "\uC7A5\uBE44 \uC2DC\uC2A4\uD15C \uC900\uBE44 \uC911";
            weaponSlotUpgradeMessageText.color = new Color(1f, 0.78f, 0.45f, 1f);
            return;
        }

        int level = equipmentManager.WeaponSlotLevel;
        int cost = equipmentManager.GetWeaponSlotUpgradeCost(level);
        weaponSlotUpgradeLevelText.text = "\uD604\uC7AC \uB808\uBCA8: Lv. " + level;
        weaponSlotUpgradeCostText.text = "\uAC15\uD654 \uBE44\uC6A9: " + cost + " Gold";
    }

    private void HandleWeaponSlotUpgradeButtonClicked()
    {
        if (!EnsureWeaponSlotUpgradePopup())
        {
            return;
        }

        if (equipmentManager == null)
        {
            weaponSlotUpgradeMessageText.text = "\uC7A5\uBE44 \uC2DC\uC2A4\uD15C \uC900\uBE44 \uC911";
            weaponSlotUpgradeMessageText.color = new Color(1f, 0.78f, 0.45f, 1f);
            RefreshWeaponSlotUpgradePopup();
            return;
        }

        bool succeeded = equipmentManager.TryUpgradeWeaponSlot(out string message);
        weaponSlotUpgradeMessageText.text = message;
        weaponSlotUpgradeMessageText.color = succeeded
            ? new Color(0.48f, 0.9f, 0.6f, 1f)
            : new Color(1f, 0.68f, 0.6f, 1f);
        RefreshWeaponSlotUpgradePopup();
    }

    private void ClearWeaponSlotUpgradeMessage()
    {
        if (weaponSlotUpgradeMessageText == null)
        {
            return;
        }

        weaponSlotUpgradeMessageText.text = string.Empty;
        weaponSlotUpgradeMessageText.color = new Color(0.8f, 0.88f, 1f, 1f);
    }

    private Transform GetDirectChild(Transform parent, string childName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    private void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
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

    private void SelectSlot(string slotName, bool allowPopup = true)
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

        if (!allowPopup)
        {
            HideWeaponSlotUpgradePopup();
            return;
        }

        if (IsWeaponSlot(slotName))
        {
            ShowWeaponSlotUpgradePopup();
        }
        else
        {
            HideWeaponSlotUpgradePopup();
        }
    }
}
