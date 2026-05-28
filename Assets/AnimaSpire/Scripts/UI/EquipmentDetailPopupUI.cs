using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class EquipmentDetailPopupUI : MonoBehaviour
{
    private Text nameText;
    private Text categoryText;
    private Text tierText;
    private Text ownedStateText;
    private Text tierCountText;
    private Text baseStatText;
    private Text descriptionText;
    private Button equipButton;
    private Button unequipButton;
    private UnityAction onClose;

    public void ShowPopup(
        EquipmentDefinition definition,
        Dictionary<EquipmentTier, int> tierCounts,
        bool isOwned,
        bool isEquipped,
        bool canEquip,
        bool canUnequip,
        UnityAction onEquip,
        UnityAction onUnequip,
        UnityAction onClose)
    {
        EnsureUi();
        this.onClose = onClose;
        RefreshContent(definition, tierCounts, isOwned, isEquipped, canEquip, canUnequip, onEquip, onUnequip);
        SetVisible(true);
        transform.SetAsLastSibling();
    }

    public void HidePopup()
    {
        SetVisible(false);
        onClose?.Invoke();
        onClose = null;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    private void EnsureUi()
    {
        RectTransform rootRect = GetComponent<RectTransform>();
        if (rootRect == null)
        {
            rootRect = gameObject.AddComponent<RectTransform>();
        }

        StretchToParent(rootRect);

        GameObject dimObject = EnsurePanel(transform, "DimBackground", new Color(0f, 0f, 0f, 0.64f));
        StretchToParent(dimObject.GetComponent<RectTransform>());
        Button dimButton = dimObject.GetComponent<Button>();
        if (dimButton == null)
        {
            dimButton = dimObject.AddComponent<Button>();
        }

        dimButton.targetGraphic = dimObject.GetComponent<Image>();
        dimButton.onClick.RemoveAllListeners();
        dimButton.onClick.AddListener(HidePopup);

        GameObject cardObject = EnsurePanel(transform, "EquipmentDetailCard", new Color(0.08f, 0.11f, 0.16f, 0.98f));
        RectTransform cardRect = cardObject.GetComponent<RectTransform>();
        cardRect.localScale = Vector3.one;
        cardRect.anchorMin = new Vector2(0.08f, 0.08f);
        cardRect.anchorMax = new Vector2(0.92f, 0.92f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.sizeDelta = Vector2.zero;
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;

        VerticalLayoutGroup cardLayout = cardObject.GetComponent<VerticalLayoutGroup>();
        if (cardLayout == null)
        {
            cardLayout = cardObject.AddComponent<VerticalLayoutGroup>();
        }

        cardLayout.padding = new RectOffset(24, 24, 20, 20);
        cardLayout.spacing = 12f;
        cardLayout.childAlignment = TextAnchor.UpperCenter;
        cardLayout.childControlWidth = true;
        cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = true;
        cardLayout.childForceExpandHeight = false;

        EnsureHeader(cardObject.transform);
        EnsureInfoArea(cardObject.transform);
        EnsureButtonArea(cardObject.transform);
    }

    private void EnsureHeader(Transform parent)
    {
        GameObject headerObject = EnsureTransparentPanel(parent, "HeaderArea", false);
        LayoutElement headerLayout = EnsureLayoutElement(headerObject);
        headerLayout.minHeight = 54f;
        headerLayout.preferredHeight = 60f;
        headerLayout.flexibleWidth = 1f;
        headerLayout.flexibleHeight = 0f;

        HorizontalLayoutGroup headerGroup = headerObject.GetComponent<HorizontalLayoutGroup>();
        if (headerGroup == null)
        {
            headerGroup = headerObject.AddComponent<HorizontalLayoutGroup>();
        }

        headerGroup.padding = new RectOffset(0, 0, 0, 0);
        headerGroup.spacing = 12f;
        headerGroup.childAlignment = TextAnchor.MiddleCenter;
        headerGroup.childControlWidth = true;
        headerGroup.childControlHeight = true;
        headerGroup.childForceExpandWidth = false;
        headerGroup.childForceExpandHeight = true;

        Text titleText = EnsureText(headerObject.transform, "TitleText", "\uC7A5\uBE44 \uC0C1\uC138", 30, TextAnchor.MiddleLeft, 0f);
        LayoutElement titleLayout = EnsureLayoutElement(titleText.gameObject);
        titleLayout.flexibleWidth = 1f;
        titleText.color = new Color(0.94f, 0.97f, 1f, 1f);

        GameObject closeObject = EnsurePanel(headerObject.transform, "CloseButton", new Color(0.18f, 0.22f, 0.3f, 0.98f));
        LayoutElement closeLayout = EnsureLayoutElement(closeObject);
        closeLayout.minWidth = 54f;
        closeLayout.preferredWidth = 58f;
        closeLayout.minHeight = 46f;
        closeLayout.preferredHeight = 52f;
        closeLayout.flexibleWidth = 0f;

        Button closeButton = closeObject.GetComponent<Button>();
        if (closeButton == null)
        {
            closeButton = closeObject.AddComponent<Button>();
        }

        closeButton.targetGraphic = closeObject.GetComponent<Image>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(HidePopup);

        Text closeText = EnsureText(closeObject.transform, "Text", "X", 26, TextAnchor.MiddleCenter, 0f);
        closeText.color = Color.white;
    }

    private void EnsureInfoArea(Transform parent)
    {
        GameObject scrollObject = EnsureTransparentPanel(parent, "InfoScrollRect", true);
        LayoutElement scrollLayout = EnsureLayoutElement(scrollObject);
        scrollLayout.minHeight = 0f;
        scrollLayout.preferredHeight = 0f;
        scrollLayout.flexibleWidth = 1f;
        scrollLayout.flexibleHeight = 1f;

        ScrollRect scrollRect = scrollObject.GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            scrollRect = scrollObject.AddComponent<ScrollRect>();
        }

        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.horizontalScrollbar = null;
        scrollRect.verticalScrollbar = null;

        GameObject viewportObject = EnsureTransparentPanel(scrollObject.transform, "Viewport", true);
        StretchToParent(viewportObject.GetComponent<RectTransform>());
        if (viewportObject.GetComponent<RectMask2D>() == null)
        {
            viewportObject.AddComponent<RectMask2D>();
        }

        GameObject contentObject = EnsureTransparentPanel(viewportObject.transform, "Content", false);
        RectTransform contentRect = contentObject.GetComponent<RectTransform>();
        contentRect.localScale = Vector3.one;
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        VerticalLayoutGroup contentGroup = contentObject.GetComponent<VerticalLayoutGroup>();
        if (contentGroup == null)
        {
            contentGroup = contentObject.AddComponent<VerticalLayoutGroup>();
        }

        contentGroup.padding = new RectOffset(4, 4, 4, 4);
        contentGroup.spacing = 10f;
        contentGroup.childAlignment = TextAnchor.UpperLeft;
        contentGroup.childControlWidth = true;
        contentGroup.childControlHeight = true;
        contentGroup.childForceExpandWidth = true;
        contentGroup.childForceExpandHeight = false;

        ContentSizeFitter fitter = contentObject.GetComponent<ContentSizeFitter>();
        if (fitter == null)
        {
            fitter = contentObject.AddComponent<ContentSizeFitter>();
        }

        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewportObject.GetComponent<RectTransform>();
        scrollRect.content = contentRect;

        nameText = EnsureInfoText(contentObject.transform, "NameText");
        categoryText = EnsureInfoText(contentObject.transform, "CategoryText");
        tierText = EnsureInfoText(contentObject.transform, "TierText");
        ownedStateText = EnsureInfoText(contentObject.transform, "OwnedStateText");
        tierCountText = EnsureInfoText(contentObject.transform, "TierCountText");
        baseStatText = EnsureInfoText(contentObject.transform, "BaseStatText");
        descriptionText = EnsureInfoText(contentObject.transform, "DescriptionText");
        EnsureInfoText(contentObject.transform, "FixedOptionText").text = "\uACE0\uC815\uC635\uC158: \uD6C4\uC18D \uAD6C\uD604 \uC608\uC815";
        EnsureInfoText(contentObject.transform, "SelectableOptionText").text = "\uC120\uD0DD\uC635\uC158: \uD6C4\uC18D \uAD6C\uD604 \uC608\uC815";
        EnsureInfoText(contentObject.transform, "TierUpgradeText").text = "\uD2F0\uC5B4 \uC218\uB7C9 / \uC2B9\uAE09: \uD6C4\uC18D \uAD6C\uD604 \uC608\uC815";
        EnsureInfoText(contentObject.transform, "BatchSynthesisText").text = "\uC77C\uAD04\uD569\uC131: \uD6C4\uC18D \uAD6C\uD604 \uC608\uC815";
        EnsureInfoText(contentObject.transform, "SlotUpgradeText").text = "\uC2AC\uB86F\uAC15\uD654 \uC601\uC5ED: \uD6C4\uC18D \uAD6C\uD604 \uC608\uC815";
    }

    private void EnsureButtonArea(Transform parent)
    {
        GameObject buttonAreaObject = EnsurePanel(parent, "ButtonArea", new Color(0.06f, 0.08f, 0.12f, 0.98f));
        LayoutElement areaLayout = EnsureLayoutElement(buttonAreaObject);
        areaLayout.minHeight = 86f;
        areaLayout.preferredHeight = 96f;
        areaLayout.flexibleWidth = 1f;
        areaLayout.flexibleHeight = 0f;

        HorizontalLayoutGroup rowGroup = buttonAreaObject.GetComponent<HorizontalLayoutGroup>();
        if (rowGroup == null)
        {
            rowGroup = buttonAreaObject.AddComponent<HorizontalLayoutGroup>();
        }

        rowGroup.padding = new RectOffset(14, 14, 14, 14);
        rowGroup.spacing = 12f;
        rowGroup.childAlignment = TextAnchor.MiddleCenter;
        rowGroup.childControlWidth = true;
        rowGroup.childControlHeight = true;
        rowGroup.childForceExpandWidth = true;
        rowGroup.childForceExpandHeight = true;

        equipButton = EnsureActionButton(buttonAreaObject.transform, "EquipButton", "\uCC29\uC6A9");
        unequipButton = EnsureActionButton(buttonAreaObject.transform, "UnequipButton", "\uD574\uC81C");
    }

    private void RefreshContent(
        EquipmentDefinition definition,
        Dictionary<EquipmentTier, int> tierCounts,
        bool isOwned,
        bool isEquipped,
        bool canEquip,
        bool canUnequip,
        UnityAction onEquip,
        UnityAction onUnequip)
    {
        if (definition == null)
        {
            nameText.text = "\uC7A5\uBE44\uBA85: -";
            categoryText.text = "\uCE74\uD14C\uACE0\uB9AC: -";
            tierText.text = "\uB300\uD45C \uD2F0\uC5B4: -";
            ownedStateText.text = "\uD68D\uB4DD \uC0C1\uD0DC: -";
            tierCountText.text = "\uD2F0\uC5B4\uBCC4 \uBCF4\uC720 \uC218\uB7C9: \uBCF4\uC720 \uC218\uB7C9 \uC5C6\uC74C";
            baseStatText.text = "\uAE30\uBCF8 \uC2A4\uD0EF: \uD6C4\uC18D \uAD6C\uD604 \uC608\uC815";
            descriptionText.text = "\uC124\uBA85: -";
            ConfigureButton(equipButton, false, onEquip);
            ConfigureButton(unequipButton, false, onUnequip);
            return;
        }

        nameText.text = "\uC7A5\uBE44\uBA85: " + definition.displayName;
        categoryText.text = "\uCE74\uD14C\uACE0\uB9AC: " + definition.category;
        tierText.text = "\uB300\uD45C \uD2F0\uC5B4: " + definition.tier;
        ownedStateText.text = "\uD68D\uB4DD \uC0C1\uD0DC: " + (isOwned ? "\uD68D\uB4DD" : "\uBBF8\uD68D\uB4DD");
        tierCountText.text = "\uD2F0\uC5B4\uBCC4 \uBCF4\uC720 \uC218\uB7C9: " + BuildTierCountSummary(tierCounts);
        baseStatText.text = "\uAE30\uBCF8 \uC2A4\uD0EF: \uACF5\uACA9\uB825 +" + definition.bonusAttackPower + " / \uC2A4\uD0AC \uD53C\uD574 +" + definition.bonusSkillDamage;
        descriptionText.text = "\uC124\uBA85: " + (string.IsNullOrEmpty(definition.description) ? "-" : definition.description);

        ConfigureButton(equipButton, canEquip, onEquip);
        ConfigureButton(unequipButton, canUnequip, onUnequip);
    }

    private void ConfigureButton(Button button, bool interactable, UnityAction action)
    {
        button.interactable = interactable;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Debug.Log("031S-2\uC5D0\uC11C \uAD6C\uD604 \uC608\uC815"));
        if (action != null)
        {
            button.onClick.AddListener(action);
        }

        Image image = button.targetGraphic as Image;
        if (image != null)
        {
            image.color = interactable
                ? new Color(0.25f, 0.48f, 0.84f, 0.98f)
                : new Color(0.16f, 0.18f, 0.23f, 0.98f);
        }
    }

    private string BuildTierCountSummary(Dictionary<EquipmentTier, int> tierCounts)
    {
        if (tierCounts == null || tierCounts.Count == 0)
        {
            return "\uBCF4\uC720 \uC218\uB7C9 \uC5C6\uC74C";
        }

        string summary = string.Empty;
        EquipmentTier[] tiers = (EquipmentTier[])System.Enum.GetValues(typeof(EquipmentTier));
        for (int i = 0; i < tiers.Length; i++)
        {
            EquipmentTier tier = tiers[i];
            if (!tierCounts.TryGetValue(tier, out int count) || count <= 0)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(summary))
            {
                summary += " / ";
            }

            summary += tier + " x" + count;
        }

        return string.IsNullOrEmpty(summary) ? "\uBCF4\uC720 \uC218\uB7C9 \uC5C6\uC74C" : summary;
    }

    private Text EnsureInfoText(Transform parent, string objectName)
    {
        Text text = EnsureText(parent, objectName, string.Empty, 22, TextAnchor.MiddleLeft, 42f);
        text.color = new Color(0.84f, 0.89f, 0.96f, 1f);
        return text;
    }

    private Button EnsureActionButton(Transform parent, string objectName, string label)
    {
        GameObject buttonObject = EnsurePanel(parent, objectName, new Color(0.16f, 0.18f, 0.23f, 0.98f));
        LayoutElement layoutElement = EnsureLayoutElement(buttonObject);
        layoutElement.minHeight = 54f;
        layoutElement.preferredHeight = 64f;
        layoutElement.flexibleWidth = 1f;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.targetGraphic = buttonObject.GetComponent<Image>();
        EnsureText(buttonObject.transform, "Text", label, 24, TextAnchor.MiddleCenter, 0f);
        return button;
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

    private GameObject EnsureTransparentPanel(Transform parent, string objectName, bool raycastTarget)
    {
        GameObject panelObject = EnsurePanel(parent, objectName, new Color(1f, 1f, 1f, 0f));
        Image image = panelObject.GetComponent<Image>();
        image.raycastTarget = raycastTarget;
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
        text.resizeTextMinSize = 14;
        text.resizeTextMaxSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = textValue;

        return text;
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
}
