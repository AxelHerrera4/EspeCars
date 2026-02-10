using UnityEngine;
using TMPro;

public class UIRace : MonoBehaviour
{
    public TMP_Text positionText;
    public TMP_Text lapText;

    public CarProgress playerCar;
    public RaceManager raceManager;

    public int totalLaps = 3;

    void Update()
    {
        int pos = raceManager.GetPosition(playerCar);
        positionText.text = pos + "°";

        lapText.text = "Lap: " + (playerCar.currentLap + 1) + "/" + totalLaps;

        if (pos == 1)
        positionText.color = Color.yellow;
        else if (pos == 2)
            positionText.color = new Color(0.8f, 0.8f, 0.8f); // plateado
        else if (pos == 3)
            positionText.color = new Color(0.8f, 0.5f, 0.2f); // bronce
        else
            positionText.color = Color.white;
        }
}
