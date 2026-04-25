using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scaling")]
    [SerializeField] private float scaleMultiplier = 1.05f;
    [SerializeField] private float animationDuration = 0.12f;
    
    [Header("Glow Effect")]
    [SerializeField] private GameObject glowOverlay; // Reference to a glow sprite child
    [SerializeField] private bool useColorHighlight = true;
    [SerializeField] private Color highlightColor = new Color(1.2f, 1.2f, 1.2f, 1f); // HDR-like brightness
    
    private Image buttonImage;
    private Image glowImage;
    private Vector3 originalScale;
    private Color originalColor;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;
        
        if (buttonImage != null) 
            originalColor = buttonImage.color;

        if (glowOverlay != null)
        {
            glowImage = glowOverlay.GetComponent<Image>();
            glowOverlay.SetActive(false); // Hide by default
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
        activeCoroutine = StartCoroutine(AnimateButton(originalScale * scaleMultiplier, highlightColor, 1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopActiveCoroutine();
        activeCoroutine = StartCoroutine(AnimateButton(originalScale, originalColor, 0f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopActiveCoroutine();
        transform.localScale = originalScale;
        if (buttonImage != null) buttonImage.color = originalColor;
        if (glowImage != null)
        {
            Color c = glowImage.color;
            c.a = 0;
            glowImage.color = c;
            glowOverlay.SetActive(false);
        }
    }

    private void StopActiveCoroutine() { if (activeCoroutine != null) StopCoroutine(activeCoroutine); }

    private IEnumerator AnimateButton(Vector3 targetScale, Color targetColor, float targetGlowAlpha)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = buttonImage != null ? buttonImage.color : Color.white;
        float startGlowAlpha = glowImage != null ? glowImage.color.a : 0;
        float elapsed = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / animationDuration);
            
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            if (buttonImage != null && useColorHighlight)
                buttonImage.color = Color.Lerp(startColor, targetColor, t);

            if (glowImage != null)
            {
                Color c = glowImage.color;
                c.a = Mathf.Lerp(startGlowAlpha, targetGlowAlpha, t);
                glowImage.color = c;
            }

            yield return null;
        }

        // Final state
        if (targetGlowAlpha <= 0 && glowOverlay != null) glowOverlay.SetActive(false);
    }
}
