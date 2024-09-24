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

        var textPrompt = GetComponentInChildren<TextMeshProUGUI>();
        textPrompt.text = $"Press {key} to win!";
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying) return;

        foreach (var k in availableKeys)
        {
            if (Input.GetKeyDown(k))
            {
                if (k == key)
                {
                    // Win
                    var textPrompt = GetComponentInChildren<TextMeshProUGUI>();
                    textPrompt.text = $"WIN!";

                    Win();
                }
                else
                {
                    // Loose
                    var textPrompt = GetComponentInChildren<TextMeshProUGUI>();
                    textPrompt.text = $"LOOSE!";

                    Loose();
                }
            }
        }
    }
}
