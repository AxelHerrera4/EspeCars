using UnityEngine;

public class CarProgress : MonoBehaviour
{
    // ================= FACULTAD =================
    public enum Facultad
    {
        IASA,
        MECATRONICA,
        CIVIL,
        SOFTWARE
    }

    public Facultad facultad;

    // ================= CHECKPOINTS =================
    public int currentCheckpoint = 0;
    public Transform[] checkpoints;

    public float distanceToNextCheckpoint;

    public bool finished = false;
    public float finishTime = 0f;

    private float raceStartTime;

    void Start()
    {
        raceStartTime = Time.time;
    }

    void Update()
    {
        if (checkpoints.Length == 0 || finished) return;

        Transform next = checkpoints[Mathf.Clamp(currentCheckpoint, 0, checkpoints.Length - 1)];
        distanceToNextCheckpoint = Vector3.Distance(transform.position, next.position);
    }

    public void ReachedCheckpoint(int checkpointIndex)
    {
        if (finished) return;

        currentCheckpoint = checkpointIndex;

        // 🎯 SI ES EL ÚLTIMO CHECKPOINT (38) → TERMINAR
        if (checkpointIndex == 38)

        
        {
            Debug.Log("Checkpoint 38 detectado!");
            finished = true;
            finishTime = Time.time - raceStartTime;

            Debug.Log(gameObject.name + " TERMINÓ EN: " + finishTime);
        }
    }
}
