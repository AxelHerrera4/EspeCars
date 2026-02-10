using UnityEngine;
using UnityEngine.UI;

public class MuteUI : MonoBehaviour
{
    // [Header("UI")]
    public Button button;                 // Botón a cambiar
    public Sprite soundOnImage;            // Icono sonido ON
    public Sprite soundOffImage;           // Icono sonido OFF
    public Image iconImage;    

    [Header("Audio")]
    public AudioSource audioSource;        // Audio a mutear

    void Start()
    {
        if (audioSource == null && MusicManager.Instance != null)
            audioSource = MusicManager.Instance.musicSource;

        RefreshIcon();
    }

    public void ButtonClicked()
    {
        // Mute con MusicManager si existe
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMute();
            audioSource = MusicManager.Instance.musicSource;
        }
        else if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
        }

        RefreshIcon();
    }

    void RefreshIcon()
    {
        if (iconImage == null || audioSource == null) return;
        iconImage.sprite = audioSource.mute ? soundOffImage : soundOnImage;
    }
}
