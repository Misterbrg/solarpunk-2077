using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private InputActionReference moveAction;

    [Header("Rotation")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float turnSpeed = 90f;

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

        if (lookAction == null)
        {
            Debug.LogError("DroneController : l'action Look n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Un masque vide ne bloque jamais le drone : on prévient sans désactiver le script.
        if (limitMask.value == 0)
        {
            Debug.LogWarning("DroneController : aucun layer n'est coché dans Limit Mask, le drone ne sera jamais bloqué.", this);
        }

        // Sans Enable(), les actions ne renvoient jamais de valeur.
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    void OnDisable()
    {
        // Chaque action est testée séparément : l'une peut être assignée sans l'autre.
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }

        if (lookAction != null)
        {
            lookAction.action.Disable();
        }
    }

    void Update()
    {
        // Rotation avant le déplacement : le drone avance dans la direction où il vient de se tourner.
        // Space.World : rotation autour de l'axe Y du monde, même si le modèle est incliné.
        float turn = lookAction.action.ReadValue<float>();
        transform.Rotate(0f, turn * turnSpeed * Time.deltaTime, 0f, Space.World);

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Déplacement relatif au drone : l'input avant/arrière suit son orientation, pas les axes du monde.
        Vector3 direction = transform.right * input.x + transform.forward * input.y;

        // Déplacement de cette image, indépendant du nombre d'images par seconde.
        Vector3 move = direction * moveSpeed * Time.deltaTime;

        // Axes du monde testés séparément : contre un mur, le drone glisse le long de celui-ci
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
        // Pas de mouvement sur cet axe : rien à tester.
        if (step == Vector3.zero) return false;

        return Physics.SphereCast(transform.position, collisionRadius,
                                  step.normalized, out _, step.magnitude, limitMask);
    }
}