using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float gameDuration = 180f;
    [SerializeField] private string endSceneName = "End";

    // Évite de terminer la partie plusieurs fois (LoadScene n'est effectif qu'à la fin de l'image).
    private bool gameEnded;
    private float timeRemaining;

    void Start()
    {
        if (scoreManager == null)
        {
            Debug.LogError("GameManager : le ScoreManager n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        if (timerText == null)
        {
            Debug.LogError("GameManager : le texte du timer n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        timeRemaining = gameDuration;
    }

    void Update()
    {
        if (gameEnded) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            // On fixe à 0 pour ne jamais afficher de temps négatif.
            timeRemaining = 0f;
            UpdateTimerUI();
            EndGame();
        }
        else
        {
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        // On arrondit à la seconde supérieure UNE SEULE FOIS sur le total, puis on découpe.
        // Avec CeilToInt(timeRemaining % 60f), 119,5 s donnerait 59,5 -> 60 et afficherait « 01:60 ».
        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void EndGame()
    {
        // Avant tout le reste : bloque tout second appel.
        gameEnded = true;

        int score = scoreManager.Score;

        // LastScore : lu par la scène End pour afficher le résultat de cette partie.
        PlayerPrefs.SetInt(SaveKeys.LastScore, score);

        if (score > PlayerPrefs.GetInt(SaveKeys.BestScore, 0))
        {
            PlayerPrefs.SetInt(SaveKeys.BestScore, score);
        }

        // Écrit sur le disque tout de suite, au cas où le jeu serait fermé sur la scène End.
        PlayerPrefs.Save();

        // Remet le temps à la normale, au cas où il aurait été mis en pause (timeScale = 0).
        Time.timeScale = 1f;

        SceneManager.LoadScene(endSceneName);
    }
}