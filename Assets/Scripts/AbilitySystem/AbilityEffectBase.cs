using UnityEngine;

public class AbilityEffectBase : MonoBehaviour
{
    protected int damage;

    public virtual void Init(int dmg)
    {
        damage = dmg;
    }
}