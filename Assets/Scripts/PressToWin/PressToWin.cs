using TMPro;
using UnityEngine;

public class PressToWin : Minigame
{
    static KeyCode[] availableKeys = { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K };

    KeyCode         key;

    protected override void Start()
    {
        base.Start();

        key = availableKeys[Random.Range(0, availableKeys.Length)];

        SetPrompt($"Press {key} to win!");

        StartPlaying();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isPlaying) return;

        foreach (var k in availableKeys)
        {
            if (Input.GetKeyDown(k))
            {
                if (k == key)
                {
                    // Win
                    SetPrompt($"WIN!");

                    Win();
                }
                else
                {
                    // Loose
                    SetPrompt($"LOOSE!");

                    Loose();
                }
            }
        }
    }
}
