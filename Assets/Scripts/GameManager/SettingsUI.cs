using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button backButton;
    private const string BGM_VOLUME = "BGMVolume";
    private const string SFX_VOLUME = "SFXVolume";

    [Header("Audio")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUIReferences(scene); // Cập nhật tham chiếu khi scene mới được tải
    }
    private void InitializeUIReferences(Scene scene)
    {
        if (scene.name == "SettingsMenu")
        {

            backButton.onClick.RemoveAllListeners(); // Xóa trước khi thêm để tránh lỗi trùng lặp
            backButton.onClick.AddListener(BackFromSettings);

            LoadAudioSettings();

            // Gắn sự kiện khi thay đổi slider
            bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }
        else
        {
            backButton.onClick.RemoveListener(BackFromSettings);
            bgmSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetBGMVolume);
            sfxSlider.onValueChanged.RemoveListener(AudioManager.Instance.SetSFXVolume);
        }
    }
    public void BackFromSettings()
    {
        // Unload scene Settings
        SceneManager.UnloadSceneAsync("SettingsMenu");
    }
    private void LoadAudioSettings()
    {
        float bgmVol = PlayerPrefs.GetFloat(BGM_VOLUME, 0.1f); // Mặc định 10%
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME, 1f);


        AudioManager.Instance.SetBGMVolume(bgmVol);
        AudioManager.Instance.SetSFXVolume(sfxVol);

        bgmSlider.value = bgmVol;
        sfxSlider.value = sfxVol;
    }
}
