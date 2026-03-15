using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    public PlayerLevel playerLevel;
    public Slider slider;

    void Start()
    {
        slider.maxValue = playerLevel.xpToNextLevel;
        slider.value = playerLevel.currentXP;
    }

    void Update()
    {
        slider.value = playerLevel.currentXP;
    }
}
