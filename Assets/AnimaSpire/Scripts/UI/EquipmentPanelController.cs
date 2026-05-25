using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelController : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private Text aMagicBookCountText;
    [SerializeField] private Text bMagicBookCountText;
    [SerializeField] private Text lastResultText;
    [SerializeField] private Text costText;
    [SerializeField] private Button summonButton;

    private bool isSubscribed;

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
        Text titleText = EnsureText("EquipmentTitleText", "Equipment Panel", 42, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.92f));
        titleText.raycastTarget = false;

        costText = EnsureText("MagicBookCostText", string.Empty, 30, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.66f), new Vector2(0.92f, 0.76f));
        aMagicBookCountText = EnsureText("AMagicBookCountText", string.Empty, 32, TextAnchor.MiddleLeft, new Vector2(0.16f, 0.52f), new Vector2(0.84f, 0.62f));
        bMagicBookCountText = EnsureText("BMagicBookCountText", string.Empty, 32, TextAnchor.MiddleLeft, new Vector2(0.16f, 0.42f), new Vector2(0.84f, 0.52f));
        lastResultText = EnsureText("LastSummonResultText", "Last Result: None", 30, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.24f), new Vector2(0.92f, 0.38f));
        summonButton = EnsureButton("SummonMagicBookButton", "Summon MagicBook", new Vector2(0.2f, 0.08f), new Vector2(0.8f, 0.2f));
    }

    private Text EnsureText(string objectName, string textValue, int fontSize, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax)
    {
        Transform existing = transform.Find(objectName);
        GameObject textObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(transform, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = textValue;

        return text;
    }

    private Button EnsureButton(string objectName, string label, Vector2 anchorMin, Vector2 anchorMax)
    {
        Transform existing = transform.Find(objectName);
        GameObject buttonObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(transform, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.2f, 0.24f, 0.32f, 0.95f);
        image.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSummonButtonClicked);

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
        text.fontSize = 32;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = label;

        return text;
    }

    private void Subscribe()
    {
        if (equipmentManager == null || isSubscribed)
        {
            return;
        }

        equipmentManager.CollectionState.OnCountChanged += OnEquipmentCountChanged;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (equipmentManager == null || !isSubscribed)
        {
            return;
        }

        equipmentManager.CollectionState.OnCountChanged -= OnEquipmentCountChanged;
        isSubscribed = false;
    }

    private void OnSummonButtonClicked()
    {
        if (equipmentManager == null)
        {
            SetLastResult("Last Result: Equipment system is not ready.");
            return;
        }

        equipmentManager.TrySummonMagicBook(out EquipmentDefinition _, out string message);
        SetLastResult($"Last Result: {message}");
        RefreshOwnedCounts();
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

    private void RefreshAll()
    {
        RefreshCost();
        RefreshOwnedCounts();
    }

    private void RefreshCost()
    {
        if (costText == null)
        {
            return;
        }

        int cost = equipmentManager != null ? equipmentManager.MagicBookCost : 10;
        costText.text = $"MagicBook Summon Cost: {cost} Gold";
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

    private void SetLastResult(string text)
    {
        if (lastResultText != null)
        {
            lastResultText.text = text;
        }
    }
}
