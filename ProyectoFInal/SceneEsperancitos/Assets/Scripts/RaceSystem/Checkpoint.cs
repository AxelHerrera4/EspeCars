using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;

    private void OnTriggerEnter(Collider other)
    {
        CarProgress car = other.GetComponentInParent<CarProgress>();
        if (car != null)
        {
            car.ReachedCheckpoint(checkpointIndex);
        }
    }
}
