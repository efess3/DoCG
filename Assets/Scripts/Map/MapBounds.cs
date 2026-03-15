using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public static MapBounds instance;

    private BoxCollider2D bounds;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Awake()
    {
        instance = this;
        bounds = GetComponent<BoxCollider2D>();

        Bounds b = bounds.bounds;

        minX = b.min.x;
        maxX = b.max.x;
        minY = b.min.y;
        maxY = b.max.y;
    }
}
