using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RaceStartManager : MonoBehaviour
{
    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip countdownClip;

    [Header("GO UI")]
    public Text goText;
    public float goDuration = 1f;

    [Header("Semaforo UI")]
    public Image background;
    public Image redLight;
    public Image yellowLight;
    public Image greenLight;

    [Header("Configuracion")]
    public float lightDuration = 1f;

    public static bool raceStarted = false;

    private PrometeoCarController[] allCars;

    void Start()
    {
        raceStarted = false;

        // Encontrar todos los carros en la escena
        allCars = FindObjectsOfType<PrometeoCarController>();

        // Bloquear movimiento de todos
        foreach (var car in allCars)
        {
            car.canMove = false;
        }

        // Asegurarnos que el GO esté oculto al inicio
        if (goText != null)
            goText.gameObject.SetActive(false);

        StartCoroutine(StartRaceSequence());
    }

    IEnumerator StartRaceSequence()
    {
        background.enabled = true;

        redLight.enabled = false;
        yellowLight.enabled = false;
        greenLight.enabled = false;

        // 🔊 Reproducir countdown inmediatamente
        if (audioSource != null && countdownClip != null)
        {
            audioSource.clip = countdownClip;
            audioSource.Play();
        }

        // Sincronización inicial
        yield return new WaitForSeconds(1f);

        // 🔴
        redLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);

        // 🟡
        yellowLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);

        // 🟢
        greenLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);

        // Activar movimiento justo cuando termina el verde
        raceStarted = true;

        foreach (var car in allCars)
        {
            car.canMove = true;
        }

        // 🔻 APAGAR TODO EL SEMÁFORO
        background.enabled = false;
        redLight.enabled = false;
        yellowLight.enabled = false;
        greenLight.enabled = false;

        // ⏳ Esperar 1 segundo después de apagarse
        yield return new WaitForSeconds(0.75f);

        // 💥 Mostrar GO!
        if (goText != null)
        {
            goText.gameObject.SetActive(true);
            goText.text = "GO!";
            goText.transform.localScale = Vector3.one;
        }

        yield return new WaitForSeconds(goDuration);

        if (goText != null)
            goText.gameObject.SetActive(false);
    }


}
