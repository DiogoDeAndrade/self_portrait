using TMPro;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField] private AudioClip eatSound;

    public int nFlies = 0;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var fly = collider.GetComponent<Fly>();
        if (fly)
        {
            if (eatSound) SoundManager.PlaySound(eatSound);
            Destroy(fly.gameObject);
            nFlies++;
        }
    }
}
