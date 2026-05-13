using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    [Header("Visual Settings")]
    [SerializeField] private Image healthFillImage;

    //?? KÉO SLIDER VÀO
    [SerializeField] private RectTransform sliderRectTransform;

    private Camera _mainCamera;

    void Awake()
    {
        SetupReferences();
    }

    void LateUpdate()
    {
        ExecutePositionFollow();
    }

    private void SetupReferences()
    {
        _mainCamera = Camera.main;
    }

    private void ExecutePositionFollow()
    {
        if (CanUpdatePosition())
        {
            ApplyScreenPosition();
        }
    }

    private bool CanUpdatePosition()
    {
        return playerTransform != null && _mainCamera != null && sliderRectTransform != null;
    }

    private void ApplyScreenPosition()
    {
        Vector3 worldPos = playerTransform.position + offset;
        Vector3 screenPoint = _mainCamera.WorldToScreenPoint(worldPos);

        // N?U screenPoint.z > 0 ngh?a là nhân v?t ?ang ? phía tr??c camera
        if (screenPoint.z > 0)
        {
            // GÁN V? TRÍ CHO SLIDER (thay vì gán cho Canvas)
            sliderRectTransform.position = screenPoint;
        }
    }

    public void UpdateHealthVisual(float healthPercent)
    {
        HandleVisualUpdate(healthPercent);
    }

    private void HandleVisualUpdate(float percent)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = percent;
        }
    }
}