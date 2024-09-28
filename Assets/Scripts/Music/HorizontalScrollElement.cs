using UnityEngine;

public class HorizontalScrollElement : MonoBehaviour
{
    [SerializeField] private float  speed = 200.0f;
    [SerializeField] private bool   reuse = true;
    [SerializeField] private float  limitX = -192.0f;

    private Minigame minigame;

    void Start()
    {
        minigame = FindFirstObjectByType<Minigame>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        pos.x -= speed * minigame.timeScale * Time.deltaTime;
        if (pos.x <= limitX)
        {
            if (reuse)
            {
                pos.x += 640.0f + 256.0f;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        transform.position = pos;
    }
}
