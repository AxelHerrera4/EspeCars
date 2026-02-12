using UnityEngine;

public class verPoder: MonoBehaviour
{
    public GameObject informacion;
    private bool juegoPausado = false;

    // Muestra / oculta la info y pausa el juego
    public void ToggleInformacion()
    {
        if (informacion != null)
        {
            bool activo = !informacion.activeSelf;
            informacion.SetActive(activo);
            
            // Pausar cuando se muestra, reanudar cuando se oculta
            Time.timeScale = activo ? 0f : 1f;
            juegoPausado = activo;
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
