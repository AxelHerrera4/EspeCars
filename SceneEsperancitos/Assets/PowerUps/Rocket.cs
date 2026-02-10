using UnityEngine;

public class Rocket : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float velocidad = 25f;
    public float tiempoDeVida = 5f;
    public float alturaImpacto = 2f;

    // --- ESTA ES LA PIEZA QUE TE FALTA ---
    // Sin esta línea, el Examen no sabe dónde guardar el nombre del dueño
    [HideInInspector] public GameObject cocheDueño;

    private Transform objetivo;
    private bool objetivoFijado = false;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida); // Seguridad
        BuscarLider();
    }

    void BuscarLider()
    {
        GameObject[] autos = GameObject.FindGameObjectsWithTag("Player");

        float mayorZ = -99999f;
        GameObject lider = null;

        foreach (GameObject auto in autos)
        {
            // Si el auto es el dueño, lo ignoramos y pasamos al siguiente
            if (auto == cocheDueño) continue;

            // Buscamos al que vaya más lejos (Mayor Z)
            if (auto.transform.position.z > mayorZ)
            {
                mayorZ = auto.transform.position.z;
                lider = auto;
            }
        }

        if (lider != null)
        {
            objetivo = lider.transform;
            objetivoFijado = true;
            Debug.Log("🚀 Cohete persiguiendo a: " + lider.name);
        }
    }

    void Update()
    {
        if (!objetivoFijado || objetivo == null)
        {
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
            return;
        }

        Vector3 destinoAlto = objetivo.position + Vector3.up * alturaImpacto;
        transform.position = Vector3.MoveTowards(transform.position, destinoAlto, velocidad * Time.deltaTime);
        transform.LookAt(destinoAlto);

        if (Vector3.Distance(transform.position, destinoAlto) < 1.0f)
        {
            GolpearObjetivo();
        }
    }

    void GolpearObjetivo()
    {
        // Golpe al JUGADOR
        PrometeoCarController jugador = objetivo.GetComponentInParent<PrometeoCarController>();
        if (jugador != null)
        {
            jugador.StartCoroutine(jugador.RecibirDisparo());
        }

        // Golpe al BOT
        EnemyAI bot = objetivo.GetComponentInParent<EnemyAI>();
        if (bot != null)
        {
            bot.StartCoroutine(bot.RecibirDisparo());
        }

        Debug.Log("💥 ¡BOOM!");
        Destroy(gameObject, 0.1f);
    }
}