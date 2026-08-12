using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UICanvasAutoScaler : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    [Range(0, 1)] public float matchWidthOrHeight = 1f;

    private void Awake()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = matchWidthOrHeight;
        }
    }
}
