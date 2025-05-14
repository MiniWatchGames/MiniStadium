using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ButtonSound : MonoBehaviour
{
    private AudioManager audiomanager;
    public AudioClip clickSound;
    private UnityEngine.UI.Button button;

    private void Start()
    {
        audiomanager = AudioManager.Instance;
        button = GetComponent<UnityEngine.UI.Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
            audiomanager.audioSource.PlayOneShot(clickSound);
    }
}
