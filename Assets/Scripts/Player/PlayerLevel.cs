using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;

    public int currentXP = 0;

    public int xpToNextLevel = 10;

    public Slider slider;

    public void AddXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;

        currentXP -= xpToNextLevel;

        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.1f);

        slider.maxValue = xpToNextLevel;

        Debug.Log("LEVEL UP: " + level);

        if (UpgradeManager.instance != null)
        {
            UpgradeManager.instance.ShowUpgrades();
        }
    }
}