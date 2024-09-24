using UnityEngine;
using OkapiKit;

public class Minigame : MonoBehaviour
{
    protected bool isPlaying = false;

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

        isPlaying = true;
    }

    protected void Win()
    {
        isPlaying = false;
    }

    protected void Loose()
    {
        isPlaying = false;
    }
}
