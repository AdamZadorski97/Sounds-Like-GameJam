using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogClip
{
    public AudioClip clip;
    public float intervalAfterClip;
}

public class PlayAudioClip : MonoBehaviour
{
    // Reference to the AudioSource component
    private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    // Boolean to track if the audio has been played
    private bool hasPlayed = false;
    [SerializeField] private bool playOnlyOneTime;

    // List of dialog clips and intervals
    [SerializeField] private List<DialogClip> dialogClips;
    private int currentDialogIndex = 0;

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
            if (playOnlyOneTime)
                hasPlayed = true;
        }
    }

    public void PlayDialog()
    {
        if (dialogClips != null && dialogClips.Count > 0)
        {
            StartCoroutine(PlayDialogSequence());
        }
    }

    private IEnumerator PlayDialogSequence()
    {
        while (currentDialogIndex < dialogClips.Count)
        {
            audioSource.clip = dialogClips[currentDialogIndex].clip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + dialogClips[currentDialogIndex].intervalAfterClip);
            currentDialogIndex++;
        }
    }
}
