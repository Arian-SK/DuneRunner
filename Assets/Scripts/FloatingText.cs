using TMPro;
using UnityEngine;

// A world-space text (like "+50") that floats up, follows the world and fades out
public class FloatingText : MonoBehaviour
{
    public float moveUpSpeed = 1.5f;   // how fast the text rises
    public float lifetime = 0.8f;      // how long the text stays alive

    TextMeshPro text;
    float timer;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    // Called by the coin to set what the text says
    public void SetText(string value)
    {
        text.text = value;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Rise upwards
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime, Space.World);

        // Also move left with the world so the text stays next to where the coin was
        if (GameManager.Instance != null)
        {
            transform.Translate(Vector3.left * GameManager.Instance.CurrentSpeed * Time.deltaTime, Space.World);
        }

        // Fade out over its lifetime
        Color c = text.color;
        c.a = 1f - timer / lifetime;
        text.color = c;

        if (timer >= lifetime) Destroy(gameObject);
    }
}