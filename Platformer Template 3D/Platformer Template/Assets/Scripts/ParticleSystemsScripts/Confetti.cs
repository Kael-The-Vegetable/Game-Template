using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confetti : MonoBehaviour
{
    public ParticleSystem confetti; // Assign your ParticleSystem in the Inspector
    public AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the Player triggered it
        {
            if (confetti != null)
            {
                confetti.Stop();  // Stop the Particle System if it is already playing
                confetti.Play();  // Restart the Particle System
                audioSource.Stop();
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Confetti ParticleSystem is not assigned in the Inspector!");
            }
        }
    }
}
