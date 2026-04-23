using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public AbilityBase[] abilities;

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        HandleAbility(0, KeyCode.Q, mousePos);
        HandleAbility(1, KeyCode.E, mousePos);
        HandleAbility(2, KeyCode.R, mousePos);
    }

    void HandleAbility(int index, KeyCode key, Vector2 mousePos)
    {
        if (abilities.Length <= index) return;

        var ability = abilities[index];

        if (Input.GetKeyDown(key))
            ability.StartAiming();

        if (Input.GetKey(key))
            ability.UpdateAiming(mousePos);

        if (Input.GetKeyUp(key))
            ability.Release(mousePos);
    }
}