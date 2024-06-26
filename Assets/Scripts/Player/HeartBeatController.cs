using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeartBeatController : MonoBehaviour
{
    public static HeartBeatController Instance { get; private set; }

    public GameObject heartbeatUI;
    public Image leftBeatUI;
    public Image rightBeatUI;
    public TextMeshProUGUI intervalText;
    public AudioClip clip1Second;
    public AudioClip clip3Seconds;
    public Color fastBeatColor = Color.red;
    public Color slowBeatColor = Color.blue;
    private AudioSource audioSource;
    [SerializeField] private float fastBeat;
    [SerializeField] private float slowBeat;
    private float blinkTimer;
    private float currentBlinkInterval;
    private CanvasGroup canvasGroup;
    private float targetAlpha;
    private float[] samples = new float[256];
    private Color targetColor;

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
        canvasGroup = heartbeatUI.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = heartbeatUI.AddComponent<CanvasGroup>();
        }

        SetBlinkInterval(false);
    }

    // Method to set the blink interval.
    public void SetBlinkInterval(bool isFast)
    {
        Debug.Log("Blink interval" + isFast);
        if (isFast)
        {
            currentBlinkInterval = fastBeat;
            audioSource.clip = clip1Second;
            targetColor = fastBeatColor;
        }
        else
        {
            currentBlinkInterval = slowBeat;
            audioSource.clip = clip3Seconds;
            targetColor = slowBeatColor;
        }
        // Play the audio clip in a loop
        PlayAudio();
    }

    private void Update()
    {
        UpdateHeartbeatUIAlpha();
        UpdateHeartbeatUIColor();
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

    // Method to update the alpha of the heartbeat UI based on the audio clip's loudness
    void UpdateHeartbeatUIAlpha()
    {
        if (heartbeatUI != null && audioSource != null && audioSource.isPlaying)
        {
            // Get the spectrum data
            audioSource.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);

            // Calculate the average loudness from the spectrum data
            float currentLoudness = 0f;
            foreach (var sample in samples)
            {
                currentLoudness += sample;
            }
            currentLoudness /= samples.Length;

            // Set the target alpha based on the loudness
            targetAlpha = Mathf.Clamp01(currentLoudness * 100f); // Adjust the multiplier as needed

            // Smoothly interpolate the alpha to the target alpha
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);
        }
    }

    // Method to update the color of the heartbeat UI based on the beat type
    void UpdateHeartbeatUIColor()
    {
        if (heartbeatUI != null)
        {
            // Smoothly interpolate the color to the target color
            Color currentColor = leftBeatUI.color;
            Color newColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * 5f);
            leftBeatUI.color = newColor;
            rightBeatUI.color = newColor;
        }
    }
}
