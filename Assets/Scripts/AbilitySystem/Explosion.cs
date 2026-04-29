using UnityEngine;
using System.Collections;
public class Explosion : MonoBehaviour
{
    public GameObject[] explosions;

    void Start()
    {
        // Na starcie wyłączamy collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Uruchamiamy odliczanie
        StartCoroutine(ActivateColliderAfterDelay(0.7f));
    }

    IEnumerator ActivateColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Collider2D>().enabled = true;
    }

    public void TriggerExplosions()
    {
        StartCoroutine(ExplodeSequence());
    }

    IEnumerator ExplodeSequence()
    {
        foreach (GameObject exp in explosions)
        {
            exp.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
    }
}