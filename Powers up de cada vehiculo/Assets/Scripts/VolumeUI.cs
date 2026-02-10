using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour
{
    public Slider volumeSlider;
    public Button muteButton; // Asigna este botón desde el Inspector

    private void Start()
    {
        if (volumeSlider == null)
            volumeSlider = GetComponentInChildren<Slider>(true);

        if (volumeSlider == null)
        {
            Debug.LogError("VolumeUI: No se encontró un Slider (asígnalo en el Inspector o pon este script en el objeto Slider).", this);
            enabled = false;
            return;
        }

        // Cargar el valor actual del manager
        if (MusicManager.Instance != null)
            volumeSlider.SetValueWithoutNotify(MusicManager.Instance.GetVolume());

        volumeSlider.onValueChanged.AddListener(OnSliderChanged);

        if (muteButton != null)
            muteButton.onClick.AddListener(OnMuteClicked);
    }


    private void OnDestroy()
    {
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);

        if (muteButton != null)
            muteButton.onClick.RemoveListener(OnMuteClicked);
    }


    private void OnSliderChanged(float v)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(v);
    }

    private void OnMuteClicked()
    {
        Debug.Log("CLICK MUTE");

        if (MusicManager.Instance != null)
        {
            Debug.Log("HAY MusicManager");
            MusicManager.Instance.ToggleMute();

            // Actualizar el slider para reflejar el cambio
            volumeSlider.SetValueWithoutNotify(MusicManager.Instance.GetVolume());
        }
        else
        {
            Debug.LogError("NO HAY MusicManager.Instance (estás iniciando desde Configuración?)");
        }
    }
}
