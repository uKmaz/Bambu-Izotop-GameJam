using UnityEngine;

[System.Serializable]
public class ObstacleData
{
    public GameObject prefab;
    public float yOffset; // Spawner’dan ne kadar yukarı/aşağı kaydırılsın
}

public class ObstacleSpawner : MonoBehaviour
{
    public ObstacleData[] obstacles;
    public float spawnRate;
    public float moveSpeed;
    private float arrowSpeed = 7f;

    private float timer;

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
        // Rastgele bir obstacle seç
        int type = Random.Range(0, obstacles.Length);
        ObstacleData data = obstacles[type];

        // Prefab
        GameObject prefabToSpawn = data.prefab;

        // Pivot farkını hesapla (SpriteRenderer taban hizalaması)
        SpriteRenderer sr = prefabToSpawn.GetComponent<SpriteRenderer>();
        float baseYOffset = 0f;
        if (sr != null)
            baseYOffset = -sr.bounds.min.y;

        if (data.prefab.name.Equals("Arrow"))
        {
            data.yOffset = Random.Range(0f, 2.5f);

        }
        // Spawner + prefab offset + manuel offset
        Vector3 spawnPos = new Vector3(
            transform.position.x,
            transform.position.y + baseYOffset + data.yOffset,
            0f
        );

        // Spawn et
        GameObject block = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Hareket ekle
        AddMovement(block, data);
    }

    void AddMovement(GameObject obj,ObstacleData data)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        if (data.prefab.name.Equals("Arrow"))
        {
            float tempMove = moveSpeed;
            moveSpeed = arrowSpeed;
            rb.linearVelocity = Vector2.left * moveSpeed;
            moveSpeed = tempMove;
        }
        else
        {
            rb.linearVelocity = Vector2.left * moveSpeed;
        }
        Destroy(obj, 5f);
    }
}
