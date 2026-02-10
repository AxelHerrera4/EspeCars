using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Ruta del Circuito")]
    public Transform rutaPadre; // Arrastra aquí el objeto "Ruta_IA"
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;
    private float memoriaVelocidadIA;

    [Header("Configuración IA")]
    public float distanciaCambio = 15f; // Distancia para ir al siguiente punto

    // Referencias internas
    private PrometeoCarController carController;
    private Rigidbody rb;
    private float tiempoAtascado = 0f;

    void Start()
    {
        UnityEngine.AI.NavMeshAgent agente = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agente != null)
        {
            memoriaVelocidadIA = agente.speed;
        }
        carController = GetComponent<PrometeoCarController>();
        rb = GetComponent<Rigidbody>();

        // 1. ENCENDEMOS EL MODO ROBOT
        carController.isAI = true;

        // 2. Buscamos todos los puntos dentro de la carpeta de la ruta
        if (rutaPadre != null)
        {
            foreach (Transform child in rutaPadre)
            {
                if (child != rutaPadre) waypoints.Add(child);
            }
        }
        else
        {
            Debug.LogError("¡OJO! No has asignado la Ruta Padre en el Inspector.");
        }
    }

    void Update()
    {
        if (waypoints.Count == 0) return;

        // --- PARTE 1: EL SENSOR (Detectamos en qué estado estamos) ---
        // Si la velocidad es casi 0, aumentamos el contador de tiempo atascado
        if (rb.linearVelocity.magnitude < 1f)
        {
            tiempoAtascado += Time.deltaTime;
        }
        else
        {
            tiempoAtascado = 0f; // Si nos movemos, reseteamos el contador
        }

        // --- PARTE 2: LA MÁQUINA DE ESTADOS (El Cerebro decide) ---

        // ESTADO A: RECUPERACIÓN (Recovery)
        // Si llevamos más de 2 segundos parados, activamos la maniobra
        if (tiempoAtascado > 2f)
        {
            ManiobraDeRescate();
        }
        // ESTADO B: CARRERA (Racing)
        // Si no estamos atascados, conducimos normal
        else
        {
            Moverse();
        }
    }

    // ACCIÓN FÍSICA: Solo contiene los movimientos para salir del atasco
    void ManiobraDeRescate()
    {
        carController.GoReverse();
        carController.TurnRight();

        // Si llevamos mucho tiempo intentando (4s), reiniciamos para probar de nuevo
        if (tiempoAtascado > 4f) tiempoAtascado = 0f;
    }

    // ACCIÓN FÍSICA: Navegación normal por la pista
    void Moverse()
    {
        // A. Calcular dirección hacia el siguiente punto
        Vector3 destino = waypoints[currentWaypoint].position;
        Vector3 vectorRelativo = transform.InverseTransformPoint(destino);

        // B. Acelerar siempre 
        carController.GoForward();

        // C. Girar según matemáticas vectoriales
        float giro = vectorRelativo.x / vectorRelativo.magnitude;

        if (giro > 0.1f) carController.TurnRight();
        else if (giro < -0.1f) carController.TurnLeft();
        else carController.ResetSteeringAngle();

        // D. Cambiar de punto si estamos cerca
        if (Vector3.Distance(transform.position, destino) < distanciaCambio)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count) currentWaypoint = 0; // Loop infinito
        }
    }

    // --- VISUALIZADOR DE RUTA (GIZMOS) ---
    void OnDrawGizmos()
    {
        if (rutaPadre == null) return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < rutaPadre.childCount; i++)
        {
            Vector3 actual = rutaPadre.GetChild(i).position;
            Vector3 siguiente = rutaPadre.GetChild((i + 1) % rutaPadre.childCount).position;

            Gizmos.DrawSphere(actual, 0.5f);
            Gizmos.DrawLine(actual, siguiente);
        }
    }

    // Variables nuevas (ponlas al principio de la clase con las demás variables)
    private bool estaAturdido = false;
    private float velocidadGuardada = 0f;

    // ... (resto de tu código) ...

    // Sustituye tu rutina RecibirDisparo por esta blindada:
    public System.Collections.IEnumerator RecibirDisparo()
    {
        UnityEngine.AI.NavMeshAgent agente = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agente != null)
        {
            // 1. CASTIGO
            agente.speed = 0;
            agente.isStopped = true; // "Freno de mano" puesto
            agente.velocity = Vector3.zero; // Parada física

            // 2. ESPERA
            yield return new WaitForSeconds(3f);

            // 3. RESTAURACIÓN PERFECTA
            agente.isStopped = false; // Quitamos freno de mano
            agente.speed = memoriaVelocidadIA; // Vuelve a su velocidad original
        }
    }
}