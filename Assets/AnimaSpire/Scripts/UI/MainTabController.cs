using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainTabController : MonoBehaviour
{
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject bottomMenuPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private EquipmentManager equipmentManager;

    private void Awake()
    {
        EnsureReferences();
        EnsureEventSystem();
        EnsureEquipmentManager();
        EnsureBottomMenuButtons();
        EnsureEquipmentPanel();
        ShowBattle();
    }

    public void ShowBattle()
    {
        SetActiveIfPresent(combatPanel, true);
        SetActiveIfPresent(infoPanel, true);
        SetActiveIfPresent(equipmentPanel, false);
    }

    public void ShowEquipment()
    {
        SetActiveIfPresent(combatPanel, false);
        SetActiveIfPresent(infoPanel, false);
        SetActiveIfPresent(equipmentPanel, true);
    }

    private void EnsureReferences()
    {
        combatPanel = FindPanelIfMissing(combatPanel, "CombatPanel");
        infoPanel = FindPanelIfMissing(infoPanel, "InfoPanel");
        bottomMenuPanel = FindPanelIfMissing(bottomMenuPanel, "BottomMenuPanel");
        equipmentPanel = FindPanelIfMissing(equipmentPanel, "EquipmentPanel");
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

    private void EnsureBottomMenuButtons()
    {
        if (bottomMenuPanel == null)
        {
            return;
        }

        EnsureButton("BattleButton", "Battle", new Vector2(0f, 0f), new Vector2(0.5f, 1f), ShowBattle);
        EnsureButton("EquipmentButton", "Equipment", new Vector2(0.5f, 0f), new Vector2(1f, 1f), ShowEquipment);
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    private Button EnsureButton(string objectName, string label, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
    {
        Transform existing = bottomMenuPanel.transform.Find(objectName);
        GameObject buttonObject = existing != null ? existing.gameObject : CreateButtonObject(objectName);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(-24f, -32f);

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
        text.fontSize = 34;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        text.text = label;

        return text;
    }

    private void EnsureEquipmentPanel()
    {
        if (equipmentPanel == null)
        {
            equipmentPanel = CreateEquipmentPanel();
        }

        equipmentPanel.transform.SetAsLastSibling();
        EnsureEquipmentPanelController();
    }

    private GameObject CreateEquipmentPanel()
    {
        GameObject panelObject = new GameObject("EquipmentPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(transform, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 0.1f);
        rectTransform.anchorMax = new Vector2(1f, 0.9f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;

        Image image = panelObject.GetComponent<Image>();
        image.color = new Color(0.09f, 0.1f, 0.13f, 0.96f);
        image.raycastTarget = true;

        return panelObject;
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
        if (!equipmentPanel.TryGetComponent(out EquipmentPanelController controller))
        {
            controller = equipmentPanel.AddComponent<EquipmentPanelController>();
        }

        controller.Initialize(equipmentManager);
    }

    private void SetActiveIfPresent(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}
