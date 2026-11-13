using UnityEngine;
using UnityEngine.InputSystem;

public class DroneWatering : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private InputActionReference waterAction;
    [SerializeField] private ParticleSystem waterJet;

    [Header("Réglages")]
    [SerializeField] private float wateringRadius = 5f;
    [SerializeField] private float growthBonus = 2f;
    [SerializeField] private float groundHeight = 0f;
    [SerializeField] private AudioManager audioManager;

    void OnEnable()
    {
        if (waterAction == null)
        {
            Debug.LogError("DroneWatering : l'action Water n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }

        if (waterJet == null)
        {
            Debug.LogError("DroneWatering : le jet de particules n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Sans Enable(), l'action ne détecte aucun appui.
        waterAction.action.Enable();
    }

    void OnDisable()
    {
        if (waterAction != null)
        {
            waterAction.action.Disable();
        }

        // Si le script est coupé en plein arrosage, le jet ne doit pas continuer tout seul.
        if (waterJet != null && waterJet.isPlaying)
        {
            waterJet.Stop();
        }
    }

    void Update()
    {
        // IsPressed : vrai tant que le bouton est maintenu (contrairement à WasPressedThisFrame).
        if (waterAction.action.IsPressed())
        {
            // Le test isPlaying évite de relancer le jet à chaque image.
            if (!waterJet.isPlaying)
            {
                waterJet.Play();
                if (audioManager != null) audioManager.PlayWater();
            }

            WaterPlantsBelow();
        }
        else if (waterJet.isPlaying)
        {
            waterJet.Stop();
        }
    }

    private void WaterPlantsBelow()
    {
        // Centre au sol : on arrose ce qui est sous le drone, pas à sa hauteur de vol.
        Vector3 center = new Vector3(transform.position.x, groundHeight, transform.position.z);

        Collider[] hits = Physics.OverlapSphere(center, wateringRadius);

        foreach (Collider hit in hits)
        {
            // GetComponentInParent couvre aussi le cas où le collider est sur un enfant du prefab.
            PlantGrowth plant = hit.GetComponentInParent<PlantGrowth>();

            // Le sol et les autres objets touchés n'ont pas de PlantGrowth : on les ignore.
            if (plant == null) continue;

            plant.Water(growthBonus);
        }
    }
}