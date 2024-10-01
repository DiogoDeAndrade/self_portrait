using TMPro;
using OkapiKit;
using UnityEngine;
using Mono.Cecil;

public class CatGame : Minigame
{
    [SerializeField, Header("CatGame")] private float              startDelay = 2.0f;
    [SerializeField] private MovementPlatformer catPlayerMovement;
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
            catPlayerMovement.enabled = false;
            return;
        }
        else
        {
            catPlayerMovement.enabled = true;
        }

        var flies = FindObjectsByType<Fly>(FindObjectsSortMode.None);
        if (flies.Length == 0)
        {
            effectPS.Play();
            Win();
        }

        if (playTime > 1.0f)
        {
            DisablePrompt();
        }
    }

    protected override void TimeExpired()
    {
        base.TimeExpired();

        catPlayerMovement.GetComponent<Animator>().SetTrigger("Dead");
        catPlayerMovement.enabled = false;
    }
}
