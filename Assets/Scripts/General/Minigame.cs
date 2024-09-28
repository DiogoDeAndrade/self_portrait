using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

public class Minigame : MonoBehaviour
{
    public enum GameState { WaitStart, Playing, Win, Loose, Terminate };

    [SerializeField, Header("Minigame")] 
    public  Sprite    image;
    [SerializeField] public  Vector2   mindPosition;
    [SerializeField] public  Vector2   mindSize;
    [SerializeField] private Color     backgroundColor = Color.black;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip looseSound;
    [SerializeField] public  float     transitionDelay = 1.0f;
    [SerializeField] public  float     timeLimit = 0.0f;
    [SerializeField, ShowIf("hasTimeLimit")]
    private Image timerProgress;
    [SerializeField] 
    public TextMeshProUGUI promptText;
    [SerializeField]
    public float timeScale = 1.0f;

    protected GameState     _gameState = GameState.WaitStart;
    protected float         gameTimer;
    protected CanvasGroup   timerCanvasGroup;
    protected CanvasGroup   promptCanvasGroup;
    protected float         playTime;
    protected Camera        minigameCamera;

    public bool isPlaying => (_gameState == GameState.Playing);
    public GameState gameState => _gameState;

    bool hasTimeLimit => (timeLimit > 0);
    bool hasPrompt => promptText != null;

    protected virtual void Start()
    {
        var canvas = GetComponentsInChildren<Canvas>();
        minigameCamera = MinigameManager.minigameCamera;
        if (minigameCamera == null)
        {
            // Find it by name (just used for testing)
            minigameCamera = GameObject.Find("MinigameCamera").GetComponent<Camera>();
        }
        foreach (var c in canvas)
        {
            if ((c.renderMode == RenderMode.ScreenSpaceCamera) || ((c.renderMode == RenderMode.ScreenSpaceOverlay) && (c.worldCamera == null)))
            {
                c.worldCamera = minigameCamera;
            }
        }
        if (minigameCamera)
        {
            minigameCamera.backgroundColor = backgroundColor;

            var cameraFollow = minigameCamera.GetComponent<CameraFollow>();
            if (cameraFollow)
            {
                cameraFollow.targetObject = null;                
            }
            minigameCamera.transform.position = new Vector3(0, 0, minigameCamera.transform.position.z);
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
        if (isPlaying)
        {
            playTime += Time.deltaTime;
        }

        if ((isPlaying) && (hasTimeLimit))
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer < 0)
            {
                TimeExpired();
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
            if (promptCanvasGroup)
            {
                promptCanvasGroup.alpha = Mathf.Clamp01(promptCanvasGroup.alpha - Time.deltaTime);
            }
        }
    }

    protected void StartPlaying()
    {
        _gameState = GameState.Playing;
    }

    public virtual void Win()
    {
        _gameState = GameState.Win;
        if (winSound) SoundManager.PlaySound(winSound, 1, 1);
        
    }

    public virtual void Loose()
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

    protected virtual void TimeExpired()
    {
        Loose();
    }
}
