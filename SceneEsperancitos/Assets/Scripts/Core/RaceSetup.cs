using UnityEngine;

/// <summary>
/// Este script configura los carros al iniciar la escena de carrera.
/// El carro que coincide con la facultad seleccionada será el jugador,
/// los demás serán controlados por IA.
/// </summary>
public class RaceSetup : MonoBehaviour
{
    [Header("Carros por Facultad")]
    [Tooltip("Carro de Software")]
    public GameObject carroSoftware;
    
    [Tooltip("Carro de IASA")]
    public GameObject carroIASA;
    
    [Tooltip("Carro de Mecatrónica")]
    public GameObject carroMecatronica;
    
    [Tooltip("Carro de Civil")]
    public GameObject carroCivil;

    [Header("Cámara")]
    [Tooltip("La cámara que seguirá al jugador (opcional)")]
    public Transform cameraFollow;

    void Start()
    {
        ConfigurarCarros();
    }

    void ConfigurarCarros()
    {
        // Obtener la facultad seleccionada del GameManager
        string facultadSeleccionada = "";
        
        if (GameManager.Instance != null)
        {
            facultadSeleccionada = GameManager.Instance.facultadSeleccionada;
            Debug.Log($"[RaceSetup] Facultad seleccionada: {facultadSeleccionada}");
        }
        else
        {
            Debug.LogWarning("[RaceSetup] GameManager no encontrado. Usando Software por defecto.");
            facultadSeleccionada = "Software";
        }

        // Configurar cada carro
        ConfigurarCarro(carroSoftware, "Software", facultadSeleccionada);
        ConfigurarCarro(carroIASA, "IASA", facultadSeleccionada);
        ConfigurarCarro(carroMecatronica, "Mecatronica", facultadSeleccionada);
        ConfigurarCarro(carroCivil, "Civil", facultadSeleccionada);
    }

    void ConfigurarCarro(GameObject carro, string nombreFacultad, string facultadJugador)
    {
        if (carro == null)
        {
            Debug.LogWarning($"[RaceSetup] Carro de {nombreFacultad} no asignado.");
            return;
        }

        PrometeoCarController controller = carro.GetComponent<PrometeoCarController>();
        EnemyAI enemyAI = carro.GetComponent<EnemyAI>();
        PowerBase power = carro.GetComponent<PowerBase>();

        bool esJugador = nombreFacultad.ToLower() == facultadJugador.ToLower();

        if (controller != null)
        {
            controller.isAI = !esJugador;
            Debug.Log($"[RaceSetup] {carro.name} - isAI: {controller.isAI}");
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = !esJugador;
            Debug.Log($"[RaceSetup] {carro.name} - EnemyAI enabled: {enemyAI.enabled}");
        }

        if (power != null)
        {
            power.usePlayerInput = esJugador;
            Debug.Log($"[RaceSetup] {carro.name} - usePlayerInput: {power.usePlayerInput}");
        }

        // Si es el jugador, configurar la cámara para seguirlo
        if (esJugador && cameraFollow != null)
        {
            // Buscar script de cámara y asignar target
            var camFollow = cameraFollow.GetComponent<CameraFollow>();
            if (camFollow != null)
            {
                camFollow.carTransform = carro.transform;
            }
        }
    }
}
