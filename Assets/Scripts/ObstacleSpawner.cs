using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;   // Assign in Inspector
    public float spawnRate = 1.5f;      // Seconds between spawns
    public float obstacleSpeed = 5f;    // Speed of movement
    public float minHeight = 1f;        // Minimum obstacle height
    public float maxHeight = 4f;             // Highest spawn position

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnObstacle();
            timer = 0f;
        }
    }

    void SpawnObstacle()
    {
        // Always spawn on floor (Spawner's Y is the floor position)
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, 0f);

        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        // Random height
        float randomHeight = Random.Range(minHeight, maxHeight);

        // Adjust the obstacle's scale
        newObstacle.transform.localScale = new Vector3(1f, randomHeight, 1f);

        // Shift it up so bottom stays on the floor
        newObstacle.transform.position += new Vector3(0f, (randomHeight - 1f) / 2f, 0f);

        // Movement
        Rigidbody2D rb = newObstacle.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // no falling
        rb.linearVelocity = Vector2.left * obstacleSpeed;

        // Cleanup
        Destroy(newObstacle, 10f);
    }
}
