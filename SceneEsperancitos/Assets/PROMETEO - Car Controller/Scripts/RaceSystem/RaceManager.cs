using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public List<CarProgress> cars;
    public int totalCheckpoints;

    void Update()
    {
        cars = cars
            .OrderByDescending(c => c.currentLap)
            .ThenByDescending(c => c.currentCheckpoint)
            .ThenBy(c => c.distanceToNextCheckpoint)
            .ToList();
    }

    public int GetPosition(CarProgress car)
    {
        return cars.IndexOf(car) + 1;
    }
}
