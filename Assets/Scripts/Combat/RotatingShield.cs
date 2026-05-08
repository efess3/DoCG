using UnityEngine;

public class RotatingShield : MonoBehaviour
{

    [Tooltip("Prędkość obrotu tarczy (w stopniach na sekundę)")]
    public float rotationSpeed = 181f;

    void Update()
    {
        // Obrót wokół własnej osi Z
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
