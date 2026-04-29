using UnityEngine;

public class HandEventProxy : MonoBehaviour
{
    public Explosion explosionSystem;

    public void TriggerExplosions()
    {
        explosionSystem.TriggerExplosions();
    }
}