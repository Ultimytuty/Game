using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] musicClips; // Assign your music clips in the inspector
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playMusic();
    }
    void playMusic()
    {
        if (musicClips != null && musicClips.Length > 0)
        {
            audioSource.clip = musicClips[Random.Range(0, musicClips.Length)];
            audioSource.Play();
            playMusike();
        }
    }

    void playMusike()
    {
        if (!audioSource.isPlaying)
        {
            playMusic();
        }
    }
}
