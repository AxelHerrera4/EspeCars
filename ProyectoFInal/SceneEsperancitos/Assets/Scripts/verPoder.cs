using UnityEngine;

public class verPoder: MonoBehaviour
{
    public GameObject informacion;
    private bool juegoPausado = false;

    // Muestra / oculta solo la info
    public void ToggleInformacion()
    {
        if (informacion != null)
        {
            bool activo = !informacion.activeSelf;
            informacion.SetActive(activo);
        }
    }

    // Muestra la info Y pausa / reanuda el juego
    public void ToggleInformacionYPausa()
    {
        if (informacion == null) return;

        juegoPausado = !juegoPausado;

        // Mostrar u ocultar UI
        informacion.SetActive(juegoPausado);

        // Pausar o reanudar el juego
        Time.timeScale = juegoPausado ? 0f : 1f;
    }
}
