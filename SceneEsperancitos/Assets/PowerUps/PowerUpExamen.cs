using UnityEngine;

public class PowerUpExamen : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject prefabCohete;
    public float velocidadGiro = 50f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadGiro * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Averiguar QUIÉN tocó el examen
        PrometeoCarController jugador = other.GetComponentInParent<PrometeoCarController>();
        EnemyAI enemigo = other.GetComponentInParent<EnemyAI>();

        // Guardamos al coche completo (el GameObject padre)
        GameObject cocheTirador = null;
        if (jugador != null) cocheTirador = jugador.gameObject;
        if (enemigo != null) cocheTirador = enemigo.gameObject;

        // Si fue un coche válido, lanzamos el cohete y le decimos quién disparó
        if (cocheTirador != null)
        {
            lanzarCohete(cocheTirador); // <-- Le pasamos el tirador
            Destroy(gameObject);
        }
    }

    void lanzarCohete(GameObject tirador)
    {
        if (prefabCohete != null)
        {
            // Crear cohete un poco más arriba
            GameObject coheteNuevo = Instantiate(prefabCohete, transform.position + Vector3.up * 3f, transform.rotation);

            // Buscar el cerebro del cohete nuevo
            Rocket scriptCohete = coheteNuevo.GetComponent<Rocket>();
            if (scriptCohete != null)
            {
                // DECIRLE QUIÉN ES SU DUEÑO PARA QUE NO LO ATAQUE
                scriptCohete.cocheDueño = tirador;
            }
        }
    }
}