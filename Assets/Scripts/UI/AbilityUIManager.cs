using UnityEngine;
using System.Collections;

public class AbilityUIManager : MonoBehaviour
{
    public AbilitySlotUI[] slots;

    private PlayerAbilities playerAbilities;

    void Start()
    {
        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        // Wait for player to be spawned/initialized if necessary
        while (playerAbilities == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerAbilities = player.GetComponent<PlayerAbilities>();
            }
            yield return new WaitForSeconds(0.1f);
        }

        LinkSlots();
    }

    void LinkSlots()
    {
        if (playerAbilities == null || playerAbilities.abilities == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < playerAbilities.abilities.Length)
            {
                slots[i].Setup(playerAbilities.abilities[i]);
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}
