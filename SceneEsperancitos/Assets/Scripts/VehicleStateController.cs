using UnityEngine;

public class VehicleStateController : MonoBehaviour
{
    [Header("Estado del vehículo")]
    public VehicleRole role;
    public Facultad facultad;

    // Referencias
    private PrometeoCarController carController;
    private EnemyAI enemyAI;

    void Awake()
    {
        carController = GetComponent<PrometeoCarController>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void ConfigureAsPlayer(Facultad selectedFacultad)
    {
        role = VehicleRole.Player;
        facultad = selectedFacultad;

        // Control
        carController.isAI = false;
        if (enemyAI != null) enemyAI.enabled = false;

        ActivatePower(facultad, true);
    }

    public void ConfigureAsEnemy(Facultad enemyFacultad)
    {
        role = VehicleRole.Enemy;
        facultad = enemyFacultad;

        // IA
        carController.isAI = true;
        if (enemyAI != null) enemyAI.enabled = true;

        ActivatePower(facultad, false);
    }

    void ActivatePower(Facultad fac, bool isPlayer)
    {
        DisableAllPowers();

        PowerBase power = null;

        switch (fac)
        {
            case Facultad.Software:
                power = GetComponent<SoftwareHackPower>();
                break;

            case Facultad.Mecatronica:
                power = GetComponent<TornillosPower>();
                break;

            case Facultad.Iasa:
                power = GetComponent<RockPower>();
                break;

            case Facultad.Civil:
                power = GetComponent<CowPower>();
                break;
        }

        if (power != null)
        {
            power.enabled = true;
            power.usePlayerInput = isPlayer;
        }
    }

    void DisableAllPowers()
    {
        foreach (PowerBase p in GetComponents<PowerBase>())
            p.enabled = false;
    }
}
