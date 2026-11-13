using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public enum PlantState { Seed, Ripe, Overripe }

    [Header("Durées de chaque étape (secondes)")]
    [SerializeField] private float seedDuration = 10f;
    [SerializeField] private float ripeDuration = 10f;
    [SerializeField] private float overripeDuration = 5f;

    [Header("Arrosage")]
    [SerializeField] private float wateredDuration = 0.2f;

    [Header("Visuels de chaque étape")]
    [SerializeField] private GameObject seedVisual;
    [SerializeField] private GameObject ripeVisual;
    [SerializeField] private GameObject overripeVisual;
    [SerializeField] private AudioClip burstClip;

    private PlantState currentState;
    private float stateTimer;
    private float growthSpeedMultiplier = 1f;
    private float wateredTimeLeft;

    // Lecture seule depuis l'extérieur : utilisé par DroneHarvester
    public PlantState CurrentState => currentState;

    void Start()
    {
        if (seedVisual == null)
        {
            Debug.LogError("PlantGrowth : le visuel Seed n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        if (ripeVisual == null)
        {
            Debug.LogError("PlantGrowth : le visuel Ripe n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        if (overripeVisual == null)
        {
            Debug.LogError("PlantGrowth : le visuel Overripe n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        SetState(PlantState.Seed);
    }

    void Update()
    {
        // Quand le délai expire (le drone n'arrose plus), retour à la vitesse normale.
        if (wateredTimeLeft > 0f)
        {
            wateredTimeLeft -= Time.deltaTime;

            if (wateredTimeLeft <= 0f)
            {
                growthSpeedMultiplier = 1f;
            }
        }

        stateTimer += Time.deltaTime * growthSpeedMultiplier;

        switch (currentState)
        {
            case PlantState.Seed:
                if (stateTimer >= seedDuration) SetState(PlantState.Ripe);
                break;

            case PlantState.Ripe:
                if (stateTimer >= ripeDuration) SetState(PlantState.Overripe);
                break;

            case PlantState.Overripe:
                if (stateTimer >= overripeDuration) Burst();
                break;
        }
    }

    // Appelée par le drone à chaque image d'arrosage : accélère la pousse et relance le délai.
    public void Water(float bonus)
    {
        growthSpeedMultiplier = bonus;
        wateredTimeLeft = wateredDuration;
    }

    // Appelée par le drone quand le joueur récolte la plante.
    public void Harvest()
    {
        Destroy(gameObject);
    }

    private void SetState(PlantState newState)
    {
        currentState = newState;

        // Remise à zéro obligatoire : sinon la plante enchaînerait toutes les étapes en une seule image.
        stateTimer = 0f;

        seedVisual.SetActive(newState == PlantState.Seed);
        ripeVisual.SetActive(newState == PlantState.Ripe);
        overripeVisual.SetActive(newState == PlantState.Overripe);
    }

    private void Burst()
    {
        if (burstClip != null) AudioSource.PlayClipAtPoint(burstClip, transform.position);
        Destroy(gameObject);

    }
}