using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    public TextMeshProUGUI HearthLeftText;
    public TextMeshProUGUI ScoreText;

    [Header("Player Prefabs")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public GameObject EndGamePanel; 

    [Header("Spawn Points")]
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;

    [Header("Spawn Settings")]
    [Tooltip("Oyuncuların toplam kaç kez ölebileceği (can hakkı)")]
    public int totalLives;
    public float spawnProtectionDuration;

    // Aktif oyuncu objelerini saklamak için referanslar
    private GameObject player1Instance;
    private GameObject player2Instance;
    
    // Rigidbody'leri cache'lemek (önbelleğe almak) için referanslar
    private Rigidbody2D rb1;
    private Rigidbody2D rb2;

    // Oyuncuların kalan canlarını takip eden sayaçlar
    private int totalLivesLeft;

    private PlayerController p1Controller;
    private PlayerController p2Controller;

    private bool isSwapping = false;

    private float Score = 0f;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

    }
    private void Update()
    {
        HearthLeftText.text = " :"+totalLivesLeft.ToString();
        Score += Time.deltaTime*2;
        ScoreText.text=((int)Score).ToString();
        
    }
    void Start()
    {
        // Can sayaçlarını başlangıç değeriyle ayarla
        totalLivesLeft = totalLives;

        // Oyunun başında her iki oyuncuyu da oluştur ve referanslarını sakla
        player1Instance = Instantiate(player1Prefab, player1SpawnPoint.position, player1SpawnPoint.rotation);
        player2Instance = Instantiate(player2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
        p1Controller = player1Instance.GetComponent<PlayerController>();
        p2Controller = player2Instance.GetComponent<PlayerController>();
        // Performans için Rigidbody bileşenlerini baştan alıp saklayalım
        rb1 = player1Instance.GetComponent<Rigidbody2D>();
        rb2 = player2Instance.GetComponent<Rigidbody2D>();
        
    }

    // Oyuncu öldüğünde bu fonksiyon çağrılacak
    public void HandlePlayerDeath(int playerNumber)
    {
        if (playerNumber == 1)
        {
            totalLivesLeft--; 

            if (totalLivesLeft > 0)
            {
                ResetPlayer(player1Instance, rb1, player1SpawnPoint);
            }
            else
            {
                totalLivesLeft = 0;
                Destroy(player1Instance);
                Destroy(player2Instance);
                CheckForGameOver();
            }
        }
        else if (playerNumber == 2)
        {
            totalLivesLeft--;
            
            if (totalLivesLeft > 0)
            {
                ResetPlayer(player2Instance, rb2, player2SpawnPoint);
            }
            else
            {
                totalLivesLeft = 0;
                Destroy(player2Instance);
                Destroy(player1Instance);
                CheckForGameOver();
            }
        }
    }

    // Oyuncuyu sıfırlayan yardımcı fonksiyon
    private void ResetPlayer(GameObject player, Rigidbody2D rb, Transform spawnPoint)
    {
        // Oyuncuyu deaktif edip pozisyonunu anında değiştirmek, görsel takılmaları engeller
        player.SetActive(false); 
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
        
        // Hızını sıfırla
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Oyuncuyu ve script'ini tekrar aktif hale getir
        player.SetActive(true);
        player.GetComponent<PlayerController>().enabled = true;
        rb.simulated = true;
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.ActivateImmunity(spawnProtectionDuration);
    }

    private void CheckForGameOver()
    {
        // Eğer her iki oyuncu da sahneden silindiyse (canları bittiyse)
        if (totalLivesLeft<1)
        {
            Debug.Log("OYUN BİTTİ! Her iki oyuncu da tüm haklarını kullandı.");
            // OYUN BİTTİ EKRANINI GÖSTER
            Time.timeScale = 0f;
            EndGamePanel.SetActive(true);
        }
    }

    public void ExchangePlayers()
    {
        Vector3 tempT = player1Instance.transform.position;
        player1Instance.transform.position = player2Instance.transform.position;
        player2Instance.transform.position = tempT;
        Vector3 tempX = player1SpawnPoint.transform.position;
        player1SpawnPoint.transform.position = player2SpawnPoint.transform.position;
        player2SpawnPoint.transform.position = tempX;

    }
}