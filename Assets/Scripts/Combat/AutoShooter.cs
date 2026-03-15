using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    public float range = 10f;
    float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab nie jest przypisany!");
            return;
        }

        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();

        if (enemies.Length == 0)
            return;

        EnemyMovement closestEnemy = null;
        float minDistance = range;

        foreach (EnemyMovement enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript == null)
        {
            Debug.LogError("Prefab Bullet nie ma skryptu Bullet!");
            return;
        }

        bulletScript.SetTarget(closestEnemy.transform);
    }

    public void IncreaseAttackSpeed(float amount)
    {
        fireRate -= amount;
        
        // Zabezpieczenie, żeby gra strzelała poprawnie - nie chcemy ujemnych ani 0 wartśoci
        if (fireRate < 0.1f)
            fireRate = 0.1f;
    }
}