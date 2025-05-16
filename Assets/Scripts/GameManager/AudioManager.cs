using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public List<AudioClip> sfxClips;
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> activeStopableSFX = new Dictionary<string, AudioSource>();

    [Header("BGM")]
    public List<AudioClip> bgmClips;
    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();

    [Header("Volume")]
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME = "BGMVolume";
    private const string SFX_VOLUME = "SFXVolume";




    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (AudioClip clip in sfxClips)
        {
            if (!sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
        }
        foreach (AudioClip clip in bgmClips)
        {
            if (!bgmDict.ContainsKey(clip.name))
                bgmDict.Add(clip.name, clip);
        }
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "StartMenu")
        {
            if(!bgmSource.isPlaying)
                PlayBGM("bgm02");
        }
        else
        {
            PlayBGM("bgm01");
        }
    }

    #region Play sfx & Bgm
    public void PlayBGM(string bgm)
    {
        if (bgmDict.TryGetValue(bgm, out AudioClip clip))
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
    public void StopBGM() => bgmSource.Stop();
    public void PlaySFX(string sfxName)
    {
        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
            Debug.Log($"SFX {sfxName} not found");
    }
    public void PlayStopableSFX(string sfxName)
    {
        if (sfxDict.TryGetValue(sfxName, out AudioClip clip))
        {
            GameObject sfxObject = new GameObject($"SFX_{sfxName}");
            AudioSource newSource = sfxObject.AddComponent<AudioSource>(); // Tạo AudioSource mới
            newSource.clip = clip;
            newSource.Play();
            activeStopableSFX[sfxName] = newSource;
            Destroy(sfxObject, clip.length); // Tự hủy sau khi phát xong
        }
    }
    public void StopSFX(string sfxName)
    {
        if (activeStopableSFX.TryGetValue(sfxName, out AudioSource source))
        {
            source.Stop();
            Destroy(source.gameObject); // Hủy GameObject chứa AudioSource
            activeStopableSFX.Remove(sfxName);
        }
    }
    #endregion

    #region Volume
    public void SetBGMVolume(float volume)
    {
        float volumeDb = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(BGM_VOLUME, volumeDb);
        PlayerPrefs.SetFloat(BGM_VOLUME, Mathf.Clamp(volume, 0.0001f, 1f));

    }
    public void SetSFXVolume(float volume)
    {
        float volumeDb = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(SFX_VOLUME, volumeDb);
        PlayerPrefs.SetFloat(SFX_VOLUME, Mathf.Clamp(volume, 0.0001f, 1f));
    }
    #endregion


}
