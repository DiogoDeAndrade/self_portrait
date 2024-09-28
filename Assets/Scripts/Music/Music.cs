using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Music : Minigame
{
    [SerializeField, Header("Music")] private float              startDelay = 2.0f;
    [SerializeField] private FlyingCat          cat;
    [SerializeField] private ParticleSystem     effectPS;

    protected override void Start()
    {
        base.Start();

        Invoke(nameof(StartPlaying), startDelay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isPlaying)
        {
            return;
        }

        if (playTime > 1.0f)
        {
            DisablePrompt();
        }

        if (cat.transform.position.y < -210.0f)
        {
            Loose();
        }
    }

    protected override void TimeExpired()
    {
        Win();
        effectPS.Play();
    }
}
