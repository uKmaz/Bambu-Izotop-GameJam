using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem; // new system

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 7f;
    public float moveSpeed = 5f;
    public float slideDuration = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private InputSystem_Actions controls;   // <— generated class from Input Actions
    private float moveInput;      // -1 left, 1 right, 0 idle
    private bool isGrounded;
    private bool isSliding;
    private float slideTimer;

    public int playerNumber;

    private bool isImmune = false;
    private Color originalColor;
    private bool gamePaused = false;


    private AudioSource audioSource;
    public AudioClip immunityStartSound;
    public AudioClip immunityEndSound;

    private GameObject activeImmunityEffect;
    public GameObject immunityEffectPrefab;
    public Color immunityColor = Color.yellow;

    private SceneTransitionManager sceneTransitionManager;

    public void SetPause(bool pause) { 
        gamePaused = pause;
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        controls = new InputSystem_Actions();

        if (gameObject.name.Contains("Player1"))
        {
            playerNumber = 1;
            // --- Movement ---
            controls.Player1.left.performed += ctx => moveInput = -1f;
            controls.Player1.left.canceled += ctx => { if (moveInput < 0) moveInput = 0; };

            controls.Player1.rigth.performed += ctx => moveInput = 1f;
            controls.Player1.rigth.canceled += ctx => { if (moveInput > 0) moveInput = 0; };

            // --- Actions ---
            controls.Player1.jump.performed += ctx => HandleJump();
            controls.Player1.slide.performed += ctx => StartSlide();
            controls.Player1.fireball.performed += ctx => Fireball();
            controls.Player1.invincible.performed += ctx => Immunity(); // example duration
            controls.Player1.timeslow.performed += ctx => TimeSlow();
            controls.Player1.pause.performed += ctx => PauseTheGame();
        }
        if (gameObject.name.Contains("Player2"))
        {
            playerNumber = 2;
            // --- Movement ---
            controls.Player2.left.performed += ctx => moveInput = -1f;
            controls.Player2.left.canceled += ctx => { if (moveInput < 0) moveInput = 0; };

            controls.Player2.rigth.performed += ctx => moveInput = 1f;
            controls.Player2.rigth.canceled += ctx => { if (moveInput > 0) moveInput = 0; };

            // --- Actions ---
            controls.Player2.jump.performed += ctx => HandleJump();
            controls.Player2.slide.performed += ctx => StartSlide();
            controls.Player2.fireball.performed += ctx => Fireball();
            controls.Player2.invincible.performed += ctx => Immunity(); // example duration
            controls.Player2.timeslow.performed += ctx => TimeSlow();
        }
    }

    void OnEnable() => ChoseEnable();
    void OnDisable() => ChoseDisable();

    void ChoseEnable()
    {
        if (playerNumber == 1)
        {
            controls.Player1.Enable();
        }
        else if (playerNumber == 2) {
            controls.Player2.Enable();
        }
    }
    void ChoseDisable()
    {
        if (playerNumber == 1)
        {
            controls.Player1.Disable();
        }
        else if (playerNumber == 2)
        {
            controls.Player2.Disable();
        }
    }

    void Start()
    {
        sceneTransitionManager = FindAnyObjectByType<SceneTransitionManager>();
    }

    void Update()
    {
        // ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        HandleMove();
        HandleSlide();
        HandleAnimations();
    }

    private void HandleMove()
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;
        rb.linearVelocity = velocity;

        if (moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;
    }

    private void HandleJump()
    {
        if (isGrounded && !isSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            AudioManager.Instance.PlaySFX("Jump");
        }
    }

    private void HandleSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f) EndSlide();
        }
    }

    private void StartSlide()
    {
        if (isSliding) return;
        isSliding = true;
        slideTimer = slideDuration;

        AudioManager.Instance.PlaySFX("Slide");
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
    }

    private void EndSlide()
    {
        isSliding = false;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    private void HandleAnimations()
    {
        animator.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded && !isSliding);
        animator.SetBool("isJumping", !isGrounded && rb.linearVelocity.y > 0.1f);
        animator.SetBool("isFalling", !isGrounded && rb.linearVelocity.y < -0.1f);
        animator.SetBool("isGrounded", isGrounded);
    }

    // ---------------- SKILLS ----------------
    private void Fireball()
    {
        SkillManager.instance.TryActivateSkill(SkillType.MagicMeteor, this);
    }

    private void TimeSlow()
    {
        SkillManager.instance.TryActivateSkill(SkillType.Slowdown, this);
    }
    private void Immunity()
    {
        SkillManager.instance.TryActivateSkill(SkillType.Immunity, this);
    }
    //---------------------------------
    public void ActivateImmunity(float duration)
    {
        if (isImmune) return; // Already immune — skip
        isImmune = true;

        Debug.Log(gameObject.name + " dokunulmaz oldu!");

        // 1️⃣ Play start sound
        if (immunityStartSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(immunityStartSound);
        }

        // 2️⃣ Create particle effect and attach it to the player
        if (immunityEffectPrefab != null)
        {
            // Prevent duplicate effect
            if (activeImmunityEffect != null)
                Destroy(activeImmunityEffect);

            activeImmunityEffect = Instantiate(immunityEffectPrefab, transform.position, Quaternion.identity, transform);
        }

        // 3️⃣ Start color flashing effect
        StartCoroutine(ImmunityFlashCoroutine(duration));
    }

    private void DeactivateImmunity()
    {
        isImmune = false;
        Debug.Log(gameObject.name + " dokunulmazlığı bitti.");

        // 1️⃣ Play end sound
        if (immunityEndSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(immunityEndSound);
        }

        // 2️⃣ Destroy particle effect if it exists
        if (activeImmunityEffect != null)
        {
            Destroy(activeImmunityEffect);
            activeImmunityEffect = null;
        }

        // 3️⃣ Reset color to original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ImmunityFlashCoroutine(float duration)
    {
        float timer = 0f;
        bool isColorChanged = false;

        while (timer < duration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isColorChanged ? originalColor : immunityColor;
            }

            isColorChanged = !isColorChanged;
            timer += 0.1f; // Flash every 0.1s
            yield return new WaitForSeconds(0.1f);
        }

        // End immunity when time runs out
        DeactivateImmunity();
    }

    private void PauseTheGame()
    {
        if (!gamePaused || Time.timeScale != 0)
        {
            Time.timeScale = 0f;
            sceneTransitionManager.LoadPause();
            gamePaused = true;
        }
        else if (gamePaused||Time.timeScale==0)
        {
            sceneTransitionManager.PauseEnded();
            Time.timeScale = 1f;
            gamePaused = false;
        }
    }

    // Death
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Obstacle") || collision.CompareTag("Meteor")) && !isImmune)
        {
            Die();
        }
    }

    private void Die()
    {
        this.enabled = false;
        rb.simulated = false;
        PlayerSpawner.instance.HandlePlayerDeath(playerNumber);
    }
}
