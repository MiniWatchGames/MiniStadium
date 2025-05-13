using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsManager : MonoBehaviour
{
    public static GraphicsManager Instance;

    private readonly Vector2Int[] supportedResolutions = new Vector2Int[]
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1360, 768),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(3480, 2160)
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplySavedSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGraphicsSettings(int resolutionIndex, int displayModeIndex)
    {
        Vector2Int resolution = supportedResolutions[resolutionIndex];
        bool isFullScreen = displayModeIndex == 0;

        Screen.SetResolution(resolution.x, resolution.y, isFullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.SetInt("DisplayMode", displayModeIndex);
        PlayerPrefs.Save();
    }

    public void ApplySavedSettings()
    {
        int resIndex = PlayerPrefs.GetInt("ResolutionIndex", 3);
        int modeIndex = PlayerPrefs.GetInt("DisplayMode", 0);
        SetGraphicsSettings(resIndex, modeIndex);
    }
}
