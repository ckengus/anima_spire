using UnityEngine;
using UnityEngine.UI;

public sealed class DebugResetProgressButtonController : MonoBehaviour
{
    private const string DefaultLabel = "DEBUG RESET";
    private const string ConfirmLabel = "RESET?";

    [SerializeField] private Button resetButton;
    [SerializeField] private Text label;
    [SerializeField] private ProgressSaveManager progressSaveManager;
    [SerializeField] private float confirmTimeoutSeconds = 5f;

    private bool waitingForConfirm;
    private float confirmDeadline;

    private void Awake()
    {
        BindButton();
        ResetLabel();
    }

    private void Update()
    {
        if (!waitingForConfirm)
        {
            return;
        }

        if (Time.unscaledTime >= confirmDeadline)
        {
            ClearConfirmState();
        }
    }

    private void OnDestroy()
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(HandleResetButtonClicked);
        }
    }

    public void Initialize(Button button, Text buttonLabel, ProgressSaveManager manager)
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(HandleResetButtonClicked);
        }

        resetButton = button;
        label = buttonLabel;
        progressSaveManager = manager;

        BindButton();
        ResetLabel();
    }

    private void BindButton()
    {
        if (resetButton == null)
        {
            resetButton = GetComponent<Button>();
        }

        if (label == null)
        {
            label = GetComponentInChildren<Text>();
        }

        if (resetButton == null)
        {
            return;
        }

        resetButton.onClick.RemoveListener(HandleResetButtonClicked);
        resetButton.onClick.AddListener(HandleResetButtonClicked);
    }

    private void HandleResetButtonClicked()
    {
        if (!waitingForConfirm)
        {
            waitingForConfirm = true;
            confirmDeadline = Time.unscaledTime + Mathf.Max(confirmTimeoutSeconds, 0.1f);
            SetLabel(ConfirmLabel);
            return;
        }

        ExecuteReset();
    }

    private void ExecuteReset()
    {
        ProgressSaveManager manager = progressSaveManager != null
            ? progressSaveManager
            : FindAnyObjectByType<ProgressSaveManager>();

        if (manager == null)
        {
            ClearConfirmState();
            Debug.LogWarning("Debug reset progress failed because ProgressSaveManager was not found.");
            return;
        }

        manager.ResetProgressForDebug();
    }

    private void ClearConfirmState()
    {
        waitingForConfirm = false;
        ResetLabel();
    }

    private void ResetLabel()
    {
        SetLabel(DefaultLabel);
    }

    private void SetLabel(string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }
}
