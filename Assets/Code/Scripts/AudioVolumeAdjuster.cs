using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeAdjuster : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource component
    public float maxVolume = 1f; // Maximum volume value
    public float minVolume = 0f; // Minimum volume value
    [Range(0f, 1f)]
    public float volumeVariable = 0.5f; // Variable controlling the volume (0 to 1 range)

    void Start()
    {
        // Check if AudioSource component is assigned
        if (audioSource == null)
        {
            // Try to find AudioSource component on the same GameObject
            audioSource = GetComponent<AudioSource>();

            // If AudioSource component is still not found, log an error
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component is not assigned and cannot be found on the GameObject.");
                return;
            }
        }

        // Set the initial volume based on the volumeVariable
        SetVolume(volumeVariable);
    }

    void Update() 
    {
        if (audioSource.volume != volumeVariable) {
            SetVolume(volumeVariable);
        }
    }

    public void AdjustVolume(float volumeValue) {
        volumeVariable = volumeValue;
        SetVolume(volumeVariable);
    }

    public void SetVolume(float value)
    {
        // Clamp the value between minVolume and maxVolume
        float clampedValue = Mathf.Clamp(value, minVolume, maxVolume);


        // Set the volume of the AudioSource
        audioSource.volume = clampedValue;
        Debug.Log("adjusted the volume of the Audio Source!");
        Debug.Log("audio source volume:" + audioSource.volume);
        Debug.Log("adjusted volume:" + clampedValue);
        Debug.Log("volume variable:" + volumeVariable);
    }
}
