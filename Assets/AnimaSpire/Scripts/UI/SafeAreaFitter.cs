using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _targetRect;
    private Rect _lastSafeArea;

    private void Awake()
    {
        _targetRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (_targetRect == null)
        {
            _targetRect = GetComponent<RectTransform>();
        }

        _lastSafeArea = default;
        ApplySafeAreaIfChanged();
    }

    private void Update()
    {
        ApplySafeAreaIfChanged();
    }

    private void ApplySafeAreaIfChanged()
    {
        Rect safeArea = Screen.safeArea;
        if (safeArea == _lastSafeArea)
        {
            return;
        }

        if (ApplySafeArea(safeArea))
        {
            _lastSafeArea = safeArea;
        }
    }

    private bool ApplySafeArea(Rect safeArea)
    {
        if (_targetRect == null || Screen.width <= 0 || Screen.height <= 0)
        {
            return false;
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x = Mathf.Clamp01(anchorMin.x / Screen.width);
        anchorMin.y = Mathf.Clamp01(anchorMin.y / Screen.height);
        anchorMax.x = Mathf.Clamp01(anchorMax.x / Screen.width);
        anchorMax.y = Mathf.Clamp01(anchorMax.y / Screen.height);

        _targetRect.anchorMin = anchorMin;
        _targetRect.anchorMax = anchorMax;
        _targetRect.offsetMin = Vector2.zero;
        _targetRect.offsetMax = Vector2.zero;

        return true;
    }
}
