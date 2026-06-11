using UnityEngine;

/// <summary>
/// Singleton manager that coordinates the creation of damage numbers in the world space.
/// </summary>
public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Spawns a floating damage number at the specified world position.
    /// </summary>
    /// <param name="damage">The amount of damage to display</param>
    /// <param name="position">The base world position (e.g. monster's position)</param>
    /// <param name="isCritical">True if the hit is critical, styling it differently</param>
    public void Show(float damage, Vector3 position, bool isCritical = false)
    {
        // Don't spawn if damage is 0 or negative
        if (damage <= 0f) return;

        // Create new gameobject for the text
        GameObject dnGo = new GameObject("DamageNumberEffect");
        
        // Add components and initialize
        DamageNumber dn = dnGo.AddComponent<DamageNumber>();
        dn.Initialize(damage, position, isCritical);
    }
}
