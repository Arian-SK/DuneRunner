using UnityEngine;

// Flying obstacle: bobs up and down.
// It can spawn at a low height (player must jump over it)
// or at a high height (player must pass under it, do not jump!)
public class Bird : Interactable
{
    public int damage = 1;
    public float bobAmplitude = 0.3f;
    public float bobFrequency = 3f;

    [Header("Spawn Heights")]
    public float lowY = -2.2f;    // low flight: jump over it
    public float highY = 0f;   // high flight: stay on the ground

    float baseY;

    void Start()
    {
        baseY = transform.position.y;
    }

    // Called by the Spawner to choose a random height (low or high)
    public float GetRandomHeight()
    {
        return Random.value < 0.5f ? lowY : highY;
    }

    protected override void Update()
    {
        base.Update();   // move left using the base class

        // Bob up and down with a sine wave
        Vector3 p = transform.position;
        p.y = baseY + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = p;
    }

    protected override void Interact(PlayerController player)
    {
        player.TakeDamage(damage);
    }
}