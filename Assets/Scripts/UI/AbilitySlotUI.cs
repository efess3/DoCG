using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public Image cooldownOverlay;
    public Image lockImage;
    public TextMeshProUGUI cooldownText;

    private AbilityBase ability;

    public void Setup(AbilityBase newAbility)
    {
        ability = newAbility;
        
        if (ability != null && ability.data != null)
        {
            iconImage.sprite = ability.data.icon;
            iconImage.enabled = ability.data.icon != null;
        }
        else
        {
            iconImage.enabled = false;
        }

        cooldownOverlay.fillAmount = 0;
        if (cooldownText != null) cooldownText.text = "";
    }

    void Update()
    {
        if (ability == null) return;

        bool unlocked = ability.isUnlocked;

        // Handle Lock UI
        if (lockImage != null)
        {
            lockImage.gameObject.SetActive(!unlocked);
        }

        if (!unlocked)
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.text = "";
            if (iconImage != null) iconImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            return;
        }

        float cdRemaining = ability.GetCooldownRemaining();
        float cdNormalized = ability.GetCooldownNormalized();

        // Update radial fill
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = cdNormalized;
        }

        // Update text
        if (cooldownText != null)
        {
            if (cdRemaining > 0)
            {
                cooldownText.text = cdRemaining > 1f ? cdRemaining.ToString("F0") : cdRemaining.ToString("F1");
            }
            else
            {
                cooldownText.text = "";
            }
        }

        // Optional: darken icon when on cooldown
        if (iconImage != null)
        {
            iconImage.color = cdRemaining > 0 ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
        }
    }
}
