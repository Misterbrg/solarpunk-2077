using UnityEngine;
using UnityEngine.InputSystem;

public class DronePlanter : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private InputActionReference plantAction;
    [SerializeField] private Transform plantsContainer;
    [SerializeField] private AudioManager audioManager;


    [Header("Réglages")]
    [SerializeField] private float groundHeight = 0f;

    void OnEnable()
    {
        // Vérifie chaque référence séparément pour que le message dise précisément ce qui manque.
        if (seedPrefab == null)
        {
            Debug.LogError("DronePlanter : le prefab de graine n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        if (plantAction == null)
        {
            Debug.LogError("DronePlanter : l'action Plant n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }

        if (plantsContainer == null)
        {
            Debug.LogError("DronePlanter : le conteneur Plants n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Sans Enable(), l'action ne détecte aucun appui.
        plantAction.action.Enable();
    }

    void OnDisable()
    {
        if (plantAction == null) return;

        // On évite de laisser l'action active quand le script est coupé.
        plantAction.action.Disable();
    }

    void Update()
    {
        // WasPressedThisFrame : un seul appui = une seule graine (pas une par image tant que le bouton est tenu).
        if (plantAction.action.WasPressedThisFrame())
        {
            PlantSeed();
        }
    }

    private void PlantSeed()
    {
        // Y = groundHeight : la graine est posée au sol, pas à la hauteur du drone en vol.
        Vector3 position = new Vector3(transform.position.x, groundHeight, transform.position.z);

        // Quaternion.identity : aucune rotation. Le conteneur comme parent garde la hiérarchie propre.
        Instantiate(seedPrefab, position, Quaternion.identity, plantsContainer);

        if (audioManager != null) audioManager.PlayPlant();
    }
}