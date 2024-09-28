using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float      speed;
    [SerializeField] private float      turnSpeed = 180.0f;
    [SerializeField] private AudioClip  carCrashSound;
    [SerializeField] private GameObject crashEffect;

    private Rigidbody2D rb;
    private Vector2     moveVector = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveVector;
    }

    // Update is called once per frame
    void Update()
    {
        if (!MinigameManager.isPlaying)
        {
            moveVector = Vector2.zero;
        }
        else
        {
            moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            moveVector.Normalize();

            moveVector *= speed;
        }

        Vector2 velocity = rb.linearVelocity;

        float targetRotation = 0.0f;
        if (velocity.x < -2.0f) targetRotation = 20.0f;
        else if (velocity.x > 2.0f) targetRotation = -20.0f;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotation), turnSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var scrollElement = collision.GetComponent<ScrollElement>();
        if (scrollElement)
        {
            SoundManager.PlaySound(carCrashSound, 1, 1);
            var carChase = FindFirstObjectByType<CarChase>();
            carChase.timeScale = 0.0f;
            carChase.Loose();
            Instantiate(crashEffect, transform.position, Quaternion.identity);
        }
    }
}
