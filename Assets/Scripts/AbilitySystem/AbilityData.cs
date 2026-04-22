using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public float cooldown = 2f;
    public int damage = 10;

    public GameObject effectPrefab;
    public GameObject previewPrefab;
}