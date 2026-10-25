using UnityEngine;

// Shared base class for everything that collides with the player (obstacles, coins, etc.)
// Each subclass only defines "what happens when the player touches it"
public abstract class Interactable : MonoBehaviour
{
    public float destroyX = -12f;   // Destroyed after passing this point

    protected virtual void Update()
    {
        if (GameManager.Instance == null) return;

        // Move left at the game's speed
        transform.Translate(Vector3.left * GameManager.Instance.CurrentSpeed * Time.deltaTime, Space.World);

        if (transform.position.x < destroyX) Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null) Interact(player);
    }

    // Every subclass must implement this
    protected abstract void Interact(PlayerController player);
}
