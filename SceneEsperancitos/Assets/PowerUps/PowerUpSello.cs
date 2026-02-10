using UnityEngine;
using System.Collections;

public class PowerUpSello : MonoBehaviour
{
    [Header("Configuración Visual")]
    public float velocidadFlote = 3f;
    public float alturaFlote = 0.5f;
    private Vector3 posicionInicial;

    [Header("Configuración de Penalización")]
    public int penalizacionVelocidad = 40; // Bajará mucho el límite
    public int divisorPotencia = 4;        // ¡DIVIDE TU FUERZA ENTRE 4! (Muy lento)
    public float duracion = 3f;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Efecto de flotar arriba/abajo
        float onda = Mathf.Sin(Time.time * velocidadFlote);
        float nuevaY = posicionInicial.y + (onda * alturaFlote);
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        PrometeoCarController auto = other.GetComponentInParent<PrometeoCarController>();

        if (auto != null)
        {
            StartCoroutine(ActivarFreno(auto));
        }
    }

    IEnumerator ActivarFreno(PrometeoCarController auto)
    {
        // 1. Ocultar
        GetComponent<Collider>().enabled = false;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers) r.enabled = false;
        this.enabled = false; // Dejar de flotar

        // 2. GUARDAR VALORES ORIGINALES
        int maxSpeedOriginal = auto.maxSpeed;
        int aceleracionOriginal = auto.accelerationMultiplier;

        // 3. ¡APLICAR EL CASTIGO!
        // Bajamos el límite de velocidad (Mínimo a 10 para que no sea 0)
        auto.maxSpeed = Mathf.Max(10, maxSpeedOriginal - penalizacionVelocidad);

        // ¡AQUÍ ESTÁ LA CLAVE! Dividimos la fuerza del motor
        // Si tenías fuerza 10, ahora tendrás fuerza 2. ¡Sentirás que el coche pesa toneladas!
        auto.accelerationMultiplier = aceleracionOriginal / divisorPotencia;

        Debug.Log("⛔ ¡SELLO ACTIVADO! Coche ralentizado y pesado.");

        // 4. Sufrir por 3 segundos
        yield return new WaitForSeconds(duracion);

        // 5. RESTAURAR
        auto.maxSpeed = maxSpeedOriginal;
        auto.accelerationMultiplier = aceleracionOriginal;

        Debug.Log("✅ Penalización terminada.");

        // 6. Destruir
        Destroy(gameObject);
    }
}