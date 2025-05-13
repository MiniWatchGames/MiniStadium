using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private float VolumeToDB(float volume)
    {
        return Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", VolumeToDB(value));
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGMVolume", VolumeToDB(value));
        PlayerPrefs.SetFloat("BGMVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", VolumeToDB(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void LoadVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }
}
