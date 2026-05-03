using UnityEngine;

public class DestroyParentIfNoChildren : MonoBehaviour
{
    void Update()
    {
        // Sprawdza, czy obiekt ma zero dzieci
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}