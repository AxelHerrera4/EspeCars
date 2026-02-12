using UnityEngine;
using TMPro;
using System.Linq;

public class RacePositionManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;

    private CarProgress[] cars;
    private CarProgress playerCar;

    void Update()
    {
        // Buscar carros si aún no los encontró
        if (cars == null || cars.Length == 0)
        {
            cars = FindObjectsOfType<CarProgress>();
        }

        // Buscar jugador si aún no lo encontró
        if (playerCar == null && cars != null)
        {
            foreach (var car in cars)
            {
                PrometeoCarController controller = car.GetComponent<PrometeoCarController>();
                if (controller != null && !controller.isAI)
                {
                    playerCar = car;
                    break;
                }
            }
        }

        if (cars == null || playerCar == null) return;

        var ordered = cars
            .OrderByDescending(c => c.currentLap)
            .ThenByDescending(c => c.currentCheckpoint)
            .ThenBy(c => c.distanceToNextCheckpoint)
            .ToList();

        int playerPosition = ordered.IndexOf(playerCar) + 1;

        positionText.text = playerPosition + "/" + cars.Length;
    }
}
