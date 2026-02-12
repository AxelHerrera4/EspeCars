using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RaceFinishManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject resultsPanel;

    [Header("Result Title")]
    public Text resultTitle;

    [Header("Gameplay UI To Hide")]
    public GameObject miniMapUI;
    public GameObject speedText;
    public GameObject controls;
    public GameObject powerCircle;
    public GameObject config;
    public GameObject positionText;

    [Header("Podium Names & Times")]
    public Text firstName;
    public Text firstTime;
    public Image firstIcon;

    public Text secondName;
    public Text secondTime;
    public Image secondIcon;

    public Text thirdName;
    public Text thirdTime;
    public Image thirdIcon;

    [Header("Faculty Sprites")]
    public Sprite iasaSprite;
    public Sprite mecatronicaSprite;
    public Sprite civilSprite;
    public Sprite softwareSprite;

    private bool resultsShown = false;

    void Update()
    {
        if (resultsShown) return;

        CarProgress[] cars = FindObjectsOfType<CarProgress>();

        foreach (var car in cars)
        {
            PrometeoCarController controller = car.GetComponent<PrometeoCarController>();

            if (controller != null && !controller.isAI && car.finished)
            {
                ShowResults(cars);
                break;
            }
        }
    }

    void ShowResults(CarProgress[] cars)
    {
        resultsShown = true;

        // Bloquear todos los carros
        foreach (var car in cars)
        {
            PrometeoCarController controller = car.GetComponent<PrometeoCarController>();
            if (controller != null)
                controller.canMove = false;

            // Simular tiempo a los que no terminaron
            if (!car.finished)
            {
                car.finishTime = Random.Range(45f, 60f);
                car.finished = true;
            }
        }

        var ordered = cars
            .OrderBy(c => c.finishTime)
            .ToList();

        // ================= DETECTAR POSICIÓN DEL JUGADOR =================

        int playerPosition = -1;

        for (int i = 0; i < ordered.Count; i++)
        {
            PrometeoCarController controller = ordered[i].GetComponent<PrometeoCarController>();

            if (controller != null && !controller.isAI)
            {
                playerPosition = i + 1;
                break;
            }
        }

        // ================= MENSAJE PRINCIPAL =================

        if (playerPosition == 1)
        {
            resultTitle.text = "🏆 ¡GANASTE LA CARRERA!";
            resultTitle.color = Color.yellow;
        }
        else if (playerPosition == 2)
        {
            resultTitle.text = "🥈 ¡Excelente segundo lugar!";
            resultTitle.color = Color.white;
        }
        else if (playerPosition == 3)
        {
            resultTitle.text = "🥉 Tercer lugar – ¡Buen trabajo!";
            resultTitle.color = new Color(0.8f, 0.5f, 0.2f);
        }
        else
        {
            resultTitle.text = "💪 ¡Sigue intentando!";
            resultTitle.color = Color.white;
        }

        // ================= PODIO =================

        if (ordered.Count > 0)
        {
            firstName.text = ordered[0].facultad.ToString();
            firstTime.text = ordered[0].finishTime.ToString("F2") + "s";
            firstIcon.sprite = GetFacultySprite(ordered[0].facultad);
        }

        if (ordered.Count > 1)
        {
            secondName.text = ordered[1].facultad.ToString();
            secondTime.text = ordered[1].finishTime.ToString("F2") + "s";
            secondIcon.sprite = GetFacultySprite(ordered[1].facultad);
        }

        if (ordered.Count > 2)
        {
            thirdName.text = ordered[2].facultad.ToString();
            thirdTime.text = ordered[2].finishTime.ToString("F2") + "s";
            thirdIcon.sprite = GetFacultySprite(ordered[2].facultad);
        }

        // ================= OCULTAR UI GAMEPLAY =================

        if (miniMapUI) miniMapUI.SetActive(false);
        if (speedText) speedText.SetActive(false);
        if (controls) controls.SetActive(false);
        if (powerCircle) powerCircle.SetActive(false);
        if (config) config.SetActive(false);
        if (positionText) positionText.SetActive(false);

        // ================= MOSTRAR PANEL =================

        resultsPanel.SetActive(true);
    }

    Sprite GetFacultySprite(CarProgress.Facultad fac)
    {
        switch (fac)
        {
            case CarProgress.Facultad.IASA: return iasaSprite;
            case CarProgress.Facultad.MECATRONICA: return mecatronicaSprite;
            case CarProgress.Facultad.CIVIL: return civilSprite;
            case CarProgress.Facultad.SOFTWARE: return softwareSprite;
        }

        return null;
    }
}
