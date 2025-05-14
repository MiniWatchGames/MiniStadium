using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Display Setting Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown; // 해상도 드롭다운
    [SerializeField] private TMP_Dropdown displayModeDropdown; // 화면모드(전체화면, 창모드) 드롭다운

    [Header("Sound Setting Elements")]
    [SerializeField] private Slider masterSlider; // 마스터 볼륨 조절 슬라이더
    [SerializeField] private Slider bgmSlider; // 배경음 볼륨 조절 슬라이더
    [SerializeField] private Slider sfxSlider; // 효과음 볼륨 조절 슬라이더
    
    [Header("Sensitivity Setting Element")]
    [SerializeField] private Slider mouseSlider; // 마우스 감도 조절 슬라이더

    [Header("Buttons")]
    [SerializeField] private Button applySettingButton; // 설정저장 버튼

    private void Start()
    {
        InitDropdowns();
        LoadUIValues();

        // [PopupPanel] Settings에 있는 버튼 클릭 이벤트 연결 함수
        OnClickButtons();
        // [PopupPanel] Settings에 있는 슬라이더 이벤트 연결 함수
        OnValueChangedSlider();
    }
    
    private void InitDropdowns()
    {
        resolutionDropdown.ClearOptions();
        displayModeDropdown.ClearOptions();
        
        resolutionDropdown.AddOptions(new List<string> { "1280x720", "1360x768", "1600x900", "1920x1080", "3840x2160" });
        displayModeDropdown.AddOptions(new List<string> { "전체화면", "창모드" });
    }
    
    
    #region [PopupPanel] Settings saveSettingButton 처리

    private void ApplySettingClicked()
    {
        Debug.Log("해당 설정값을 저장합니다.");

        GraphicsManager.Instance.SetGraphicsSettings(resolutionDropdown.value, displayModeDropdown.value);
        AudioManager.Instance.SetMasterVolume(masterSlider.value);
        AudioManager.Instance.SetBGMVolume(bgmSlider.value);
        AudioManager.Instance.SetSFXVolume(sfxSlider.value);
        SensitivityManager.Instance.SetSensitivity(mouseSlider.value);
    }
    
    #endregion

    private void LoadUIValues()
    {
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", 3);
        displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode", 0);

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 1f);


        resolutionDropdown.RefreshShownValue();
        displayModeDropdown.RefreshShownValue();
    }
    
    #region UI(Button, Slider) 이벤트 연결 함수

    private void OnClickButtons()
    {
        // 설정저장 버튼 이벤트 연결
        applySettingButton.onClick.AddListener(ApplySettingClicked);
    }

    private void OnValueChangedSlider()
    {
        if (AudioManager.Instance != null)
        {
            // AudioManager 연동
            masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
            bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }

        if (SensitivityManager.Instance != null)
        {
            // 마우스 감도 슬라이더 연동
            mouseSlider.onValueChanged.AddListener(SensitivityManager.Instance.SetSensitivity);
        }
    }
    
    #endregion
}
