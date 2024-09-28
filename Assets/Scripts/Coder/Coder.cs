using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coder : Minigame
{
    [SerializeField, Header("Coder")] 
    private float              startDelay = 2.0f;
    [SerializeField]
    private TextMeshProUGUI    code;
    [SerializeField]
    private AudioClip          keyClickSound;
    [SerializeField]
    private GameObject         videoPlayer;

    private List<KeyCode> lastKeys;
    private int keyArrayIndex = 0;

    protected override void Start()
    {
        base.Start();

        code.maxVisibleCharacters = 0;
        lastKeys = new List<KeyCode>();
        for (int i = 0; i < 5; i++)
        {
            lastKeys.Add(KeyCode.None);
        }

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

        if (Input.anyKeyDown)
        {
            DisablePrompt();

            var keyCode = GetPressedKey();

            if (code.maxVisibleCharacters < code.text.Length)
            {
                if (lastKeys.IndexOf(keyCode) == -1)
                {
                    code.maxVisibleCharacters = Mathf.Clamp(code.maxVisibleCharacters + 4, 0, code.text.Length);
                    SoundManager.PlaySound(keyClickSound, 1.0f, Random.Range(0.8f, 1.4f));

                    lastKeys[keyArrayIndex] = keyCode;
                    keyArrayIndex = (keyArrayIndex + 1) % lastKeys.Count;
                }
            }
            else
            {
                Win();
                videoPlayer.SetActive(true);
            }
        }
    }

    KeyCode GetPressedKey()
    {
        // Loop through all possible KeyCode values
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                return kcode;
            }
        }
        return KeyCode.None;
    }
}
