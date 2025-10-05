using UnityEngine;

public class VerticalTileRepeater : MonoBehaviour
{
    [Header("Scroll")]
    public float speed = 3f;                // aşağı yönde hız (eksi verirsen yukarı akar)

    [Header("Camera bounds (y)")]
    public float cameraTopY = 5.25f;        // ekranın görünen üst sınırı
    public float cameraBottomY = -5.25f;    // ekranın görünen alt sınırı

    [Header("Auto size")]
    public bool useSpriteHeight = true;     // tile yüksekliğini sprite'tan ölç
    public float manualTileHeight = 10f;    // sprite yoksa elle ver

    private float resetY;   // alt sınırın biraz altı
    private float startY;   // üst sınırın biraz üstü
    private float halfH;

    void Awake()
    {
        float tileH = manualTileHeight;

        // SpriteRenderer varsa yüksekliği otomatik al
        var sr = GetComponent<SpriteRenderer>();
        if (useSpriteHeight && sr != null)
            tileH = sr.bounds.size.y;

        halfH = tileH * 0.5f;

        // Tile tamamen çıktıktan sonra resetlesin diye yarım yükseklik pay bırakıyoruz
        resetY = cameraBottomY - halfH;
        startY = cameraTopY + halfH;
    }

    void Update()
    {
        // Dikey kaydırma (aşağı doğru)
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // Alt sınırı geçince tepeye ışınla
        if (transform.position.y < resetY)
        {
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }
    }
}
