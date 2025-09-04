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

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool isSliding = false;
    private float slideTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // InputManager üzerinden kaydedilmiş tuşları yükle
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
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
    }

    private void EndSlide()
    {
        isSliding = false;

        // scale geri normale
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Karakter "Ground" etiketli bir şeyden AYRILDIĞI AN tetiklenir.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
