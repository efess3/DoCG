using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MeteorStormEffect : AbilityEffectBase
{
    public float fallSpeed = 20f;
    public float spriteRotationOffset = 45f; // Adjust this in the Inspector if your sprite is drawn diagonally
    
    private Vector2 targetPos;
    private bool isFalling = false;
    private bool hasExploded = false;
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        // Disable collider initially so it doesn't hit enemies while falling
        if (col != null) col.enabled = false;
    }

    public void SetTarget(Vector2 target)
    {
        targetPos = target;
        isFalling = true;
        
        // Correct movement direction: from current position TO target
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        // Standard 2D angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Apply the calculated angle plus an offset so the sprite points perfectly
        transform.rotation = Quaternion.Euler(0, 0, angle + spriteRotationOffset);
    }

    private void Update()
    {
        if (!isFalling || hasExploded) return;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, fallSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;
        isFalling = false;
        
        // Reset rotation so explosion animation looks correct
        transform.rotation = Quaternion.identity;

        // Enable collider for the explosion phase
        if (col != null) col.enabled = true;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Explode");
            // DestroySelf() should be called by an Animation Event at the end of the explosion
        }
        else
        {
            // Fallback if no animator
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasExploded) return; // Only deal damage during explosion phase

        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            other.GetComponent<MobHealth>()?.TakeDamage(damage);
        }
    }

    // Called via Animation Event at the end of the explosion animation
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
