using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Camera     _minigameCamera;
    [SerializeField] private bool       autoStart;
    [SerializeField] private Minigame[] minigamePrefabs;

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
    }
}
