using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;

    [Header("Limites")]
    [SerializeField] private LayerMask limitMask;
    [SerializeField] private float collisionRadius = 2.5f;

    void OnEnable()
    {
        if (moveAction == null)
        {
            Debug.LogError("DroneController : l'action Move n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Un masque vide ne bloque jamais le drone : on prévient sans désactiver le script.
        if (limitMask.value == 0)
        {
            Debug.LogWarning("DroneController : aucun layer n'est coché dans Limit Mask, le drone ne sera jamais bloqué.", this);
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

        // Déplacement de cette image, indépendant du nombre d'images par seconde.
        Vector3 move = direction * moveSpeed * Time.deltaTime;

        // Axes testés séparément : contre un mur, le drone glisse le long de celui-ci
        // au lieu de s'arrêter net en diagonale.
        Vector3 moveX = new Vector3(move.x, 0f, 0f);
        Vector3 moveZ = new Vector3(0f, 0f, move.z);

        if (!IsBlocked(moveX))
        {
            transform.Translate(moveX, Space.World);
        }

        if (!IsBlocked(moveZ))
        {
            transform.Translate(moveZ, Space.World);
        }
    }

    // Lance une sphère de la taille du drone dans le sens du pas : vrai si une limite se trouve sur le trajet.
    private bool IsBlocked(Vector3 step)
    {
        // Pas de mouvement sur cet axe : rien à tester (et normalized d'un vecteur nul ne servirait à rien).
        if (step == Vector3.zero) return false;

        return Physics.SphereCast(transform.position, collisionRadius,
                                  step.normalized, out _, step.magnitude, limitMask);
    }
}