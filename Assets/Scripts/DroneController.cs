using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;

    void OnEnable()
    {
        if (moveAction == null)
        {
            Debug.LogError("DroneController : l'action Move n'est pas assignée dans l'Inspector.", this);
            // Désactive le script : Update ne tournera pas tant que l'action est manquante.
            enabled = false;
            return;
        }

        // Sans Enable(), l'action ne renvoie jamais de valeur.
        moveAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction == null) return;

        // On désactive l'action pour éviter qu'elle reste active quand le script est coupé.
        moveAction.action.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Y à 0 : la hauteur est fixée par la position du drone, pas par l'input.
        // Le Y du clavier/stick (avant/arrière) devient l'axe Z du monde.
        Vector3 direction = new Vector3(input.x, 0f, input.y);

        // Space.World : le drone se déplace selon les axes du monde, même s'il est tourné.
        // Time.deltaTime : vitesse indépendante du nombre d'images par seconde.
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }
}