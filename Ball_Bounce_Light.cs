using System.Collections;
using UnityEngine;

public class Ball_Bounce_Light : MonoBehaviour
{
    private Light bounceLight; // Reference to the Light component
    public float lightDurationOnPlane = 0.2f; // Duration for light when hitting the plane
    public float lightDurationOnInCup = 3.0f; // Duration for light when hitting the InCup

    private AudioSource audioSource; // Reference to the AudioSource component
    public AudioClip bounceSound; // The sound to play on bounce
    public AudioClip cupSound; // The sound to play when hitting the cup

    public Color lightColorOnPlane = Color.white; // Light color for plane collision
    public Color lightColorOnInCup = Color.green; // Light color for cup collision

    // Enum to select the type of effect to play (Sound, Light, or Both)
    public enum CollisionEffect { None, Sound, Light, Both }

    // Set which effect you want on each collision
    public CollisionEffect planeEffect = CollisionEffect.Both; // Default to Both for the plane
    public CollisionEffect cupEffect = CollisionEffect.Both; // Default to Both for the cup

    private void Start()
    {
        // Get the Light component attached to the ball
        bounceLight = GetComponent<Light>();
        if (bounceLight != null)
        {
            bounceLight.enabled = false; // Ensure the light starts off
        }
        else
        {
            Debug.LogError("No Light component found on the ball.");
        }

        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign audio settings
        audioSource.playOnAwake = false; // Prevent the sound from playing at start
        if (bounceSound != null)
        {
            audioSource.clip = bounceSound; // Assign the bounce sound
        }
        else
        {
            Debug.LogError("No bounce sound assigned.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collided with: {collision.gameObject.name}"); // Log the name of the collided object

        // Check if the ball has collided with the plane
        if (collision.gameObject.CompareTag("plane"))
        {
            Debug.Log("Collision with plane detected!"); // Confirm collision with the tagged object

            // Check the effect mode for the plane
            if (planeEffect == CollisionEffect.Sound || planeEffect == CollisionEffect.Both)
            {
                // Play the bounce sound for the plane
                if (audioSource != null && bounceSound != null)
                {
                    Debug.Log("Playing bounce sound."); // Log sound trigger
                    audioSource.clip = bounceSound; // Set the bounce sound
                    audioSource.Play();
                }
            }

            // Activate the light if the effect mode is Light or Both
            if (planeEffect == CollisionEffect.Light || planeEffect == CollisionEffect.Both)
            {
                bounceLight.color = lightColorOnPlane; // Set the light color
                StartCoroutine(ActivateLight(lightDurationOnPlane)); // Activate light with duration
            }
        }
        // Check if the ball has collided with the cup
        else if (collision.gameObject.CompareTag("InCup"))
        {
            Debug.Log("Collision with cup detected!"); // Confirm collision with the tagged object

            // Check the effect mode for the cup
            if (cupEffect == CollisionEffect.Sound || cupEffect == CollisionEffect.Both)
            {
                // Play the cup sound
                if (audioSource != null && cupSound != null)
                {
                    Debug.Log("Playing cup sound."); // Log sound trigger
                    audioSource.clip = cupSound; // Set the cup sound
                    audioSource.Play();
                }
            }

            // Activate the light if the effect mode is Light or Both
            if (cupEffect == CollisionEffect.Light || cupEffect == CollisionEffect.Both)
            {
                bounceLight.color = lightColorOnInCup; // Set the light color
                StartCoroutine(ActivateLight(lightDurationOnInCup)); // Activate light with duration
            }
        }
    }

    private IEnumerator ActivateLight(float duration)
    {
        if (bounceLight != null)
        {
            Debug.Log("Light activated."); // Log light activation
            bounceLight.enabled = true; // Turn on the light
            yield return new WaitForSeconds(duration); // Wait for the specified duration
            bounceLight.enabled = false; // Turn off the light
            Debug.Log("Light deactivated."); // Log light deactivation
        }
        else
        {
            Debug.LogError("No Light component found on the ball.");
        }
    }
}
