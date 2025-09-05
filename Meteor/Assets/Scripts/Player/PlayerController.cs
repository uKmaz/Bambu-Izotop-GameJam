using UnityEngine;

[System.Serializable]
public class PlayerControls
{
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode slideKey = KeyCode.S;
    public KeyCode skillKey = KeyCode.Space;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
}

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public PlayerControls controls;

    [Header("Movement Settings")]
    public float jumpForce = 7f;
    public float moveSpeed = 5f;
    public float slideDuration = 0.5f;

    [Header("Skill Settings")]
    public float skillCooldown = 5f;
    private float skillTimer = 0f;
    private bool skillActive = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;


    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool isSliding = false;
    private float slideTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (gameObject.name.Contains("Player1"))
        {
            controls.jumpKey = InputManager.GetKey("Player1_Jump", controls.jumpKey);
            controls.slideKey = InputManager.GetKey("Player1_Slide", controls.slideKey);
            controls.skillKey = InputManager.GetKey("Player1_Skill", controls.skillKey);
            controls.moveLeftKey = InputManager.GetKey("Player1_Left", controls.moveLeftKey);
            controls.moveRightKey = InputManager.GetKey("Player1_Right", controls.moveRightKey);
        }
        else if (gameObject.name.Contains("Player2"))
        {
            controls.jumpKey = InputManager.GetKey("Player2_Jump", controls.jumpKey);
            controls.slideKey = InputManager.GetKey("Player2_Slide", controls.slideKey);
            controls.skillKey = InputManager.GetKey("Player2_Skill", controls.skillKey);
            controls.moveLeftKey = InputManager.GetKey("Player2_Left", controls.moveLeftKey);
            controls.moveRightKey = InputManager.GetKey("Player2_Right", controls.moveRightKey);
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        HandleMove();
        HandleJump();
        HandleSlide();
        HandleSkill();
    }

    private void HandleMove()
    {
        float moveX = 0f;

        if (Input.GetKey(controls.moveLeftKey))
            moveX = -1f;
        if (Input.GetKey(controls.moveRightKey))
            moveX = 1f;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveX * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(controls.jumpKey) && isGrounded && !isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyUp(controls.jumpKey) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }
    private void HandleSlide()
    {
        if (Input.GetKeyDown(controls.slideKey) && !isSliding)
        {
            StartSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }
    }

    private void HandleSkill()
    {
        if (skillTimer > 0f) skillTimer -= Time.deltaTime;

        if (Input.GetKeyDown(controls.skillKey) && skillTimer <= 0f)
        {
            ActivateSkill();
            skillTimer = skillCooldown;
        }
    }

    private void ActivateSkill()
    {
        // Basit örnek: 1 saniye immunity
        skillActive = true;
        Debug.Log(gameObject.name + " used skill!");

        // 1 saniye sonra skill bitsin
        Invoke(nameof(EndSkill), 1f);
    }

    private void EndSkill()
    {
        skillActive = false;
        Debug.Log(gameObject.name + " skill ended");
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // örnek: collider scale düşür
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
    }

    private void EndSlide()
    {
        isSliding = false;

        // scale geri normale
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    private void Die()
    {
        // Konsola öldüğünü belirten bir mesaj yazdır
        Debug.Log(gameObject.name + " öldü!");

        // 1. Karakterin daha fazla hareket etmesini engelle
        // Bu script'i devre dışı bırakarak Update() döngüsünü durdururuz
        this.enabled = false;

        // 2. Fiziksel hareketini anında durdur
        rb.linearVelocity = Vector2.zero;

        // 3. (Opsiyonel ama önerilir) Karakterin öldükten sonra sağa sola savrulmasını engelle
        rb.simulated = false;

        // BURAYA OYUN BİTİŞ EKRANINI TETİKLEYECEK KOD GELECEK
        // Örneğin: GameManager.instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();

        }
    }
}