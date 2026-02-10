using UnityEngine;
using System.Collections; // Necesario para usar Corutinas (tiempos de espera)

public class PowerUpRotator : MonoBehaviour
{
    // --- CONFIGURACIÓN VISUAL ---
    [Header("Animación Visual")]
    public float velocidadGiro = 50f;
    public float alturaFlote = 0.5f;
    public float velocidadFlote = 1f;
    private Vector3 posInicial;

    // --- CONFIGURACIÓN DEL TURBO ---
    [Header("Efecto Turbo")]
    public float fuerzaTurbo = 2f; // Multiplica la velocidad actual (ej: x2)
    public float tiempoDuracion = 2f; // Cuánto dura el efecto en segundos
    private bool consumido = false; // Evita que se active doble vez

    void Start()
    {
        // Guardamos la altura inicial para que flote desde ahí
        posInicial = transform.position;
    }

    void Update()
    {
        // Solo animamos si el objeto NO ha sido consumido todavía
        if (!consumido)
        {
            // 1. GIRO: Rota sobre su propio eje
            transform.Rotate(Vector3.up * velocidadGiro * Time.deltaTime);

            // 2. FLOTE: Sube y baja suavemente
            float nuevoY = posInicial.y + Mathf.Sin(Time.time * velocidadFlote) * alturaFlote;
            transform.position = new Vector3(posInicial.x, nuevoY, posInicial.z);
        }
    }

    // --- LÓGICA DEL CHOQUE ---
    private void OnTriggerEnter(Collider other)
    {
        if (consumido) return; // Si ya se usó, ignorar

        // Buscamos si chocamos con un auto (el script puede estar en el padre)
        PrometeoCarController auto = other.GetComponentInParent<PrometeoCarController>();

        if (auto != null)
        {
            consumido = true; // ¡Capturado!
            StartCoroutine(DarSuperVelocidad(auto));
        }
    }

    // --- RUTINA DE TIEMPO (CORREGIDA) ---
    // --- RUTINA DE TIEMPO CORREGIDA (PARA OBJETOS COMPLEJOS) ---
    IEnumerator DarSuperVelocidad(PrometeoCarController auto)
    {
        // 1. Ocultamos VISUALMENTE (Buscamos en los hijos por si el modelo está dentro)
        // Usamos 'GetComponentsInChildren' (en plural) para encontrar todas las partes de la taza
        Renderer[] partesVisuales = GetComponentsInChildren<Renderer>();
        foreach (Renderer parte in partesVisuales)
        {
            parte.enabled = false;
        }

        // 2. Desactivamos el choque (El collider sí lo tienes en el padre, así que esto está bien)
        GetComponent<Collider>().enabled = false;

        // 3. Guardamos la velocidad original
        float aceleracionOriginal = auto.accelerationMultiplier;

        // 4. ¡Aplicamos el TURBO! (Con (int) para evitar errores)
        auto.accelerationMultiplier = (int)(aceleracionOriginal * fuerzaTurbo);

        Debug.Log("¡Turbo activado! Velocidad x" + fuerzaTurbo);

        // 5. Esperamos los segundos configurados
        yield return new WaitForSeconds(tiempoDuracion);

        // 6. Devolvemos la velocidad a la normalidad
        auto.accelerationMultiplier = (int)aceleracionOriginal;

        // 7. Destruimos el objeto
        Destroy(gameObject);
    }
}