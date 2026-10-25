using UnityEngine;

// Randomly spawns obstacles and coins on the right side of the screen
public class Spawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;   // Cactus and Bird
    public GameObject coinPrefab;

    public float minInterval = 1.2f;
    public float maxInterval = 2.5f;
    [Range(0f, 1f)] public float coinChance = 0.3f;
    public float coinMinY = 0f;
    public float coinMaxY = 2.5f;

    float timer;

    void Start()
    {
        timer = 1.5f;   // Wait a bit before the first obstacle
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (!gm.IsPlaying) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Spawn();

        // As speed increases, the time interval shrinks so the real distance between obstacles stays constant
        float interval = Random.Range(minInterval, maxInterval);
        timer = interval * gm.startSpeed / gm.BaseSpeed;
    }

    void Spawn()
    {
        if (Random.value < coinChance && coinPrefab != null)
        {
            // Spawn a coin at a random height
            Vector3 pos = new Vector3(transform.position.x, Random.Range(coinMinY, coinMaxY), 0f);
            Instantiate(coinPrefab, pos, Quaternion.identity);
        }
        else
        {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            // Use the prefab's own height by default
            float y = prefab.transform.position.y;

            // If it is a bird, pick a random height (low or high)
            Bird bird = prefab.GetComponent<Bird>();
            if (bird != null) y = bird.GetRandomHeight();

            Vector3 pos = new Vector3(transform.position.x, y, 0f);
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }
}

