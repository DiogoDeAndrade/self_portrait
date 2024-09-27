using System.Collections;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Camera     _minigameCamera;
    [SerializeField] private bool       autoStart;
    [SerializeField] private Minigame[] minigamePrefabs;

    [SerializeField] private CanvasGroup    rawImageCanvasGroup; 
    [SerializeField] private RectTransform  photoRectTransform; 
    [SerializeField] private float          transitionDuration = 2f; 

    private RectTransform rawImageRectTransform; 

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
        }
    }

    public void SelectRandomMinigame()
    {
        int r = Random.Range(0, minigamePrefabs.Length);
        var prefab = minigamePrefabs[r];
        
        Minigame mg = Instantiate(prefab);

        TransitionToMinigame(mg);
    }

    void TransitionToMinigame(Minigame mg)
    {
        // Calculate the scale factor based on the initial size of the RawImage and the target size
        float scaleFactor = 1280.0f / rawImageRectTransform.sizeDelta.x;

        StartCoroutine(TransitionToMinigameCR(Vector2.zero, scaleFactor));
    }

    IEnumerator TransitionToMinigameCR(Vector2 rawImageTargetPosition, float scaleFactor)
    {
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

        rawImageCanvasGroup.alpha = 0.0f;

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
    }
}
