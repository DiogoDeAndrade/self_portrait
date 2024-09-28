using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private bool    onlySpawnWhilePlaying;
    [SerializeField, MinMaxSlider(0.1f, 3.0f)]
    private Vector2 spawnTimer = new Vector2(1.0f, 2.0f);
    [SerializeField, MinMaxSlider(0.1f, 3.0f)]
    private Vector2 scaleRange = Vector2.one;
    [SerializeField, MinMaxSlider(-360.0f, 360.0f)]
    private Vector2 rotationRange = Vector2.zero;
    [SerializeField] 
    private GameObject[] prefabs;

    private Minigame        minigame;
    private BoxCollider2D   spawnArea;
    private float           timer;
    private List<Vector3>   spawnPos;

    void Start()
    {
        minigame = FindFirstObjectByType<Minigame>();
        spawnArea = GetComponent<BoxCollider2D>();
        timer = Random.Range(spawnTimer.x, spawnTimer.y);

        if (spawnArea == null)
        {
            spawnPos = new List<Vector3>();
            foreach (Transform t in transform)
            {
                spawnPos.Add(t.position);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (onlySpawnWhilePlaying)
        {
            if (!MinigameManager.isPlaying) return;
        }

        if (minigame.timeScale == 0.0f) return;

        timer -= Time.deltaTime * minigame.timeScale;
        if (timer <= 0.0f)
        {
            timer = Random.Range(spawnTimer.x, spawnTimer.y);

            int r = Random.Range(0, prefabs.Length);
            var prefab = prefabs[r];

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            float rotation = Random.Range(rotationRange.x, rotationRange.y);

            Vector3 position = GetSpawnPosition();
            position.z = -scale * 0.25f;

            GameObject newObject = Instantiate(prefab, transform.parent);
            newObject.transform.position = position;
            newObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation);
            newObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    Vector3 GetSpawnPosition()
    {
        if (spawnArea)
        {
            Bounds bounds = spawnArea.bounds;
            Vector3 position = Vector3.zero;
            position.x = Random.Range(bounds.min.x, bounds.max.x);
            position.y = Random.Range(bounds.min.y, bounds.max.y);

            return position;
        }
        else
        {
            int r = Random.Range(0, spawnPos.Count);
            return spawnPos[r];
        }
    }
}
