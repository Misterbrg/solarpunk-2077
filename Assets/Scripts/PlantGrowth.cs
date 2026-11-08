using UnityEngine;

public class PlantGrowth : MonoBehaviour
{
    public enum PlantState { Seed, Ripe, Overripe }

    [Header("Durées de chaque étape (secondes)")]
    [SerializeField] private float seedDuration = 10f;
    [SerializeField] private float ripeDuration = 10f;
    [SerializeField] private float overripeDuration = 5f;

    [Header("Visuels de chaque étape")]
    [SerializeField] private GameObject seedVisual;
    [SerializeField] private GameObject ripeVisual;
    [SerializeField] private GameObject overripeVisual;

    private PlantState currentState;
    private float stateTimer;
    private float growthSpeedMultiplier = 1f;

    // Lecture seule depuis l'extérieur : le futur script de récolte pourra tester CurrentState == Ripe.
    public PlantState CurrentState => currentState;

    void Start()
    {
        // Vérifie chaque visuel séparément pour que le message dise précisément ce qui manque.
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
        // Le multiplicateur permettra d'accélérer la pousse (arrosage) sans toucher aux durées.
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

    private void SetState(PlantState newState)
    {
        currentState = newState;

        // Remise à zéro obligatoire : sinon la plante enchaînerait toutes les étapes en une seule image.
        stateTimer = 0f;

        // Un seul visuel actif à la fois : celui qui correspond à l'état courant.
        seedVisual.SetActive(newState == PlantState.Seed);
        ripeVisual.SetActive(newState == PlantState.Ripe);
        overripeVisual.SetActive(newState == PlantState.Overripe);
    }

    private void Burst()
    {
        Destroy(gameObject);
    }
}