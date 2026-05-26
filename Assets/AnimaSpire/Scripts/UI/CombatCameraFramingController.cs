using UnityEngine;

public sealed class CombatCameraFramingController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float combatAreaRatio = 0.5f;
    [SerializeField] private float focusViewportY = 0.65f;

    private Vector3 baseCameraPosition;
    private bool hasBaseCameraPosition;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        CaptureBaseCameraPosition();
    }

    private void Start()
    {
        ApplyFraming();
    }

    public void Initialize(Camera cameraToFrame, float areaRatio, float viewportFocusY)
    {
        targetCamera = cameraToFrame != null ? cameraToFrame : targetCamera;
        combatAreaRatio = Mathf.Clamp01(areaRatio);
        focusViewportY = Mathf.Clamp01(viewportFocusY);
        CaptureBaseCameraPosition();
        ApplyFraming();
    }

    public void ApplyFraming()
    {
        if (targetCamera == null || !targetCamera.orthographic)
        {
            return;
        }

        CaptureBaseCameraPosition();

        float viewportOffsetFromCenter = focusViewportY - 0.5f;
        float worldYOffset = -viewportOffsetFromCenter * targetCamera.orthographicSize * 2f;

        Vector3 position = baseCameraPosition;
        position.y += worldYOffset;
        targetCamera.transform.position = position;
    }

    private void CaptureBaseCameraPosition()
    {
        if (hasBaseCameraPosition || targetCamera == null)
        {
            return;
        }

        baseCameraPosition = targetCamera.transform.position;
        hasBaseCameraPosition = true;
    }
}
