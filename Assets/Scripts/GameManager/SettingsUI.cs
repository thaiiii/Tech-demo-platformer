
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI")]


    [Header("Audio")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public AudioMixer audioMixer;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //InitializeUIReferences(); // Cập nhật tham chiếu khi scene mới được tải
        InitializeGameObjectReferences(); //Cập nhật tham chiếu các game object khác
    }
    private void InitializeGameObjectReferences()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // Load saved volume (hoặc mặc định là 0 dB)
            float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;

            SetBGMVolume(bgmVolume);
            SetSFXVolume(sfxVolume);

            // Gắn sự kiện khi thay đổi slider
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            
            gameObject.SetActive(false);
        }
        else
        {
            bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
        }
    }

    public void SetBGMVolume(float value)
    {
        float volumeDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVolume", volumeDb);
        PlayerPrefs.SetFloat("BGMVolume", Mathf.Clamp(value, 0.0001f, 1f));
    }

    public void SetSFXVolume(float value)
    {
        float volumeDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", volumeDb);
        PlayerPrefs.SetFloat("SFXVolume", Mathf.Clamp(value, 0.0001f, 1f));
    }
}
