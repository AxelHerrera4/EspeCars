using UnityEngine;
using System.Collections;

public class TornilloObstacle : MonoBehaviour
{
    public float slowMultiplier = 0.10f; // 10% de velocidad
    public float slowDuration = 8f;
    public float lifetime = 15f;

    private bool hasHitCar = false;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitCar) return;

        // Ignorar otros tornillos y el piso
        if (other.gameObject.name.Contains("Tornillo") || other.gameObject.name.Contains("floor"))
            return;

        Debug.Log($"[TORNILLO] OnTriggerEnter con {other.gameObject.name}, tag: {other.tag}");

        // Buscar ISlowable en el objeto, sus padres, o en el root
        ISlowable slowable = other.GetComponentInParent<ISlowable>();
        
        // Si no lo encuentra, buscar en el transform root
        if (slowable == null)
        {
            slowable = other.transform.root.GetComponent<ISlowable>();
        }

        if (slowable != null)
        {
            Debug.Log($"[TORNILLO] Encontrado ISlowable en {other.transform.root.name}, aplicando slow por {slowDuration} segundos");
            hasHitCar = true;
            
            // Obtener el MonoBehaviour para ejecutar la corrutina en el carro, no en el tornillo
            MonoBehaviour targetMB = slowable as MonoBehaviour;
            if (targetMB != null)
            {
                targetMB.StartCoroutine(SlowRoutineOnTarget(slowable));
            }
            
            // Destruir el tornillo inmediatamente después de golpear
            Destroy(gameObject, 0.1f);
        }
        else if (other.attachedRigidbody != null)
        {
            // Intentar buscar en el rigidbody adjunto
            slowable = other.attachedRigidbody.GetComponent<ISlowable>();
            if (slowable != null)
            {
                Debug.Log($"[TORNILLO] Encontrado ISlowable via Rigidbody en {other.attachedRigidbody.name}");
                hasHitCar = true;
                
                MonoBehaviour targetMB = slowable as MonoBehaviour;
                if (targetMB != null)
                {
                    targetMB.StartCoroutine(SlowRoutineOnTarget(slowable));
                }
                
                Destroy(gameObject, 0.1f);
            }
        }
    }

    // Esta corrutina se ejecuta en el carro objetivo, no en el tornillo
    IEnumerator SlowRoutineOnTarget(ISlowable slowable)
    {
        slowable.ApplySlow(slowMultiplier);
        Debug.Log($"[TORNILLO] Slow aplicado, esperando {slowDuration} segundos...");
        
        yield return new WaitForSeconds(slowDuration);
        
        Debug.Log($"[TORNILLO] Tiempo de slow terminado, restaurando velocidad");
        slowable.RemoveSlow();
    }
}