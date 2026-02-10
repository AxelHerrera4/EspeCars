using UnityEngine;
using System.Collections;

public class SoftwareHackPower : PowerBase
{
    [Header("Hack Settings")]
    public float hackRadius = 15f;
    public float hackDuration = 5f;

    [Header("Visual Effect")]
    public Color nebulaColor = new Color(0f, 1f, 1f, 0.5f); // Cyan translúcido
    public Color nebulaColor2 = new Color(0.5f, 0f, 1f, 0.5f); // Púrpura
    public float effectDuration = 3f;

    protected override void ActivatePower()
    {
        Debug.Log($"[HACK POWER] Activando hack en {gameObject.name} con radio {hackRadius}");
        
        // Crear efecto visual de nebulosa
        StartCoroutine(CreateNebulaEffect());
        
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            hackRadius
        );

        Debug.Log($"[HACK POWER] Encontrados {hits.Length} colliders");

        foreach (Collider hit in hits)
        {
            // Ignorar el propio objeto y su jerarquía
            if (hit.transform.root == transform.root) continue;

            // Buscar IHackable en el objeto y sus padres
            IHackable hackable = hit.GetComponentInParent<IHackable>();
            
            // Si no lo encuentra, buscar en el transform root
            if (hackable == null)
            {
                hackable = hit.transform.root.GetComponent<IHackable>();
            }
            
            // Buscar via Rigidbody
            if (hackable == null && hit.attachedRigidbody != null)
            {
                hackable = hit.attachedRigidbody.GetComponent<IHackable>();
            }
            
            if (hackable != null)
            {
                // Verificar que no sea el mismo objeto que lanzó el poder
                MonoBehaviour hackableMB = hackable as MonoBehaviour;
                if (hackableMB != null && hackableMB.transform.root != transform.root)
                {
                    Debug.Log($"[HACK POWER] Hackeando a {hit.transform.root.name}");
                    hackable.ApplyHack(hackDuration);
                }
            }
        }
    }

    IEnumerator CreateNebulaEffect()
    {
        // Crear esfera de nebulosa principal
        GameObject nebula = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nebula.name = "HackNebula";
        nebula.transform.position = transform.position;
        
        // Desactivar collider
        Destroy(nebula.GetComponent<Collider>());
        
        // Crear material con shader transparente
        Renderer renderer = nebula.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = nebulaColor;
        renderer.material = mat;
        
        // Crear segunda esfera interior
        GameObject nebula2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nebula2.name = "HackNebulaInner";
        nebula2.transform.position = transform.position;
        Destroy(nebula2.GetComponent<Collider>());
        
        Renderer renderer2 = nebula2.GetComponent<Renderer>();
        Material mat2 = new Material(Shader.Find("Sprites/Default"));
        mat2.color = nebulaColor2;
        renderer2.material = mat2;
        
        // Crear partículas digitales (pequeñas esferas)
        GameObject[] particles = new GameObject[20];
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            particles[i].name = "HackParticle";
            Destroy(particles[i].GetComponent<Collider>());
            
            // Posición aleatoria dentro de la esfera
            Vector3 randomDir = Random.insideUnitSphere * hackRadius * 0.8f;
            particles[i].transform.position = transform.position + randomDir;
            particles[i].transform.localScale = Vector3.one * Random.Range(0.1f, 0.4f);
            
            Renderer pRenderer = particles[i].GetComponent<Renderer>();
            Material pMat = new Material(Shader.Find("Sprites/Default"));
            pMat.color = Random.value > 0.5f ? nebulaColor : nebulaColor2;
            pRenderer.material = pMat;
        }
        
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * hackRadius * 2f;
        Vector3 endScale2 = Vector3.one * hackRadius * 1.5f;
        
        // Animación de expansión
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / effectDuration;
            float easeOut = 1f - Mathf.Pow(1f - t, 3f); // Ease out cubic
            
            // Expandir esferas
            nebula.transform.localScale = Vector3.Lerp(startScale, endScale, easeOut);
            nebula2.transform.localScale = Vector3.Lerp(startScale, endScale2, easeOut);
            
            // Rotar esferas
            nebula.transform.Rotate(0f, 180f * Time.deltaTime, 0f);
            nebula2.transform.Rotate(0f, -240f * Time.deltaTime, 0f);
            
            // Desvanecer
            Color c1 = nebulaColor;
            c1.a = Mathf.Lerp(0.5f, 0f, t);
            mat.color = c1;
            
            Color c2 = nebulaColor2;
            c2.a = Mathf.Lerp(0.5f, 0f, t);
            mat2.color = c2;
            
            // Mover partículas hacia afuera y desvanecer
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null)
                {
                    Vector3 dir = (particles[i].transform.position - transform.position).normalized;
                    particles[i].transform.position += dir * Time.deltaTime * 5f;
                    particles[i].transform.Rotate(Random.insideUnitSphere * 360f * Time.deltaTime);
                    
                    Renderer pRenderer = particles[i].GetComponent<Renderer>();
                    Color pColor = pRenderer.material.color;
                    pColor.a = Mathf.Lerp(0.8f, 0f, t);
                    pRenderer.material.color = pColor;
                }
            }
            
            yield return null;
        }
        
        // Limpiar
        Destroy(nebula);
        Destroy(nebula2);
        foreach (var p in particles)
        {
            if (p != null) Destroy(p);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hackRadius);
    }
}
