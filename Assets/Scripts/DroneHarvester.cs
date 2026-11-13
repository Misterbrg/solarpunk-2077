using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneHarvester : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private InputActionReference harvestAction;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private int pointsPerHarvest = 10;
           

    // Plantes dont le trigger chevauche actuellement le drone.
    private readonly List<PlantGrowth> plantsBelow = new List<PlantGrowth>();

    void OnEnable()
    {
        if (harvestAction == null)
        {
            Debug.LogError("DroneHarvester : l'action Harvest n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Sans Enable(), l'action ne détecte aucun appui.
        harvestAction.action.Enable();

        if (scoreManager == null)
        {
            Debug.LogError("DroneHarvester : le ScoreManager n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }
    }

    void OnDisable()
    {
        if (harvestAction != null)
        {
            harvestAction.action.Disable();
        }

        // Le script coupé ne reçoit plus les Exit : on repart d'une liste vide à la réactivation.
        plantsBelow.Clear();
    }

    void Update()
    {
        // WasPressedThisFrame : un appui = une tentative de récolte.
        if (harvestAction.action.WasPressedThisFrame())
        {
            TryHarvest();
        }
      
    }

    private void TryHarvest()
    {
        // Une plante détruite (pourrie) ne déclenche jamais OnTriggerExit : on purge les références mortes.
        // Unity surcharge == : une plante détruite est égale à null.
        plantsBelow.RemoveAll(plant => plant == null);

        foreach (PlantGrowth plant in plantsBelow)
        {
            if (plant.CurrentState != PlantGrowth.PlantState.Ripe) continue;

            plant.Harvest();
        scoreManager.AddScore(pointsPerHarvest);
        plantsBelow.Remove(plant);

            if (audioManager != null) audioManager.PlayHarvest();
            // On sort du foreach juste après Remove : modifier la liste puis continuer à la parcourir provoquerait une erreur.
            // Et une seule plante récoltée par appui.
            break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // GetComponentInParent couvre le cas où le collider est sur un enfant du prefab.
        PlantGrowth plant = other.GetComponentInParent<PlantGrowth>();

        // Le sol et les autres objets n'ont pas de PlantGrowth : on les ignore.
        if (plant == null) return;
        // Contains : une plante à plusieurs colliders ne doit pas être ajoutée en double.
        if (!plantsBelow.Contains(plant))
        {
            plantsBelow.Add(plant);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlantGrowth plant = other.GetComponentInParent<PlantGrowth>();
        if (plant == null) return;

        plantsBelow.Remove(plant);
    }
}