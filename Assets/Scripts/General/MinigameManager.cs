using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Camera         _minigameCamera;
    [SerializeField] private bool           autoStart;
    [SerializeField] private int            maxLives = 3;
    [SerializeField] private LifeDisplay    lifeDisplay;
    [SerializeField] private Minigame[]     minigamePrefabs;
    [SerializeField] private Minigame       recoveryMinigamePrefab;
    [SerializeField] private Sprite         gameOverImage;
    [SerializeField] private RectTransform  gameOverText;

    [SerializeField] private CanvasGroup    rawImageCanvasGroup; 
    [SerializeField] private RectTransform  photoRectTransform;
    [SerializeField] private Image          oldPhotoImage;
    [SerializeField] private Image          newPhotoImage;
    [SerializeField] private float          transitionDuration = 2f; 

    private RectTransform rawImageRectTransform; 
    private Minigame      currentMinigame;
    private int           lives;
    private bool          recovery;
    private Coroutine     transitionCR;

    public static bool isPlaying
    {
        get
        {
            if (Instance)
            {
                return Instance.currentMinigame.isPlaying;
            }
            else
            {
                var mg = FindFirstObjectByType<Minigame>();
                return mg.isPlaying;
            }
        }
    }
    public static Minigame.GameState gameState
    {
        get
        {
            if (Instance)
            {
                return Instance.currentMinigame.gameState;
            }
            else
            {
                var mg = FindFirstObjectByType<Minigame>();
                return mg.gameState;
            }
        }
    }

    static MinigameManager Instance;

    public static Camera minigameCamera => (Instance != null) ? (Instance._minigameCamera) : (null);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (minigameCamera == null) _minigameCamera = GetComponent<Camera>();

        rawImageRectTransform = rawImageCanvasGroup.GetComponent<RectTransform>();

        if (autoStart)
        {
            // Check if there's already a minigame active
            var minigame = FindAnyObjectByType<Minigame>();
            if (minigame == null)
            {
                SelectRandomMinigame();
            }
            else
            {
                TransitionToMinigame(minigame);
            }
        }

        lives = maxLives;
        lifeDisplay.lives = lives;
    }

    private void Update()
    {
        if ((currentMinigame != null) && (transitionCR == null))
        {
            switch (currentMinigame.gameState)
            {
                case Minigame.GameState.WaitStart:
                    break;
                case Minigame.GameState.Playing:
                    break;
                case Minigame.GameState.Win:
                    TransitionFromMinigame();
                    break;
                case Minigame.GameState.Loose:
                    TransitionFromMinigame();
                    break;
                default:
                    break;
            }
        }
    }

    public void SelectRandomMinigame()
    {
        recovery = false;

        int r = Random.Range(0, minigamePrefabs.Length);
        var prefab = minigamePrefabs[r];
        
        Minigame mg = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        mg.gameObject.SetActive(false);

        TransitionToMinigame(mg);
    }

    public void GoToRecovery()
    {
        recovery = true;

        Minigame mg = Instantiate(recoveryMinigamePrefab, Vector3.zero, Quaternion.identity);
        mg.gameObject.SetActive(false);

        TransitionToMinigame(mg);
    }

    void TransitionToMinigame(Minigame mg)
    {
        currentMinigame = mg;

        // Calculate the scale factor based on the initial size of the RawImage and the target size
        rawImageRectTransform.anchoredPosition = currentMinigame.mindPosition;
        rawImageRectTransform.sizeDelta = currentMinigame.mindSize;

        float scaleFactor = 1280.0f / rawImageRectTransform.sizeDelta.x;

        transitionCR = StartCoroutine(TransitionToMinigameCR(Vector2.zero, scaleFactor));
    }

    void TransitionFromMinigame()
    {
        float scaleFactor = currentMinigame.mindSize.x / 1280.0f;
        Vector2 targetPos = currentMinigame.mindPosition;

        transitionCR = StartCoroutine(TransitionFromMinigameCR(targetPos, scaleFactor));
    }

    IEnumerator TransitionToMinigameCR(Vector2 rawImageTargetPosition, float scaleFactor)
    {
        // Transition from old image to new image
        oldPhotoImage.sprite = newPhotoImage.sprite;
        newPhotoImage.sprite = currentMinigame.image;

        var oldCanvasGroup = oldPhotoImage.GetComponent<CanvasGroup>();
        var newCanvasGroup = newPhotoImage.GetComponent<CanvasGroup>();

        rawImageCanvasGroup.alpha = 0.0f;
        float a = 0.0f;
        while (a < 1.0f)
        {
            oldCanvasGroup.alpha = 1.0f - a;
            newCanvasGroup.alpha = a;

            a += Time.deltaTime;

            yield return null;
        }

        oldCanvasGroup.alpha = 0.0f;
        newCanvasGroup.alpha = 1.0f;

        yield return new WaitForSeconds(2);

        currentMinigame.gameObject.SetActive(true);

        // Store the original sizes and positions of the RawImage and Photo
        Vector2 originalRawImageSize = rawImageRectTransform.sizeDelta;
        Vector2 originalPhotoSize = photoRectTransform.sizeDelta;
        Vector2 originalRawImagePosition = rawImageRectTransform.anchoredPosition;
        Vector2 originalPhotoPosition = photoRectTransform.anchoredPosition;
        float   duration = transitionDuration;

        // Target size for the RawImage (1280x720)
        Vector2 targetRawImageSize = originalRawImageSize * scaleFactor;

        // Calculate the final size for the Photo based on the scale factor
        Vector2 targetPhotoSize = originalPhotoSize * scaleFactor;

        // We will calculate the movement offset based on the difference between RawImage's initial and target position
        Vector2 rawImageMovementDelta = rawImageTargetPosition - originalRawImagePosition;

        float timeElapsed = 0;

        // Lerp the size and position of both RectTransforms to the target size and position
        while (timeElapsed < duration)
        {
            // Calculate the interpolation factor
            float t = timeElapsed / duration;

            rawImageCanvasGroup.alpha = t;

            // Smooth step interpolation for smoother scaling and movement
            Vector2 newRawImageSize = Vector2.Lerp(originalRawImageSize, targetRawImageSize, Mathf.SmoothStep(0, 1, t));
            Vector2 newPhotoSize = Vector2.Lerp(originalPhotoSize, targetPhotoSize, Mathf.SmoothStep(0, 1, t));

            Vector2 newRawImagePosition = Vector2.Lerp(originalRawImagePosition, rawImageTargetPosition, Mathf.SmoothStep(0, 1, t));

            // Calculate the movement delta of the RawImage so far
            Vector2 movementSoFar = newRawImagePosition - originalRawImagePosition;

            // Move the Photo by the same relative amount
            Vector2 newPhotoPosition = (originalPhotoPosition + movementSoFar) * scaleFactor;

            // Apply the new sizes and positions
            rawImageRectTransform.sizeDelta = newRawImageSize;
            photoRectTransform.sizeDelta = newPhotoSize;

            rawImageRectTransform.anchoredPosition = newRawImagePosition;
            photoRectTransform.anchoredPosition = newPhotoPosition;

            // Increment the time elapsed
            timeElapsed += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the final size and position are set to the target size and position
        rawImageRectTransform.sizeDelta = targetRawImageSize;
        photoRectTransform.sizeDelta = targetPhotoSize;

        rawImageRectTransform.anchoredPosition = rawImageTargetPosition;
        photoRectTransform.anchoredPosition = originalPhotoPosition + rawImageMovementDelta * scaleFactor;

        rawImageCanvasGroup.alpha = 1.0f;

        transitionCR = null;
    }

    IEnumerator TransitionFromMinigameCR(Vector2 rawImageTargetPosition, float scaleFactor)
    {
        if (currentMinigame.transitionDelay > 0)
        {
            yield return new WaitForSeconds(currentMinigame.transitionDelay);
        }
        // Transition from old image to new image
        var oldCanvasGroup = oldPhotoImage.GetComponent<CanvasGroup>();
        var newCanvasGroup = newPhotoImage.GetComponent<CanvasGroup>();

        oldCanvasGroup.alpha = 0.0f;
        newCanvasGroup.alpha = 1.0f;

        // Store the original sizes and positions of the RawImage and Photo
        Vector2 originalRawImageSize = rawImageRectTransform.sizeDelta;
        Vector2 originalPhotoSize = photoRectTransform.sizeDelta;
        Vector2 originalRawImagePosition = rawImageRectTransform.anchoredPosition;
        Vector2 originalPhotoPosition = photoRectTransform.anchoredPosition;
        float duration = transitionDuration;

        // Target size for the RawImage
        Vector2 targetRawImageSize = originalRawImageSize * scaleFactor;

        // Calculate the final size for the Photo based on the scale factor
        Vector2 targetPhotoSize = originalPhotoSize * scaleFactor;

        // We will calculate the movement offset based on the difference between RawImage's initial and target position
        Vector2 rawImageMovementDelta = rawImageTargetPosition - originalRawImagePosition;

        float timeElapsed = 0;

        // Lerp the size and position of both RectTransforms to the target size and position
        while (timeElapsed < duration)
        {
            // Calculate the interpolation factor
            float t = timeElapsed / duration;

            rawImageCanvasGroup.alpha = 1.0f - t;

            // Smooth step interpolation for smoother scaling and movement
            Vector2 newRawImageSize = Vector2.Lerp(originalRawImageSize, targetRawImageSize, Mathf.SmoothStep(0, 1, t));
            Vector2 newPhotoSize = Vector2.Lerp(originalPhotoSize, targetPhotoSize, Mathf.SmoothStep(0, 1, t));

            Vector2 newRawImagePosition = Vector2.Lerp(originalRawImagePosition, rawImageTargetPosition, Mathf.SmoothStep(0, 1, t));
            
            // Move the Photo by the same relative amount
            Vector2 newPhotoPosition = Vector2.Lerp(originalPhotoPosition, Vector2.zero, Mathf.SmoothStep(0, 1, t));

            // Apply the new sizes and positions
            rawImageRectTransform.sizeDelta = newRawImageSize;
            photoRectTransform.sizeDelta = newPhotoSize;

            rawImageRectTransform.anchoredPosition = newRawImagePosition;
            photoRectTransform.anchoredPosition = newPhotoPosition;

            // Increment the time elapsed
            timeElapsed += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the final size and position are set to the target size and position
        rawImageRectTransform.sizeDelta = targetRawImageSize;
        photoRectTransform.sizeDelta = targetPhotoSize;

        rawImageRectTransform.anchoredPosition = rawImageTargetPosition;
        photoRectTransform.anchoredPosition = Vector2.zero;

        rawImageCanvasGroup.alpha = 0.0f;

        var gameState = currentMinigame.gameState;
        Destroy(currentMinigame.gameObject);
        currentMinigame = null;

        transitionCR = null;

        // Now select next minigame
        if (gameState == Minigame.GameState.Loose)
        {
            lives--;
            lifeDisplay.lives = lives;
            if (lives <= 0)
            {
                // Go to recovery game, unless that was already the game
                if (recovery)
                {
                    // Final game over
                    transitionCR = StartCoroutine(GameOverCR());
                }
                else
                {
                    // Go to recovery minigame
                    GoToRecovery();
                }
            }
            else
            {
                SelectRandomMinigame();
            }
        }
        else if (gameState == Minigame.GameState.Win)
        {
            if (recovery)
            {
                lives = 1;
                lifeDisplay.lives = lives;
            }
            SelectRandomMinigame();
        }
    }

    IEnumerator GameOverCR()
    {
        // Transition from old image to new image
        oldPhotoImage.sprite = newPhotoImage.sprite;
        newPhotoImage.sprite = gameOverImage;

        var oldCanvasGroup = oldPhotoImage.GetComponent<CanvasGroup>();
        var newCanvasGroup = newPhotoImage.GetComponent<CanvasGroup>();

        rawImageCanvasGroup.alpha = 0.0f;
        float a = 0.0f;
        while (a < 1.0f)
        {
            oldCanvasGroup.alpha = 1.0f - a;
            newCanvasGroup.alpha = a;

            a += Time.deltaTime;

            yield return null;
        }

        oldCanvasGroup.alpha = 0.0f;
        newCanvasGroup.alpha = 1.0f;

        float s = 0.0f;
        while (s < 0.75f)
        {
            float ss = Mathf.SmoothStep(0, 1, s);
            gameOverText.localScale = new Vector3(ss, ss, ss);
            s += Time.deltaTime * 0.25f;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        while (a > 0.0f)
        {
            newCanvasGroup.alpha = a;

            a -= Time.deltaTime;

            yield return null;
        }

        newCanvasGroup.alpha = 1.0f;

        // Back to main menu

        transitionCR = null;
    }
}
