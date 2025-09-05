using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Oyundaki tüm yetenekleri, bekleme sürelerini ve etkilerini merkezi olarak yönetir.
/// Singleton yapısı ile sahnedeki diğer script'ler tarafından kolayca erişilebilir.
/// </summary>
public enum SkillType
{
    None,
    Immunity,
    Slowdown
}

public class SkillManager : MonoBehaviour
{
    // Singleton deseni
    public static SkillManager instance;

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

    // Her yeteneğin kendi ortak bekleme süresini tutan yapı
    private Dictionary<SkillType, float> skillCooldowns;
    private bool isTimeSlowed = false; // Zamanın zaten yavaş olup olmadığını kontrol eder

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
            }
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
                    break;
                case SkillType.Slowdown:
                    skillCooldowns[skillToUse] = slowdownCooldown;
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
                player.ActivateImmunity(immunityDuration);
                break;
            case SkillType.Slowdown:
                StartCoroutine(SlowdownCoroutine());
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
        yield return new WaitForSecondsRealtime(slowdownDuration);

        Time.timeScale = 1f;
        isTimeSlowed = false;
        Debug.Log("Zaman normale döndü.");
    }
}