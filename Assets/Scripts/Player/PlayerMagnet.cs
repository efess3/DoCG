using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    public float magnetRadius = 5f;

    void Update()
    {
        Collider2D[] crystals = Physics2D.OverlapCircleAll(transform.position, magnetRadius);

        foreach (Collider2D c in crystals)
        {
            ExpCrystal crystal = c.GetComponent<ExpCrystal>();

            if (crystal != null)
            {
                crystal.StartMoving(magnetRadius);
            }
        }
    }

    public void IncreaseMagnetRadius(float amount)
    {
        magnetRadius += amount;
    }
}