using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public float cooldown = 2f;
    public int damage = 10;
    public float castTimeLock = 0.3f;
    public Vector2 previewOffset;

    public GameObject effectPrefab;
    public GameObject previewPrefab;
}