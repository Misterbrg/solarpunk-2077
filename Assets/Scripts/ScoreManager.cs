using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private int score;

    // Lecture seule depuis l'extérieur : seul AddScore peut modifier le score.
    public int Score => score;

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("ScoreManager : le texte du score n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Sécurité : AddScore peut être appelé même si Start a désactivé le script.
        if (scoreText == null) return;

        scoreText.text = "Score : " + score;
    }
}