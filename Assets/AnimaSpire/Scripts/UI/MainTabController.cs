using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainTabController : MonoBehaviour
{
    private const float CombatAreaRatio = 0.5f;
    private const float CombatSceneContentRatio = 0.6f;
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
    private const float CombatSkillSlotSize = 72f;

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
    [SerializeField] private Sprite combatInfoPanelBackgroundSprite;
    [SerializeField] private Sprite combatSkillPanelBackgroundSprite;
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
    private GameObject heroScrollView;
    private GameObject guildHubPlaceholderRoot;
    private GameObject shopHubPlaceholderRoot;
    private Text globalTabPlaceholderTitle;
    private Text globalTabPlaceholderMessage;
    private GameObject fullscreenModalOverlay;
    private RectTransform fullscreenModalSafeAreaRoot;
    private GameObject modalDim;
    private GameObject preparingModal;

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
        EnsureCombatSurfaceLayout();
        EnsureTabContentPanel();
        EnsureHeroPlaceholderPanel();
        EnsureGuildHubPlaceholderPanel();
        EnsureShopHubPlaceholderPanel();
        EnsureEquipmentPanel();
        EnsureBottomMenuButtons();
        EnsureLaboratoryPanel();
        EnsureEquipmentSynthesisPanel();
        EnsurePreparingModal();
        SetGlobalFrameSiblingOrder();
        ShowBattle();
    }

    private void Start()
    {
        DisableLegacyInfoPanelHeroHpText();
    }

    private void OnRectTransformDimensionsChange()
    {
        ApplyHudAnchorLayout();
        ApplyTabContentSafeAreaOffset();
        ApplyFullscreenModalSafeAreaOffset();
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
        SetActiveIfPresent(heroScrollView, false);
        SetActiveIfPresent(guildHubPlaceholderRoot, false);
        SetActiveIfPresent(shopHubPlaceholderRoot, false);
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
        SetActiveIfPresent(heroScrollView, false);
        SetActiveIfPresent(guildHubPlaceholderRoot, false);
        SetActiveIfPresent(shopHubPlaceholderRoot, false);
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
        SetActiveIfPresent(heroScrollView, false);
        SetActiveIfPresent(guildHubPlaceholderRoot, false);
        SetActiveIfPresent(shopHubPlaceholderRoot, false);
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
        SetActiveIfPresent(heroScrollView, false);
        SetActiveIfPresent(guildHubPlaceholderRoot, false);
        SetActiveIfPresent(shopHubPlaceholderRoot, false);
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
        float combatSceneHeight = (1f - BottomMenuRatio) * CombatSceneContentRatio;
        float middleMax = 1f - combatSceneHeight;

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

    private void EnsureCombatSurfaceLayout()
    {
        if (combatPanel == null)
        {
            return;
        }

        Transform surfaceRoot = EnsureCombatSurfaceRoot();
        Transform sceneViewArea = EnsureCombatSurfaceArea(
            surfaceRoot,
            "CombatSceneViewArea",
            Vector2.zero,
            Vector2.one);
        EnsureCombatSurfaceImage(sceneViewArea, "CombatSceneViewSurface", new Color(0.14f, 0.18f, 0.22f, 0.16f));
        EnsureCombatSurfaceLabel(sceneViewArea, "CombatSceneViewLabel", "Combat Scene View", new Vector2(0.04f, 0.72f), new Vector2(0.96f, 0.94f), 18);

        surfaceRoot.SetAsFirstSibling();
        combatHudRectTransform?.SetAsLastSibling();

        if (infoPanel == null)
        {
            return;
        }

        CleanupMisplacedCombatSurfaceArea("CombatInfoArea", infoPanel.transform, surfaceRoot, combatPanel.transform);
        CleanupMisplacedCombatSurfaceArea("CombatSkillArea", infoPanel.transform, surfaceRoot, combatPanel.transform);
        DisableLegacyInfoPanelHeroHpText();

        float infoPanelHeight = Mathf.Max(0.01f, 1f - BottomMenuRatio - ((1f - BottomMenuRatio) * CombatSceneContentRatio));
        float skillAreaHeightRatio = Mathf.Clamp01(BottomMenuRatio / infoPanelHeight);

        Transform infoArea = EnsureCombatSurfaceArea(
            infoPanel.transform,
            "CombatInfoArea",
            new Vector2(0f, skillAreaHeightRatio),
            Vector2.one);
        EnsureCombatSurfaceImage(
            infoArea,
            "CombatInfoSurface",
            new Color(0.08f, 0.12f, 0.16f, 0.34f),
            combatInfoPanelBackgroundSprite);
        EnsureCombatSurfaceLabel(infoArea, "CombatInfoLabel", "Combat Info", new Vector2(0.04f, 0.18f), new Vector2(0.96f, 0.82f), 18);

        Transform skillArea = EnsureCombatSurfaceArea(
            infoPanel.transform,
            "CombatSkillArea",
            Vector2.zero,
            new Vector2(1f, skillAreaHeightRatio));
        EnsureCombatSurfaceImage(
            skillArea,
            "CombatSkillSurface",
            new Color(0.08f, 0.1f, 0.13f, 0.42f),
            combatSkillPanelBackgroundSprite);
        EnsureCombatSurfaceLabel(skillArea, "CombatSkillLabel", "Skills", new Vector2(0.04f, 0.78f), new Vector2(0.96f, 0.96f), 16);

        Transform skillButtonRow = EnsureSkillButtonRow(skillArea);
        for (int i = 1; i <= 4; i++)
        {
            EnsureSkillSlot(skillButtonRow, $"SkillSlot_{i:00}");
        }
    }

    private void DisableLegacyInfoPanelHeroHpText()
    {
        if (infoPanel == null)
        {
            return;
        }

        Transform heroHpText = infoPanel.transform.Find("HeroHpText");
        if (heroHpText != null)
        {
            heroHpText.gameObject.SetActive(false);
        }
    }

    private void CleanupMisplacedCombatSurfaceArea(string objectName, Transform targetParent, params Transform[] misplacedParents)
    {
        Transform target = targetParent.Find(objectName);

        for (int i = 0; i < misplacedParents.Length; i++)
        {
            Transform misplacedParent = misplacedParents[i];
            if (misplacedParent == null || misplacedParent == targetParent)
            {
                continue;
            }

            Transform misplaced = misplacedParent.Find(objectName);
            if (misplaced == null)
            {
                continue;
            }

            if (target == null)
            {
                misplaced.SetParent(targetParent, false);
                target = misplaced;
            }
            else if (misplaced != target)
            {
                DestroyCombatSurfaceObject(misplaced.gameObject);
            }
        }
    }

    private void DestroyCombatSurfaceObject(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }

    private Transform EnsureCombatSurfaceRoot()
    {
        Transform existing = combatPanel.transform.Find("CombatSurfaceRoot");
        GameObject rootObject = existing != null
            ? existing.gameObject
            : new GameObject("CombatSurfaceRoot", typeof(RectTransform));
        rootObject.transform.SetParent(combatPanel.transform, false);

        RectTransform rectTransform = rootObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = rootObject.AddComponent<RectTransform>();
        }

        StretchToParent(rectTransform);

        return rootObject.transform;
    }

    private Transform EnsureCombatSurfaceArea(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax)
    {
        Transform existing = parent.Find(objectName);
        GameObject areaObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform));
        areaObject.transform.SetParent(parent, false);

        RectTransform rectTransform = areaObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = areaObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        return areaObject.transform;
    }

    private Image EnsureCombatSurfaceImage(Transform parent, string objectName, Color color, Sprite sprite = null)
    {
        Transform existing = parent.Find(objectName);
        GameObject imageObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        imageObject.transform.SetAsFirstSibling();

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = imageObject.AddComponent<RectTransform>();
        }

        StretchToParent(rectTransform);

        Image image = imageObject.GetComponent<Image>();
        if (image == null)
        {
            image = imageObject.AddComponent<Image>();
        }

        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = sprite.border != Vector4.zero
                ? Image.Type.Sliced
                : Image.Type.Simple;
            image.color = Color.white;
            image.preserveAspect = false;
        }
        else
        {
            image.sprite = null;
            image.type = Image.Type.Simple;
            image.color = color;
            image.preserveAspect = false;
        }

        image.raycastTarget = false;

        return image;
    }

    private Text EnsureCombatSurfaceLabel(Transform parent, string objectName, string label, Vector2 anchorMin, Vector2 anchorMax, int fontSize)
    {
        Transform existing = parent.Find(objectName);
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);
        textObject.transform.SetAsLastSibling();

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = textObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        if (text == null)
        {
            text = textObject.AddComponent<Text>();
        }

        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 10;
        text.resizeTextMaxSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(1f, 1f, 1f, 0.72f);
        text.raycastTarget = false;
        text.text = label;

        return text;
    }

    private Transform EnsureSkillButtonRow(Transform parent)
    {
        Transform existing = parent.Find("SkillButtonRow");
        GameObject rowObject = existing != null
            ? existing.gameObject
            : new GameObject("SkillButtonRow", typeof(RectTransform));
        rowObject.transform.SetParent(parent, false);

        RectTransform rectTransform = rowObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = rowObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = new Vector2(0.06f, 0.08f);
        rectTransform.anchorMax = new Vector2(0.94f, 0.86f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        HorizontalLayoutGroup layoutGroup = rowObject.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = rowObject.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(12, 12, 0, 0);
        layoutGroup.spacing = 18f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        rowObject.transform.SetAsLastSibling();
        return rowObject.transform;
    }

    private void EnsureSkillSlot(Transform parent, string objectName)
    {
        Transform existing = parent.Find(objectName);
        GameObject slotObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(LayoutElement));
        slotObject.transform.SetParent(parent, false);

        RectTransform rectTransform = slotObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = slotObject.AddComponent<RectTransform>();
        }

        rectTransform.sizeDelta = new Vector2(CombatSkillSlotSize, CombatSkillSlotSize);
        rectTransform.localScale = Vector3.one;

        Image slotImage = slotObject.GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.raycastTarget = false;
        }

        LayoutElement layoutElement = slotObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = slotObject.AddComponent<LayoutElement>();
        }

        layoutElement.minWidth = CombatSkillSlotSize;
        layoutElement.preferredWidth = CombatSkillSlotSize;
        layoutElement.minHeight = CombatSkillSlotSize;
        layoutElement.preferredHeight = CombatSkillSlotSize;
        layoutElement.flexibleWidth = 0f;
        layoutElement.flexibleHeight = 0f;

        EnsureSkillSlotImage(slotObject.transform, "FrameImage", new Color(0.7f, 0.76f, 0.84f, 0.34f), Vector2.zero, Vector2.one, true);
        EnsureSkillSlotImage(slotObject.transform, "IconImage", new Color(0.2f, 0.25f, 0.31f, 0.42f), new Vector2(0.16f, 0.16f), new Vector2(0.84f, 0.84f), true);
        EnsureSkillSlotImage(slotObject.transform, "CooldownDim", new Color(0f, 0f, 0f, 0.58f), Vector2.zero, Vector2.one, false);
        EnsureSkillSlotText(slotObject.transform, "CooldownText", "0", false);
        EnsureSkillSlotImage(slotObject.transform, "LockOverlay", new Color(0f, 0f, 0f, 0.68f), Vector2.zero, Vector2.one, false);
        EnsureSkillSlotImage(slotObject.transform, "HighlightImage", new Color(1f, 0.92f, 0.45f, 0.34f), Vector2.zero, Vector2.one, false);
    }

    private Image EnsureSkillSlotImage(Transform parent, string objectName, Color color, Vector2 anchorMin, Vector2 anchorMax, bool isActive)
    {
        Transform existing = parent.Find(objectName);
        GameObject imageObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        imageObject.transform.SetAsLastSibling();

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = imageObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.GetComponent<Image>();
        if (image == null)
        {
            image = imageObject.AddComponent<Image>();
        }

        image.color = color;
        image.raycastTarget = false;

        imageObject.SetActive(isActive);
        return image;
    }

    private Text EnsureSkillSlotText(Transform parent, string objectName, string label, bool isActive)
    {
        Transform existing = parent.Find(objectName);
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);
        textObject.transform.SetAsLastSibling();

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = textObject.AddComponent<RectTransform>();
        }

        StretchToParent(rectTransform);

        Text text = textObject.GetComponent<Text>();
        if (text == null)
        {
            text = textObject.AddComponent<Text>();
        }

        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 22;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 12;
        text.resizeTextMaxSize = 22;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = label;

        textObject.SetActive(isActive);
        return text;
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
        currentEquipmentEntryContext = EquipmentEntryContext.None;
        currentGlobalTabState = GlobalTabState.Hero;
        ApplyGlobalTabSurfaceState();
        HideGlobalTabPlaceholder();
        EnsureHeroPlaceholderPanel();
        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
        SetActiveIfPresent(tabContentPanel, true);
        SetActiveIfPresent(heroScrollView, true);
        SetActiveIfPresent(guildHubPlaceholderRoot, false);
        SetActiveIfPresent(shopHubPlaceholderRoot, false);
        SetActiveIfPresent(equipmentPanel, false);
        equipmentPanelController?.HidePanel();
        laboratoryPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();
        heroScrollView.transform.SetAsLastSibling();
        SetBottomMenuAsLastSibling();
    }

    private void ShowLaboratoryTabPlaceholder()
    {
        ShowLaboratory();
    }

    private void ShowGuildTabPlaceholder()
    {
        ShowHubPlaceholder(GlobalTabState.Guild, EnsureGuildHubPlaceholderPanel);
    }

    private void ShowShopTabPlaceholder()
    {
        ShowHubPlaceholder(GlobalTabState.Shop, EnsureShopHubPlaceholderPanel);
    }

    private void ShowHubPlaceholder(GlobalTabState tabState, System.Action ensureHubPanel)
    {
        currentEquipmentEntryContext = EquipmentEntryContext.None;
        currentGlobalTabState = tabState;
        ApplyGlobalTabSurfaceState();
        HideGlobalTabPlaceholder();
        ensureHubPanel?.Invoke();

        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
        SetActiveIfPresent(tabContentPanel, true);
        SetActiveIfPresent(heroScrollView, false);
        SetActiveIfPresent(guildHubPlaceholderRoot, tabState == GlobalTabState.Guild);
        SetActiveIfPresent(shopHubPlaceholderRoot, tabState == GlobalTabState.Shop);
        SetActiveIfPresent(equipmentPanel, false);
        equipmentPanelController?.HidePanel();
        laboratoryPanelController?.HidePanel();
        equipmentSynthesisPanelController?.HidePanel();

        GameObject hubRoot = tabState == GlobalTabState.Guild
            ? guildHubPlaceholderRoot
            : shopHubPlaceholderRoot;
        hubRoot?.transform.SetAsLastSibling();
        SetBottomMenuAsLastSibling();
    }

    public void ShowPreparingModal()
    {
        EnsurePreparingModal();
        if (modalDim == null || preparingModal == null)
        {
            return;
        }

        if (fullscreenModalOverlay != null)
        {
            fullscreenModalOverlay.SetActive(true);
            fullscreenModalOverlay.transform.SetAsLastSibling();
        }

        ApplyFullscreenModalSafeAreaOffset();
        modalDim.SetActive(true);
        preparingModal.SetActive(true);
        modalDim.transform.SetAsFirstSibling();
        preparingModal.transform.SetAsLastSibling();
    }

    public void HidePreparingModal()
    {
        if (preparingModal != null)
        {
            preparingModal.SetActive(false);
        }

        if (modalDim != null)
        {
            modalDim.SetActive(false);
        }

        if (fullscreenModalOverlay != null)
        {
            fullscreenModalOverlay.SetActive(false);
        }
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
        SetActiveIfPresent(heroScrollView, false);
        SetActiveIfPresent(guildHubPlaceholderRoot, false);
        SetActiveIfPresent(shopHubPlaceholderRoot, false);
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

    private void EnsurePreparingModal()
    {
        Transform modalOverlay = EnsureFullscreenModalOverlay();
        if (modalOverlay == null)
        {
            return;
        }

        DisableLegacyPopupModalObjects();

        modalDim = EnsureFullscreenModalDim(modalOverlay);
        fullscreenModalSafeAreaRoot = EnsureFullscreenModalSafeAreaRoot(modalOverlay);
        preparingModal = EnsurePreparingModalCard(fullscreenModalSafeAreaRoot);
        ApplyFullscreenModalSafeAreaOffset();

        modalDim.SetActive(false);
        preparingModal.SetActive(false);
        modalDim.transform.SetAsFirstSibling();
        preparingModal.transform.SetAsLastSibling();
        fullscreenModalOverlay.SetActive(false);
    }

    private Transform EnsureFullscreenModalOverlay()
    {
        Transform parent = FindSceneDescendantByName("FullscreenPresentationLayer");
        if (parent == null)
        {
            parent = FindOverlayCanvas();
        }

        if (parent == null)
        {
            return null;
        }

        Transform existing = parent.Find("FullscreenModalOverlay");
        fullscreenModalOverlay = existing != null
            ? existing.gameObject
            : new GameObject("FullscreenModalOverlay", typeof(RectTransform));
        fullscreenModalOverlay.transform.SetParent(parent, false);
        fullscreenModalOverlay.layer = parent.gameObject.layer;

        RectTransform rectTransform = fullscreenModalOverlay.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = fullscreenModalOverlay.AddComponent<RectTransform>();
        }

        StretchToParent(rectTransform);
        fullscreenModalOverlay.transform.SetAsLastSibling();

        return fullscreenModalOverlay.transform;
    }

    private GameObject EnsureFullscreenModalDim(Transform modalOverlay)
    {
        Transform existing = modalOverlay.Find("FullscreenModalDim");
        GameObject dimObject = existing != null
            ? existing.gameObject
            : new GameObject("FullscreenModalDim", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dimObject.transform.SetParent(modalOverlay, false);
        dimObject.layer = modalOverlay.gameObject.layer;

        RectTransform rectTransform = dimObject.GetComponent<RectTransform>();
        StretchToParent(rectTransform);

        Image image = dimObject.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.58f);
        image.raycastTarget = true;

        return dimObject;
    }

    private RectTransform EnsureFullscreenModalSafeAreaRoot(Transform modalOverlay)
    {
        Transform existing = modalOverlay.Find("FullscreenModalSafeAreaRoot");
        GameObject safeAreaObject = existing != null
            ? existing.gameObject
            : new GameObject("FullscreenModalSafeAreaRoot", typeof(RectTransform));
        safeAreaObject.transform.SetParent(modalOverlay, false);
        safeAreaObject.layer = modalOverlay.gameObject.layer;

        RectTransform rectTransform = safeAreaObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = safeAreaObject.AddComponent<RectTransform>();
        }

        safeAreaObject.transform.SetAsLastSibling();
        return rectTransform;
    }

    private void ApplyFullscreenModalSafeAreaOffset()
    {
        if (fullscreenModalSafeAreaRoot == null)
        {
            return;
        }

        fullscreenModalSafeAreaRoot.anchorMin = Vector2.zero;
        fullscreenModalSafeAreaRoot.anchorMax = Vector2.one;
        fullscreenModalSafeAreaRoot.anchoredPosition = Vector2.zero;
        fullscreenModalSafeAreaRoot.sizeDelta = Vector2.zero;

        RectTransform parentRect = fullscreenModalSafeAreaRoot.parent as RectTransform;
        if (parentRect == null || Screen.width <= 0 || Screen.height <= 0)
        {
            fullscreenModalSafeAreaRoot.offsetMin = Vector2.zero;
            fullscreenModalSafeAreaRoot.offsetMax = Vector2.zero;
            return;
        }

        Rect safeArea = Screen.safeArea;
        Vector2 parentSize = parentRect.rect.size;
        fullscreenModalSafeAreaRoot.offsetMin = new Vector2(
            safeArea.xMin / Screen.width * parentSize.x,
            safeArea.yMin / Screen.height * parentSize.y);
        fullscreenModalSafeAreaRoot.offsetMax = new Vector2(
            -(Screen.width - safeArea.xMax) / Screen.width * parentSize.x,
            -(Screen.height - safeArea.yMax) / Screen.height * parentSize.y);
    }

    private void DisableLegacyPopupModalObjects()
    {
        Transform popupOverlay = FindPopupOverlay();
        if (popupOverlay == null)
        {
            return;
        }

        Transform legacyDim = popupOverlay.Find("ModalDim");
        if (legacyDim != null)
        {
            legacyDim.gameObject.SetActive(false);
        }

        Transform legacyModal = popupOverlay.Find("PreparingModal");
        if (legacyModal != null)
        {
            legacyModal.gameObject.SetActive(false);
        }
    }

    private GameObject EnsurePreparingModalCard(Transform modalSafeAreaRoot)
    {
        Transform existing = modalSafeAreaRoot.Find("PreparingModal");
        GameObject modalObject = existing != null
            ? existing.gameObject
            : new GameObject("PreparingModal", typeof(RectTransform));
        modalObject.transform.SetParent(modalSafeAreaRoot, false);
        modalObject.layer = modalSafeAreaRoot.gameObject.layer;

        RectTransform rectTransform = modalObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(520f, 300f);

        GameObject cardObject = EnsureModalCardSurface(modalObject.transform);
        EnsureModalText(cardObject.transform);
        EnsureModalCloseButton(cardObject.transform);

        return modalObject;
    }

    private GameObject EnsureModalCardSurface(Transform parent)
    {
        Transform existing = parent.Find("ModalCard");
        GameObject cardObject = existing != null
            ? existing.gameObject
            : new GameObject("ModalCard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cardObject.transform.SetParent(parent, false);

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        StretchToParent(rectTransform);

        Image image = cardObject.GetComponent<Image>();
        image.color = new Color(0.11f, 0.12f, 0.16f, 0.96f);
        image.raycastTarget = true;

        return cardObject;
    }

    private Text EnsureModalText(Transform parent)
    {
        Transform existing = parent.Find("PreparingText");
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject("PreparingText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.12f, 0.48f);
        rectTransform.anchorMax = new Vector2(0.88f, 0.82f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 34;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 20;
        text.resizeTextMaxSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = "\uC900\uBE44\uC911\uC785\uB2C8\uB2E4";

        return text;
    }

    private Button EnsureModalCloseButton(Transform parent)
    {
        Transform existing = parent.Find("CloseButton");
        GameObject buttonObject = existing != null
            ? existing.gameObject
            : new GameObject("CloseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.32f, 0.14f);
        rectTransform.anchorMax = new Vector2(0.68f, 0.34f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.25f, 0.30f, 0.38f, 1f);
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HidePreparingModal);

        Text text = EnsureModalButtonText(buttonObject.transform);
        text.text = "\uD655\uC778";

        return button;
    }

    private Text EnsureModalButtonText(Transform parent)
    {
        Transform existing = parent.Find("Text");
        GameObject textObject = existing != null
            ? existing.gameObject
            : new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        StretchToParent(rectTransform);

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 24;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 16;
        text.resizeTextMaxSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
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
            FindObjectsInactive.Include);
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

    private void EnsureHeroPlaceholderPanel()
    {
        EnsureTabContentPanel();

        Transform existing = tabContentPanel.transform.Find("GO_HeroScrollView");
        heroScrollView = existing != null
            ? existing.gameObject
            : new GameObject("GO_HeroScrollView", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect));
        heroScrollView.transform.SetParent(tabContentPanel.transform, false);
        heroScrollView.transform.SetAsLastSibling();
        StretchToParent(heroScrollView.GetComponent<RectTransform>());

        Image scrollImage = heroScrollView.GetComponent<Image>();
        scrollImage.color = new Color(1f, 1f, 1f, 0f);
        scrollImage.raycastTarget = true;

        RectTransform viewport = EnsureHeroViewport(heroScrollView.transform);
        RectTransform content = EnsureHeroScrollContent(viewport);

        ScrollRect scrollRect = heroScrollView.GetComponent<ScrollRect>();
        scrollRect.content = content;
        scrollRect.viewport = viewport;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.inertia = true;
        scrollRect.scrollSensitivity = 28f;
        scrollRect.horizontalScrollbar = null;
        scrollRect.verticalScrollbar = null;

        EnsureHeroProfileArea(content);
        EnsureHeroStatsArea(content);
        EnsureHeroEquipmentArea(content);
        EnsureHeroActionArea(content);

        heroScrollView.SetActive(false);
    }

    private void EnsureGuildHubPlaceholderPanel()
    {
        string[] titles =
        {
            "\uCD9C\uC11D\uCCB4\uD06C",
            "\uD559\uD68C\uC6D0 \uBA85\uB2E8",
            "\uD559\uD68C \uBBF8\uC158",
            "\uD559\uD68C \uC0C1\uC810",
            "\uACF5\uD5CC \uBCF4\uC0C1",
            "\uD559\uD68C \uAE30\uB85D"
        };
        string[] descriptions =
        {
            "\uD558\uB8E8 \uD55C \uBC88 \uCC38\uC5EC\uD558\uB294 \uD559\uD68C \uCD9C\uC11D \uBCF4\uC0C1",
            "\uD559\uD68C\uC6D0 \uC815\uBCF4\uC640 \uD65C\uB3D9 \uC0C1\uD0DC",
            "\uD559\uD68C \uACF5\uB3D9 \uBAA9\uD45C\uC640 \uC9C4\uD589 \uD604\uD669",
            "\uD559\uD68C \uD65C\uB3D9\uC73C\uB85C \uC5BB\uB294 \uAD50\uD658 \uD488\uBAA9",
            "\uACF5\uD5CC\uB3C4 \uB204\uC801 \uBCF4\uC0C1",
            "\uD559\uD68C \uD65C\uB3D9 \uC774\uB825\uACFC \uC54C\uB9BC"
        };

        guildHubPlaceholderRoot = EnsureHubPlaceholderPanel(
            "GO_GuildHubPlaceholderRoot",
            "\uD559\uD68C / \uAE38\uB4DC",
            "\uD559\uD68C \uD65C\uB3D9 \uD5C8\uBE0C\uC785\uB2C8\uB2E4. \uAC1C\uBCC4 \uAE30\uB2A5\uC740 \uC900\uBE44 \uC911\uC785\uB2C8\uB2E4.",
            titles,
            descriptions);
    }

    private void EnsureShopHubPlaceholderPanel()
    {
        string[] titles =
        {
            "\uC77C\uC77C \uC0C1\uD488",
            "\uCD94\uCC9C \uC0C1\uD488",
            "\uC7AC\uD654 \uAD50\uD658",
            "\uD328\uD0A4\uC9C0"
        };
        string[] descriptions =
        {
            "\uB9E4\uC77C \uAC31\uC2E0\uB420 \uC608\uC815\uC758 \uC0C1\uD488 \uBAA9\uB85D",
            "\uD604\uC7AC \uC131\uC7A5 \uAD6C\uAC04\uC5D0 \uB9DE\uB294 \uCD94\uCC9C \uD488\uBAA9",
            "\uC7AC\uD654\uB97C \uB2E4\uB978 \uC790\uC6D0\uC73C\uB85C \uAD50\uD658",
            "\uD328\uD0A4\uC9C0 \uC0C1\uD488\uACFC \uD2B9\uBCC4 \uAD6C\uC131"
        };

        shopHubPlaceholderRoot = EnsureHubPlaceholderPanel(
            "GO_ShopHubPlaceholderRoot",
            "\uC0C1\uC810",
            "\uC0C1\uD488 \uCE74\uD14C\uACE0\uB9AC \uD5C8\uBE0C\uC785\uB2C8\uB2E4. \uAD6C\uB9E4 \uAE30\uB2A5\uC740 \uC544\uC9C1 \uC5F0\uACB0\uB418\uC9C0 \uC54A\uC558\uC2B5\uB2C8\uB2E4.",
            titles,
            descriptions);
    }

    private GameObject EnsureHubPlaceholderPanel(string objectName, string title, string description, string[] cardTitles, string[] cardDescriptions)
    {
        EnsureTabContentPanel();

        Transform existing = tabContentPanel.transform.Find(objectName);
        GameObject rootObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        rootObject.transform.SetParent(tabContentPanel.transform, false);
        StretchToParent(rootObject.GetComponent<RectTransform>());

        Image image = rootObject.GetComponent<Image>();
        image.color = new Color(0.035f, 0.045f, 0.06f, 0.28f);
        image.raycastTarget = false;

        ClearChildren(rootObject.transform);
        CreateHubHeader(rootObject.transform, title, description);
        CreateHubCardGrid(rootObject.transform, cardTitles, cardDescriptions);
        CreateHubNotice(rootObject.transform);

        rootObject.SetActive(false);
        return rootObject;
    }

    private void CreateHubHeader(Transform root, string title, string description)
    {
        GameObject header = CreateHubArea(root, "HubHeaderArea", new Vector2(0.06f, 0.78f), new Vector2(0.94f, 0.96f), new Color(0.05f, 0.07f, 0.1f, 0.58f));
        CreateHeroText(header.transform, "HubTitle", title, new Vector2(0.04f, 0.45f), new Vector2(0.96f, 0.9f), 30, TextAnchor.MiddleLeft);
        CreateHeroText(header.transform, "HubDescription", description, new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.46f), 18, TextAnchor.MiddleLeft);
    }

    private void CreateHubCardGrid(Transform root, string[] cardTitles, string[] cardDescriptions)
    {
        GameObject grid = CreateHubArea(root, "HubCardGridArea", new Vector2(0.06f, 0.22f), new Vector2(0.94f, 0.76f), new Color(0f, 0f, 0f, 0f));

        for (int i = 0; i < cardTitles.Length; i++)
        {
            int column = i % 2;
            int row = i / 2;
            int rowCount = Mathf.CeilToInt(cardTitles.Length / 2f);
            float rowHeight = 1f / Mathf.Max(1, rowCount);
            float xMin = column == 0 ? 0f : 0.515f;
            float xMax = column == 0 ? 0.485f : 1f;
            float yMax = 1f - (row * rowHeight);
            float yMin = yMax - rowHeight + 0.04f;

            CreateHubCard(
                grid.transform,
                $"HubFeatureCard_{i:00}",
                cardTitles[i],
                cardDescriptions[i],
                new Vector2(xMin, yMin),
                new Vector2(xMax, yMax - 0.02f));
        }
    }

    private void CreateHubNotice(Transform root)
    {
        GameObject notice = CreateHubArea(root, "HubNoticeArea", new Vector2(0.06f, 0.08f), new Vector2(0.94f, 0.2f), new Color(0.05f, 0.055f, 0.075f, 0.62f));
        CreateHeroText(notice.transform, "NoticeTitle", "\uC900\uBE44 \uC911", new Vector2(0.04f, 0.48f), new Vector2(0.96f, 0.88f), 20, TextAnchor.MiddleLeft);
        CreateHeroText(notice.transform, "NoticeBody", "\uCE74\uB4DC\uB97C \uB204\uB974\uBA74 \uAE30\uC874 \uC900\uBE44\uC911 \uC548\uB0B4\uAC00 \uD45C\uC2DC\uB429\uB2C8\uB2E4.", new Vector2(0.04f, 0.12f), new Vector2(0.96f, 0.52f), 16, TextAnchor.MiddleLeft);
    }

    private GameObject CreateHubArea(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject areaObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        areaObject.transform.SetParent(parent, false);

        RectTransform rectTransform = areaObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = areaObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return areaObject;
    }

    private void CreateHubCard(Transform parent, string objectName, string title, string description, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject cardObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        cardObject.transform.SetParent(parent, false);

        RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = cardObject.GetComponent<Image>();
        image.color = new Color(0.085f, 0.105f, 0.14f, 0.88f);
        image.raycastTarget = true;

        Button button = cardObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.interactable = true;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowPreparingModal);

        CreateHeroImage(cardObject.transform, "IconPlaceholder", new Vector2(0.06f, 0.28f), new Vector2(0.24f, 0.76f), new Color(0.22f, 0.26f, 0.32f, 0.72f));
        CreateHeroText(cardObject.transform, "CardTitle", title, new Vector2(0.3f, 0.58f), new Vector2(0.94f, 0.86f), 20, TextAnchor.MiddleLeft);
        CreateHeroText(cardObject.transform, "CardDescription", description, new Vector2(0.3f, 0.24f), new Vector2(0.94f, 0.58f), 15, TextAnchor.MiddleLeft);
        CreateHeroText(cardObject.transform, "CardStatus", "\uC900\uBE44\uC911", new Vector2(0.62f, 0.04f), new Vector2(0.94f, 0.22f), 14, TextAnchor.MiddleRight);
    }

    private RectTransform EnsureHeroViewport(Transform parent)
    {
        Transform existing = parent.Find("GO_HeroViewport");
        GameObject viewportObject = existing != null
            ? existing.gameObject
            : new GameObject("GO_HeroViewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D));
        viewportObject.transform.SetParent(parent, false);
        StretchToParent(viewportObject.GetComponent<RectTransform>());

        if (viewportObject.TryGetComponent(out Mask mask))
        {
            DestroyComponent(mask);
        }

        if (!viewportObject.TryGetComponent(out RectMask2D _))
        {
            viewportObject.AddComponent<RectMask2D>();
        }

        Image image = viewportObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);
        image.raycastTarget = true;

        return viewportObject.GetComponent<RectTransform>();
    }

    private RectTransform EnsureHeroScrollContent(RectTransform viewport)
    {
        Transform existing = viewport.transform.Find("GO_HeroScrollContent");
        GameObject contentObject = existing != null
            ? existing.gameObject
            : new GameObject("GO_HeroScrollContent", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentObject.transform.SetParent(viewport, false);

        RectTransform rectTransform = contentObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(0f, 0f);

        VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(24, 24, 24, 28);
        layoutGroup.spacing = 18f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter fitter = contentObject.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return rectTransform;
    }

    private void EnsureHeroProfileArea(Transform content)
    {
        Transform section = EnsureHeroSection(content, "UI_HeroProfileArea", 260f);
        ClearChildren(section);

        GameObject portrait = CreateHeroImage(section, "ProfileImagePlaceholder", new Vector2(0.05f, 0.18f), new Vector2(0.34f, 0.86f), new Color(0.2f, 0.24f, 0.3f, 0.72f));
        CreateHeroText(portrait.transform, "PortraitLabel", "\uC601\uC6C5", Vector2.zero, Vector2.one, 30, TextAnchor.MiddleCenter);
        CreateHeroText(section, "ProfileTitle", "\uB300\uD45C \uC601\uC6C5", new Vector2(0.39f, 0.68f), new Vector2(0.95f, 0.88f), 28, TextAnchor.MiddleLeft);
        CreateHeroText(section, "ProfileName", "\uC774\uB984: Placeholder", new Vector2(0.39f, 0.5f), new Vector2(0.95f, 0.66f), 22, TextAnchor.MiddleLeft);
        CreateHeroText(section, "ProfileLevel", "Lv. 000", new Vector2(0.39f, 0.34f), new Vector2(0.64f, 0.48f), 20, TextAnchor.MiddleLeft);
        CreateHeroText(section, "ProfileGrade", "\uB4F1\uAE09: -", new Vector2(0.66f, 0.34f), new Vector2(0.95f, 0.48f), 20, TextAnchor.MiddleLeft);
        CreateHeroText(section, "ProfileElement", "\uC18D\uC131: -", new Vector2(0.39f, 0.18f), new Vector2(0.95f, 0.32f), 20, TextAnchor.MiddleLeft);
    }

    private void EnsureHeroStatsArea(Transform content)
    {
        Transform section = EnsureHeroSection(content, "UI_HeroStatsArea", 300f);
        ClearChildren(section);

        CreateHeroText(section, "StatsTitle", "\uC2A4\uD0EF", new Vector2(0.05f, 0.78f), new Vector2(0.95f, 0.94f), 26, TextAnchor.MiddleLeft);
        string[] statLabels =
        {
            "\uACF5\uACA9\uB825 000",
            "\uCCB4\uB825 000",
            "\uBC29\uC5B4\uB825 000",
            "\uACF5\uACA9\uC18D\uB3C4 000",
            "\uCE58\uBA85\uD0C0 000"
        };

        for (int i = 0; i < statLabels.Length; i++)
        {
            float top = 0.72f - (i * 0.13f);
            CreateHeroText(section, $"StatLabel_{i:00}", statLabels[i], new Vector2(0.08f, top - 0.1f), new Vector2(0.92f, top), 20, TextAnchor.MiddleLeft);
        }
    }

    private void EnsureHeroEquipmentArea(Transform content)
    {
        Transform section = EnsureHeroSection(content, "UI_HeroEquipmentArea", 380f);
        ClearChildren(section);

        CreateHeroText(section, "EquipmentTitle", "\uC7A5\uBE44", new Vector2(0.05f, 0.84f), new Vector2(0.95f, 0.96f), 26, TextAnchor.MiddleLeft);
        string[] equipmentLabels =
        {
            "\uBB34\uAE30",
            "\uBC29\uC5B4\uAD6C",
            "\uC7A5\uAC11",
            "\uC2E0\uBC1C",
            "\uBAA9\uAC78\uC774",
            "\uBC18\uC9C0"
        };

        for (int i = 0; i < equipmentLabels.Length; i++)
        {
            int column = i % 3;
            int row = i / 3;
            float xMin = 0.06f + (column * 0.31f);
            float yMax = row == 0 ? 0.76f : 0.42f;
            GameObject slot = CreateHeroImage(section, $"EquipmentSlot_{i:00}", new Vector2(xMin, yMax - 0.24f), new Vector2(xMin + 0.26f, yMax), new Color(0.18f, 0.21f, 0.27f, 0.72f));
            CreateHeroText(slot.transform, "SlotLabel", equipmentLabels[i], Vector2.zero, Vector2.one, 18, TextAnchor.MiddleCenter);
        }
    }

    private void EnsureHeroActionArea(Transform content)
    {
        Transform section = EnsureHeroSection(content, "UI_HeroActionArea", 220f);
        ClearChildren(section);

        CreateHeroText(section, "ActionTitle", "\uC561\uC158", new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.9f), 26, TextAnchor.MiddleLeft);
        string[] actionLabels =
        {
            "\uAC15\uD654",
            "\uC7A5\uBE44",
            "\uD3B8\uC131",
            "\uC0C1\uC138"
        };

        for (int i = 0; i < actionLabels.Length; i++)
        {
            float xMin = 0.06f + (i * 0.235f);
            CreateHeroDisabledButton(section, $"ActionButton_{i:00}", actionLabels[i], new Vector2(xMin, 0.22f), new Vector2(xMin + 0.19f, 0.58f));
        }
    }

    private Transform EnsureHeroSection(Transform content, string objectName, float preferredHeight)
    {
        Transform existing = content.Find(objectName);
        GameObject sectionObject = existing != null
            ? existing.gameObject
            : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        sectionObject.transform.SetParent(content, false);

        RectTransform rectTransform = sectionObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.sizeDelta = new Vector2(0f, preferredHeight);

        LayoutElement layoutElement = sectionObject.GetComponent<LayoutElement>();
        layoutElement.minHeight = preferredHeight;
        layoutElement.preferredHeight = preferredHeight;
        layoutElement.flexibleHeight = 0f;

        Image image = sectionObject.GetComponent<Image>();
        image.color = new Color(0.06f, 0.075f, 0.1f, 0.68f);
        image.raycastTarget = false;

        return sectionObject.transform;
    }

    private GameObject CreateHeroImage(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return imageObject;
    }

    private Text CreateHeroText(Transform parent, string objectName, string label, Vector2 anchorMin, Vector2 anchorMax, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
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
        text.text = label;

        return text;
    }

    private void CreateHeroDisabledButton(Transform parent, string objectName, string label, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.22f, 0.25f, 0.31f, 0.42f);
        image.raycastTarget = false;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.interactable = false;
        button.onClick.RemoveAllListeners();

        CreateHeroText(buttonObject.transform, "Label", label, Vector2.zero, Vector2.one, 18, TextAnchor.MiddleCenter);
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyCombatSurfaceObject(parent.GetChild(i).gameObject);
        }
    }

    private void DestroyComponent(Component component)
    {
        if (component == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(component);
        }
        else
        {
            DestroyImmediate(component);
        }
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
