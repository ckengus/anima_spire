using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
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
    private const float GlobalTabButtonWidth = 82f;
    private const float GlobalTabButtonHeight = 72f;

    [SerializeField] private GameObject headerPanel;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject bottomMenuPanel;
    [SerializeField] private GameObject bottomGlobalTabArea;
    [SerializeField] private GameObject tabContentPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject laboratoryPanel;
    [SerializeField] private GameObject equipmentSynthesisPanel;
    [SerializeField] private Transform equipmentRootTarget;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private ProgressSaveManager progressSaveManager;

    private RectTransform combatHudRectTransform;
    private RectTransform equipmentPanelRectTransform;
    private HeroEquipmentPanelUI equipmentPanelController;
    private LaboratoryPanelUI laboratoryPanelController;
    private EquipmentSynthesisPanelUI equipmentSynthesisPanelController;

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
        EnsureLaboratoryPanel();
        EnsureEquipmentSynthesisPanel();
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
        laboratoryPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();
        SetBottomMenuAsLastSibling();
    }

    public void ShowEquipment()
    {
        ShowWardrobe();
    }

    public void ShowWardrobe()
    {
        SetActiveIfPresent(combatPanel, true);
        SetActiveIfPresent(infoPanel, true);
        SetActiveIfPresent(tabContentPanel, true);
        SetActiveIfPresent(equipmentPanel, true);
        laboratoryPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();
        equipmentPanelController?.ShowPanel();
        SetBottomMenuAsLastSibling();
    }

    public void ShowLaboratory()
    {
        SetActiveIfPresent(combatPanel, true);
        SetActiveIfPresent(infoPanel, true);
        SetActiveIfPresent(tabContentPanel, false);
        SetActiveIfPresent(equipmentPanel, false);
        equipmentPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();

        EnsureLaboratoryPanel();
        laboratoryPanelController?.ShowPanel();
        SetBottomMenuAsLastSibling();
    }

    public void ShowSynthesisRoom()
    {
        SetActiveIfPresent(combatPanel, true);
        SetActiveIfPresent(infoPanel, true);
        SetActiveIfPresent(tabContentPanel, false);
        SetActiveIfPresent(equipmentPanel, false);
        equipmentPanelController?.HidePanel();
        laboratoryPanelController?.HidePanel();

        EnsureEquipmentSynthesisPanel();
        equipmentSynthesisPanelController?.ShowPanel();
        SetBottomMenuAsLastSibling();
    }

    private void EnsureReferences()
    {
        headerPanel = FindPanelIfMissing(headerPanel, "HeaderPanel");
        combatPanel = FindPanelIfMissing(combatPanel, "CombatPanel");
        infoPanel = FindPanelIfMissing(infoPanel, "InfoPanel");
        bottomMenuPanel = FindPanelIfMissing(bottomMenuPanel, "BottomMenuPanel");
        bottomGlobalTabArea = FindPanelIfMissing(bottomGlobalTabArea, "BottomGlobalTabArea");
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
        if (found == null)
        {
            found = FindDescendantByName(transform, panelName);
        }

        if (found == null)
        {
            found = FindSceneDescendantByName(panelName);
        }

        return found != null ? found.gameObject : null;
    }

    private Transform FindDescendantByName(Transform parent, string objectName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == objectName)
            {
                return child;
            }

            Transform found = FindDescendantByName(child, objectName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private Transform FindSceneDescendantByName(string objectName)
    {
        Scene scene = gameObject.scene;
        if (!scene.IsValid())
        {
            return null;
        }

        GameObject[] rootObjects = scene.GetRootGameObjects();
        for (int i = 0; i < rootObjects.Length; i++)
        {
            Transform root = rootObjects[i].transform;
            if (root.name == objectName)
            {
                return root;
            }

            Transform found = FindDescendantByName(root, objectName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
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
        CleanupBottomMenuPanelTabs();
        EnsureBottomGlobalTabArea();
    }

    private void CleanupBottomMenuPanelTabs()
    {
        if (bottomMenuPanel == null)
        {
            return;
        }

        ClearBottomMenuTabChildren();
        bottomMenuPanel.SetActive(false);
    }

    private void ClearBottomMenuTabChildren()
    {
        for (int i = bottomMenuPanel.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = bottomMenuPanel.transform.GetChild(i);
            if (!IsBottomMenuTabChild(child.name))
            {
                continue;
            }

            child.SetParent(null, false);
            Destroy(child.gameObject);
        }
    }

    private bool IsBottomMenuTabChild(string objectName)
    {
        return objectName == "BattleButton"
            || objectName == "EquipmentButton"
            || objectName.StartsWith("GlobalTabButton_")
            || objectName.StartsWith("GlobalTabSpacer_")
            || objectName.StartsWith("GlobalTabCard_");
    }

    private void EnsureBottomGlobalTabArea()
    {
        if (bottomGlobalTabArea == null)
        {
            bottomGlobalTabArea = new GameObject("BottomGlobalTabArea", typeof(RectTransform)).gameObject;
            Transform parent = FindSceneDescendantByName("SafeAreaUIRoot");
            if (parent == null)
            {
                parent = FindSceneDescendantByName("SafeAreaRoot");
            }

            bottomGlobalTabArea.transform.SetParent(parent != null ? parent : transform, false);
            ApplyAnchors(bottomGlobalTabArea, Vector2.zero, new Vector2(1f, BottomMenuRatio));
        }

        bottomGlobalTabArea.SetActive(true);

        EnsureTabBarBackground(bottomGlobalTabArea.transform);
        Transform iconRow = EnsureGlobalTabIconRow(bottomGlobalTabArea.transform);
        ClearChildren(iconRow);
        EnsureGlobalTabSpacer(iconRow, "GlobalTabSpacer_Left");
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Combat", "Label_Combat", "\uC804\uD22C", new Color(0.72f, 0.18f, 0.18f, 0.96f), ShowBattle);
        EnsureGlobalTabSpacer(iconRow, "GlobalTabSpacer_01");
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Hero", "Label_Hero", "\uC601\uC6C5", new Color(0.2f, 0.23f, 0.3f, 0.96f), ShowHeroTabPlaceholder);
        EnsureGlobalTabSpacer(iconRow, "GlobalTabSpacer_02");
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Laboratory", "Label_Laboratory", "\uC5F0\uAD6C\uC2E4", new Color(0.18f, 0.36f, 0.68f, 0.96f), ShowLaboratory);
        EnsureGlobalTabSpacer(iconRow, "GlobalTabSpacer_03");
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Guild", "Label_Guild", "\uD559\uD68C", new Color(0.22f, 0.25f, 0.32f, 0.96f), ShowGuildTabPlaceholder);
        EnsureGlobalTabSpacer(iconRow, "GlobalTabSpacer_04");
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Shop", "Label_Shop", "\uC0C1\uC810", new Color(0.25f, 0.28f, 0.34f, 0.96f), ShowShopTabPlaceholder);
        EnsureGlobalTabSpacer(iconRow, "GlobalTabSpacer_Right");

        iconRow.SetAsLastSibling();
        SetBottomMenuAsLastSibling();
    }

    private void EnsureTabBarBackground(Transform parent)
    {
        Transform existing = parent.Find("TabBarBackground");
        GameObject backgroundObject = existing != null
            ? existing.gameObject
            : new GameObject("TabBarBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backgroundObject.transform.SetParent(parent, false);

        RectTransform rectTransform = backgroundObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = backgroundObject.GetComponent<Image>();
        image.color = new Color(0.02f, 0.025f, 0.035f, 0.92f);
        image.raycastTarget = false;

        backgroundObject.transform.SetAsFirstSibling();
    }

    private Transform EnsureGlobalTabIconRow(Transform parent)
    {
        Transform existing = parent.Find("GlobalTabIconRow");
        GameObject rowObject = existing != null
            ? existing.gameObject
            : new GameObject("GlobalTabIconRow", typeof(RectTransform));
        rowObject.transform.SetParent(parent, false);

        RectTransform rectTransform = rowObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        HorizontalLayoutGroup layoutGroup = rowObject.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = rowObject.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(12, 12, 8, 8);
        layoutGroup.spacing = 0f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        return rowObject.transform;
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }
    }

    private GameObject EnsureGlobalTabSpacer(Transform parent, string objectName)
    {
        GameObject spacerObject = new GameObject(objectName, typeof(RectTransform), typeof(LayoutElement));
        spacerObject.transform.SetParent(parent, false);

        LayoutElement layoutElement = spacerObject.GetComponent<LayoutElement>();
        layoutElement.minWidth = 0f;
        layoutElement.preferredWidth = 0f;
        layoutElement.flexibleWidth = 1f;
        layoutElement.flexibleHeight = 0f;

        return spacerObject;
    }

    private Button EnsureGlobalTabCard(Transform parent, string objectName, string labelObjectName, string label, Color color, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(GlobalTabButtonWidth, GlobalTabButtonHeight);

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.minWidth = GlobalTabButtonWidth;
        layoutElement.preferredWidth = GlobalTabButtonWidth;
        layoutElement.minHeight = GlobalTabButtonHeight;
        layoutElement.preferredHeight = GlobalTabButtonHeight;
        layoutElement.flexibleWidth = 0f;
        layoutElement.flexibleHeight = 0f;

        Image image = buttonObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);

        Text text = EnsureGlobalTabLabel(buttonObject.transform, labelObjectName, label);
        text.text = label;
        text.fontSize = 18;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = 18;

        return button;
    }

    private Text EnsureGlobalTabLabel(Transform buttonTransform, string objectName, string label)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(buttonTransform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = 18;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = label;

        return text;
    }

    private void ShowHeroTabPlaceholder()
    {
        ShowUnavailableGlobalTab("Hero tab is not implemented yet.");
    }

    private void ShowLaboratoryTabPlaceholder()
    {
        ShowUnavailableGlobalTab("Laboratory tab shell is not implemented yet.");
    }

    private void ShowGuildTabPlaceholder()
    {
        ShowUnavailableGlobalTab("Guild tab is not implemented yet.");
    }

    private void ShowShopTabPlaceholder()
    {
        ShowUnavailableGlobalTab("Shop tab is not implemented yet.");
    }

    private void ShowUnavailableGlobalTab(string message)
    {
        Debug.Log(message);
        ShowBattle();
        SetBottomMenuAsLastSibling();
    }

    private void EnsureBottomMenuLayout()
    {
        HorizontalLayoutGroup layoutGroup = bottomMenuPanel.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = bottomMenuPanel.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(12, 12, 8, 8);
        layoutGroup.spacing = 0f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
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
        controller.SetEquipmentManager(equipmentManager);
        equipmentPanelController = controller;
        equipmentPanelController.HidePanel();
    }

    private void EnsureLaboratoryPanel()
    {
        Transform mainContentArea = FindSceneDescendantByName("MainContentArea");
        if (mainContentArea == null)
        {
            Transform safeAreaRoot = FindSceneDescendantByName("SafeAreaRoot");
            GameObject mainContentObject = new GameObject("MainContentArea", typeof(RectTransform));
            mainContentObject.transform.SetParent(safeAreaRoot != null ? safeAreaRoot : transform, false);
            ApplyAnchors(mainContentObject, Vector2.zero, Vector2.one);
            mainContentArea = mainContentObject.transform;
        }

        if (laboratoryPanel == null)
        {
            Transform existing = mainContentArea.Find("LaboratoryPanel");
            laboratoryPanel = existing != null
                ? existing.gameObject
                : new GameObject("LaboratoryPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        }

        laboratoryPanel.transform.SetParent(mainContentArea, false);
        StretchToParent(laboratoryPanel.GetComponent<RectTransform>());

        Image image = laboratoryPanel.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.06f, 0.075f, 0.1f, 0.98f);
            image.raycastTarget = true;
        }

        if (!laboratoryPanel.TryGetComponent(out LaboratoryPanelUI controller))
        {
            controller = laboratoryPanel.AddComponent<LaboratoryPanelUI>();
        }

        controller.SetCallbacks(ShowWardrobe, ShowSynthesisRoom);
        laboratoryPanelController = controller;
        laboratoryPanelController.HidePanel();
    }

    private void EnsureEquipmentSynthesisPanel()
    {
        Transform mainContentArea = FindSceneDescendantByName("MainContentArea");
        if (mainContentArea == null)
        {
            Transform safeAreaRoot = FindSceneDescendantByName("SafeAreaRoot");
            GameObject mainContentObject = new GameObject("MainContentArea", typeof(RectTransform));
            mainContentObject.transform.SetParent(safeAreaRoot != null ? safeAreaRoot : transform, false);
            ApplyAnchors(mainContentObject, Vector2.zero, Vector2.one);
            mainContentArea = mainContentObject.transform;
        }

        if (equipmentSynthesisPanel == null)
        {
            Transform existing = mainContentArea.Find("EquipmentSynthesisPanel");
            equipmentSynthesisPanel = existing != null
                ? existing.gameObject
                : new GameObject("EquipmentSynthesisPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        }

        equipmentSynthesisPanel.transform.SetParent(mainContentArea, false);
        StretchToParent(equipmentSynthesisPanel.GetComponent<RectTransform>());

        Image image = equipmentSynthesisPanel.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.06f, 0.075f, 0.1f, 0.98f);
            image.raycastTarget = true;
        }

        if (!equipmentSynthesisPanel.TryGetComponent(out EquipmentSynthesisPanelUI controller))
        {
            controller = equipmentSynthesisPanel.AddComponent<EquipmentSynthesisPanelUI>();
        }

        controller.SetCallbacks(HandleEquipmentSynthesisClicked);
        equipmentSynthesisPanelController = controller;
        equipmentSynthesisPanelController.HidePanel();
    }

    private void HandleEquipmentSynthesisClicked()
    {
        EnsureEquipmentManager();

        if (equipmentManager == null)
        {
            equipmentSynthesisPanelController?.SetResultText("\uC7A5\uBE44 \uC2DC\uC2A4\uD15C\uC774 \uC900\uBE44\uB418\uC9C0 \uC54A\uC558\uC2B5\uB2C8\uB2E4.");
            return;
        }

        equipmentManager.TrySynthesizeEquipment(out EquipmentDefinition _, out string message);
        equipmentSynthesisPanelController?.SetResultText(message);
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

    private void SetBottomMenuAsLastSibling()
    {
        if (bottomGlobalTabArea != null)
        {
            bottomGlobalTabArea.transform.SetAsLastSibling();
            Transform popupOverlay = bottomGlobalTabArea.transform.parent != null
                ? bottomGlobalTabArea.transform.parent.Find("PopupOverlay")
                : null;
            popupOverlay?.SetAsLastSibling();

            Transform fullScreenPopupOverlay = FindSceneDescendantByName("FullScreenPopupOverlay");
            fullScreenPopupOverlay?.SetAsLastSibling();
            return;
        }

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
