using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    private AudioSource stopableSfxSource;

    [Header("Audio Clips")]
    public List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioSource> activeStopableSFX = new Dictionary<string, AudioSource>();
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            foreach (AudioClip clip in sfxClips)
            {
                if (!sfxDict.ContainsKey(clip.name)) 
                    sfxDict.Add(clip.name, clip);
            }
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            StopBGM();
        else
        {
            PlayBGM();
        }
    }
    public void PlayBGM()
    {
        bgmSource.loop = true;
        bgmSource.Play();
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
}
