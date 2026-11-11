using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    public enum ScoreType { Last, Best }

    [Header("Références")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Réglages")]
    [SerializeField] private ScoreType scoreType = ScoreType.Best;
    [SerializeField] private string prefix = "Meilleur score : ";

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("ScoreDisplay : le texte n'est pas assigné dans l'Inspector.", this);
            enabled = false;
            return;
        }

        // Choix de la clé selon le type : le même script sert pour le menu et pour la scène End.
        string key = scoreType == ScoreType.Last ? SaveKeys.LastScore : SaveKeys.BestScore;

        // 0 par défaut : aucune partie jouée = aucun score enregistré.
        int score = PlayerPrefs.GetInt(key, 0);

        scoreText.text = prefix + score;
    }
}
