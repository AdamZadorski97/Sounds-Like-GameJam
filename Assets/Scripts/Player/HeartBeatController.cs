using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeartBeatController : MonoBehaviour
{
    public static HeartBeatController Instance { get; private set; }

    public GameObject heartbeatUI;
    public TextMeshProUGUI intervalText;
    public AudioClip clip1Second;
    public AudioClip clip3Seconds;
    private AudioSource audioSource;

    private float blinkTimer;
    private float currentBlinkInterval;

    private void Awake()
    {
        // If an instance already exists and it's not this one, destroy this instance.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this.
            Instance = this;
            // Optionally, make this instance persist across scene loads.
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Initialize the audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        SetBlinkInterval(3);
    }

    // Method to set the blink interval.
    public void SetBlinkInterval(float blinkInterval)
    {
        currentBlinkInterval = blinkInterval;
        intervalText.text = $"Interval: {blinkInterval:F2} seconds";

        // Change the audio clip based on the interval
        if (blinkInterval == 1f)
        {
            audioSource.clip = clip1Second;
        }
        else if (blinkInterval == 3f)
        {
            audioSource.clip = clip3Seconds;
        }

        // Play the audio clip in a loop
        PlayAudio();
    }

    private void Update()
    {
        HandleBlinking();
    }

    void HandleBlinking()
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= currentBlinkInterval)
        {
            if (heartbeatUI != null)
            {
                heartbeatUI.SetActive(!heartbeatUI.activeSelf);
            }
            blinkTimer = 0f;
        }
    }

    // Method to play the audio clip in a loop
    void PlayAudio()
    {
        if (audioSource.clip != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
