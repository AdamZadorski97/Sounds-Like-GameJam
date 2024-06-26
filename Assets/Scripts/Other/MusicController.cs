using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance { get; private set; } // Singleton instance

    public AudioSource audioSource; // Reference to the AudioSource component
    public float transitionDuration = 2.0f; // Duration of the lerp transition
    public List<MusicClip> musicClips; // List of music clips
    private Coroutine currentTransition; // To keep track of the current transition coroutine

    private void Awake()
    {
        // Ensure only one instance of the singleton exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true; // Set the AudioSource to loop
    }

    private void Start()
    {
      //  Debug.Log("Start method called. Attempting to change music to 'Calm'.");
        ChangeMusicByName("Calm");
    }

    // Method to change music with a lerp transition
    public void ChangeMusic(AudioClip newClip)
    {
       // Debug.Log("ChangeMusic called.");
        // Stop the current transition if it's running
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }

        // Start a new transition
        currentTransition = StartCoroutine(LerpMusic(newClip));
    }

    private IEnumerator LerpMusic(AudioClip newClip)
    {
      //  Debug.Log("LerpMusic coroutine started.");
        float currentTime = 0f;
        float initialVolume = audioSource.volume;

        // Fade out current music
        while (currentTime < transitionDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(initialVolume, 0, currentTime / transitionDuration);
            yield return null;
        }

        // Change the clip and fade in new music
        audioSource.clip = newClip;
    //    Debug.Log("Audiosource play");
        audioSource.Play();

        currentTime = 0f;
        while (currentTime < transitionDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, initialVolume, currentTime / transitionDuration);
            yield return null;
        }

        audioSource.volume = initialVolume; // Ensure the volume is set to initial value at the end

        // Clear the current transition coroutine reference
        currentTransition = null;
    }

    // Method to change music using a string name
    public void ChangeMusicByName(string clipName)
    {
     //   Debug.Log($"ChangeMusicByName called with clipName: {clipName}");
        AudioClip newClip = null;


        foreach (MusicClip musicClip in musicClips)
        {
            if (musicClip.name == clipName)
            {
                newClip = musicClip.clip;
                break;
            }
        }

        if (newClip != null)
        {
        //    Debug.Log($"AudioClip found: {clipName}");
            ChangeMusic(newClip);
        }
        else
        {
            Debug.LogWarning("AudioClip with name " + clipName + " not found!");
        }
    }
}

[System.Serializable]
public class MusicClip
{
    public string name;  // Name of the music clip
    public AudioClip clip;  // Reference to the AudioClip
}
