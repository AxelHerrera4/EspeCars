using UnityEngine;
using System.Collections;

public class CowPower : PowerBase
{
    [Header("Cow Settings")]
    public GameObject cowPrefab;
    public float spawnDistanceBehind = 5f;
    public float lifetime = 10f;
    public float destroyEffectDuration = 1f;

    [Header("Visual Effects")]
    public Color spawnColor = new Color(0f, 1f, 0f, 0.7f); // Verde
    public Color spawnColor2 = new Color(1f, 1f, 0f, 0.7f); // Amarillo
    public Color flashColor = new Color(0.5f, 1f, 0.5f, 1f); // Verde brillante

    protected override void ActivatePower()
    {
        Vector3 basePos = transform.position - transform.forward * spawnDistanceBehind;
        Vector3 spawnPos = new Vector3(basePos.x, 0.009f, basePos.z);

        // ¡DESTELLO INICIAL DESLUMBRANTE!
        StartCoroutine(FlashEffect(spawnPos, flashColor));
        StartCoroutine(CowSummonEffect(spawnPos));

        Quaternion rotation = Quaternion.Euler(90f, 0f, -90f);
        GameObject cow = Instantiate(cowPrefab, spawnPos, rotation);
        
        // Efecto de aparición
        StartCoroutine(SpawnEffect(spawnPos));
        StartCoroutine(DestroyWithEffect(cow));
    }

    IEnumerator CowSummonEffect(Vector3 position)
    {
        // Crear portal mágico
        GameObject portal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        portal.name = "CowPortal";
        Destroy(portal.GetComponent<Collider>());
        portal.transform.position = position;
        portal.transform.localScale = new Vector3(0.1f, 0.02f, 0.1f);
        
        Renderer pRenderer = portal.GetComponent<Renderer>();
        Material pMat = new Material(Shader.Find("Sprites/Default"));
        pMat.color = Color.white;
        pRenderer.material = pMat;
        
        // Crear espirales
        GameObject[] spirals = new GameObject[12];
        for (int i = 0; i < spirals.Length; i++)
        {
            spirals[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spirals[i].name = "CowSpiral";
            Destroy(spirals[i].GetComponent<Collider>());
            
            spirals[i].transform.position = position;
            spirals[i].transform.localScale = Vector3.one * 0.15f;
            
            Renderer sRenderer = spirals[i].GetComponent<Renderer>();
            Material sMat = new Material(Shader.Find("Sprites/Default"));
            sMat.color = i % 2 == 0 ? flashColor : spawnColor;
            sRenderer.material = sMat;
        }
        
        float elapsed = 0f;
        float duration = 1.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expandir portal con brillo
            float portalScale = Mathf.Lerp(0.1f, 5f, t);
            portal.transform.localScale = new Vector3(portalScale, 0.02f, portalScale);
            portal.transform.Rotate(0, 360f * Time.deltaTime, 0);
            
            Color pc = Color.Lerp(Color.white, flashColor, t);
            pc.a = Mathf.Lerp(1f, 0f, t);
            pMat.color = pc;
            
            // Animar espirales subiendo en espiral
            for (int i = 0; i < spirals.Length; i++)
            {
                if (spirals[i] != null)
                {
                    float spiralAngle = (i * 30f) + elapsed * 360f;
                    float radius = Mathf.Lerp(0.5f, 2f, t) * (1f - t);
                    float height = t * 3f;
                    
                    Vector3 spiralPos = position + new Vector3(
                        Mathf.Cos(spiralAngle * Mathf.Deg2Rad) * radius,
                        height,
                        Mathf.Sin(spiralAngle * Mathf.Deg2Rad) * radius
                    );
                    spirals[i].transform.position = spiralPos;
                    spirals[i].transform.localScale = Vector3.one * 0.15f * (1f - t);
                    
                    Renderer sRenderer = spirals[i].GetComponent<Renderer>();
                    Color sc = sRenderer.material.color;
                    sc.a = Mathf.Lerp(1f, 0f, t);
                    sRenderer.material.color = sc;
                }
            }
            
            yield return null;
        }
        
        Destroy(portal);
        foreach (var s in spirals)
        {
            if (s != null) Destroy(s);
        }
    }

    IEnumerator SpawnEffect(Vector3 position)
    {
        // Crear anillo de luz
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "CowSpawnRing";
        ring.transform.position = position;
        ring.transform.localScale = new Vector3(0.1f, 0.02f, 0.1f);
        Destroy(ring.GetComponent<Collider>());
        
        Renderer ringRenderer = ring.GetComponent<Renderer>();
        Material ringMat = new Material(Shader.Find("Sprites/Default"));
        ringMat.color = spawnColor;
        ringRenderer.material = ringMat;
        
        // Crear partículas de polvo
        GameObject[] dustParticles = new GameObject[12];
        for (int i = 0; i < dustParticles.Length; i++)
        {
            dustParticles[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dustParticles[i].name = "CowDust";
            Destroy(dustParticles[i].GetComponent<Collider>());
            
            float angle = i * (360f / dustParticles.Length);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            dustParticles[i].transform.position = position + dir * 0.5f;
            dustParticles[i].transform.localScale = Vector3.one * Random.Range(0.1f, 0.3f);
            
            Renderer pRenderer = dustParticles[i].GetComponent<Renderer>();
            Material pMat = new Material(Shader.Find("Sprites/Default"));
            pMat.color = Random.value > 0.5f ? spawnColor : spawnColor2;
            pRenderer.material = pMat;
        }
        
        float elapsed = 0f;
        float duration = 1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expandir anillo
            float scale = Mathf.Lerp(0.1f, 4f, t);
            ring.transform.localScale = new Vector3(scale, 0.02f, scale);
            
            // Desvanecer anillo
            Color c = spawnColor;
            c.a = Mathf.Lerp(0.7f, 0f, t);
            ringMat.color = c;
            
            // Mover partículas hacia arriba y afuera
            for (int i = 0; i < dustParticles.Length; i++)
            {
                if (dustParticles[i] != null)
                {
                    Vector3 dir = (dustParticles[i].transform.position - position).normalized;
                    dir.y = 0.5f;
                    dustParticles[i].transform.position += dir * Time.deltaTime * 3f;
                    
                    Renderer pRenderer = dustParticles[i].GetComponent<Renderer>();
                    Color pColor = pRenderer.material.color;
                    pColor.a = Mathf.Lerp(0.7f, 0f, t);
                    pRenderer.material.color = pColor;
                }
            }
            
            yield return null;
        }
        
        Destroy(ring);
        foreach (var p in dustParticles)
        {
            if (p != null) Destroy(p);
        }
    }

    IEnumerator DestroyWithEffect(GameObject cow)
    {
        // Esperar el tiempo de vida
        yield return new WaitForSeconds(lifetime);
        
        if (cow == null) yield break;
        
        Vector3 originalScale = cow.transform.localScale;
        float elapsed = 0f;
        
        // Efecto de encogimiento con rotación
        while (elapsed < destroyEffectDuration && cow != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / destroyEffectDuration;
            
            // Encoger
            cow.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            
            // Rotar mientras encoge
            cow.transform.Rotate(0f, 360f * Time.deltaTime * 3f, 0f);
            
            // Elevar un poco
            cow.transform.position += Vector3.up * Time.deltaTime * 2f;
            
            yield return null;
        }
        
        if (cow != null)
        {
            Destroy(cow);
        }
    }
}
