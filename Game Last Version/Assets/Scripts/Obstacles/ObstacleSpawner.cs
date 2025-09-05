using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefab;   
    public float spawnRate = 3f;
    public float moveSpeed = 5f;
    public float minHeight = 1f;
    public float maxHeight = 2f;


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
        // 1. Rastgele bir engel prefab'ı seç
        int type = Random.Range(0, obstaclePrefab.Length);
        GameObject prefabToSpawn = obstaclePrefab[type];

        // 2. Seçilen prefab'ın SpriteRenderer bileşenini al
        SpriteRenderer sr = prefabToSpawn.GetComponent<SpriteRenderer>();

        // Eğer prefab'da SpriteRenderer yoksa hata vermemesi için kontrol
        if (sr == null)
        {
            Debug.LogError(prefabToSpawn.name + " prefab'ında SpriteRenderer bileşeni bulunamadı! Hizalama yapılamıyor.");
            // Hata durumunda objeyi yine de spawner'ın kendi pozisyonunda oluşturabiliriz.
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            return;
        }

        // 3. PİVOT NOKTASINDAN BAĞIMSIZ OFSET HESAPLAMA
        // Prefab'ın pivot noktası ile en alt noktası arasındaki Y eksenindeki mesafeyi buluyoruz.
        // bounds.min.y, objenin en alt pixel'inin Y koordinatını verir (prefab'ın merkezi 0,0,0'da varsayılarak).
        // Bu genellikle negatif bir değerdir, bu yüzden onu pozitif bir ofsete çevirmek için - ile çarpıyoruz.
        float yOffset = -sr.bounds.min.y;

        // Önemli Not: Bu hesaplama, prefab'ın scale değerinin (1,1,1) olduğunu varsayar.
        // Eğer prefab'ın kendi içinde scale değeri farklıysa, ofseti scale ile çarpmak gerekir.
        // yOffset *= prefabToSpawn.transform.localScale.y; // Genellikle bu satıra ihtiyaç olmaz.

        // 4. Spawner'ın pozisyonunu TABAN olarak alıp, objeyi yukarı kaydıracak yeni pozisyonu hesapla
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + yOffset, 0f);

        // 5. Engeli, hesaplanan doğru pozisyonda oluştur
        GameObject block = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 6. Hareketi ekle
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
