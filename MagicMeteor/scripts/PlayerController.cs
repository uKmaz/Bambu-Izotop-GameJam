using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[System.Serializable]
public struct PlayerControls
{
    public KeyCode jumpKey;
    public KeyCode slideKey;
    public KeyCode slowdownKey;
    public KeyCode immunityKey;
    public KeyCode moveLeftKey;
    public KeyCode moveRightKey;
    public KeyCode meteorKey;
}

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public PlayerControls controls;

    [Header("Movement Settings")]
    public float jumpForce = 7f;
    public float moveSpeed = 5f;
    public float slideDuration = 0.5f;

    private bool isImmune = false;
    private float originalMoveSpeed;

    public float shootingCooldown = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;


    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool isSliding = false;
    private float slideTimer = 0f;

    private SpriteRenderer spriteRenderer; // Oyuncunun rengini değiştirmek için
    private Color originalColor;
    [Header("Immunity Effect Settings")]
    [Tooltip("Dokunulmazlık aktifken oyuncunun bürüneceği renk.")]
    public Color immunityColor = Color.yellow;

    [Tooltip("Dokunulmazlık başladığında oyuncunun etrafında belirecek parçacık efekti.")]
    public GameObject immunityEffectPrefab;
    private GameObject activeImmunityEffect; // Oluşturulan efekti saklamak için

    [Tooltip("Dokunulmazlık başladığında çalacak ses.")]
    public AudioClip immunityStartSound;
    [Tooltip("Dokunulmazlık bittiğinde çalacak ses.")]
    public AudioClip immunityEndSound;
    private AudioSource audioSource;


    public int playerNumber;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();        

        if (gameObject.name.Contains("Player1"))
        {
            playerNumber = 1;
            controls.jumpKey = InputManager.GetKey("Player1_Jump", controls.jumpKey);
            controls.slideKey = InputManager.GetKey("Player1_Slide", controls.slideKey);
            controls.slowdownKey = InputManager.GetKey("Player1_Slowdown", controls.slowdownKey);
            controls.immunityKey = InputManager.GetKey("Player1_Immunity", controls.immunityKey);
            controls.moveLeftKey = InputManager.GetKey("Player1_Left", controls.moveLeftKey);
            controls.moveRightKey = InputManager.GetKey("Player1_Right", controls.moveRightKey);
            controls.meteorKey = InputManager.GetKey("Player1_Meteor", controls.meteorKey);

        }
        else if (gameObject.name.Contains("Player2"))
        {
            playerNumber = 2;
            controls.jumpKey = InputManager.GetKey("Player2_Jump", controls.jumpKey);
            controls.slideKey = InputManager.GetKey("Player2_Slide", controls.slideKey);
            controls.slowdownKey = InputManager.GetKey("Player2_Slowdown", controls.slowdownKey);
            controls.immunityKey = InputManager.GetKey("Player2_Immunity", controls.immunityKey);
            controls.moveLeftKey = InputManager.GetKey("Player2_Left", controls.moveLeftKey);
            controls.moveRightKey = InputManager.GetKey("Player2_Right", controls.moveRightKey);
            controls.meteorKey = InputManager.GetKey("Player2_Meteor", controls.meteorKey);

        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; 
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        HandleMove();
        HandleJump();
        HandleSlide();
        HandleSkills();
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


    private void HandleSkills()
    {
        // '1' tuşu Immunity için
        if (Input.GetKey(controls.immunityKey))
        {
            SkillManager.instance.TryActivateSkill(SkillType.Immunity, this);
        }

        // '2' tuşu Slowdown için
        if (Input.GetKey(controls.slowdownKey))
        {
            // Zamanı yavaşlatma isteğini SkillManager'a gönderiyoruz
            SkillManager.instance.TryActivateSkill(SkillType.Slowdown, this);
        }
        if (Input.GetKey(controls.meteorKey)) {
            SkillManager.instance.TryActivateSkill(SkillType.MagicMeteor, this);
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // örnek: collider scale düşür
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y/2, transform.localScale.z);
    }

    private void EndSlide()
    {
        isSliding = false;

        // scale geri normale
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y *2, transform.localScale.z);
    }

    private void Die()
    {
        // Oyuncunun kontrolünü ve fiziğini anında durdur
        this.enabled = false; // Bu script'in Update() döngüsünü durdurur
        rb.simulated = false; // Fiziği durdurur, diğer objelerle etkileşime girmez

        // Spawner'a durumu bildir, o her şeyi halledecek
        PlayerSpawner.instance.HandlePlayerDeath(playerNumber);

        // ÖNEMLİ: Destroy(gameObject) satırını siliyoruz! Artık kendini yok etmiyor.
    }


    //SKILLS

    public void ActivateImmunity(float duration)
    {
        if (isImmune) return; // Zaten dokunulmazsa tekrar başlatma

        isImmune = true;
        Debug.Log(gameObject.name + " dokunulmaz oldu!");

        // 1. Sesi Çal
        /*
        if (immunityStartSound != null)
        {
            audioSource.PlayOneShot(immunityStartSound);
        }
        */
        // 2. Parçacık Efektini Oluştur ve Oyuncuya Bağla
        /*
        if (immunityEffectPrefab != null)
        {
            activeImmunityEffect = Instantiate(immunityEffectPrefab, transform.position, Quaternion.identity, transform);
        }
        */

        // 3. Yanıp Sönme Efektini Başlat
        StartCoroutine(ImmunityFlashCoroutine(duration));

        // Yeteneği sonlandırmak için Invoke yerine Coroutine'in bitişini bekleyeceğiz.
    }

    private void DeactivateImmunity()
    {
        isImmune = false;
        Debug.Log(gameObject.name + " dokunulmazlığı bitti.");

        // 1. Bitiş Sesini Çal
        /*
        if (immunityEndSound != null)
        {
            audioSource.PlayOneShot(immunityEndSound);
        }
        */
        // 2. Parçacık Efektini Yok Et
        /*
        if (activeImmunityEffect != null)
        {
            Destroy(activeImmunityEffect);
        }
        */
        // 3. Rengi normale döndürdüğümüzden emin ol
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    private IEnumerator ImmunityFlashCoroutine(float duration)
    {
        float timer = 0f;
        bool isColorChanged = false;

        // Belirlenen süre boyunca döngüde kal
        while (timer < duration)
        {
            // Rengi değiştir
            spriteRenderer.color = isColorChanged ? originalColor : immunityColor;
            isColorChanged = !isColorChanged;

            timer += 0.1f; // Her 0.1 saniyede bir renk değiştir
            yield return new WaitForSeconds(0.1f);
        }

        // Süre dolduğunda dokunulmazlığı bitir
        DeactivateImmunity();
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        originalMoveSpeed = moveSpeed;
        moveSpeed *= multiplier;
        Debug.Log(gameObject.name + " hızlandı! Yeni hız: " + moveSpeed);
        Invoke(nameof(DeactivateSpeedBoost), duration);
    }

    private void DeactivateSpeedBoost()
    {
        moveSpeed = originalMoveSpeed;
        Debug.Log(gameObject.name + " hızı normale döndü.");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isImmune || (collision.gameObject.CompareTag("Meteor") && !isImmune))
        {
            Die();

        }
    }
}
