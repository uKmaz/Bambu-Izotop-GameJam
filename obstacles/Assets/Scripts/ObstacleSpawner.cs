using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefab;   // Single block prefab
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
            //currentPrefab = obstaclePrefab[4];
            SpawnSingle();
            timer = 0f;
        }
    }

    void SpawnRandomObstacle()
    {
        int type = Random.Range(1, 6); // Pick 1�4

        switch (type)
        {
            case 1:
                currentPrefab = obstaclePrefab[0];
                break;
            case 2:
                currentPrefab = obstaclePrefab[1];
                break;
            case 3:
                currentPrefab = obstaclePrefab[2];
                break;
            case 4:
                currentPrefab = obstaclePrefab[3];
                break;
            case 5:
                currentPrefab = obstaclePrefab[4];
                break;
        }
    }

    void SpawnSingle()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, floor, 0f);
        GameObject block = Instantiate(currentPrefab, spawnPos, Quaternion.identity);

        //float randomHeight = Random.Range(minHeight, maxHeight);
        //block.transform.localScale = new Vector3(1f, randomHeight, 1f);

        AddMovement(block);
    }
    /*
    void SpawnPipePair()
    {
        float randomY = Random.Range(-1.5f, 1.5f);

        // Bottom block
        GameObject bottom = Instantiate(obstaclePrefab, new Vector3(transform.position.x,floor, 0), Quaternion.identity);
        bottom.transform.localScale = new Vector3(1f, Random.Range(minHeight, maxHeight), 1f);

        // Top block
        GameObject top = Instantiate(obstaclePrefab, new Vector3(transform.position.x,  ceiling , 0), Quaternion.identity);
        top.transform.localScale = new Vector3(1f, Random.Range(minHeight, maxHeight)-gapSize, 1f);

        AddMovement(bottom);
        AddMovement(top);
    }

    void SpawnSideBySide()
    {
        int count = Random.Range(2, 5); // 2�4 blocks

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = new Vector3(transform.position.x + i * 1.5f, floor, 0f);
            GameObject block = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

            float randomHeight = Random.Range(minHeight, maxHeight);
            block.transform.localScale = new Vector3(1f, randomHeight, 1f);

            AddMovement(block);
        }
    }

    void SpawnCeiling()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, ceiling, 0f); // adjust ceiling Y
        GameObject block = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        float randomHeight = Random.Range(minHeight, maxHeight);
        block.transform.localScale = new Vector3(1f, randomHeight, 1f);

        AddMovement(block);
    }*/

    void AddMovement(GameObject obj)
    {
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.left * moveSpeed;
        Destroy(obj, 5f);
    }
}
