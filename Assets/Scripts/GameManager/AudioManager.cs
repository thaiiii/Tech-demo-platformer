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

    [Header("Audio Clips")]
    public List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
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








}
