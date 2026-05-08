using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    [Header("Scaling")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float animationDuration = 0.15f;

    [Header("Visual Effects")]
    [SerializeField] private bool useColorHighlight = true;
    [SerializeField] private Color highlightColor = new Color(1.2f, 1.2f, 1.2f, 1f);
    [SerializeField] private GameObject glowOverlay;

    [Header("Text Effects")]
    [SerializeField] private bool highlightText = true;
    [SerializeField] private Color textHighlightColor = Color.white;
    private Color originalTextColor;
    private TextMeshProUGUI buttonText;

    [Header("Audio")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    private Image buttonImage;
    private Image glowImage;
    private Vector3 originalScale;
    private Color originalColor;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;

        if (buttonImage != null)
            originalColor = buttonImage.color;

        if (buttonText != null)
            originalTextColor = buttonText.color;

        // Ensure we have an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (glowOverlay != null)
        {
            glowImage = glowOverlay.GetComponent<Image>();
            glowOverlay.SetActive(false);
            if (glowImage != null)
            {
                Color c = glowImage.color;
                c.a = 0;
                glowImage.color = c;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopActiveCoroutine();
        if (glowOverlay != null) glowOverlay.SetActive(true);

        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound, 0.5f);

        activeCoroutine = StartCoroutine(AnimateButton(originalScale * hoverScale, highlightColor, textHighlightColor, 1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(AnimateButton(originalScale, originalColor, originalTextColor, 0f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopActiveCoroutine();
        // Darken RGB but keep original Alpha from highlightColor
        Color clickColor = new Color(highlightColor.r * 0.8f, highlightColor.g * 0.8f, highlightColor.b * 0.8f, highlightColor.a);
        activeCoroutine = StartCoroutine(AnimateButton(originalScale * clickScale, clickColor, textHighlightColor, 1f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound, 0.7f);

        StopActiveCoroutine();

        // Check if object is still active before starting coroutine
        if (gameObject.activeInHierarchy)
        {
            activeCoroutine = StartCoroutine(AnimateButton(originalScale * hoverScale, highlightColor, textHighlightColor, 1f));
        }
        else
        {
            // Reset instantly if object is being disabled
            transform.localScale = originalScale;
            if (buttonImage != null) buttonImage.color = originalColor;
            if (buttonText != null) buttonText.color = originalTextColor;
        }
    }

    private void StopActiveCoroutine() { if (activeCoroutine != null) StopCoroutine(activeCoroutine); }

    private IEnumerator AnimateButton(Vector3 targetScale, Color targetColor, Color targetTextColor, float targetGlowAlpha)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = buttonImage != null ? buttonImage.color : Color.white;
        Color startTextColor = buttonText != null ? buttonText.color : Color.white;
        float startGlowAlpha = glowImage != null ? glowImage.color.a : 0;
        float elapsed = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / animationDuration);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (buttonImage != null && useColorHighlight)
                buttonImage.color = Color.Lerp(startColor, targetColor, t);

            if (buttonText != null && highlightText)
                buttonText.color = Color.Lerp(startTextColor, targetTextColor, t);

            if (glowImage != null)
            {
                Color c = glowImage.color;
                c.a = Mathf.Lerp(startGlowAlpha, targetGlowAlpha, t);
                glowImage.color = c;
            }

            yield return null;
        }

        // Final state reinforcement
        transform.localScale = targetScale;
        if (buttonImage != null && useColorHighlight) buttonImage.color = targetColor;
        if (buttonText != null && highlightText) buttonText.color = targetTextColor;

        if (targetGlowAlpha <= 0 && glowOverlay != null) glowOverlay.SetActive(false);
    }
}
