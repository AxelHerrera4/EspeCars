using UnityEngine;

public class CarProgress : MonoBehaviour
{
    public int currentCheckpoint = 0;
    public int currentLap = 0;

    public Transform[] checkpoints;
    public int totalLaps = 1;

    public float distanceToNextCheckpoint;

    void Update()
    {
        if (checkpoints.Length == 0) return;

        // Seguridad extra
        if (currentCheckpoint >= checkpoints.Length)
            currentCheckpoint = 0;

        Transform next = checkpoints[currentCheckpoint];
        distanceToNextCheckpoint = Vector3.Distance(transform.position, next.position);
    }

    public void ReachedCheckpoint(int checkpointIndex)
    {
        // Solo avanzar si pasa el checkpoint correcto
        if (checkpointIndex == currentCheckpoint)
        {
            currentCheckpoint++;

            if (currentCheckpoint >= checkpoints.Length)
            {
                currentCheckpoint = 0;
                currentLap++;

                if (currentLap > totalLaps)
                {
                    currentLap = totalLaps;
                }
            }
        }
    }
}
