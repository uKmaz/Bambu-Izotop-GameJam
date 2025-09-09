using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Oyundaki tüm yetenekleri, bekleme sürelerini ve etkilerini merkezi olarak yönetir.
/// Singleton yapısı ile sahnedeki diğer script'ler tarafından kolayca erişilebilir.
/// </summary>
public enum SkillType
{
    None,
    Immunity,
    Slowdown,
    MagicMeteor
}

public class SkillManager : MonoBehaviour
{
    // Singleton deseni
    public static SkillManager instance;

    public Image SkillshotImmunityOverlay;
    public Image SkillshotSpedupOverlay;
    public Image SkillshotMetaorOverlay;

    public TextMeshProUGUI SkillshotImmunityCooldownText;
    public TextMeshProUGUI SkillshotSpedupCooldownText;
    public TextMeshProUGUI SkillshotMetaorCooldownText;

    [Header("Immunity Skill Settings")]
    [Tooltip("Dokunulmazlığın ne kadar süreceği (saniye).")]
    public float immunityDuration = 1.5f;
    [Tooltip("Dokunulmazlık yeteneğinin bekleme süresi (saniye).")]
    public float immunityCooldown = 15f;

    [Header("Slowdown Skill Settings")]
    [Tooltip("Zaman yavaşlatmanın ne kadar süreceği (saniye).")]
    public float slowdownDuration = 2f;
    [Tooltip("Zaman yavaşlatma yeteneğinin bekleme süresi (saniye).")]
    public float slowdownCooldown = 20f;
    [Tooltip("Zamanın ne kadar yavaşlayacağı (1 = normal, 0.5 = %50 hız).")]
    public float slowdownFactor = 0.5f;

    [Header("Magic Meteor Skill Settings")]
    [Tooltip("Meteor Hızı")]
    public float magicMeteorSpeed = 2f;
    [Tooltip("Magic Meteor yeteneği bekleme süresi")]
    public float magicMeteorCooldown = 20f;

    // Her yeteneğin kendi ortak bekleme süresini tutan yapı
    private Dictionary<SkillType, float> skillCooldowns;
    private bool isTimeSlowed = false; // Zamanın zaten yavaş olup olmadığını kontrol eder
    [SerializeField] private GameObject MeteorPrefab;

    private void Awake()
    {
        // Singleton kurulumu
        if (instance == null) instance = this;
        else Destroy(gameObject);

        // Dictionary'yi oyun başında bir kez oluştur ve doldur
        InitializeCooldowns();
    }

    private void InitializeCooldowns()
    {
        skillCooldowns = new Dictionary<SkillType, float>();
        // Enum'daki tüm yetenek türleri için dictionary'ye bir giriş ekle
        // Başlangıçta tüm bekleme süreleri 0'dır, yani kullanılmaya hazırdır.
        foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
        {
            skillCooldowns.Add(skill, 0f);
        }
        SkillshotImmunityOverlay.fillAmount = 0f;
        SkillshotSpedupOverlay.fillAmount = 0f;
        SkillshotMetaorOverlay.fillAmount = 0f;
    }

    private void Update()
    {
        // Tüm yeteneklerin bekleme sürelerini her kare azalt
        var skillKeys = new List<SkillType>(skillCooldowns.Keys);
        foreach (SkillType skill in skillKeys)
        {

            if (skillCooldowns[skill] > 0)
            {
                skillCooldowns[skill] -= Time.deltaTime;
                SkillOverlay(skill, skillCooldowns[skill]);
            }
            else
            {
                SkillOverlay(skill, 0f);
            }
        }
    }

    private void SkillOverlay(SkillType a, float cooldownRemaining)
    {
        if (a.Equals(SkillType.Immunity))
        {
            SkillshotImmunityOverlay.fillAmount = cooldownRemaining / immunityCooldown;
            SkillshotImmunityCooldownText.text = cooldownRemaining == 0 ? "" : ((int)cooldownRemaining).ToString();
        }
        else if (a.Equals(SkillType.Slowdown))
        {
            SkillshotSpedupOverlay.fillAmount = cooldownRemaining / slowdownCooldown;
            SkillshotSpedupCooldownText.text = cooldownRemaining == 0 ? "" : ((int)cooldownRemaining).ToString();
        }
        else if (a.Equals(SkillType.MagicMeteor))
        {
            SkillshotMetaorOverlay.fillAmount = cooldownRemaining / magicMeteorCooldown;
            SkillshotMetaorCooldownText.text = cooldownRemaining == 0 ? "" : ((int)cooldownRemaining).ToString();
        }
    }
    /// <summary>
    /// PlayerController tarafından çağrılır. Bir yeteneği aktive etmeyi dener.
    /// </summary>
    /// <param name="skillToUse">Aktive edilecek yetenek türü.</param>
    /// <param name="player">Yeteneği isteyen oyuncu.</param>
    /// <returns>Yetenek başarıyla aktive edildiyse true döner.</returns>
    public bool TryActivateSkill(SkillType skillToUse, PlayerController player)
    {
        // Yetenek bekleme süresinde mi diye kontrol et
        if (skillCooldowns[skillToUse] <= 0)
        {
            // Slowdown için özel kontrol: Zaman zaten yavaşsa tekrar aktive etme
            if (skillToUse == SkillType.Slowdown && isTimeSlowed)
            {
                Debug.Log("Zaman zaten yavaşlatılmış durumda, tekrar aktive edilemez.");
                return false;
            }

            // Yeteneğe özel bekleme süresini ayarla
            switch (skillToUse)
            {
                case SkillType.Immunity:
                    skillCooldowns[skillToUse] = immunityCooldown;
                    SkillshotImmunityOverlay.fillAmount = 1f;
                    SkillshotImmunityCooldownText.text = immunityCooldown.ToString();
                    break;
                case SkillType.Slowdown:
                    skillCooldowns[skillToUse] = slowdownCooldown;
                    SkillshotSpedupOverlay.fillAmount = 1f;
                    SkillshotSpedupCooldownText.text = slowdownCooldown.ToString();
                    break;
                case SkillType.MagicMeteor:
                    skillCooldowns[skillToUse] += magicMeteorCooldown;
                    SkillshotMetaorOverlay.fillAmount = 1f;
                    SkillshotMetaorCooldownText.text = magicMeteorCooldown.ToString();
                    break;
            }

            // Yeteneğin etkisini uygula
            ApplySkillEffect(skillToUse, player);
            return true; // Başarılı
        }
        else
        {
            // Yetenek bekleme süresindeyse bilgi ver
            Debug.Log(skillToUse + " yeteneği bekleme süresinde! Kalan süre: " + skillCooldowns[skillToUse].ToString("F1"));
            return false; // Başarısız
        }
    }

    /// <summary>
    /// Yeteneğin etkisini ilgili oyuncuya veya genel oyun durumuna uygular.
    /// </summary>
    private void ApplySkillEffect(SkillType skill, PlayerController player)
    {
        switch (skill)
        {
            case SkillType.Immunity:
                AudioManager.Instance.PlaySFX("Immunity");
                player.ActivateImmunity(immunityDuration);
                break;
            case SkillType.Slowdown:
                AudioManager.Instance.PlaySFX("Slowdown");

                StartCoroutine(SlowdownCoroutine());
                break;
            case SkillType.MagicMeteor:
                AudioManager.Instance.PlaySFX("Fireball");
                MagicMeteor(player);
                break;
        }
    }

    /// <summary>
    /// Zamanı yavaşlatan ve belirli bir süre sonra normale döndüren Coroutine.
    /// Time.timeScale'den etkilenmemesi için WaitForSecondsRealtime kullanır.
    /// </summary>
    private IEnumerator SlowdownCoroutine()
    {
        isTimeSlowed = true;
        Time.timeScale = slowdownFactor;
        Debug.Log("Zaman yavaşlatıldı! TimeScale: " + Time.timeScale);

        // Gerçek zamanda bekle (oyunun yavaşlatılmış zamanında değil)
        AudioManager.Instance.PlaySFXReversed("Slowdown");

        yield return new WaitForSecondsRealtime(slowdownDuration);

        Time.timeScale = 1f;
        isTimeSlowed = false;
        Debug.Log("Zaman normale döndü.");
    }

    private void MagicMeteor(PlayerController player)
    {

        GameObject MagicMeteorInstance = Instantiate(MeteorPrefab, player.transform.position, Quaternion.identity);

        Rigidbody2D rb = MagicMeteorInstance.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.right * magicMeteorSpeed;
    }
}