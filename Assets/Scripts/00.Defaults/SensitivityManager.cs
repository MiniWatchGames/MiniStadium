using UnityEngine;

public class SensitivityManager : MonoBehaviour
{
    public static SensitivityManager Instance;

    private float sensitivity = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSensitivity();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }

    public float GetSensitivity()
    {
        return sensitivity;
    }

    private void LoadSensitivity()
    {
        sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }
}
