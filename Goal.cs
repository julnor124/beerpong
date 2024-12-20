using System.Collections;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public AudioClip collisionSound; // Assign the collision sound in the inspector
    private AudioSource audioSource;
    private Renderer cylinderRenderer; // Reference to the Renderer component
    private Light cylinderLight; // Reference to the Light component
    private Color originalColor; // Store the original color of the cylinder

    // Enum to select the type of effect to play (Sound, Light, or Both)
    public enum CollisionEffect { None, Sound, Light, Both }

    // Set which effect you want for the collision with the Player
    public CollisionEffect collisionEffect = CollisionEffect.Both; // Default to Both (Sound + Light)

    void Start()
    {
        // Get or add an AudioSource component
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Prevent sound from playing on load

        // Get the Renderer component to change material color
        cylinderRenderer = gameObject.GetComponent<Renderer>();
        if (cylinderRenderer == null)
        {
            Debug.LogError("No Renderer component found on the cylinder. Please add one.");
        }
        else
        {
            // Store the original color of the material
            originalColor = cylinderRenderer.material.color;
        }

        // Get the Light component
        cylinderLight = gameObject.GetComponent<Light>();
        if (cylinderLight == null)
        {
            Debug.LogError("No Light component found on the cylinder. Please add one in the Editor.");
        }
        else
        {
            cylinderLight.enabled = false; // Ensure the light starts off
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle the effects based on the selected enum value
            if (collisionEffect == CollisionEffect.Sound || collisionEffect == CollisionEffect.Both)
            {
                // Play the collision sound
                if (audioSource != null && collisionSound != null)
                {
                    audioSource.clip = collisionSound;
                    audioSource.Play();
                }
            }

            if (collisionEffect == CollisionEffect.Light || collisionEffect == CollisionEffect.Both)
            {
                // Change the material color to red
                if (cylinderRenderer != null)
                {
                    cylinderRenderer.material.color = Color.red;
                }

                // Turn on the light
                if (cylinderLight != null)
                {
                    cylinderLight.enabled = true;
                    cylinderLight.color = Color.red; // Set the light color to red
                    cylinderLight.intensity = 3f; // Optionally adjust intensity
                    cylinderLight.range = 5f; // Adjust range to cover the cylinder
                }
            }

            // Start a coroutine to reset both color and light after 2 seconds
            StartCoroutine(ResetAppearanceAfterDelay(2f));
        }
    }

    private IEnumerator ResetAppearanceAfterDelay(float delay)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(delay);

        // Reset the material color to its original color
        if (cylinderRenderer != null)
        {
            cylinderRenderer.material.color = originalColor;
        }

        // Turn off the light
        if (cylinderLight != null)
        {
            cylinderLight.enabled = false;
        }
    }
}
