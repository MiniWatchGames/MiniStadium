using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    /*
    [Header("Display Settings")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown displayModeDropdown;

    [Header("Sound Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider effectSlider;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource effectAudioSource;

    [Header("Control Settings")]
    [SerializeField] private Slider mouseSensitivitySlider;

    [Header("UI Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;

    private List<Vector2Int> resolutions = new List<Vector2Int>()
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1360, 768),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(3480, 2160)
    };

    private FullScreenMode currentScreenMode = FullScreenMode.Windowed;

    public static float MouseSensitivity { get; private set; } = 1.0f;

    private void Start()
    {
        SetupResolutionDropdown();
        SetupDisplayModeDropdown();
        SetupSoundSliders();
        SetupMouseSensitivitySlider();
        SetupButtons();
        LoadSettings(); // 실행할 때 저장된 설정 불러오기
    }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int defaultResolutionIndex = 0;

        for (int i = 0; i < resolutions.Count; i++)
        {
            options.Add($"{resolutions[i].x} x {resolutions[i].y}");
            if (resolutions[i].x == 1920 && resolutions[i].y == 1080)
                defaultResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = defaultResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void SetupDisplayModeDropdown()
    {
        displayModeDropdown.ClearOptions();
        List<string> options = new List<string> { "전체화면", "창화면" };
        displayModeDropdown.AddOptions(options);
        displayModeDropdown.value = 0; // 기본 전체화면
        displayModeDropdown.RefreshShownValue();

        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
    }

    private void SetupSoundSliders()
    {
        bgmSlider.value = bgmAudioSource.volume;
        effectSlider.value = effectAudioSource.volume;

        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        effectSlider.onValueChanged.AddListener(OnEffectVolumeChanged);
    }

    private void SetupMouseSensitivitySlider()
    {
        mouseSensitivitySlider.minValue = 0.5f;
        mouseSensitivitySlider.maxValue = 5f;
        mouseSensitivitySlider.value = MouseSensitivity;

        mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
    }

    private void SetupButtons()
    {
        saveButton.onClick.AddListener(SaveSettings);
        closeButton.onClick.AddListener(CloseSettingsPanel);
    }

    private void OnResolutionChanged(int index)
    {
        ApplyResolution(index, currentScreenMode);
    }

    private void OnDisplayModeChanged(int index)
    {
        currentScreenMode = (index == 0) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        ApplyResolution(resolutionDropdown.value, currentScreenMode); 
    }

    private void ApplyResolution(int resolutionIndex, FullScreenMode screenMode)
    {
        Vector2Int selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.x, selectedResolution.y, screenMode);
    }

    private void OnBGMVolumeChanged(float value)
    {
        bgmAudioSource.volume = value;
    }

    private void OnEffectVolumeChanged(float value)
    {
        effectAudioSource.volume = value;
    }

    private void OnMouseSensitivityChanged(float value)
    {
        MouseSensitivity = value;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("DisplayMode", displayModeDropdown.value);
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetFloat("EffectVolume", effectSlider.value);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
        PlayerPrefs.Save();
        
        Debug.Log("설정 저장 완료");
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex");
            displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode");
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume");
            effectSlider.value = PlayerPrefs.GetFloat("EffectVolume");
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity");
            
            // 불러온 후 직접 적용
            currentScreenMode = (displayModeDropdown.value == 0)
                ? FullScreenMode.FullScreenWindow
                : FullScreenMode.Windowed;
            ApplyResolution(resolutionDropdown.value, currentScreenMode);
            bgmAudioSource.volume = bgmSlider.value;
            effectAudioSource.volume = effectSlider.value;
            MouseSensitivity = mouseSensitivitySlider.value;
        }
    }

    private void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
    */
}
