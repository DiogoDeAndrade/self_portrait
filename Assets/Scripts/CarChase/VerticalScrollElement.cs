using UnityEngine;

public class VerticalScrollElement : MonoBehaviour
{
    [SerializeField] private float  speed = 200.0f;
    [SerializeField] private bool   reuse = true;
    [SerializeField] private float  limitY = -192.0f;

    private Minigame minigame;

    void Start()
    {
        minigame = FindFirstObjectByType<Minigame>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        pos.y -= speed * minigame.timeScale * Time.deltaTime;
        if (pos.y <= limitY)
        {
            if (reuse)
            {
                pos.y += 384.0f + 128.0f;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        transform.position = pos;
    }
}
