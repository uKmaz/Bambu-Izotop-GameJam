using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefab;   
    public float spawnRate = 3f;
    public float moveSpeed = 5f;
    public float minHeight = 1f;
    public float maxHeight = 2f;
    public GameObject currentPrefab;

    public float ceiling = 0f;

    public float floor=0f;

    private float timer;

    public float gapSize = 3f; // adjust

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            SpawnRandomObstacle();
            timer = 0f;
        }
    }

    void SpawnRandomObstacle()
    {
        int type = Random.Range(0,  obstaclePrefab.Length);
        Vector3 spawnPos = new Vector3(transform.position.x, floor, 0f);
        GameObject block = Instantiate(obstaclePrefab[type], spawnPos, Quaternion.identity);
        AddMovement(block);
    }

    void AddMovement(GameObject obj)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.left * moveSpeed;
        Destroy(obj, 5f);
    }
}
