using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Writer : Minigame
{
    [SerializeField, Header("Writer")] 
    private float              startDelay = 2.0f;
    [SerializeField]
    private TextMeshProUGUI    text;
    [SerializeField]
    private TextMeshProUGUI    textOK;
    [SerializeField]
    private AudioClip          keyClickSound;
    [SerializeField]
    private AudioClip          dingSound;
    [SerializeField, TextArea]
    private string[]           possibleTexts;

    protected override void Start()
    {
        base.Start();

        int r = Random.Range(0, possibleTexts.Length);
        text.text = textOK.text = possibleTexts[r];

        textOK.maxVisibleCharacters = 0;

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
             
            if (textOK.maxVisibleCharacters < textOK.text.Length)
            {
                string input = Input.inputString.ToLower();
                if (!string.IsNullOrEmpty(input))
                {
                    char c = input[0];
                    char toMatch = char.ToLower(textOK.text[textOK.maxVisibleCharacters]);

                    if ((c == toMatch) ||
                        ((toMatch == ' ') && ((textOK.maxVisibleCharacters + 1) < textOK.text.Length) && (c == char.ToLower(textOK.text[textOK.maxVisibleCharacters + 1]))))
                    {
                        textOK.maxVisibleCharacters = Mathf.Clamp(textOK.maxVisibleCharacters + 1, 0, textOK.text.Length);
                        if ((c != ' ') && (toMatch == ' '))
                        {
                            // Skip the space
                            textOK.maxVisibleCharacters = Mathf.Clamp(textOK.maxVisibleCharacters + 1, 0, textOK.text.Length);
                        }
                        SoundManager.PlaySound(keyClickSound, Random.Range(0.2f, 0.5f), 1.0f);

                        if (textOK.maxVisibleCharacters == textOK.text.Length)
                        {
                            Win();
                            SoundManager.PlaySound(dingSound, 1.0f, 1.0f);
                        }
                    }
                }
            }
        }
    }
}
