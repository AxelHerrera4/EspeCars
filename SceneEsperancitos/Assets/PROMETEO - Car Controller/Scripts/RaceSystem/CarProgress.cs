using UnityEngine;

public class CarProgress : MonoBehaviour
{
    public int currentCheckpoint = 0;
    public int currentLap = 1;
    public int totalLaps = 2;
    public bool finished = false;

    public float distanceToNextCheckpoint;
    public Transform[] checkpoints;

    void Update()
    {
        if (checkpoints.Length > 0)
        {
            Transform next = checkpoints[currentCheckpoint];
            distanceToNextCheckpoint =
                Vector3.Distance(transform.position, next.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        

        Checkpoint cp = other.GetComponent<Checkpoint>();
        if (cp == null) return;

        if (cp.checkpointIndex == currentCheckpoint)
        {
            currentCheckpoint++;

            if (currentCheckpoint >= checkpoints.Length)
            {
                currentCheckpoint = 0;
                currentLap++;

                Debug.Log("Nueva vuelta: " + currentLap);

                if (currentLap > totalLaps)
                {
                    finished = true;
                    Debug.Log("Carrera terminada");
                }
            }
        }
    }
}
