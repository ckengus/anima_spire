using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainTabController : MonoBehaviour
{
    private const float CombatAreaRatio = 0.5f;
    private const float CombatFocusViewportY = 0.65f;
    private const float BottomMenuRatio = 0.1f;
    private const float HudHeight = 88f;
    private const float HudHorizontalPadding = 28f;
    private const float HudSafeAreaPadding = 20f;
    private const float TabContentPadding = 28f;
    private const float TabContentHudGap = 16f;

    [SerializeField] private GameObject headerPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject bottomMenuPanel;
    [SerializeField] private GameObject tabContentPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private Transform equipmentRootTarget;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private ProgressSaveManager progressSaveManager;

    private RectTransform combatHudRectTransform;
    private RectTransform equipmentPanelRectTransform;
    private HeroEquipmentPanelUI equipmentPanelController;

    private void Awake()
    {
        EnsureReferences();
        EnsureEventSystem();
        EnsureEquipmentManager();
        EnsureThreeAreaLayout();
        EnsureCombatCameraFraming();
        EnsureCombatHud();
        EnsureTabContentPanel();
        EnsureEquipmentPanel();
        EnsureBottomMenuButtons();
        ShowBattle();
    }

    private void OnRectTransformDimensionsChange()
    {
        ApplyHudSafeAreaOffset();
        ApplyTabContentSafeAreaOffset();
    }

    public void ShowBattle()
    {
        SetActiveIfPresent(combatPanel, true);
        SetActiveIfPresent(infoPanel, true);
        SetActiveIfPresent(tabContentPanel, false);
        SetActiveIfPresent(equipmentPanel, false);
        equipmentPanelController?.HidePanel();
    }

    public void ShowEquipment()
    {
        SetActiveIfPresent(combatPanel, true);
        SetActiveIfPresent(infoPanel, true);
        SetActiveIfPresent(tabContentPanel, true);
        SetActiveIfPresent(equipmentPanel, true);
        equipmentPanelController?.ShowPanel();
        SetBottomMenuAsLastSibling();
    }

    private void EnsureReferences()
    {
        headerPanel = FindPanelIfMissing(headerPanel, "HeaderPanel");
        combatPanel = FindPanelIfMissing(combatPanel, "CombatPanel");
        infoPanel = FindPanelIfMissing(infoPanel, "InfoPanel");
        bottomMenuPanel = FindPanelIfMissing(bottomMenuPanel, "BottomMenuPanel");
        tabContentPanel = FindPanelIfMissing(tabContentPanel, "TabContentPanel");
        equipmentPanel = FindPanelIfMissing(equipmentPanel, "EquipmentPanel");

        if (progressSaveManager == null)
        {
            progressSaveManager = FindAnyObjectByType<ProgressSaveManager>();
        }
    }

    private GameObject FindPanelIfMissing(GameObject panel, string panelName)
    {
        if (panel != null)
        {
            return panel;
        }

        Transform found = transform.Find(panelName);
        return found != null ? found.gameObject : null;
    }

    private void EnsureThreeAreaLayout()
    {
        float middleMin = BottomMenuRatio;
        float middleMax = 1f - CombatAreaRatio;

        ApplyAnchors(combatPanel, new Vector2(0f, middleMax), Vector2.one);
        ApplyAnchors(infoPanel, new Vector2(0f, middleMin), new Vector2(1f, middleMax));
        ApplyAnchors(bottomMenuPanel, Vector2.zero, new Vector2(1f, BottomMenuRatio));

        if (headerPanel != null)
        {
            headerPanel.SetActive(false);
        }
    }

    private void ApplyAnchors(GameObject target, Vector2 anchorMin, Vector2 anchorMax)
    {
        if (target == null || !target.TryGetComponent(out RectTransform rectTransform))
        {
            return;
        }

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }

    private void EnsureCombatCameraFraming()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        if (!mainCamera.TryGetComponent(out CombatCameraFramingController controller))
        {
            controller = mainCamera.gameObject.AddComponent<CombatCameraFramingController>();
        }

        controller.Initialize(mainCamera, CombatAreaRatio, CombatFocusViewportY);
    }

    private void EnsureCombatHud()
    {
        if (combatPanel == null)
        {
            return;
        }

        const string hudName = "TopCombatHud";
        Transform existing = combatPanel.transform.Find(hudName);
        GameObject hudObject = existing != null ? existing.gameObject : new GameObject(hudName, typeof(RectTransform));
        hudObject.transform.SetParent(combatPanel.transform, false);
        hudObject.transform.SetAsLastSibling();

        combatHudRectTransform = hudObject.GetComponent<RectTransform>();
        combatHudRectTransform.anchorMin = new Vector2(0f, 1f);
        combatHudRectTransform.anchorMax = Vector2.one;
        combatHudRectTransform.pivot = new Vector2(0.5f, 1f);
        combatHudRectTransform.sizeDelta = new Vector2(0f, HudHeight);

        HorizontalLayoutGroup layoutGroup = hudObject.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = hudObject.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(0, 0, 0, 0);
        layoutGroup.spacing = 12f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = true;

        EnsureStatusHudCard(hudObject.transform);
        EnsureDebugResetButton(hudObject.transform);
        ApplyHudSafeAreaOffset();
    }

    private void EnsureStatusHudCard(Transform hudTransform)
    {
        const string objectName = "StatusHudCard";
        Transform existing = hudTransform.Find(objectName);
        GameObject cardObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cardObject.transform.SetParent(hudTransform, false);

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        LayoutElement layoutElement = cardObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = cardObject.AddComponent<LayoutElement>();
        }

        layoutElement.minHeight = HudHeight;
        layoutElement.preferredHeight = HudHeight;
        layoutElement.flexibleWidth = 1f;

        Image image = cardObject.GetComponent<Image>();
        image.color = new Color(0.05f, 0.07f, 0.1f, 0.82f);
        image.raycastTarget = false;

        if (!cardObject.TryGetComponent(out HeaderStatusUI _))
        {
            cardObject.AddComponent<HeaderStatusUI>();
        }
    }

    private void EnsureDebugResetButton(Transform hudTransform)
    {
        if (hudTransform == null)
        {
            return;
        }

        const string objectName = "DebugResetProgressButton";
        Transform existing = hudTransform.Find(objectName);
        if (existing == null && headerPanel != null)
        {
            existing = headerPanel.transform.Find(objectName);
        }

        GameObject buttonObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(hudTransform, false);
        buttonObject.transform.SetAsLastSibling();

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = buttonObject.AddComponent<LayoutElement>();
        }

        layoutElement.minWidth = 164f;
        layoutElement.preferredWidth = 180f;
        layoutElement.minHeight = HudHeight;
        layoutElement.preferredHeight = HudHeight;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.36f, 0.12f, 0.12f, 0.92f);
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        Text text = EnsureDebugResetButtonText(buttonObject.transform);

        if (!buttonObject.TryGetComponent(out DebugResetProgressButtonController controller))
        {
            controller = buttonObject.AddComponent<DebugResetProgressButtonController>();
        }

        controller.Initialize(button, text, progressSaveManager);
    }

    private void ApplyHudSafeAreaOffset()
    {
        if (combatHudRectTransform == null)
        {
            return;
        }

        float topInset = GetSafeAreaTopInsetInCanvasUnits();
        float topPadding = Mathf.Max(HudSafeAreaPadding, topInset + HudSafeAreaPadding);

        combatHudRectTransform.offsetMin = new Vector2(HudHorizontalPadding, -topPadding - HudHeight);
        combatHudRectTransform.offsetMax = new Vector2(-HudHorizontalPadding, -topPadding);
    }

    private float GetSafeAreaTopInsetInCanvasUnits()
    {
        if (Screen.height <= 0)
        {
            return 0f;
        }

        float topInsetPixels = Mathf.Max(0f, Screen.height - Screen.safeArea.yMax);
        RectTransform canvasRect = transform as RectTransform;
        float canvasHeight = canvasRect != null && canvasRect.rect.height > 0f
            ? canvasRect.rect.height
            : Screen.height;

        return topInsetPixels / Screen.height * canvasHeight;
    }

    private void EnsureBottomMenuButtons()
    {
        if (bottomMenuPanel == null)
        {
            return;
        }

        EnsureBottomMenuLayout();
        EnsureButton("BattleButton", "Battle", ShowBattle);
        EnsureButton("EquipmentButton", "Equipment", ShowEquipment);
        SetBottomMenuAsLastSibling();
    }

    private void EnsureBottomMenuLayout()
    {
        HorizontalLayoutGroup layoutGroup = bottomMenuPanel.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = bottomMenuPanel.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(28, 28, 24, 24);
        layoutGroup.spacing = 24f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    private Button EnsureButton(string objectName, string label, UnityEngine.Events.UnityAction onClick)
    {
        Transform existing = bottomMenuPanel.transform.Find(objectName);
        GameObject buttonObject = existing != null ? existing.gameObject : CreateButtonObject(objectName);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = buttonObject.AddComponent<LayoutElement>();
        }

        layoutElement.minHeight = 96f;
        layoutElement.preferredHeight = 120f;
        layoutElement.flexibleWidth = 1f;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.12f, 0.14f, 0.18f, 0.9f);
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);

        Text text = EnsureButtonText(buttonObject.transform, label);
        text.text = label;

        return button;
    }

    private GameObject CreateButtonObject(string objectName)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(bottomMenuPanel.transform, false);
        return buttonObject;
    }

    private Text EnsureButtonText(Transform buttonTransform, string label)
    {
        Transform existing = buttonTransform.Find("Text");
        if (existing != null && existing.TryGetComponent(out Text existingText))
        {
            return existingText;
        }

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(buttonTransform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 38;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 24;
        text.resizeTextMaxSize = 38;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = label;

        return text;
    }

    private Text EnsureDebugResetButtonText(Transform buttonTransform)
    {
        Transform existing = buttonTransform.Find("Text");
        GameObject textObject = existing != null ? existing.gameObject : new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(buttonTransform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 22;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = 22;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = "DEBUG RESET";

        return text;
    }

    private void EnsureTabContentPanel()
    {
        if (tabContentPanel == null)
        {
            tabContentPanel = new GameObject("TabContentPanel", typeof(RectTransform));
            tabContentPanel.transform.SetParent(transform, false);
        }

        ApplyAnchors(tabContentPanel, new Vector2(0f, BottomMenuRatio), Vector2.one);
        EnsureTabContentDimPanel();
        tabContentPanel.SetActive(false);
    }

    private void EnsureTabContentDimPanel()
    {
        const string objectName = "TabContentDim";
        Transform existing = tabContentPanel.transform.Find(objectName);
        GameObject dimObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dimObject.transform.SetParent(tabContentPanel.transform, false);
        dimObject.transform.SetAsFirstSibling();

        RectTransform rectTransform = dimObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = dimObject.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.58f);
        image.raycastTarget = true;
    }

    private void EnsureEquipmentPanel()
    {
        EnsureTabContentPanel();

        if (equipmentPanel == null)
        {
            equipmentPanel = CreateEquipmentPanel();
        }

        equipmentPanel.transform.SetParent(tabContentPanel.transform, false);
        equipmentPanel.transform.SetAsLastSibling();
        equipmentPanelRectTransform = equipmentPanel.GetComponent<RectTransform>();
        ApplyTabContentSafeAreaOffset();
        EnsureEquipmentPanelController();
    }

    private GameObject CreateEquipmentPanel()
    {
        GameObject panelObject = new GameObject("EquipmentPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(tabContentPanel.transform, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = panelObject.GetComponent<Image>();
        image.color = new Color(0.09f, 0.1f, 0.13f, 0.96f);
        image.raycastTarget = true;

        return panelObject;
    }

    private void ApplyTabContentSafeAreaOffset()
    {
        if (equipmentPanelRectTransform == null)
        {
            return;
        }

        float topInset = GetSafeAreaTopInsetInCanvasUnits();
        float topPadding = topInset + HudSafeAreaPadding + HudHeight + TabContentHudGap;

        equipmentPanelRectTransform.anchorMin = Vector2.zero;
        equipmentPanelRectTransform.anchorMax = Vector2.one;
        equipmentPanelRectTransform.offsetMin = new Vector2(TabContentPadding, TabContentPadding);
        equipmentPanelRectTransform.offsetMax = new Vector2(-TabContentPadding, -topPadding);
    }

    private void EnsureEquipmentManager()
    {
        if (equipmentManager != null)
        {
            return;
        }

        equipmentManager = EquipmentManager.EnsureInstance();
    }

    private void EnsureEquipmentPanelController()
    {
        if (!equipmentPanel.TryGetComponent(out HeroEquipmentPanelUI controller))
        {
            controller = equipmentPanel.AddComponent<HeroEquipmentPanelUI>();
        }

        controller.SetEquipmentRootTarget(equipmentRootTarget);
        equipmentPanelController = controller;
        equipmentPanelController.HidePanel();
    }

    private void SetBottomMenuAsLastSibling()
    {
        if (bottomMenuPanel != null)
        {
            bottomMenuPanel.transform.SetAsLastSibling();
        }
    }

    private void SetActiveIfPresent(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}
