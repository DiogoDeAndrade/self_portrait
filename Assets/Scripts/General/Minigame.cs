using UnityEngine;
using UnityEngine.UI;
using OkapiKit;
using NaughtyAttributes;
using TMPro;

public class Minigame : MonoBehaviour
{
    public enum GameState { WaitStart, Playing, Win, Loose, Terminate };

    [SerializeField] public  Sprite    image;
    [SerializeField] public  Vector2   mindPosition;
    [SerializeField] public  Vector2   mindSize;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip looseSound;
    [SerializeField] public  float     transitionDelay = 1.0f;
    [SerializeField] public  float     timeLimit = 0.0f;
    [SerializeField, ShowIf("hasTimeLimit")]
    private Image timerProgress;
    [SerializeField] 
    public TextMeshProUGUI promptText;

    protected GameState     _gameState = GameState.WaitStart;
    protected float         gameTimer;
    protected CanvasGroup   timerCanvasGroup;
    protected CanvasGroup   promptCanvasGroup;

    public bool isPlaying => (_gameState == GameState.Playing);
    public GameState gameState => _gameState;

    bool hasTimeLimit => (timeLimit > 0);
    bool hasPrompt => promptText != null;

    protected virtual void Start()
    {
        var canvas = GetComponentsInChildren<Canvas>();
        foreach (var c in canvas)
        {
            if ((c.renderMode == RenderMode.ScreenSpaceCamera) || ((c.renderMode == RenderMode.ScreenSpaceOverlay) && (c.worldCamera == null)))
            {
                c.worldCamera = MinigameManager.minigameCamera;
            }
        }

        if (hasTimeLimit)
        {
            gameTimer = timeLimit;
            if (timerProgress)
            {
                timerCanvasGroup = timerProgress.GetComponent<CanvasGroup>();
                if ((timerCanvasGroup == null) && (timerProgress.transform.parent != null))
                {
                    timerCanvasGroup = timerProgress.transform.parent.GetComponent<CanvasGroup>();
                }
            }
        }

        if (promptText)
        {
            promptCanvasGroup = promptText.GetComponent<CanvasGroup>();
        }
    }

    protected virtual void Update()
    {
        if ((isPlaying) && (hasTimeLimit))
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer < 0)
            {
                Loose();    
            }

            if (timerProgress)
            {
                timerProgress.fillAmount = gameTimer / timeLimit;
            }

            if (timerCanvasGroup)
            {
                timerCanvasGroup.alpha = Mathf.Clamp01(timerCanvasGroup.alpha + Time.deltaTime);
            }
        }
        else
        {
            if (timerCanvasGroup)
            {
                timerCanvasGroup.alpha = Mathf.Clamp01(timerCanvasGroup.alpha - Time.deltaTime);
            }
        }

        if ((promptCanvasGroup) && (!string.IsNullOrEmpty(promptText.text)))
        {
            promptCanvasGroup.alpha = Mathf.Clamp01(promptCanvasGroup.alpha + Time.deltaTime);
        }
        else
        {
            promptCanvasGroup.alpha = Mathf.Clamp01(promptCanvasGroup.alpha - Time.deltaTime);
        }
    }

    protected void StartPlaying()
    {
        _gameState = GameState.Playing;
    }

    protected virtual void Win()
    {
        _gameState = GameState.Win;
        if (winSound) SoundManager.PlaySound(winSound, 1, 1);
        
    }

    protected virtual void Loose()
    {
        _gameState = GameState.Loose;
        if (looseSound) SoundManager.PlaySound(looseSound, 1, 1);
    }

    protected void SetPrompt(string prompt)
    {
        if (promptText)
        {
            promptText.text = prompt;
        }
    }

    protected void DisablePrompt()
    {
        SetPrompt("");
    }
}
