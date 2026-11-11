using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtons : MonoBehaviour
{
    // À brancher sur l'événement OnClick d'un bouton : on tape le nom de la scène dans le champ du paramètre.
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("SceneButtons : le nom de la scène est vide.", this);
            return;
        }

        // Au cas où le temps aurait été figé (pause) dans la scène précédente.
        Time.timeScale = 1f;

        // Le nom doit correspondre exactement à une scène listée dans les Build Settings.
        SceneManager.LoadScene(sceneName);
    }

    // À brancher sur un bouton « Quitter ».
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Dans l'éditeur, Application.Quit() ne fait rien : on arrête le Play Mode à la place.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
