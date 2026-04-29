using UnityEngine;

public class HandOfHellAbility : AbilityBase
{
    protected override void CreatePreview()
    {
        if (data.previewPrefab == null) return;
        
        previewInstance = Instantiate(data.previewPrefab);
    }

    // 2. Aktualizujemy pozycję podglądu co klatkę tam, gdzie jest myszka
    public override void UpdateAiming(Vector2 targetPos)
    {
        if (!isAiming || previewInstance == null) return;

        // Ustawiamy pozycję podglądu bezpośrednio na cel (myszkę)
        previewInstance.transform.position = targetPos;

    }

    protected override void Activate(Vector2 targetPos)
    {
        // Tworzymy efekt uderzenia dokładnie tam, gdzie był podgląd
        GameObject effect = Instantiate(
            data.effectPrefab,
            targetPos,
            Quaternion.identity
        );

        // Inicjalizacja skryptu efektu (obrażenia itp.)
        var effectScript = effect.GetComponent<AbilityEffectBase>();
        effectScript?.Init(data.damage);
    }
}