using UnityEngine;
using OkapiKit;

public class Minigame : MonoBehaviour
{
    public enum GameState { WaitStart, Playing, Win, Loose, Terminate };

    [SerializeField] public  Sprite    image;
    [SerializeField] public  Vector2   mindPosition;
    [SerializeField] public  Vector2   mindSize;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip looseSound;
    [SerializeField] public  float     transitionDelay = 1.0f;

    protected GameState _gameState = GameState.WaitStart;

    public bool isPlaying => (_gameState == GameState.Playing);
    public GameState gameState => _gameState;

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
    }

    protected void StartPlaying()
    {
        _gameState = GameState.Playing;
    }

    protected void Win()
    {
        _gameState = GameState.Win;
        if (winSound) SoundManager.PlaySound(winSound, 1, 1);
        
    }

    protected void Loose()
    {
        _gameState = GameState.Loose;
        if (looseSound) SoundManager.PlaySound(looseSound, 1, 1);
    }

    public void Terminate()
    {
        _gameState = GameState.Terminate;
    }
}
