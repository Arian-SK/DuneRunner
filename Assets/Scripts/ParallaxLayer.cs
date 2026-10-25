using UnityEngine;

// Infinite background: each layer moves with its own speed factor.
// Put this script on a SpriteRenderer (the sprite's width must be >= the screen width)
public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;   // Far layer = small, near layer (ground) = 1

    Transform tileA;
    Transform tileB;
    float width;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        width = GetComponent<SpriteRenderer>().bounds.size.x;

        // Create a copy next to itself to make the infinite loop
        GameObject clone = Instantiate(gameObject, transform.position + Vector3.right * width,
                                       transform.rotation, transform.parent);
        clone.GetComponent<ParallaxLayer>().enabled = false;   // It doesn't move itself; we move it

        tileA = transform;
        tileB = clone.transform;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        float dx = GameManager.Instance.CurrentSpeed * parallaxFactor * Time.deltaTime;
        tileA.Translate(Vector3.left * dx, Space.World);
        tileB.Translate(Vector3.left * dx, Space.World);

        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // If a tile has fully left the screen on the left, it jumps to the right of the other tile
        if (tileA.position.x + width / 2f < camLeft) tileA.position = tileB.position + Vector3.right * width;
        if (tileB.position.x + width / 2f < camLeft) tileB.position = tileA.position + Vector3.right * width;
    }
}
