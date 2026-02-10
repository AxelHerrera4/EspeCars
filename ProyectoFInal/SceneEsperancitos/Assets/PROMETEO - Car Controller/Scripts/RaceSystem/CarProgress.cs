using UnityEngine;

public class CarProgress : MonoBehaviour
{
    public int currentCheckpoint = 0;
    public int currentLap = 0;

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
        Checkpoint cp = other.GetComponent<Checkpoint>();

        if (cp != null && cp.checkpointIndex == currentCheckpoint)
        {
            currentCheckpoint++;

            if (currentCheckpoint >= checkpoints.Length)
            {
                currentCheckpoint = 0;
                currentLap++;
            }
        }
    }
}
