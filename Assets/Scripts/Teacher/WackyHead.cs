using UnityEngine;

public class WackyHead : MonoBehaviour
{
    public float wobbleSpeed = 4.0f;   // Speed of wobbling back and forth
    public float wobbleAmount = 10.0f; // How much the object overshoots (springiness)
    public float settleSpeed = 2.0f;   // How quickly the object settles into the target rotation

    private Vector3 prevPos;
    private float rotationVelocity = 0.0f; // Used to store the current velocity of the spring effect
    private float currentAngle = 0.0f;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        prevPos = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 delta = transform.position - prevPos;
        prevPos = transform.position;

        // Determine target angle based on movement direction
        float targetAngle = 0.0f;
        if (Mathf.Abs(delta.x) > 2.0f)
        {
            targetAngle = wobbleAmount;
        }

        // Custom spring-based interpolation for wobbly effect
        float spring = 0.5f;  // Stiffness of the spring
        float damping = 0.97f; // How fast the wobble slows down
        float force = (targetAngle - currentAngle) * spring;
        rotationVelocity = (rotationVelocity + force) * damping;
        currentAngle += rotationVelocity * Time.fixedDeltaTime;

        // Apply the rotation
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, currentAngle);
    }
}
