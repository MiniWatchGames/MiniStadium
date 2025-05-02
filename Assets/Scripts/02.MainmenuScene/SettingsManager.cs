using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown displayModeDropdown;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider mouseSensitivitySlider;

    [Header("Buttons")]
    public Button saveButton;
    public Button closeButton;

    [Header("Audio")]
    // public AudioMixer audioMixer; // 마스터 믹서 연결

    private Resolution[] resolutions;

    private readonly Vector2Int[] supportedResolutions = new Vector2Int[]
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1360, 768),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(3480, 2160)
    };

    private void Start()
    {
        InitResolutionDropdown();
        InitDisplayModeDropdown();
        LoadSettings();

        saveButton.onClick.AddListener(SaveSettings);
        closeButton.onClick.AddListener(CloseSettings);
    }

    private void InitResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < supportedResolutions.Length; i++)
        {
            var res = supportedResolutions[i];
            string option = res.x + " x " + res.y;
            options.Add(option);

            if (res.x == Screen.width && res.y == Screen.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 3); // 기본 설정 : 1920 x 1080
        resolutionDropdown.RefreshShownValue();
    }

    private void InitDisplayModeDropdown()
    {
        displayModeDropdown.ClearOptions();
        displayModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "전체화면", "창모드" });
        displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode", 0); // 기본 설정 : 전체 화면
        displayModeDropdown.RefreshShownValue();
    }

    private void SaveSettings()
    {
        // 해상도 적용
        Vector2Int res = supportedResolutions[resolutionDropdown.value];
        bool isFullScreen = displayModeDropdown.value == 0;
        Screen.SetResolution(res.x, res.y, isFullScreen);
        
        // 소리 설정 적용
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
        // audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmSlider.value) * 20);
        // audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20);
        
        // 마우스 감도 설정 적용
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
        
        // 플레이어 설정 값 저장
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("DisplayMode", displayModeDropdown.value);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 3);
        displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode", 0);

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        
        // audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterSlider.value) * 20);
        // audioMixer.SetFloat("BGMVolume", Mathf.Log10(bgmSlider.value) * 20);
        // audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20);
    }

    private void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
