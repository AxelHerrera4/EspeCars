using UnityEngine;
using System.Collections;

public class RockPower : PowerBase
{
    [Header("Rock Settings")]
    public GameObject rockPrefab;
    public int cantidad = 3;
    public float separation = 1.5f;
    public float spawnDistanceBehind = 4f;
    public float rockLifeTime = 8f;

    [Header("Visual Effects")]
    public Color rockEffectColor = new Color(0.6f, 0.4f, 0.2f, 0.7f); // Marrón
    public Color rockEffectColor2 = new Color(0.8f, 0.6f, 0.3f, 0.7f); // Naranja tierra
    public Color flashColor = new Color(1f, 0.7f, 0.3f, 1f); // Naranja brillante

    protected override void ActivatePower()
    {
        Vector3 basePos = transform.position - transform.forward * spawnDistanceBehind;

        // ¡DESTELLO INICIAL DESLUMBRANTE!
        StartCoroutine(FlashEffect(basePos, flashColor));
        StartCoroutine(EarthquakeEffect(basePos));

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 offset = transform.right * ((i - (cantidad - 1) / 2f) * separation);
            Vector3 spawnPos = basePos + offset;

            GameObject rock = Instantiate(
                rockPrefab,
                spawnPos,
                Quaternion.identity
            );
            
            // Efecto de impacto al aparecer
            StartCoroutine(RockSpawnEffect(spawnPos));
            StartCoroutine(RockDestroyEffect(rock, rockLifeTime));
        }
    }

    IEnumerator EarthquakeEffect(Vector3 center)
    {
        // Crear grietas radiales
        GameObject[] cracks = new GameObject[8];
        for (int i = 0; i < cracks.Length; i++)
        {
            cracks[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cracks[i].name = "RockCrack";
            Destroy(cracks[i].GetComponent<Collider>());
            
            float angle = i * (360f / cracks.Length);
            cracks[i].transform.position = center;
            cracks[i].transform.rotation = Quaternion.Euler(90f, angle, 0);
            cracks[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.05f);
            
            Renderer cRenderer = cracks[i].GetComponent<Renderer>();
            Material cMat = new Material(Shader.Find("Sprites/Default"));
            cMat.color = flashColor;
            cRenderer.material = cMat;
        }
        
        // Crear columnas de polvo
        GameObject[] dustPillars = new GameObject[4];
        for (int i = 0; i < dustPillars.Length; i++)
        {
            dustPillars[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            dustPillars[i].name = "DustPillar";
            Destroy(dustPillars[i].GetComponent<Collider>());
            
            float angle = i * 90f + 45f;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            dustPillars[i].transform.position = center + dir * 1.5f;
            dustPillars[i].transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
            
            Renderer dRenderer = dustPillars[i].GetComponent<Renderer>();
            Material dMat = new Material(Shader.Find("Sprites/Default"));
            dMat.color = rockEffectColor;
            dRenderer.material = dMat;
        }
        
        float elapsed = 0f;
        float duration = 1f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expandir grietas
            for (int i = 0; i < cracks.Length; i++)
            {
                if (cracks[i] != null)
                {
                    float crackLength = Mathf.Lerp(0.1f, 4f, t);
                    cracks[i].transform.localScale = new Vector3(0.15f * (1f - t * 0.5f), crackLength, 0.05f);
                    
                    float angle = i * (360f / cracks.Length);
                    Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                    cracks[i].transform.position = center + dir * crackLength * 0.5f;
                    
                    Renderer cRenderer = cracks[i].GetComponent<Renderer>();
                    Color c = flashColor;
                    c.a = Mathf.Lerp(1f, 0f, t);
                    cRenderer.material.color = c;
                }
            }
            
            // Subir columnas de polvo
            for (int i = 0; i < dustPillars.Length; i++)
            {
                if (dustPillars[i] != null)
                {
                    float height = Mathf.Lerp(0.1f, 3f, t * (1f - t) * 4f);
                    dustPillars[i].transform.localScale = new Vector3(0.5f * (1f - t * 0.5f), height, 0.5f * (1f - t * 0.5f));
                    dustPillars[i].transform.position += Vector3.up * Time.deltaTime * 2f;
                    
                    Renderer dRenderer = dustPillars[i].GetComponent<Renderer>();
                    Color dc = rockEffectColor;
                    dc.a = Mathf.Lerp(0.7f, 0f, t);
                    dRenderer.material.color = dc;
                }
            }
            
            yield return null;
        }
        
        foreach (var c in cracks)
        {
            if (c != null) Destroy(c);
        }
        foreach (var d in dustPillars)
        {
            if (d != null) Destroy(d);
        }
    }

    IEnumerator RockSpawnEffect(Vector3 position)
    {
        // Crear onda de impacto
        GameObject shockwave = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shockwave.name = "RockShockwave";
        shockwave.transform.position = position;
        shockwave.transform.localScale = new Vector3(0.1f, 0.01f, 0.1f);
        Destroy(shockwave.GetComponent<Collider>());
        
        Renderer swRenderer = shockwave.GetComponent<Renderer>();
        Material swMat = new Material(Shader.Find("Sprites/Default"));
        swMat.color = rockEffectColor;
        swRenderer.material = swMat;
        
        // Crear escombros voladores
        GameObject[] debris = new GameObject[8];
        for (int i = 0; i < debris.Length; i++)
        {
            debris[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            debris[i].name = "RockDebris";
            Destroy(debris[i].GetComponent<Collider>());
            
            debris[i].transform.position = position + Vector3.up * 0.2f;
            debris[i].transform.localScale = Vector3.one * Random.Range(0.05f, 0.15f);
            debris[i].transform.rotation = Random.rotation;
            
            Renderer dRenderer = debris[i].GetComponent<Renderer>();
            Material dMat = new Material(Shader.Find("Sprites/Default"));
            dMat.color = Random.value > 0.5f ? rockEffectColor : rockEffectColor2;
            dRenderer.material = dMat;
        }
        
        Vector3[] debrisVelocities = new Vector3[debris.Length];
        for (int i = 0; i < debris.Length; i++)
        {
            debrisVelocities[i] = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(2f, 5f),
                Random.Range(-3f, 3f)
            );
        }
        
        float elapsed = 0f;
        float duration = 1.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expandir onda
            float scale = Mathf.Lerp(0.1f, 3f, t);
            shockwave.transform.localScale = new Vector3(scale, 0.01f, scale);
            
            // Desvanecer onda
            Color c = rockEffectColor;
            c.a = Mathf.Lerp(0.7f, 0f, t);
            swMat.color = c;
            
            // Mover escombros con gravedad
            for (int i = 0; i < debris.Length; i++)
            {
                if (debris[i] != null)
                {
                    debrisVelocities[i].y -= 9.8f * Time.deltaTime;
                    debris[i].transform.position += debrisVelocities[i] * Time.deltaTime;
                    debris[i].transform.Rotate(Random.insideUnitSphere * 360f * Time.deltaTime);
                    
                    Renderer dRenderer = debris[i].GetComponent<Renderer>();
                    Color dColor = dRenderer.material.color;
                    dColor.a = Mathf.Lerp(0.7f, 0f, t);
                    dRenderer.material.color = dColor;
                }
            }
            
            yield return null;
        }
        
        Destroy(shockwave);
        foreach (var d in debris)
        {
            if (d != null) Destroy(d);
        }
    }

    IEnumerator RockDestroyEffect(GameObject rock, float lifetime)
    {
        yield return new WaitForSeconds(lifetime - 1f);
        
        if (rock == null) yield break;
        
        Vector3 originalScale = rock.transform.localScale;
        float elapsed = 0f;
        float duration = 1f;
        
        // Parpadeo antes de destruir
        while (elapsed < duration && rock != null)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Parpadear (escalar arriba y abajo)
            float pulse = 1f + Mathf.Sin(elapsed * 15f) * 0.1f * (1f - t);
            rock.transform.localScale = originalScale * pulse * Mathf.Lerp(1f, 0f, t * t);
            
            yield return null;
        }
        
        if (rock != null)
        {
            Destroy(rock);
        }
    }
}
