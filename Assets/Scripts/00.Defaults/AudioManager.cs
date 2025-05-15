using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource audioSource;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip[] bgmClips;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void SetAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            // SignIn Scene
            case 1:
                SetAudioClip(bgmClips[0]);
                break;
            // Mainmenu Scene
            case 2:
                SetAudioClip(bgmClips[1]);
                break;
            // Matching Scene
            case 3:
                SetAudioClip(bgmClips[2]);
                break;
            // Ingame Scene
            case 4:
                SetAudioClip(bgmClips[3]);
                audioSource.volume = 0.1f;
                break;
        }
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
