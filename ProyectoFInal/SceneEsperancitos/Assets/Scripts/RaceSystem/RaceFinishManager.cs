using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class RaceFinishManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject resultsPanel;
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

        // BLOQUEAR MOVIMIENTO
        foreach (var car in cars)
        {
            PrometeoCarController controller = car.GetComponent<PrometeoCarController>();
            if (controller != null)
                controller.canMove = false;
        }

        // ================= CALCULAR POSICION REAL (como RacePositionManager) =================

        var orderedByRace = cars
            .OrderByDescending(c => c.currentCheckpoint)
            .ThenBy(c => c.distanceToNextCheckpoint)
            .ToList();

        CarProgress playerCar = orderedByRace
            .First(c => c.GetComponent<PrometeoCarController>()?.isAI == false);

        int playerPosition = orderedByRace.IndexOf(playerCar) + 1;

        // ================= MENSAJE =================

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
        else
        {
            resultTitle.text = "💪 ¡Sigue intentando!";
            resultTitle.color = Color.white;
        }

        // ================= CREAR PODIO BETA PRO PLAYER =================

        List<CarProgress> aiCars = orderedByRace
            .Where(c => c != playerCar)
            .ToList();

        Shuffle(aiCars);

        CarProgress first = null;
        CarProgress second = null;
        CarProgress third = null;

        if (playerPosition == 1)
        {
            first = playerCar;
            second = aiCars[0];
            third = aiCars[1];
        }
        else if (playerPosition == 2)
        {
            first = aiCars[0];
            second = playerCar;
            third = aiCars[1];
        }
        else
        {
            first = aiCars[0];
            second = aiCars[1];
            third = playerCar;
        }

        SetupPodium(first, second, third);

        // ================= OCULTAR UI =================

        if (miniMapUI) miniMapUI.SetActive(false);
        if (speedText) speedText.SetActive(false);
        if (controls) controls.SetActive(false);
        if (powerCircle) powerCircle.SetActive(false);
        if (config) config.SetActive(false);
        if (positionText) positionText.SetActive(false);

        resultsPanel.SetActive(true);
    }

    void SetupPodium(CarProgress first, CarProgress second, CarProgress third)
    {
        // Si alguna IA no terminó, le damos tiempo basado en el jugador
        CarProgress player = FindObjectsOfType<CarProgress>()
            .First(c => c.GetComponent<PrometeoCarController>()?.isAI == false);

        float playerTime = player.finishTime;

        if (!first.finished)
            first.finishTime = playerTime + Random.Range(-2f, 2f);

        if (!second.finished)
            second.finishTime = playerTime + Random.Range(2f, 6f);

        if (!third.finished)
            third.finishTime = playerTime + Random.Range(6f, 10f);

        // Mostrar tiempos reales
        firstName.text = first.facultad.ToString();
        firstTime.text = first.finishTime.ToString("F2") + "s";
        firstIcon.sprite = GetFacultySprite(first.facultad);

        secondName.text = second.facultad.ToString();
        secondTime.text = second.finishTime.ToString("F2") + "s";
        secondIcon.sprite = GetFacultySprite(second.facultad);

        thirdName.text = third.facultad.ToString();
        thirdTime.text = third.finishTime.ToString("F2") + "s";
        thirdIcon.sprite = GetFacultySprite(third.facultad);
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

    void Shuffle(List<CarProgress> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CarProgress temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
