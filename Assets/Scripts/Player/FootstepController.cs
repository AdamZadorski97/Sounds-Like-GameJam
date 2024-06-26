using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    // Audio Source to play footstep sounds
    private AudioSource audioSource;

    // Array of footstep sounds
    public AudioClip[] footstepSounds;

    // Minimum velocity to trigger footstep sounds
    public float minVelocity = 0.1f;

    // Time between steps
    public float stepInterval = 0.5f;
    private float stepTimer;

    // To calculate velocity
    private Vector3 lastPosition;
    private Vector3 velocity;

    void Start()
    {
        // Get the Audio Source component
        audioSource = GetComponent<AudioSource>();

        // Initialize the step timer
        stepTimer = stepInterval;

        // Initialize last position
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate velocity
        velocity = (transform.position - lastPosition) / Time.deltaTime;

        // Check if the object is moving
        if (velocity.magnitude > minVelocity)
        {
            // Update the step timer
            stepTimer -= Time.deltaTime;

            // If the timer reaches zero, play a footstep sound
            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = stepInterval; // Reset the step timer
            }
        }
        else
        {
            // If the object is not moving, reset the step timer
            stepTimer = stepInterval;
        }

        // Update last position
        lastPosition = transform.position;
    }

    void PlayFootstepSound()
    {
        // Select a random footstep sound from the array
        AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
        Debug.Log("playFootstep");
        // Play the selected footstep sound
        audioSource.PlayOneShot(footstepSound);
    }
}
