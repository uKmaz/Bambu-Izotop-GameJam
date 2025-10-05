using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;        // Sesin adı (örn: "Jump", "Explosion")
    public AudioClip clip;     // Atanacak ses dosyası
    [Range(0f, 1f)] public float volume = 1f;
    [Range(.1f, 3f)] public float pitch = 1f;
    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;  // Singleton

    [Header("Müzikler")]
    public Sound[] musicSounds;

    [Header("SFX (Efektler)")]
    public Sound[] sfxSounds;

    private AudioSource musicSource;
    private Dictionary<string, Sound> musicDict;
    private Dictionary<string, Sound> sfxDict;

    void Awake()
    {
        // Singleton kurulumu
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne geçişlerinde kaybolmasın
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Ses kaynakları
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        // SFX’ler için ayrı kaynaklar (dinamik yaratılacak)
        musicDict = new Dictionary<string, Sound>();
        sfxDict = new Dictionary<string, Sound>();

        foreach (var s in musicSounds) musicDict[s.name] = s;
        foreach (var s in sfxSounds) sfxDict[s.name] = s;
    }

    private void Start()
    {
        Debug.Log("Music array length: " + musicSounds.Length);
        Debug.Log(musicSounds[0].name);
        PlayMusic("Music");
    }

    // 🎶 Müzik Çalma
    public void PlayMusic(string name)
    {
        if (!musicDict.ContainsKey(name))
        {
            Debug.LogWarning("Müzik bulunamadı: " + name);
            return;
        }

        Sound s = musicDict[name];
        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;
        musicSource.loop = s.loop;
        musicSource.Play();
    }

    // 🔊 SFX Çalma
    public void PlaySFX(string name)
    {
        if (!sfxDict.ContainsKey(name))
        {
            Debug.LogWarning("SFX bulunamadı: " + name);
            return;
        }

        Sound s = sfxDict[name];
        AudioSource.PlayClipAtPoint(s.clip, Camera.main.transform.position, s.volume);
    }

    public void PlaySFXReversed(string name)
    {
        if (!sfxDict.ContainsKey(name))
        {
            Debug.LogWarning("SFX bulunamadı: " + name);
            return;
        }

        Sound s = sfxDict[name];

        // Geçici AudioSource oluştur
        GameObject tempGO = new GameObject("SFX_" + name + "_Reversed");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = s.clip;
        aSource.volume = s.volume;
        aSource.loop = false;

        // Ters çalma
        aSource.pitch = -1f;
        aSource.time = s.clip.length - 0.01f;

        aSource.Play();
        Destroy(tempGO, s.clip.length / Mathf.Abs(aSource.pitch));
    }


    // 🎵 Müzik Durdur
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public bool MusicIsPlaying()
    {
        if (musicSource != null)
            return musicSource.isPlaying;
        return false;
    }
}
