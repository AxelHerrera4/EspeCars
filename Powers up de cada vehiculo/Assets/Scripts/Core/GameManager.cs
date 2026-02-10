using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Seleccion del jugador")]
    public Facultad facultadElegida;
    public string facultadSeleccionada; // "Software", "IASA", "Mecatronica", "Civil"
    public bool isPlayerReady;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetSeleccion(Facultad facultad)
    {
        facultadElegida = facultad;
        facultadSeleccionada = facultad.ToString();
        isPlayerReady = true;
    }

    public void SetSeleccion(string facultadNombre)
    {
        facultadSeleccionada = facultadNombre;
        isPlayerReady = true;
    }
}
