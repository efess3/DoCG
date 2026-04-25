using UnityEngine;
using System.Collections;
public class Explosion : MonoBehaviour
{
    public GameObject[] explosions;

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