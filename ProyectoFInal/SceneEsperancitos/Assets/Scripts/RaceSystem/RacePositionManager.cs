using UnityEngine;
using UnityEngine.UI;   
using System.Linq;

public class RacePositionManager : MonoBehaviour
{
    public Text positionText;



    private CarProgress[] cars;
    private CarProgress playerCar;

    void Update()
    {
        if (cars == null || cars.Length == 0)
        {
            cars = FindObjectsOfType<CarProgress>();
        }

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
            .OrderByDescending(c => c.currentCheckpoint)
            .ThenBy(c => c.distanceToNextCheckpoint)
            .ToList();

        int playerPosition = ordered.IndexOf(playerCar) + 1;

        positionText.text = playerPosition + "/" + cars.Length;
    }
}
