using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private CanvasGroup spellcasterLogo;
    [SerializeField] private CanvasGroup title;
    [SerializeField] private CanvasGroup gameName;
    [SerializeField] private CanvasGroup options;
    [SerializeField] private CanvasGroup mainFader;
    [SerializeField] private AudioSource logoMusic;
    [SerializeField] private AudioClip   titleMusic;

    bool enableOptions = false;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        spellcasterLogo.alpha = 0.0f;
        title.alpha = 0.0f;
        gameName.alpha = 0.0f;
        mainFader.alpha = 0.0f;
        options.alpha = 0.0f;

        enableOptions = false;

        StartCoroutine(TitleCR());
    }

    IEnumerator TitleCR()
    {
        yield return new WaitForSecondsOrKey(0.1f);

        logoMusic?.Play();

        yield return new WaitForSecondsOrKey(5.0f);

        var fadeCR = FadeIn(spellcasterLogo, 1.0f);

        yield return fadeCR;

        if (logoMusic)
        {
            while (logoMusic.isPlaying)
            {
                if (Input.anyKeyDown)
                {
                    break;
                }
                yield return null;
            }

            FadeOut(logoMusic, 0.5f);
        }

        var fadeCR1 = FadeOut(spellcasterLogo, 1.0f);

        var fadeCR2 = FadeIn(title, 2.0f);       

        SoundManager.FadeIn(titleMusic);

        yield return fadeCR1;
        yield return fadeCR2;

        yield return new WaitForSecondsOrKey(4.0f);

        fadeCR1 = FadeOut(title, 1.0f, 0.025f);
        fadeCR2 = FadeIn(gameName, 1.0f);

        yield return fadeCR1;
        yield return fadeCR2;

        yield return new WaitForSecondsOrKey(1.0f);

        fadeCR = FadeIn(options, 1.0f);

        yield return fadeCR;

        Cursor.visible = false;
        enableOptions = true;
    }

    Coroutine FadeIn(CanvasGroup canvasGroup, float duration, float target = 1.0f)
    {
        return StartCoroutine(FadeCanvasCR(canvasGroup, duration, 1.0f, target));
    }
    Coroutine FadeOut(CanvasGroup canvasGroup, float duration, float target = 0.0f)
    {
        return StartCoroutine(FadeCanvasCR(canvasGroup, duration, -1.0f, target));
    }

    IEnumerator FadeCanvasCR(CanvasGroup canvasGroup, float duration, float inc, float target)
    {
        while (true)
        {
            canvasGroup.alpha = Mathf.Clamp(canvasGroup.alpha + inc * Time.deltaTime / duration, (inc < 0) ? (target) : (0.0f), (inc > 0) ? (target) : (1.0f));
            if ((canvasGroup.alpha == target) && (inc < 0.0f)) break;
            else if ((canvasGroup.alpha == target) && (inc > 0.0f)) break;

            if (Input.anyKeyDown) break;

            yield return null;
        }

        canvasGroup.alpha = target;
    }

    Coroutine FadeOut(AudioSource source, float duration)
    {
        return StartCoroutine(FadeMusicCR(source, duration, -1.0f));
    }

    IEnumerator FadeMusicCR(AudioSource source, float duration, float inc)
    {
        while (true)
        {
            source.volume = Mathf.Clamp01(source.volume + inc * Time.deltaTime / duration);
            if ((source.volume == 0.0f) && (inc < 0.0f)) break;
            else if ((source.volume == 1.0f) && (inc > 0.0f)) break;

            yield return null;
        }
    }

    public void StartGame()
    {
        if (enableOptions)
        {
            StartCoroutine(StartGameCR());
        }
    }

    IEnumerator StartGameCR()
    {
        enableOptions = false;

        yield return FadeOut(mainFader, 1.0f);

        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        if (enableOptions)
        {
            StartCoroutine(QuitGameCR());
        }
    }

    IEnumerator QuitGameCR()
    {
        enableOptions = false;

        yield return FadeOut(mainFader, 1.0f);

        Application.Quit();
    }
}
