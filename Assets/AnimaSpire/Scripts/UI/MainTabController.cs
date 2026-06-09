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
    private const float GlobalTabIconSize = 68f;

    private enum GlobalTabState
    {
        Combat,
        Hero,
        Laboratory,
        Guild,
        Shop
    }

    private enum EquipmentEntryContext
    {
        None,
        HeroTab,
        LaboratoryWardrobe
    }

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
    [SerializeField] private Sprite heroTabBackgroundSprite;
    [SerializeField] private Sprite laboratoryTabBackgroundSprite;
    [SerializeField] private Sprite guildTabBackgroundSprite;
    [SerializeField] private Sprite shopTabBackgroundSprite;
    [SerializeField] private Sprite bottomGlobalTabBackgroundSprite;
    [SerializeField] private Sprite globalTabCombatIconSprite;
    [SerializeField] private Sprite globalTabHeroIconSprite;
    [SerializeField] private Sprite globalTabLaboratoryIconSprite;
    [SerializeField] private Sprite globalTabGuildIconSprite;
    [SerializeField] private Sprite globalTabShopIconSprite;

    private RectTransform combatHudRectTransform;
    private RectTransform equipmentPanelRectTransform;
    private HeroEquipmentPanelUI equipmentPanelController;
    private LaboratoryPanelUI laboratoryPanelController;
    private EquipmentSynthesisPanelUI equipmentSynthesisPanelController;
    private GlobalTabState currentGlobalTabState = GlobalTabState.Combat;
    private EquipmentEntryContext currentEquipmentEntryContext = EquipmentEntryContext.None;
    private GameObject globalTabSurfaceRoot;
    private Image globalTabBackgroundSurface;
    private GameObject nonCombatPageBackgroundLayer;
    private Image nonCombatPageBackgroundImage;
    private AspectRatioFitter nonCombatPageBackgroundAspectFitter;
    private GameObject globalTabPlaceholderPanel;
    private Text globalTabPlaceholderTitle;
    private Text globalTabPlaceholderMessage;

    private void Awake()
    {
        EnsureReferences();
        EnsureGlobalFrameHierarchy();
        EnsureGlobalTabSurfaceRoot();
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
        SetGlobalFrameSiblingOrder();
        ShowBattle();
    }

    private void OnRectTransformDimensionsChange()
    {
        ApplyHudAnchorLayout();
        ApplyTabContentSafeAreaOffset();
    }

    public void ShowBattle()
    {
        currentEquipmentEntryContext = EquipmentEntryContext.None;
        currentGlobalTabState = GlobalTabState.Combat;
        ApplyGlobalTabSurfaceState();
        HideGlobalTabPlaceholder();
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
        ShowEquipment(EquipmentEntryContext.HeroTab);
    }

    public void ShowWardrobe()
    {
        ShowEquipment(EquipmentEntryContext.LaboratoryWardrobe);
    }

    private void ShowEquipment(EquipmentEntryContext entryContext)
    {
        currentEquipmentEntryContext = entryContext;

        if (entryContext == EquipmentEntryContext.HeroTab)
        {
            currentGlobalTabState = GlobalTabState.Hero;
        }
        else if (entryContext == EquipmentEntryContext.LaboratoryWardrobe)
        {
            currentGlobalTabState = GlobalTabState.Laboratory;
        }

        ApplyGlobalTabSurfaceState();
        HideGlobalTabPlaceholder();
        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
        SetActiveIfPresent(tabContentPanel, true);
        SetActiveIfPresent(equipmentPanel, true);
        laboratoryPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();
        equipmentPanelController?.ShowPanel();
        SetBottomMenuAsLastSibling();
    }

    public void ShowLaboratory()
    {
        currentEquipmentEntryContext = EquipmentEntryContext.None;
        currentGlobalTabState = GlobalTabState.Laboratory;
        ApplyGlobalTabSurfaceState();
        HideGlobalTabPlaceholder();
        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
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
        currentGlobalTabState = GlobalTabState.Laboratory;
        ApplyGlobalTabSurfaceState();
        HideGlobalTabPlaceholder();
        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
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

    private Transform FindSafeAreaRoot()
    {
        Transform safeAreaRoot = FindSceneDescendantByName("SafeAreaUIRoot");
        if (safeAreaRoot == null)
        {
            safeAreaRoot = FindSceneDescendantByName("SafeAreaRoot");
        }

        return safeAreaRoot;
    }

    private Transform FindOverlayCanvas()
    {
        return FindSceneDescendantByName("UI_OverlayCanvas");
    }

    private Transform FindBackgroundLayer()
    {
        return FindSceneDescendantByName("BackgroundLayer");
    }

    private Transform FindGameContentRoot()
    {
        return FindSceneDescendantByName("GameContentRoot");
    }

    private Transform FindPageRoot()
    {
        return FindSceneDescendantByName("PageRoot");
    }

    private Transform FindPopupOverlay()
    {
        return FindSceneDescendantByName("PopupOverlay");
    }

    private Transform FindFullScreenForegroundOverlay()
    {
        Transform foregroundOverlay = FindSceneDescendantByName("FullScreenForegroundOverlay");
        if (foregroundOverlay == null)
        {
            foregroundOverlay = FindSceneDescendantByName("FullScreenPopupOverlay");
        }

        return foregroundOverlay;
    }

    private Transform FindGlobalSurfaceParent()
    {
        Transform backgroundLayer = FindBackgroundLayer();
        if (backgroundLayer != null)
        {
            return backgroundLayer;
        }

        Transform overlayCanvas = FindOverlayCanvas();
        return overlayCanvas != null ? overlayCanvas : transform;
    }

    private Transform FindTabContentParent()
    {
        Transform mainContentArea = FindSceneDescendantByName("MainContentArea");
        if (mainContentArea != null)
        {
            return mainContentArea;
        }

        Transform pageRoot = FindPageRoot();
        if (pageRoot != null)
        {
            return pageRoot;
        }

        Transform safeAreaRoot = FindSafeAreaRoot();
        if (safeAreaRoot != null)
        {
            return safeAreaRoot;
        }

        Transform overlayCanvas = FindOverlayCanvas();
        return overlayCanvas != null ? overlayCanvas : transform;
    }

    private Transform FindMainContentParent()
    {
        Transform pageRoot = FindPageRoot();
        if (pageRoot != null)
        {
            return pageRoot;
        }

        Transform gameContentRoot = FindGameContentRoot();
        if (gameContentRoot != null)
        {
            return gameContentRoot;
        }

        Transform safeAreaRoot = FindSafeAreaRoot();
        if (safeAreaRoot != null)
        {
            return safeAreaRoot;
        }

        Transform overlayCanvas = FindOverlayCanvas();
        return overlayCanvas != null ? overlayCanvas : transform;
    }

    private Transform FindBottomGlobalTabParent()
    {
        Transform gameContentRoot = FindGameContentRoot();
        if (gameContentRoot != null)
        {
            return gameContentRoot;
        }

        Transform safeAreaRoot = FindSafeAreaRoot();
        return safeAreaRoot != null ? safeAreaRoot : transform;
    }

    private void EnsureGlobalFrameHierarchy()
    {
        Transform safeAreaRoot = FindSafeAreaRoot();
        if (safeAreaRoot == null)
        {
            return;
        }

        Transform gameContentRoot = EnsureFrameRoot(safeAreaRoot, "GameContentRoot");
        Transform pageRoot = EnsureFrameRoot(gameContentRoot, "PageRoot");

        ParentIfPresent("TopBarOverlay", gameContentRoot);
        ParentIfPresent("PopupOverlay", gameContentRoot);
        ParentIfPresent("BottomGlobalTabArea", gameContentRoot);
        ParentIfPresent("CombatContentArea", pageRoot);
        ParentIfPresent("MainContentArea", pageRoot);

        SetGlobalFrameSiblingOrder();
    }

    private Transform EnsureFrameRoot(Transform parent, string objectName)
    {
        Transform existing = FindSceneDescendantByName(objectName);
        GameObject rootObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform));

        rootObject.transform.SetParent(parent, false);
        rootObject.layer = parent.gameObject.layer;

        RectTransform rectTransform = rootObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = rootObject.AddComponent<RectTransform>();
        }

        StretchToParent(rectTransform);
        return rootObject.transform;
    }

    private void ParentIfPresent(string objectName, Transform parent)
    {
        Transform target = FindSceneDescendantByName(objectName);
        if (target == null || parent == null || target == parent)
        {
            return;
        }

        target.SetParent(parent, false);
    }

    private Transform EnsureMainContentArea()
    {
        Transform mainContentArea = FindSceneDescendantByName("MainContentArea");
        if (mainContentArea != null)
        {
            return mainContentArea;
        }

        GameObject mainContentObject = new GameObject("MainContentArea", typeof(RectTransform));
        mainContentObject.transform.SetParent(FindMainContentParent(), false);
        ApplyAnchors(mainContentObject, Vector2.zero, Vector2.one);

        return mainContentObject.transform;
    }

    private void EnsureGlobalTabSurfaceRoot()
    {
        Transform parent = FindGlobalSurfaceParent();
        Transform existingRoot = parent.Find("GlobalTabSurfaceRoot");
        if (existingRoot == null)
        {
            existingRoot = FindSceneDescendantByName("GlobalTabSurfaceRoot");
        }

        globalTabSurfaceRoot = existingRoot != null
            ? existingRoot.gameObject
            : new GameObject("GlobalTabSurfaceRoot", typeof(RectTransform));

        globalTabSurfaceRoot.transform.SetParent(parent, false);
        StretchToParent(globalTabSurfaceRoot.GetComponent<RectTransform>());

        Transform existingSurface = globalTabSurfaceRoot.transform.Find("GlobalTabBackgroundSurface");
        GameObject surfaceObject = existingSurface != null
            ? existingSurface.gameObject
            : new GameObject("GlobalTabBackgroundSurface", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        surfaceObject.transform.SetParent(globalTabSurfaceRoot.transform, false);
        StretchToParent(surfaceObject.GetComponent<RectTransform>());

        globalTabBackgroundSurface = surfaceObject.GetComponent<Image>();
        globalTabBackgroundSurface.raycastTarget = false;

        EnsureBackgroundLayerInfrastructure();
        SetGlobalTabSurfaceSiblingOrder();
        ApplyGlobalTabSurfaceState();
    }

    private void EnsureBackgroundLayerInfrastructure()
    {
        if (globalTabSurfaceRoot == null)
        {
            EnsureGlobalTabSurfaceRoot();
            return;
        }

        Transform existingBackgroundLayer = globalTabSurfaceRoot.transform.Find("NonCombatPageBackgroundLayer");
        nonCombatPageBackgroundLayer = existingBackgroundLayer != null
            ? existingBackgroundLayer.gameObject
            : new GameObject("NonCombatPageBackgroundLayer", typeof(RectTransform));
        nonCombatPageBackgroundLayer.transform.SetParent(globalTabSurfaceRoot.transform, false);
        nonCombatPageBackgroundLayer.transform.SetAsFirstSibling();
        StretchToParent(nonCombatPageBackgroundLayer.GetComponent<RectTransform>());

        Transform existingBackgroundImage = nonCombatPageBackgroundLayer.transform.Find("NonCombatPageBackgroundImage");
        GameObject backgroundImageObject = existingBackgroundImage != null
            ? existingBackgroundImage.gameObject
            : new GameObject("NonCombatPageBackgroundImage", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backgroundImageObject.transform.SetParent(nonCombatPageBackgroundLayer.transform, false);
        StretchToParent(backgroundImageObject.GetComponent<RectTransform>());

        nonCombatPageBackgroundImage = backgroundImageObject.GetComponent<Image>();
        nonCombatPageBackgroundImage.raycastTarget = false;
        EnsureNonCombatPageBackgroundAspectFitter();

        if (globalTabBackgroundSurface != null)
        {
            globalTabBackgroundSurface.enabled = false;
            globalTabBackgroundSurface.color = Color.clear;
            globalTabBackgroundSurface.raycastTarget = false;
        }
    }

    private void SetGlobalTabSurfaceSiblingOrder()
    {
        if (globalTabSurfaceRoot == null)
        {
            return;
        }

        Transform safeAreaRoot = FindSafeAreaRoot();
        if (safeAreaRoot != null && safeAreaRoot.parent == globalTabSurfaceRoot.transform.parent)
        {
            int safeAreaIndex = safeAreaRoot.GetSiblingIndex();
            int currentIndex = globalTabSurfaceRoot.transform.GetSiblingIndex();
            int targetIndex = currentIndex < safeAreaIndex ? safeAreaIndex - 1 : safeAreaIndex;
            globalTabSurfaceRoot.transform.SetSiblingIndex(Mathf.Max(0, targetIndex));
        }
        else
        {
            globalTabSurfaceRoot.transform.SetAsFirstSibling();
        }

        Transform fullScreenForegroundOverlay = FindFullScreenForegroundOverlay();
        fullScreenForegroundOverlay?.SetAsLastSibling();
    }

    private void ApplyGlobalTabSurfaceState()
    {
        EnsureGlobalTabSurfaceRootIfMissing();

        if (globalTabSurfaceRoot == null || nonCombatPageBackgroundImage == null)
        {
            return;
        }

        if (currentGlobalTabState == GlobalTabState.Combat)
        {
            SetNonCombatPageBackgroundSprite(null);
            SetNonCombatPageBackgroundVisible(false);
            globalTabSurfaceRoot.SetActive(false);
            return;
        }

        Sprite backgroundSprite = GetGlobalTabBackgroundSprite(currentGlobalTabState);
        bool showBackground = backgroundSprite != null;

        globalTabSurfaceRoot.SetActive(showBackground);
        SetNonCombatPageBackgroundVisible(showBackground);

        if (showBackground)
        {
            SetNonCombatPageBackgroundSprite(backgroundSprite);
        }
        else
        {
            SetNonCombatPageBackgroundSprite(null);
            SetNonCombatPageBackgroundColor(GetGlobalTabSurfaceColor(currentGlobalTabState));
        }
    }

    private void EnsureGlobalTabSurfaceRootIfMissing()
    {
        if (globalTabSurfaceRoot == null || globalTabBackgroundSurface == null || nonCombatPageBackgroundImage == null)
        {
            EnsureGlobalTabSurfaceRoot();
        }
    }

    private void SetNonCombatPageBackgroundVisible(bool isVisible)
    {
        if (nonCombatPageBackgroundLayer != null)
        {
            nonCombatPageBackgroundLayer.SetActive(isVisible);
        }

        if (nonCombatPageBackgroundImage != null)
        {
            nonCombatPageBackgroundImage.enabled = isVisible;
            nonCombatPageBackgroundImage.raycastTarget = false;
        }

        if (globalTabBackgroundSurface != null)
        {
            globalTabBackgroundSurface.enabled = false;
            globalTabBackgroundSurface.raycastTarget = false;
        }
    }

    private void SetNonCombatPageBackgroundColor(Color color)
    {
        if (nonCombatPageBackgroundImage == null)
        {
            return;
        }

        nonCombatPageBackgroundImage.color = color;
        nonCombatPageBackgroundImage.raycastTarget = false;
    }

    private void SetNonCombatPageBackgroundSprite(Sprite sprite)
    {
        if (nonCombatPageBackgroundImage == null)
        {
            return;
        }

        nonCombatPageBackgroundImage.sprite = sprite;
        nonCombatPageBackgroundImage.type = Image.Type.Simple;
        nonCombatPageBackgroundImage.color = Color.white;
        nonCombatPageBackgroundImage.preserveAspect = false;
        nonCombatPageBackgroundImage.raycastTarget = false;
        nonCombatPageBackgroundImage.enabled = sprite != null;
        UpdateNonCombatPageBackgroundAspectRatio(sprite);
    }

    private Sprite GetGlobalTabBackgroundSprite(GlobalTabState tabState)
    {
        switch (tabState)
        {
            case GlobalTabState.Hero:
                return heroTabBackgroundSprite;
            case GlobalTabState.Laboratory:
                return laboratoryTabBackgroundSprite;
            case GlobalTabState.Guild:
                return guildTabBackgroundSprite;
            case GlobalTabState.Shop:
                return shopTabBackgroundSprite;
            case GlobalTabState.Combat:
            default:
                return null;
        }
    }

    private void EnsureNonCombatPageBackgroundAspectFitter()
    {
        if (nonCombatPageBackgroundImage == null)
        {
            return;
        }

        RectTransform rectTransform = nonCombatPageBackgroundImage.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        nonCombatPageBackgroundAspectFitter = nonCombatPageBackgroundImage.GetComponent<AspectRatioFitter>();
        if (nonCombatPageBackgroundAspectFitter == null)
        {
            nonCombatPageBackgroundAspectFitter = nonCombatPageBackgroundImage.gameObject.AddComponent<AspectRatioFitter>();
        }

        nonCombatPageBackgroundAspectFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
    }

    private void UpdateNonCombatPageBackgroundAspectRatio(Sprite sprite)
    {
        EnsureNonCombatPageBackgroundAspectFitter();

        if (nonCombatPageBackgroundAspectFitter == null || sprite == null || sprite.rect.height <= 0f)
        {
            return;
        }

        nonCombatPageBackgroundAspectFitter.aspectRatio = sprite.rect.width / sprite.rect.height;
    }

    private Color GetGlobalTabSurfaceColor(GlobalTabState tabState)
    {
        switch (tabState)
        {
            case GlobalTabState.Hero:
                return new Color(0.075f, 0.09f, 0.13f, 1f);
            case GlobalTabState.Laboratory:
                return new Color(0.045f, 0.07f, 0.105f, 1f);
            case GlobalTabState.Guild:
                return new Color(0.065f, 0.075f, 0.105f, 1f);
            case GlobalTabState.Shop:
                return new Color(0.08f, 0.075f, 0.095f, 1f);
            case GlobalTabState.Combat:
            default:
                return new Color(0f, 0f, 0f, 0f);
        }
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
        ApplyHudAnchorLayout();
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

    private void ApplyHudAnchorLayout()
    {
        if (combatHudRectTransform == null)
        {
            return;
        }

        combatHudRectTransform.offsetMin = new Vector2(HudHorizontalPadding, -HudSafeAreaPadding - HudHeight);
        combatHudRectTransform.offsetMax = new Vector2(-HudHorizontalPadding, -HudSafeAreaPadding);
    }

    private float GetSafeAreaTopInsetInCanvasUnits()
    {
        if (Screen.height <= 0)
        {
            return 0f;
        }

        float topInsetPixels = Mathf.Max(0f, Screen.height - Screen.safeArea.yMax);
        Transform overlayCanvas = FindSceneDescendantByName("UI_OverlayCanvas");
        RectTransform canvasRect = overlayCanvas != null
            ? overlayCanvas as RectTransform
            : null;
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
            bottomGlobalTabArea.transform.SetParent(FindBottomGlobalTabParent(), false);
            ApplyAnchors(bottomGlobalTabArea, Vector2.zero, new Vector2(1f, BottomMenuRatio));
        }

        bottomGlobalTabArea.SetActive(true);

        EnsureTabBarBackground(bottomGlobalTabArea.transform);
        Transform iconRow = EnsureGlobalTabIconRow(bottomGlobalTabArea.transform);
        DisableGlobalTabSpacers(iconRow);
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Combat", "Label_Combat", globalTabCombatIconSprite, 0.14f, ShowBattle);
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Hero", "Label_Hero", globalTabHeroIconSprite, 0.32f, ShowHeroTabPlaceholder);
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Laboratory", "Label_Laboratory", globalTabLaboratoryIconSprite, 0.5f, ShowLaboratory);
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Guild", "Label_Guild", globalTabGuildIconSprite, 0.68f, ShowGuildTabPlaceholder);
        EnsureGlobalTabCard(iconRow, "GlobalTabCard_Shop", "Label_Shop", globalTabShopIconSprite, 0.86f, ShowShopTabPlaceholder);

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
        if (bottomGlobalTabBackgroundSprite != null)
        {
            image.sprite = bottomGlobalTabBackgroundSprite;
            image.type = bottomGlobalTabBackgroundSprite.border != Vector4.zero
                ? Image.Type.Sliced
                : Image.Type.Simple;
            image.color = Color.white;
            image.preserveAspect = false;
        }
        else
        {
            image.color = new Color(0.02f, 0.025f, 0.035f, 0.92f);
        }

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
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
        }

        ContentSizeFitter contentSizeFitter = rowObject.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
        {
            contentSizeFitter.enabled = false;
        }

        LayoutElement layoutElement = rowObject.GetComponent<LayoutElement>();
        if (layoutElement != null)
        {
            layoutElement.enabled = false;
        }

        return rowObject.transform;
    }

    private void DisableGlobalTabSpacers(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.name.StartsWith("GlobalTabSpacer_") || child.name.StartsWith("Spacer_"))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private Button EnsureGlobalTabCard(Transform parent, string objectName, string labelObjectName, Sprite iconSprite, float anchorX, UnityEngine.Events.UnityAction onClick)
    {
        Transform existing = parent.Find(objectName);
        bool isNewObject = existing == null;
        GameObject buttonObject = isNewObject
            ? new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button))
            : existing.gameObject;
        if (buttonObject.transform.parent != parent)
        {
            buttonObject.transform.SetParent(parent, false);
        }

        RemoveGlobalTabCardUnsupportedComponents(buttonObject);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        if (isNewObject)
        {
            rectTransform.anchorMin = new Vector2(anchorX, 0.5f);
            rectTransform.anchorMax = new Vector2(anchorX, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(GlobalTabIconSize, GlobalTabIconSize);
        }

        Image image = buttonObject.GetComponent<Image>();
        if (image == null)
        {
            image = buttonObject.AddComponent<Image>();
        }

        image.sprite = iconSprite;
        image.color = Color.white;
        image.raycastTarget = true;
        image.preserveAspect = true;

        Button button = buttonObject.GetComponent<Button>();
        if (button == null)
        {
            button = buttonObject.AddComponent<Button>();
        }

        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);

        HideGlobalTabLabel(buttonObject.transform, labelObjectName);

        return button;
    }

    private void RemoveGlobalTabCardUnsupportedComponents(GameObject buttonObject)
    {
        RemoveComponentIfPresent<StandaloneInputModule>(buttonObject);
        RemoveComponentIfPresent<EventSystem>(buttonObject);
        RemoveComponentIfPresent<InputSystemUIInputModule>(buttonObject);
        RemoveComponentIfPresent<LayoutElement>(buttonObject);
    }

    private void RemoveComponentIfPresent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component != null)
        {
            Destroy(component);
        }
    }

    private void HideGlobalTabLabel(Transform buttonTransform, string objectName)
    {
        for (int i = 0; i < buttonTransform.childCount; i++)
        {
            Transform child = buttonTransform.GetChild(i);
            if (child.name == objectName || child.name.StartsWith("Label_"))
            {
                child.gameObject.SetActive(false);
            }
        }
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
        ShowGlobalTabPlaceholder(GlobalTabState.Hero, "\uC601\uC6C5", "\uC601\uC6C5 \uD0ED\uC740 \uC784\uC2DC \uD45C\uC2DC \uC0C1\uD0DC\uC785\uB2C8\uB2E4.");
    }

    private void ShowLaboratoryTabPlaceholder()
    {
        ShowLaboratory();
    }

    private void ShowGuildTabPlaceholder()
    {
        ShowGlobalTabPlaceholder(GlobalTabState.Guild, "\uD559\uD68C", "\uD559\uD68C \uD0ED\uC740 \uC784\uC2DC \uD45C\uC2DC \uC0C1\uD0DC\uC785\uB2C8\uB2E4.");
    }

    private void ShowShopTabPlaceholder()
    {
        ShowGlobalTabPlaceholder(GlobalTabState.Shop, "\uC0C1\uC810", "\uC0C1\uC810 \uD0ED\uC740 \uC784\uC2DC \uD45C\uC2DC \uC0C1\uD0DC\uC785\uB2C8\uB2E4.");
    }

    private void ShowUnavailableGlobalTab(string message)
    {
        Debug.Log(message);
        ShowGlobalTabPlaceholder(GlobalTabState.Hero, "\uC900\uBE44 \uC911", message);
        SetBottomMenuAsLastSibling();
    }

    private void ShowGlobalTabPlaceholder(GlobalTabState tabState, string title, string message)
    {
        currentGlobalTabState = tabState;
        ApplyGlobalTabSurfaceState();
        EnsureGlobalTabPlaceholderPanel();
        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
        SetActiveIfPresent(tabContentPanel, false);
        SetActiveIfPresent(equipmentPanel, false);
        equipmentPanelController?.HidePanel();
        laboratoryPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();

        globalTabPlaceholderTitle.text = title;
        globalTabPlaceholderMessage.text = message;
        globalTabPlaceholderPanel.SetActive(true);
        globalTabPlaceholderPanel.transform.SetAsLastSibling();
        SetBottomMenuAsLastSibling();
    }

    private void EnsureGlobalTabPlaceholderPanel()
    {
        Transform mainContentArea = EnsureMainContentArea();
        Transform existing = mainContentArea.Find("GlobalTabPlaceholderPanel");
        globalTabPlaceholderPanel = existing != null
            ? existing.gameObject
            : new GameObject("GlobalTabPlaceholderPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        globalTabPlaceholderPanel.transform.SetParent(mainContentArea, false);
        StretchToParent(globalTabPlaceholderPanel.GetComponent<RectTransform>());

        Image image = globalTabPlaceholderPanel.GetComponent<Image>();
        image.color = new Color(0.02f, 0.024f, 0.032f, 0f);
        image.raycastTarget = false;

        globalTabPlaceholderTitle = EnsurePlaceholderText(
            globalTabPlaceholderPanel.transform,
            "PlaceholderTitle",
            new Vector2(0.08f, 0.5f),
            new Vector2(0.92f, 0.62f),
            30,
            TextAnchor.LowerCenter);

        globalTabPlaceholderMessage = EnsurePlaceholderText(
            globalTabPlaceholderPanel.transform,
            "PlaceholderMessage",
            new Vector2(0.08f, 0.38f),
            new Vector2(0.92f, 0.5f),
            20,
            TextAnchor.UpperCenter);
    }

    private Text EnsurePlaceholderText(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor alignment)
    {
        Transform existing = parent.Find(objectName);
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
    }

    private void HideGlobalTabPlaceholder()
    {
        if (globalTabPlaceholderPanel != null)
        {
            globalTabPlaceholderPanel.SetActive(false);
        }
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
        EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        EventSystem primaryEventSystem = null;

        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem eventSystem = eventSystems[i];
            if (eventSystem == null)
            {
                continue;
            }

            if (primaryEventSystem == null || eventSystem.gameObject.activeInHierarchy)
            {
                primaryEventSystem = eventSystem;
            }

            if (eventSystem.gameObject.activeInHierarchy)
            {
                break;
            }
        }

        if (primaryEventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
            primaryEventSystem = eventSystemObject.GetComponent<EventSystem>();
        }
        else
        {
            primaryEventSystem.gameObject.SetActive(true);
        }

        DisableStandaloneInputModule(primaryEventSystem.gameObject);
        InputSystemUIInputModule inputSystemModule = primaryEventSystem.GetComponent<InputSystemUIInputModule>();
        if (inputSystemModule == null)
        {
            inputSystemModule = primaryEventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        inputSystemModule.enabled = true;

        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem eventSystem = eventSystems[i];
            if (eventSystem == null || eventSystem == primaryEventSystem)
            {
                continue;
            }

            DisableStandaloneInputModule(eventSystem.gameObject);
            eventSystem.gameObject.SetActive(false);
        }
    }

    private void DisableStandaloneInputModule(GameObject eventSystemObject)
    {
        StandaloneInputModule standaloneInputModule = eventSystemObject.GetComponent<StandaloneInputModule>();
        if (standaloneInputModule != null)
        {
            standaloneInputModule.enabled = false;
        }
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
            tabContentPanel.transform.SetParent(FindTabContentParent(), false);
        }
        else if (tabContentPanel.transform.parent == transform)
        {
            tabContentPanel.transform.SetParent(FindTabContentParent(), false);
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
        controller.SetPopupRootTarget(FindPopupOverlay());
        controller.SetEquipmentManager(equipmentManager);
        equipmentPanelController = controller;
        equipmentPanelController.HidePanel();
    }

    private void EnsureLaboratoryPanel()
    {
        Transform mainContentArea = EnsureMainContentArea();

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
            image.enabled = false;
            image.raycastTarget = false;
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
        Transform mainContentArea = EnsureMainContentArea();

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
        SetGlobalFrameSiblingOrder();

        if (bottomGlobalTabArea != null)
        {
            Transform fullScreenForegroundOverlay = FindFullScreenForegroundOverlay();
            fullScreenForegroundOverlay?.SetAsLastSibling();
            return;
        }

        if (bottomMenuPanel != null)
        {
            bottomMenuPanel.transform.SetAsLastSibling();
        }

        Transform fallbackForegroundOverlay = FindFullScreenForegroundOverlay();
        fallbackForegroundOverlay?.SetAsLastSibling();
    }

    private void SetGlobalFrameSiblingOrder()
    {
        Transform gameContentRoot = FindGameContentRoot();
        if (gameContentRoot == null)
        {
            return;
        }

        MoveDirectChildToLast(gameContentRoot, "TopBarOverlay");
        MoveDirectChildToLast(gameContentRoot, "PageRoot");
        MoveDirectChildToLast(gameContentRoot, "BottomGlobalTabArea");
        MoveDirectChildToLast(gameContentRoot, "PopupOverlay");
    }

    private void MoveDirectChildToLast(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child != null)
        {
            child.SetAsLastSibling();
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
