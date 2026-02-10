using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class PowerBase : MonoBehaviour
{
    [Header("Power Settings")]
    public float cooldownTime = 15f;
    public float initialDelay = 10f;  // Esperar 10 segundos al inicio
    public int maxUses = 2;           // Solo 2 usos
    protected float currentCooldown;
    protected int usesRemaining;
    protected float gameStartTime;

    [Header("UI")]
    public Image powerCircle;   // Barra circular (HUD) - SOLO PARA JUGADOR

    [Header("UI Animation")]
    public Color chargedColor = new Color(0f, 1f, 0.5f, 1f);    // Verde brillante
    public Color chargingColor = new Color(1f, 0.8f, 0f, 1f);   // Amarillo
    public Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gris
    private Color originalCircleColor;
    private bool wasCharged = false;
    private Coroutine pulseCoroutine;

    [Header("Input")]
    public bool usePlayerInput = true; // false para IA

    protected virtual void Start()
    {
        gameStartTime = Time.time;
        usesRemaining = maxUses;
        currentCooldown = 0f;  // Empieza en 0, debe esperar initialDelay + cargar

        // Solo configurar UI para el jugador
        if (usePlayerInput && powerCircle != null)
        {
            originalCircleColor = powerCircle.color;
            powerCircle.fillAmount = 0f;
            powerCircle.color = emptyColor;
        }
    }

    protected virtual void Update()
    {
        HandleCooldown();
        
        // Solo animar UI para el jugador
        if (usePlayerInput)
        {
            UpdatePowerCircleAnimation();
        }

        if (usePlayerInput && CanActivate() && PlayerPressedPower())
        {
            TryActivate();
        }
    }

    // ================= CORE LOGIC =================

    protected void HandleCooldown()
    {
        // Verificar si pasó el delay inicial
        float timeSinceStart = Time.time - gameStartTime;
        if (timeSinceStart < initialDelay)
        {
            // Durante el delay inicial, mostrar progreso del delay
            if (usePlayerInput && powerCircle != null)
            {
                powerCircle.fillAmount = timeSinceStart / initialDelay;
                powerCircle.color = emptyColor;
            }
            return;
        }

        // Después del delay inicial, manejar cooldown normal
        if (currentCooldown < cooldownTime)
        {
            currentCooldown += Time.deltaTime;

            // Solo actualizar UI para el jugador
            if (usePlayerInput && powerCircle != null)
            {
                powerCircle.fillAmount = currentCooldown / cooldownTime;
                powerCircle.color = Color.Lerp(chargingColor, chargedColor, currentCooldown / cooldownTime);
            }
        }
    }

    protected void UpdatePowerCircleAnimation()
    {
        if (powerCircle == null) return;

        bool isNowCharged = CanActivate();

        // Detectar cuando se carga completamente
        if (isNowCharged && !wasCharged)
        {
            // Acaba de cargarse - iniciar animación de pulso
            if (pulseCoroutine != null) StopCoroutine(pulseCoroutine);
            pulseCoroutine = StartCoroutine(ChargedPulseAnimation());
        }
        
        // Si no hay usos restantes, mostrar vacío
        if (usesRemaining <= 0)
        {
            powerCircle.fillAmount = 0f;
            powerCircle.color = emptyColor;
            if (pulseCoroutine != null)
            {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
        }

        wasCharged = isNowCharged;
    }

    IEnumerator ChargedPulseAnimation()
    {
        // Flash inicial brillante
        for (int i = 0; i < 3; i++)
        {
            powerCircle.color = Color.white;
            powerCircle.transform.localScale = Vector3.one * 1.3f;
            yield return new WaitForSeconds(0.1f);
            
            powerCircle.color = chargedColor;
            powerCircle.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.1f);
        }

        // Pulso continuo mientras está cargado
        while (CanActivate() && usesRemaining > 0)
        {
            float pulse = Mathf.Sin(Time.time * 4f) * 0.1f + 1f;
            powerCircle.transform.localScale = Vector3.one * pulse;
            
            // Brillo suave
            float brightness = Mathf.Sin(Time.time * 3f) * 0.2f + 0.8f;
            powerCircle.color = chargedColor * brightness + Color.white * (1f - brightness) * 0.3f;
            
            yield return null;
        }

        powerCircle.transform.localScale = Vector3.one;
    }

    protected bool CanActivate()
    {
        float timeSinceStart = Time.time - gameStartTime;
        return timeSinceStart >= initialDelay && 
               currentCooldown >= cooldownTime && 
               usesRemaining > 0;
    }

    protected bool PlayerPressedPower()
    {
        return Input.GetKey(KeyCode.LeftShift) &&
               Input.GetKeyDown(KeyCode.Space);
    }

    protected void TryActivate()
    {
        if (usesRemaining <= 0) return;

        ActivatePower();
        currentCooldown = 0f;
        usesRemaining--;

        // Solo actualizar UI para el jugador
        if (usePlayerInput && powerCircle != null)
        {
            powerCircle.fillAmount = 0f;
            
            if (usesRemaining <= 0)
            {
                // Sin usos restantes - animación de agotado
                StartCoroutine(EmptyAnimation());
            }
            else
            {
                powerCircle.color = chargingColor;
            }
        }
    }

    IEnumerator EmptyAnimation()
    {
        // Flash rojo y encogimiento
        for (int i = 0; i < 4; i++)
        {
            powerCircle.color = Color.red;
            powerCircle.transform.localScale = Vector3.one * 0.8f;
            yield return new WaitForSeconds(0.15f);
            
            powerCircle.color = emptyColor;
            powerCircle.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.15f);
        }
        
        powerCircle.color = emptyColor;
        powerCircle.transform.localScale = Vector3.one * 0.7f; // Queda pequeño
    }

    // ================= ABSTRACT =================
    // Cada poder define QUÉ HACE
    protected abstract void ActivatePower();

    // ================= IA SUPPORT =================
    public void ActivateFromAI()
    {
        if (CanActivate())
        {
            TryActivate();
        }
    }

    // ================= FLASH EFFECT (para heredar) =================
    protected IEnumerator FlashEffect(Vector3 position, Color flashColor)
    {
        // Crear destello central
        GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flash.name = "PowerFlash";
        Destroy(flash.GetComponent<Collider>());
        flash.transform.position = position + Vector3.up * 0.5f;
        flash.transform.localScale = Vector3.one * 0.1f;
        
        Renderer flashRenderer = flash.GetComponent<Renderer>();
        Material flashMat = new Material(Shader.Find("Sprites/Default"));
        flashMat.color = flashColor;
        flashRenderer.material = flashMat;
        
        // Crear rayos de luz
        GameObject[] rays = new GameObject[8];
        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rays[i].name = "FlashRay";
            Destroy(rays[i].GetComponent<Collider>());
            
            float angle = i * (360f / rays.Length);
            rays[i].transform.position = position + Vector3.up * 0.5f;
            rays[i].transform.rotation = Quaternion.Euler(0, angle, 90f);
            rays[i].transform.localScale = new Vector3(0.05f, 0.1f, 0.05f);
            
            Renderer rRenderer = rays[i].GetComponent<Renderer>();
            Material rMat = new Material(Shader.Find("Sprites/Default"));
            rMat.color = Color.white;
            rRenderer.material = rMat;
        }
        
        float elapsed = 0f;
        float duration = 0.6f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Expandir destello
            float scale = Mathf.Lerp(0.1f, 5f, t * 0.5f);
            flash.transform.localScale = Vector3.one * scale;
            
            // Desvanecer destello
            Color c = flashColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            flashMat.color = c;
            
            // Expandir rayos
            for (int i = 0; i < rays.Length; i++)
            {
                if (rays[i] != null)
                {
                    float rayLength = Mathf.Lerp(0.1f, 3f, t);
                    rays[i].transform.localScale = new Vector3(0.08f - t * 0.06f, rayLength, 0.08f - t * 0.06f);
                    
                    float angle = i * (360f / rays.Length);
                    Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                    rays[i].transform.position = position + Vector3.up * 0.5f + dir * rayLength * 0.5f;
                    
                    Renderer rRenderer = rays[i].GetComponent<Renderer>();
                    Color rColor = Color.Lerp(Color.white, flashColor, t);
                    rColor.a = Mathf.Lerp(1f, 0f, t);
                    rRenderer.material.color = rColor;
                }
            }
            
            yield return null;
        }
        
        Destroy(flash);
        foreach (var r in rays)
        {
            if (r != null) Destroy(r);
        }
    }
}
