using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float minX, maxX, minY, maxY; // granice mapy

    void LateUpdate()
    {
        if(player == null) return;

        float camX = Mathf.Clamp(player.position.x, minX, maxX);
        float camY = Mathf.Clamp(player.position.y, minY, maxY);

        transform.position = new Vector3(camX, camY, transform.position.z);
    }
}
