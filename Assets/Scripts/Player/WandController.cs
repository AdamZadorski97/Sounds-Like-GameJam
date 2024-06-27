using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // Import the Audio namespace
using UnityEngine.XR; // Import the XR namespace

[Serializable]
public class SpellCombo
{
    public string combo; // The combo string, e.g., "C/E/G"
    public string spellName; // The name of the spell, e.g., "Fireball"
}

public class WandController : MonoBehaviour
{
    private AudioSource audioSource; // The AudioSource component that will play the piano notes
    public AudioClip[] pianoNotes; // Array to hold piano note AudioClips
    public AudioMixer audioMixer; // Reference to the AudioMixer
    private int currentNoteIndex = -1; // Index of the currently playing note
    public float maxVolume = 0.0f; // Maximum volume in decibels
    public float minVolume = -80.0f; // Minimum volume in decibels
    public int noteMultiplier = 2; // Multiplier to skip notes
    public float noteCooldown = 0.5f; // Minimum time interval between notes in seconds
    private float lastNoteTime = 0f; // Time when the last note was played
    [SerializeField] private TMPro.TextMeshPro wandText;
    private bool isGrabbed;
    [SerializeField] private List<string> noteSequence = new List<string>(); // List to store the sequence of notes played
    public List<SpellCombo> spellCombos; // List of special combo spells
    public GameObject shieldPrefab;
    void Start()
    {
        // Get the AudioSource component attached to the wand object
        audioSource = GetComponent<AudioSource>();
    }

    public void SwitchGrabbed(bool state)
    {
        isGrabbed = state;
    }

    void Update()
    {
        int noteIndex = CalculateNoteIndexBasedOnDirection();
        wandText.text = pianoNotes[noteIndex].name;

        // Check if the "A" button on the Oculus controller is pressed and cooldown has passed
        if (IsAButtonPressed() && Time.time >= lastNoteTime + noteCooldown && isGrabbed)
        {
            PlayNote(noteIndex);
            currentNoteIndex = noteIndex;
            lastNoteTime = Time.time; // Update the last note time
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the object the wand collided with is a projectile
        ProjectileController projectile = collision.gameObject.GetComponent<ProjectileController>();
        if (projectile != null)
        {
            // Calculate the bounce direction
            Vector3 hitPoint = collision.contacts[0].point; // Get the point of collision
            Vector3 bounceDirection = (hitPoint - transform.position).normalized; // Calculate the direction to bounce the projectile
            projectile.Bounce(bounceDirection); // Apply the bounce to the projectile
        }
    }

    // Calculate the index of the piano note to play based on the wand's direction
    private int CalculateNoteIndexBasedOnDirection()
    {
        // Calculate the dot product between the wand's forward vector and the world up vector
        float dotProduct = Vector3.Dot(transform.right, Vector3.up);

        // Normalize the dot product to a value between 0 and 1
        float normalizedDot = (dotProduct + 1) / 2.0f;

        // Calculate the index for the piano notes array
        int index = Mathf.RoundToInt(normalizedDot * (pianoNotes.Length - 1));

        // Apply the multiplier to skip notes
        index = (index / noteMultiplier) * noteMultiplier;

        // Clamp the index to the valid range
        index = Mathf.Clamp(index, 0, pianoNotes.Length - 1);

        return index;
    }

    // Play the specified piano note
    private void PlayNote(int index)
    {
        audioSource.clip = pianoNotes[index];
        audioSource.Play();

        // Adjust the volume of the AudioMixer
        float volume = Mathf.Lerp(minVolume, maxVolume, (float)index / (pianoNotes.Length - 1));
        audioMixer.SetFloat("Volume", volume);

        // Add the note name to the sequence
        string noteName = pianoNotes[index].name;
        noteSequence.Add(noteName);

        // Log the current note sequence for debugging
        Debug.Log("Current Note Sequence: " + string.Join("/", noteSequence));

        // Check for spell combos
        CheckForSpellCombos();
    }

    // Check for special spell combos
    private void CheckForSpellCombos()
    {
        // Combine the note sequence into a single string with "/" as separator
        string combinedNotes = string.Join("", noteSequence);

        // Log the combined notes for debugging
        Debug.Log("Checking Combined Notes: " + combinedNotes);

        // Check if the combined string matches any combo in the spellCombos list
        foreach (SpellCombo combo in spellCombos)
        {
            if (combinedNotes.EndsWith(combo.combo))
            {
                // If a match is found, invoke the corresponding method
                InvokeSpell(combo.spellName);
                PlayerController.Instance.checkSpell.UseSpell(combo.spellName);
                // Clear the note sequence after a successful combo
                noteSequence.Clear();
                break;
            }
        }
    }

    // Invoke the spell corresponding to the detected combo
    private void InvokeSpell(string spellName)
    {
        if (spellName == "SpawnShield")
        {
            SpawnShield();
        }
    }

    // Check if the "A" button on the Oculus controller is pressed
    private bool IsAButtonPressed()
    {
        bool isPressed = false;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, devices);

        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
            {
                isPressed = true;
                break;
            }
        }

        return isPressed;
    }



    public void SpawnShield()
    {
        GameObject spawnedShield = Instantiate(shieldPrefab);
        spawnedShield.transform.localScale = Vector3.zero;
        spawnedShield.transform.DOScale(Vector3.one, 0.5f);
    }
}
