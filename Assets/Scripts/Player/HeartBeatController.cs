using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class HeartBeatController : MonoBehaviour
{
    public static HeartBeatController Instance { get; private set; }

    public GameObject heartbeatUI;
    public TextMeshProUGUI intervalText;

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
        SetBlinkInterval(1);
    }

    // Method to set the blink interval.
    public void SetBlinkInterval(float blinkInterval)
    {
        currentBlinkInterval = blinkInterval;
        intervalText.text = $"Interval: {blinkInterval:F2} seconds";
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
}
