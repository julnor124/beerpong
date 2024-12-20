using UnityEngine;

public class CupDetection : MonoBehaviour
{
    public GameObject parentCup; // Reference to the parent cup object

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
// Find the BallThrower component in the scene
            BallThrower ballThrower = FindObjectOfType<BallThrower>();
            if (ballThrower != null)
            {
                ballThrower.ResetBall(); // Call ResetBall to reset the ball
            }
            else
            {
                Debug.LogError("BallThrower not found in the scene.");
            }

            // Destroy the sphere (this game object)
            Destroy(gameObject);

            // Destroy the parent cup
            if (parentCup != null)
            {
                Destroy(parentCup);
            }
        }
    }

    private void BallDisappear(GameObject ball)
    {
        // Disable ball physics and hide it
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.useGravity = false;
        }
        ball.SetActive(false); // Hide the ball

        // Reappear the ball after 2 seconds
        Invoke(nameof(ReappearBall), 2f);

        void ReappearBall()
        {
            ball.SetActive(true); // Show the ball
            if (ballRb != null)
            {
                ballRb.useGravity = false; // Gravity remains off until reset
            }

            // Trigger reset in BallThrower
            BallThrower ballThrower = ball.GetComponentInParent<BallThrower>();
            if (ballThrower != null)
            {
                ballThrower.ResetBall(); // Reset ball to starting state
            }
        }
    }
}
