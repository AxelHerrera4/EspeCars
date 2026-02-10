using UnityEngine;
using System.Collections;

public class PowerUpCarpa : MonoBehaviour
{
    [Header("Configuración Visual")]
    [Tooltip("Qué tan rápido sube y baja")]
    public float velocidadFlote = 3f;
    [Tooltip("Qué tan alto sube y baja (en metros)")]
    public float alturaFlote = 0.5f;

    private Vector3 posicionInicial;

    [Header("Configuración del Power-Up")]
    public int velocidadExtra = 60;
    public int multiplicadorPotencia = 3;
    public float duracion = 5f;

    void Start()
    {
        // Guardamos dónde estaba la carpa al empezar para flotar alrededor de ese punto
        posicionInicial = transform.position;
    }

    void Update()
    {
        // --- EFECTO DE FLOTAR (Arriba y Abajo) ---
        // Usamos el tiempo para generar una onda suave que va de -1 a 1
        float onda = Mathf.Sin(Time.time * velocidadFlote);

        // Calculamos la nueva altura Y
        float nuevaY = posicionInicial.y + (onda * alturaFlote);

        // Aplicamos la nueva posición, manteniendo X y Z iguales
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);

        // OPCIONAL: Si quieres que ADEMÁS gire un poquito mientras flota, descomenta esta línea:
        // transform.Rotate(Vector3.up * 20f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PrometeoCarController auto = other.GetComponentInParent<PrometeoCarController>();

        if (auto != null)
        {
            StartCoroutine(ActivarSuperVelocidad(auto));
        }
    }

    IEnumerator ActivarSuperVelocidad(PrometeoCarController auto)
    {
        // 1. Ocultar visualmente y desactivar collider
        GetComponent<Collider>().enabled = false;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) r.enabled = false;
        // Desactivamos este script para que deje de moverse mientras está oculto
        this.enabled = false;

        // 2. GUARDAR VALORES ORIGINALES
        int maxSpeedOriginal = auto.maxSpeed;
        int aceleracionOriginal = auto.accelerationMultiplier;

        // 3. ¡ACTIVAR TURBO!
        auto.maxSpeed = maxSpeedOriginal + velocidadExtra;
        auto.accelerationMultiplier = aceleracionOriginal * multiplicadorPotencia;

        Debug.Log("🚀 ¡TURBO ACTIVADO!");

        // 4. Esperar
        yield return new WaitForSeconds(duracion);

        // 5. RESTAURAR
        auto.maxSpeed = maxSpeedOriginal;
        auto.accelerationMultiplier = aceleracionOriginal;

        // 6. Destruir
        Destroy(gameObject);
    }
}