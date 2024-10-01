using NaughtyAttributes;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Fly : MonoBehaviour
{
    [SerializeField] private float speed = 200.0f;
    [SerializeField, MinMaxSlider(0.1f, 4.0f)]
    private Vector2 pauseTime = new Vector2(2.0f, 3.0f);

    public BoxCollider2D    bounds;
    public Tilemap          tilemap;

    private float   waitTimer = 0.0f;
    private Vector3 waypoint;

    void Start()
    {
        waitTimer = 0.0f;
        waypoint = transform.position;
        transform.localScale = Vector3.zero;

        StartCoroutine(GrowInCR());
    }

    IEnumerator GrowInCR()
    {
        float s = 0.0f;

        while (s < 1.0f)
        {
            float ss = Mathf.SmoothStep(0, 1, s);
            transform.localScale = new Vector3(ss, ss, ss);
            s += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (waitTimer > 0.0f)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0.0f)
            {
                SelectNextWaypoint();
            }
        }
        else
        {
            Vector3 delta = waypoint - transform.position;

            transform.position = Vector3.MoveTowards(transform.position, waypoint, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, waypoint) < 1.0f)
            {
                waitTimer = Random.Range(pauseTime.x, pauseTime.y);
            }

            if ((delta.x < 0) && (transform.right.x > 0)) transform.rotation = Quaternion.Euler(0, 180, 0);
            else if ((delta.x > 0) && (transform.right.x < 0)) transform.rotation = Quaternion.identity;
        }
    }

    void SelectNextWaypoint()
    {
        int tries = 0;
        while (tries < 5)
        {
            var b = bounds.bounds;
            waypoint = Vector3.zero;
            waypoint.x = Random.Range(b.min.x, b.max.x);
            waypoint.y = Random.Range(b.min.y, b.max.y);
            waypoint.z = transform.position.z;

            // Check if this point is on top of tilemap
            var tilePos = tilemap.WorldToCell(waypoint);
            var tile = tilemap.GetTile(tilePos);
            if (tile == null)
            {
                // Valid pos, done
                return;
            }

            tries++;
        }

        waypoint = transform.position;
    }
}
