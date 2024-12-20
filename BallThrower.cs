using UnityEngine; // This is correct placement for 'using'

public class BallThrower : MonoBehaviour
{
    private GameObject Ball;

    float startTime, endTime, swipeDistance, swipeTime;
    private Vector2 startPos;
    private Vector2 endPos;

    public float MinSwipeDist = 20f; // Minimum swipe distance to register a throw
    private float BallSpeed = 0;
    public float MaxBallSpeed = 10f; // Maximum speed for the throw (increase if needed)
    private Vector3 throwDirection;

    private bool thrown, holding;
    private Vector3 newPosition;
    Rigidbody rb;

    // Table height for scaling the throw
    private float tableHeight = 0.7f; // Replace with the height of your table
    private float scalingFactor = 1f;

    void Start()
    {
        setupBall();
    }

    void setupBall()
    {
        GameObject _ball = GameObject.FindGameObjectWithTag("Player");
        if (_ball == null)
        {
            Debug.LogError("Ball object with tag 'Player' not found. Please ensure the ball object is tagged correctly.");
            return;
        }

        Ball = _ball;
        rb = Ball.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the Ball object. Please add a Rigidbody to the Ball.");
            return;
        }
        ResetBall();
    }

    public void ResetBall()
    {
        thrown = holding = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        Ball.transform.position = transform.position;
    }

    void PickupBall()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 1f; // Ensure the ball follows the cursor
        newPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Ball.transform.position = Vector3.Lerp(Ball.transform.position, newPosition, 15f * Time.deltaTime);
    }

    private void Update()
    {
        if (holding)
            PickupBall();

        if (thrown)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.transform == Ball.transform)
                {
                    startTime = Time.time;
                    startPos = Input.mousePosition;
                    holding = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!holding) return;

            endTime = Time.time;
            endPos = Input.mousePosition;
            swipeDistance = (endPos - startPos).magnitude;
            swipeTime = endTime - startTime;

            if (swipeDistance > MinSwipeDist && swipeTime < 0.5f)
            {
                CalculateThrow();
                rb.AddForce(throwDirection * BallSpeed * 0.5f, ForceMode.Impulse); // Adjust force application
                rb.useGravity = true;
                holding = false;
                thrown = true;

                // Draw the throw direction for debugging
                Debug.DrawLine(Ball.transform.position, Ball.transform.position + throwDirection * BallSpeed, Color.red, 2f);

                Invoke("ResetBall", 4f);
            }
            else
            {
                ResetBall();
            }
        }
    }

    void CalculateThrow()
    {
        // Adjust scaling factor based on table height
        scalingFactor = tableHeight * 2f; // This ensures that speed is more fitting for the table's size

        // Calculate throw direction based on swipe
        Vector3 swipeDirection = (endPos - startPos).normalized;
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Combine forward and right vectors to calculate throw direction
        throwDirection = (forward * swipeDirection.y + right * swipeDirection.x).normalized;

        // Clamp the vertical component of the throw direction to prevent extreme angles
        throwDirection.y = Mathf.Clamp(throwDirection.y, 0.1f, 0.5f);

        // Adjust speed calculation with scaling factor and clamp it
        BallSpeed = Mathf.Clamp(swipeDistance / Mathf.Max(swipeTime, 0.1f) * 0.03f * scalingFactor, 3f, MaxBallSpeed);
    }
}
