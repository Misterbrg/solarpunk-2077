using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject pausePanel;

    [Header("Scripts du drone à couper pendant la pause")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private bool isPaused;

    void OnEnable()
    {
        if (pauseAction == null)
        {
            Debug.LogError("PauseMenu : l'action Pause n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Sans Enable(), l'action ne détecte aucun appui.
        pauseAction.action.Enable();
    }

    void OnDisable()
    {
        if (pauseAction == null) return;

        pauseAction.action.Disable();
    }

    void Start()
    {
        if (pausePanel == null)
        {
            Debug.LogError("PauseMenu : le panneau de pause n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Le panneau est masqué au démarrage, même s'il est actif dans l'éditeur.
        pausePanel.SetActive(false);
    }

    void Update()
    {
        // Cet Update continue de tourner pendant la pause (timeScale = 0 ne l'arrête pas) :
        // c'est ce qui permet de reprendre avec la même touche.
        if (pauseAction.action.WasPressedThisFrame())
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        isPaused = true;

        // Gèle tout ce qui dépend du temps : timer, croissance des plantes, mouvement du drone.
        Time.timeScale = 0f;

        pausePanel.SetActive(true);

        // Les actions (planter, arroser, récolter) ne dépendent pas du temps : sans ça, le joueur pourrait les utiliser en pause.
        SetScriptsEnabled(false);
    }

    // Public : à brancher aussi sur le bouton « Reprendre » du panneau.
    public void Resume()
    {
        isPaused = false;

        Time.timeScale = 1f;

        pausePanel.SetActive(false);

        SetScriptsEnabled(true);
    }

    private void SetScriptsEnabled(bool value)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            // Une case vide dans le tableau ne doit pas faire planter la pause.
            if (script == null) continue;

            script.enabled = value;
        }
    }
}
