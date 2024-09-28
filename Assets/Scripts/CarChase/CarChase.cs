using TMPro;
using UnityEngine;

public class CarChase : Minigame
{
    [SerializeField, Header("CarChase")] 
    private float           startDelay = 2.0f;
    [SerializeField]
    public float            timeScale = 1.0f;

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

        if (playTime > 0.5f)
        {
            DisablePrompt();
        }
    }

    protected override void TimeExpired()
    {
        Win();
    }
}
