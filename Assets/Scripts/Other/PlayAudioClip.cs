using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{
    // Reference to the AudioSource component
    private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    // Boolean to track if the audio has been played
    private bool hasPlayed = false;
    [SerializeField] bool playOnlyOneTime;
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        // Check if the audio has been played before
        if (!hasPlayed)
        {
            audioSource.PlayOneShot(clip);
            // Set hasPlayed to true to prevent future plays
            if(playOnlyOneTime)
            hasPlayed = true;
        }
    }
}