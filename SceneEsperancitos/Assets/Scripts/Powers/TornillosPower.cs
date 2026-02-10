using UnityEngine;
using System.Collections;

public class TornillosPower : PowerBase
{
    [Header("Tornillos Settings")]
    public GameObject tornilloPrefab;
    public int cantidad = 6;
    public float separation = 1.2f;
    public float spawnDistanceBehind = 4f;
    public float spawnHeight = 0.1f;

    [Header("Visual Effects")]
    public Color sparkColor = new Color(1f, 0.8f, 0f, 0.8f); // Amarillo brillante
    public Color sparkColor2 = new Color(1f, 0.5f, 0f, 0.8f); // Naranja
    public Color flashColor = new Color(1f, 1f, 0.5f, 1f); // Blanco-amarillo deslumbrante

    protected override void ActivatePower()
    {
        if (tornilloPrefab == null)
        {
            Debug.LogError("[TORNILLOS POWER] tornilloPrefab no está asignado!");
            return;
        }

        Debug.Log($"[TORNILLOS POWER] Spawneando {cantidad} tornillos detrás de {gameObject.name}");

        Vector3 basePosition =
            transform.position - transform.forward * spawnDistanceBehind;
        basePosition.y += spawnHeight;

        // ¡DESTELLO INICIAL DESLUMBRANTE!
        StartCoroutine(FlashEffect(basePosition, flashColor));
        StartCoroutine(DazzleEffect(basePosition));

        // Efecto de dispersión general
        StartCoroutine(ScatterEffect(basePosition));

        for (int i = 0; i < cantidad; i++)
        {
            Vector3 offset = transform.right * (i - cantidad / 2f) * separation;
            Vector3 spawnPos = basePosition + offset;
            
            GameObject tornillo = Instantiate(tornilloPrefab, spawnPos, Quaternion.identity);
            
            // Efecto de chispa individual
            StartCoroutine(SparkEffect(spawnPos));
        }
    }

    IEnumerator DazzleEffect(Vector3 center)
    {
        // Crear anillo brillante que se expande
        GameObject dazzleRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        dazzleRing.name = "TornilloDazzle";
        Destroy(dazzleRing.GetComponent<Collider>());
        dazzleRing.transform.position = center;
        dazzleRing.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);
        
        Renderer dRenderer = dazzleRing.GetComponent<Renderer>();
        Material dMat = new Material(Shader.Find("Sprites/Default"));
        dMat.color = Color.white;
        dRenderer.material = dMat;
        
        // Crear estrellas de brillo
        GameObject[] stars = new GameObject[6];
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = GameObject.CreatePrimitive(PrimitiveType.Quad);
            stars[i].name = "DazzleStar";
            Destroy(stars[i].GetComponent<Collider>());
            
            float angle = i * (360f / stars.Length);
            stars[i].transform.position = center + Vector3.up * 0.5f;
            stars[i].transform.rotation = Quaternion.Euler(0, angle, 0);
            stars[i].transform.localScale = new Vector3(0.3f, 2f, 1f);
            
            Renderer sRenderer = stars[i].GetComponent<Renderer>();
            Material sMat = new Material(Shader.Find("Sprites/Default"));
            sMat.color = flashColor;
            sRenderer.material = sMat;
        }
        
        float elapsed = 0f;
        float duration = 0.8f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expandir anillo deslumbrante
            float ringScale = Mathf.Lerp(0.1f, 8f, t);
            dazzleRing.transform.localScale = new Vector3(ringScale, 0.05f, ringScale);
            
            // Desvanecer
            Color c = Color.white;
            c.a = Mathf.Lerp(1f, 0f, t);
            dMat.color = c;
            
            // Rotar y desvanecer estrellas
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    stars[i].transform.Rotate(0, 180f * Time.deltaTime, 0);
                    float starScale = Mathf.Lerp(2f, 4f, t) * (1f - t);
                    stars[i].transform.localScale = new Vector3(0.3f * (1f - t), starScale, 1f);
                    
                    Renderer sRenderer = stars[i].GetComponent<Renderer>();
                    Color sc = flashColor;
                    sc.a = Mathf.Lerp(1f, 0f, t);
                    sRenderer.material.color = sc;
                }
            }
            
            yield return null;
        }
        
        Destroy(dazzleRing);
        foreach (var s in stars)
        {
            if (s != null) Destroy(s);
        }
    }

    IEnumerator ScatterEffect(Vector3 center)
    {
        // Crear línea de efecto
        GameObject[] lines = new GameObject[cantidad + 1];
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lines[i].name = "TornilloLine";
            Destroy(lines[i].GetComponent<Collider>());
            
            float xPos = (i - lines.Length / 2f) * separation;
            lines[i].transform.position = center + transform.right * xPos;
            lines[i].transform.localScale = new Vector3(0.05f, 0.5f, 0.05f);
            
            Renderer lRenderer = lines[i].GetComponent<Renderer>();
            Material lMat = new Material(Shader.Find("Sprites/Default"));
            lMat.color = sparkColor;
            lRenderer.material = lMat;
        }
        
        float elapsed = 0f;
        float duration = 0.8f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != null)
                {
                    // Subir y desvanecer
                    lines[i].transform.position += Vector3.up * Time.deltaTime * 2f;
                    lines[i].transform.localScale = new Vector3(0.05f, Mathf.Lerp(0.5f, 0f, t), 0.05f);
                    
                    Renderer lRenderer = lines[i].GetComponent<Renderer>();
                    Color c = sparkColor;
                    c.a = Mathf.Lerp(0.8f, 0f, t);
                    lRenderer.material.color = c;
                }
            }
            
            yield return null;
        }
        
        foreach (var l in lines)
        {
            if (l != null) Destroy(l);
        }
    }

    IEnumerator SparkEffect(Vector3 position)
    {
        // Crear chispas
        GameObject[] sparks = new GameObject[5];
        Vector3[] sparkVelocities = new Vector3[sparks.Length];
        
        for (int i = 0; i < sparks.Length; i++)
        {
            sparks[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sparks[i].name = "Spark";
            Destroy(sparks[i].GetComponent<Collider>());
            
            sparks[i].transform.position = position;
            sparks[i].transform.localScale = Vector3.one * Random.Range(0.03f, 0.08f);
            
            Renderer sRenderer = sparks[i].GetComponent<Renderer>();
            Material sMat = new Material(Shader.Find("Sprites/Default"));
            sMat.color = Random.value > 0.5f ? sparkColor : sparkColor2;
            sRenderer.material = sMat;
            
            sparkVelocities[i] = new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(1f, 3f),
                Random.Range(-2f, 2f)
            );
        }
        
        float elapsed = 0f;
        float duration = 0.6f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            for (int i = 0; i < sparks.Length; i++)
            {
                if (sparks[i] != null)
                {
                    sparkVelocities[i].y -= 5f * Time.deltaTime;
                    sparks[i].transform.position += sparkVelocities[i] * Time.deltaTime;
                    
                    Renderer sRenderer = sparks[i].GetComponent<Renderer>();
                    Color c = sRenderer.material.color;
                    c.a = Mathf.Lerp(0.8f, 0f, t);
                    sRenderer.material.color = c;
                }
            }
            
            yield return null;
        }
        
        foreach (var s in sparks)
        {
            if (s != null) Destroy(s);
        }
    }
}
