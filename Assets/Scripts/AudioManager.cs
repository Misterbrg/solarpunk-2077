using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sons")]
    [SerializeField] private AudioClip plantClip;
    [SerializeField] private AudioClip waterClip;
    [SerializeField] private AudioClip harvestClip;

    void Start()
    {
        if (sfxSource == null)
        {
            Debug.LogError("AudioManager : l'AudioSource des effets sonores n'est pas assignée dans l'Inspector.", this);
            enabled = false;
            return;
        }
    }

    public void PlayPlant()
    {
        Play(plantClip);
    }

    public void PlayWater()
    {
        Play(waterClip);
    }

    public void PlayHarvest()
    {
        Play(harvestClip);
    }

    private void Play(AudioClip clip)
    {
        // Un clip non assigné est ignoré sans erreur : le jeu reste jouable sans tous les sons.
        if (clip == null) return;

        // enabled = false dans Start n'empêche pas d'appeler ces méthodes depuis un autre script :
        // ce test évite un NullReferenceException si la source manque.
        if (sfxSource == null) return;

        // PlayOneShot : plusieurs sons peuvent se chevaucher sur la même source sans se couper.
        sfxSource.PlayOneShot(clip);
    }
}