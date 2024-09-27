using System.Collections;
using UnityEngine;

public class HeartTrail : MonoBehaviour
{
    [SerializeField] private float  speedX = 100.0f;
    [SerializeField] private float  amplitudeY = 100.0f;
    [SerializeField] AudioSource    flatlineAudioSource;
    [SerializeField] AudioSource    beepAudioSource;

    private float   t = 0.0f;
    TrailRenderer   trailRenderer;
    float           movingAverage;
    int             restartIn = 0;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        movingAverage = 0.0f;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        pos.x = pos.x + speedX * Time.deltaTime;
        if (pos.x >= 320.0f)
        {
            trailRenderer.emitting = false;
            if (pos.x > 340.0f)
            {
                pos.x -= 660.0f;
            }
            restartIn = 2;
        }
        else
        {
            trailRenderer.emitting = (restartIn-- < 0);
        }

        float height = GetValue(t);

        pos.y = amplitudeY * height;

        t += Time.deltaTime * 2.0f;

        if (t > 2.2f)
        {
            if ((MinigameManager.gameState == Minigame.GameState.WaitStart) ||
                (MinigameManager.gameState == Minigame.GameState.Win))
            {
                t -= 2.2f;
            }
        }
        
        transform.position = pos;

        height = Mathf.Abs(height);
        movingAverage = ((movingAverage * 20.0f) + height) / 21.0f;

        flatlineAudioSource.volume = (movingAverage < 0.001f) ? (1.0f) : (0.0f);

        if ((height > 0.75f) && (!beepAudioSource.isPlaying))
        {
            beepAudioSource.Play();
        }
    }

    float GetValue(float t)
    {
        float   B = 7.2f;
        float   C = 10.0f;
        int     n = 1;
        float   T = 1.0f;
        return Mathf.Clamp(Mathf.Exp(-B * Mathf.Pow(t - n * T, 2)) * Mathf.Sin(C * (t - n * T)), -1, 1);
    }

    public void Pump()
    {
        if (t > 0.5f)
        {
            t = 0.5f;
        }
    }
}
