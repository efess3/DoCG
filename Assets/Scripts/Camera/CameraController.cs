using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Shake Settings")]
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.2f;
    private float dampingSpeed = 1.0f;
    private Vector3 targetPos;

    public static CameraFollow Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void LateUpdate()
    {
        if (player == null) return;

        targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);

        if (shakeDuration > 0 && GameSettingsManager.ScreenShake)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0; // Keep Z constant
            transform.position = targetPos + shakeOffset;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            transform.position = targetPos;
        }
    }

    /// <summary>
    /// Triggers screen shake with a specific duration and magnitude.
    /// </summary>
    public void TriggerShake(float duration = 0.15f, float magnitude = 0.2f)
    {
        if (!GameSettingsManager.ScreenShake) return;
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
