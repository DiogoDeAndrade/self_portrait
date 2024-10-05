using UnityEngine;

public class FlyingCat : MonoBehaviour
{
    [SerializeField] private float          speed;
    [SerializeField] private float          turnSpeed = 180.0f;
    [SerializeField] private float          boostCooldown = 0.5f;
    [SerializeField] private float          gravity = 15.0f;
    [SerializeField] private ParticleSystem boostFX;
    [SerializeField] private float          fuelDrain = 10.0f;
    [SerializeField] private RectTransform  fuelDisplay;
    [SerializeField] private AudioClip      boostSound;
    [SerializeField] private AudioClip      moreFuelSound;
    [SerializeField] private AudioClip      lessFuelSound;

    private Rigidbody2D rb;
    private float       cooldown = 0;
    private float       fuel = 100.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        fuel = 100.0f;
    }

    private void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;

        if (!MinigameManager.isPlaying)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else 
        {
            rb.gravityScale = gravity / Mathf.Abs(Physics2D.gravity.y);
            if ((Input.GetButtonDown("Jump")) && (cooldown <= 0.0f) && (fuel >= fuelDrain))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, speed);
                cooldown = boostCooldown;
                boostFX.Play();
                fuel -= fuelDrain;
                fuelDisplay.localScale = new Vector3(fuel / 100.0f, 1.0f, 1.0f);
                if (boostSound) SoundManager.PlaySound(boostSound, 0.25f, 1);
                else if (moreFuelSound) SoundManager.PlaySound(moreFuelSound, 0.25f, 2);
            }
        }

        Vector2 velocity = rb.linearVelocity;

        float targetRotation = 0.0f;
        if (velocity.y < -2.0f) targetRotation = -20.0f;
        else if (velocity.y > 2.0f) targetRotation = 20.0f;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, targetRotation), turnSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!MinigameManager.isPlaying) return;

        var note = collider.GetComponent<Note>();
        if (note)
        {
            if (note.ammount > 0)
            {
                // Power chord
                if (moreFuelSound) SoundManager.PlaySound(moreFuelSound, 0.25f, 1);
            }
            else
            {
                if (lessFuelSound) SoundManager.PlaySound(lessFuelSound, 0.25f, 1);
            }
            fuel = Mathf.Clamp(fuel + note.ammount, 0, 100);

            Destroy(collider.gameObject);
        }
    }
}
