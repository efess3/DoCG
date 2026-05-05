using UnityEngine;
using System.Collections;

public class MeteorStormAbility : AbilityBase
{
    [Header("Meteor Storm Settings")]
    public int meteorCount = 10;
    public float spawnRadius = 3f;
    public float spawnDelay = 0.10f;
    public float spawnHeight = 5f;
    public float spawnOffsetX = -5f;

    protected override void CreatePreview()
    {
        if (data.previewPrefab == null) return;
        previewInstance = Instantiate(data.previewPrefab);
    }

    public override void UpdateAiming(Vector2 targetPos)
    {
        if (!isAiming || previewInstance == null) return;

        // Keep the preview centered on the character instead of following the mouse
        previewInstance.transform.position = transform.position;
    }

    protected override void Activate(Vector2 targetPos)
    {
        // Ignore the mouse position and spawn meteors around the character
        StartCoroutine(SpawnMeteors(transform.position));
    }

    private IEnumerator SpawnMeteors(Vector2 targetPos)
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // Calculate a random offset around the target position
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 finalTargetPos = targetPos + randomOffset;
            
            // Spawn high up and slightly to the left, to create a falling effect towards bottom-right
            Vector2 spawnPos = finalTargetPos + new Vector2(spawnOffsetX, spawnHeight);

            GameObject meteor = Instantiate(data.effectPrefab, spawnPos, Quaternion.identity);
            
            var effectScript = meteor.GetComponent<MeteorStormEffect>();
            if (effectScript != null)
            {
                effectScript.Init(data.damage);
                effectScript.SetTarget(finalTargetPos);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
