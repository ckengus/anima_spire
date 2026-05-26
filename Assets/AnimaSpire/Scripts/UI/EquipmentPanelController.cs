using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelController : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private Text aMagicBookCountText;
    [SerializeField] private Text bMagicBookCountText;
    [SerializeField] private Text currentMagicBookText;
    [SerializeField] private Text lastResultText;
    [SerializeField] private Text costText;
    [SerializeField] private Text weaponSlotLevelText;
    [SerializeField] private Text weaponSlotUpgradeCostText;
    [SerializeField] private Button summonButton;
    [SerializeField] private Button equipAButton;
    [SerializeField] private Button equipBButton;
    [SerializeField] private Button upgradeWeaponSlotButton;

    private bool isCollectionSubscribed;
    private bool isLoadoutSubscribed;
    private bool isSlotSubscribed;

    private void Awake()
    {
        EnsureReferences();
        EnsureUi();
        RefreshAll();
    }

    private void OnEnable()
    {
        EnsureReferences();
        Subscribe();
        RefreshAll();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Initialize(EquipmentManager manager)
    {
        equipmentManager = manager;
        Subscribe();
        RefreshAll();
    }

    private void EnsureReferences()
    {
        if (equipmentManager == null)
        {
            equipmentManager = FindAnyObjectByType<EquipmentManager>();
        }
    }

    private void EnsureUi()
    {
        Transform contentRoot = EnsureContentRoot();

        Text titleText = EnsureText(contentRoot, "EquipmentTitleText", "Equipment", 44, TextAnchor.MiddleCenter, 88f);
        titleText.raycastTarget = false;

        costText = EnsureText(contentRoot, "MagicBookCostText", string.Empty, 32, TextAnchor.MiddleCenter, 64f);
        aMagicBookCountText = EnsureText(contentRoot, "AMagicBookCountText", string.Empty, 34, TextAnchor.MiddleLeft, 64f);
        bMagicBookCountText = EnsureText(contentRoot, "BMagicBookCountText", string.Empty, 34, TextAnchor.MiddleLeft, 64f);
        currentMagicBookText = EnsureText(contentRoot, "CurrentMagicBookText", "MagicBook: None", 32, TextAnchor.MiddleCenter, 76f);
        weaponSlotLevelText = EnsureText(contentRoot, "WeaponSlotLevelText", "Weapon Slot Lv. 0", 30, TextAnchor.MiddleCenter, 56f);
        weaponSlotUpgradeCostText = EnsureText(contentRoot, "WeaponSlotUpgradeCostText", "Upgrade Cost: 10 Gold", 30, TextAnchor.MiddleCenter, 56f);
        upgradeWeaponSlotButton = EnsureButton(contentRoot, "UpgradeWeaponSlotButton", "Upgrade Weapon Slot", OnUpgradeWeaponSlotButtonClicked, 88f);
        lastResultText = EnsureText(contentRoot, "LastSummonResultText", "Result: None", 30, TextAnchor.MiddleCenter, 76f);

        Transform equipButtonRow = EnsureHorizontalRow(contentRoot, "EquipButtonRow", 104f);
        equipAButton = EnsureButton(equipButtonRow, "EquipAMagicBookButton", "Equip A", OnEquipAButtonClicked);
        equipBButton = EnsureButton(equipButtonRow, "EquipBMagicBookButton", "Equip B", OnEquipBButtonClicked);
        summonButton = EnsureButton(contentRoot, "SummonMagicBookButton", "Summon MagicBook", OnSummonButtonClicked, 112f);
    }

    private Transform EnsureContentRoot()
    {
        const string objectName = "EquipmentContent";
        Transform existing = FindChildByName(transform, objectName);
        GameObject contentObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform));
        contentObject.transform.SetParent(transform, false);

        RectTransform rectTransform = contentObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.06f, 0.06f);
        rectTransform.anchorMax = new Vector2(0.94f, 0.94f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = contentObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(12, 12, 8, 8);
        layoutGroup.spacing = 14f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        return contentObject.transform;
    }

    private Transform EnsureHorizontalRow(Transform parent, string objectName, float preferredHeight)
    {
        Transform existing = FindChildByName(parent, objectName);
        GameObject rowObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform));
        rowObject.transform.SetParent(parent, false);

        LayoutElement layoutElement = rowObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = rowObject.AddComponent<LayoutElement>();
        }

        layoutElement.minHeight = preferredHeight;
        layoutElement.preferredHeight = preferredHeight;

        HorizontalLayoutGroup layoutGroup = rowObject.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = rowObject.AddComponent<HorizontalLayoutGroup>();
        }

        layoutGroup.spacing = 20f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;

        return rowObject.transform;
    }

    private Text EnsureText(Transform parent, string objectName, string textValue, int fontSize, TextAnchor alignment, float preferredHeight)
    {
        Transform existing = FindChildByName(transform, objectName);
        GameObject textObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = textObject.AddComponent<LayoutElement>();
        }

        layoutElement.minHeight = preferredHeight;
        layoutElement.preferredHeight = preferredHeight;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 22;
        text.resizeTextMaxSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = textValue;

        return text;
    }

    private Button EnsureButton(Transform parent, string objectName, string label, UnityEngine.Events.UnityAction onClick, float preferredHeight = 96f)
    {
        Transform existing = FindChildByName(transform, objectName);
        GameObject buttonObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

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

        layoutElement.minHeight = preferredHeight;
        layoutElement.preferredHeight = preferredHeight;
        layoutElement.flexibleWidth = 1f;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.2f, 0.24f, 0.32f, 0.95f);
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClick);

        Text text = EnsureButtonText(buttonObject.transform, label);
        text.text = label;

        return button;
    }

    private Text EnsureButtonText(Transform buttonTransform, string label)
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
        text.fontSize = 34;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 22;
        text.resizeTextMaxSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = label;

        return text;
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == objectName)
            {
                return children[i];
            }
        }

        return null;
    }

    private void Subscribe()
    {
        if (equipmentManager == null)
        {
            return;
        }

        if (!isCollectionSubscribed)
        {
            equipmentManager.CollectionState.OnCountChanged += OnEquipmentCountChanged;
            isCollectionSubscribed = true;
        }

        if (!isLoadoutSubscribed)
        {
            equipmentManager.LoadoutState.OnEquippedMagicBookChanged += OnEquippedMagicBookChanged;
            isLoadoutSubscribed = true;
        }

        if (!isSlotSubscribed)
        {
            equipmentManager.OnWeaponSlotUpgraded += OnWeaponSlotUpgraded;
            isSlotSubscribed = true;
        }
    }

    private void Unsubscribe()
    {
        if (equipmentManager == null)
        {
            return;
        }

        if (isCollectionSubscribed)
        {
            equipmentManager.CollectionState.OnCountChanged -= OnEquipmentCountChanged;
            isCollectionSubscribed = false;
        }

        if (isLoadoutSubscribed)
        {
            equipmentManager.LoadoutState.OnEquippedMagicBookChanged -= OnEquippedMagicBookChanged;
            isLoadoutSubscribed = false;
        }

        if (isSlotSubscribed)
        {
            equipmentManager.OnWeaponSlotUpgraded -= OnWeaponSlotUpgraded;
            isSlotSubscribed = false;
        }
    }

    private void OnSummonButtonClicked()
    {
        if (equipmentManager == null)
        {
            SetLastResult("Result: Equipment system is not ready.");
            return;
        }

        equipmentManager.TrySummonMagicBook(out EquipmentDefinition _, out string message);
        SetLastResult($"Result: {message}");
        RefreshOwnedCounts();
    }

    private void OnEquipAButtonClicked()
    {
        TryEquipMagicBook(EquipmentId.AMagicBook);
    }

    private void OnEquipBButtonClicked()
    {
        TryEquipMagicBook(EquipmentId.BMagicBook);
    }

    private void OnUpgradeWeaponSlotButtonClicked()
    {
        if (equipmentManager == null)
        {
            SetLastResult("Result: Equipment system is not ready.");
            return;
        }

        equipmentManager.TryUpgradeWeaponSlot(out string message);
        SetLastResult($"Result: {message}");
        RefreshWeaponSlot();
        RefreshCurrentMagicBook();
    }

    private void TryEquipMagicBook(EquipmentId id)
    {
        if (equipmentManager == null)
        {
            SetLastResult("Result: Equipment system is not ready.");
            return;
        }

        equipmentManager.TryEquipMagicBook(id, out string message);
        SetLastResult($"Result: {message}");
        RefreshCurrentMagicBook();
    }

    private void OnEquipmentCountChanged(EquipmentId id, EquipmentTier tier, int count)
    {
        if (tier != EquipmentTier.T0)
        {
            return;
        }

        if (id == EquipmentId.AMagicBook)
        {
            aMagicBookCountText.text = $"A MagicBook T0: {count}";
            return;
        }

        if (id == EquipmentId.BMagicBook)
        {
            bMagicBookCountText.text = $"B MagicBook T0: {count}";
        }
    }

    private void OnEquippedMagicBookChanged(EquipmentStackKey? key)
    {
        SetCurrentMagicBookText(key);
    }

    private void OnWeaponSlotUpgraded(int slotLevel)
    {
        RefreshWeaponSlot();
        RefreshCurrentMagicBook();
    }

    private void RefreshAll()
    {
        RefreshCost();
        RefreshOwnedCounts();
        RefreshCurrentMagicBook();
        RefreshWeaponSlot();
    }

    private void RefreshCost()
    {
        if (costText == null)
        {
            return;
        }

        int cost = equipmentManager != null ? equipmentManager.MagicBookCost : 10;
        costText.text = $"Summon Cost: {cost} Gold";
    }

    private void RefreshWeaponSlot()
    {
        int slotLevel = equipmentManager != null ? equipmentManager.WeaponSlotLevel : 0;

        if (weaponSlotLevelText != null)
        {
            weaponSlotLevelText.text = $"Weapon Slot Lv. {slotLevel}";
        }

        if (weaponSlotUpgradeCostText != null)
        {
            int cost = equipmentManager != null ? equipmentManager.GetWeaponSlotUpgradeCost(slotLevel) : 10;
            weaponSlotUpgradeCostText.text = $"Upgrade Cost: {cost} Gold";
        }
    }

    private void RefreshOwnedCounts()
    {
        if (aMagicBookCountText == null || bMagicBookCountText == null)
        {
            return;
        }

        int aCount = equipmentManager != null ? equipmentManager.GetOwnedCount(EquipmentId.AMagicBook, EquipmentTier.T0) : 0;
        int bCount = equipmentManager != null ? equipmentManager.GetOwnedCount(EquipmentId.BMagicBook, EquipmentTier.T0) : 0;
        aMagicBookCountText.text = $"A MagicBook T0: {aCount}";
        bMagicBookCountText.text = $"B MagicBook T0: {bCount}";
    }

    private void RefreshCurrentMagicBook()
    {
        if (currentMagicBookText == null)
        {
            return;
        }

        if (equipmentManager != null && equipmentManager.TryGetEquippedMagicBook(out EquipmentStackKey key))
        {
            SetCurrentMagicBookText(key);
            return;
        }

        SetCurrentMagicBookText(null);
    }

    private void SetCurrentMagicBookText(EquipmentStackKey? key)
    {
        if (currentMagicBookText == null)
        {
            return;
        }

        if (!key.HasValue)
        {
            currentMagicBookText.text = "MagicBook: None";
            return;
        }

        EquipmentStackKey value = key.Value;
        if (EquipmentCatalog.TryGetDefinition(value.id, value.tier, out EquipmentDefinition definition))
        {
            int totalBonus = equipmentManager != null ? equipmentManager.GetEquippedMagicBookBonusAttackPower() : definition.bonusAttackPower;
            currentMagicBookText.text = $"MagicBook: {definition.displayName} {value.tier} (+{totalBonus} ATK)";
            return;
        }

        currentMagicBookText.text = $"MagicBook: {value.id} {value.tier}";
    }

    private void SetLastResult(string text)
    {
        if (lastResultText != null)
        {
            lastResultText.text = text;
        }
    }
}
