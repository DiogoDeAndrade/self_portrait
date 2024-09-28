using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum Mode { SimpleFeedbackLoop = 0, CameraTrap = 1, ExponentialDecay = 2 };

    [SerializeField] Mode mode = Mode.SimpleFeedbackLoop;
    [SerializeField] public Transform targetObject;
    [SerializeField] float followSpeed = 0.9f;
    [SerializeField] Rect rect = new Rect(-100.0f, -100.0f, 200.0f, 200.0f);
    [SerializeField] public BoxCollider2D cameraLimits;

    private new Camera camera;
    private Bounds allObjectsBound;
    private List<Transform> potentialTransforms = new();

    void Start()
    {
        camera = GetComponent<Camera>();

        if (mode == Mode.CameraTrap)
        {
            float currentZ = transform.position.z;
            Vector3 targetPos = GetTargetPos();
            transform.position = new Vector3(targetPos.x, targetPos.y, currentZ);

            CheckBounds();
        }
    }

    void FixedUpdate()
    {
        switch (mode)
        {
            case Mode.SimpleFeedbackLoop:
                FixedUpdate_SimpleFeedbackLoop();
                break;
            case Mode.CameraTrap:
                FixedUpdate_Box();
                break;
            case Mode.ExponentialDecay:
                FixedUpdate_ExponentialDecay();
                break;
        }
    }

    void FixedUpdate_SimpleFeedbackLoop()
    {
        float currentZ = transform.position.z;

        Vector3 err = GetTargetPos() - transform.position;

        Vector3 newPos = transform.position + err * followSpeed;
        newPos.z = currentZ;

        transform.position = newPos;

        CheckBounds();
    }
    void FixedUpdate_ExponentialDecay()
    {
        // Nice explanation of this: https://www.youtube.com/watch?v=LSNQuFEDOyQ&ab_channel=FreyaHolm%C3%A9r
        Vector3 targetPos = GetTargetPos();

        Vector3 newPos = targetPos + (transform.position - targetPos) * Mathf.Pow((1.0f - followSpeed), Time.fixedDeltaTime);
        newPos.z = transform.position.z;

        transform.position = newPos;

        CheckBounds();
    }

    void FixedUpdate_Box()
    {
        float currentZ = transform.position.z;
        Vector3 targetPos = GetTargetPos();
        Vector2 delta = transform.position;
        Rect r = rect;
        r.position += delta;

        if (targetPos.x > r.xMax) r.position += new Vector2(targetPos.x - r.xMax, 0);
        if (targetPos.x < r.xMin) r.position += new Vector2(targetPos.x - r.xMin, 0);
        if (targetPos.y < r.yMin) r.position += new Vector2(0, targetPos.y - r.yMin);
        if (targetPos.y > r.yMax) r.position += new Vector2(0, targetPos.y - r.yMax);

        transform.position = new Vector3(r.center.x, r.center.y, currentZ);

        CheckBounds();
    }

    void CheckBounds()
    {
        if (cameraLimits == null) return;

        Bounds r = cameraLimits.bounds;

        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;

        float xMin = transform.position.x - halfWidth;
        float xMax = transform.position.x + halfWidth;
        float yMin = transform.position.y - halfHeight;
        float yMax = transform.position.y + halfHeight;

        Vector3 position = transform.position;

        if (xMin <= r.min.x) position.x = r.min.x + halfWidth;
        else if (xMax >= r.max.x) position.x = r.max.x - halfWidth;
        if (yMin <= r.min.y) position.y = r.min.y + halfHeight;
        else if (yMax >= r.max.y) position.y = r.max.y - halfHeight;

        transform.position = position;
    }

    public Vector3 GetTargetPos()
    {
        if (targetObject != null) return targetObject.transform.position;

        return transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetTargetPos(), 0.5f);

        if (mode == Mode.CameraTrap)
        {
            Vector2 delta = transform.position;
            Rect r = rect;
            r.position += delta;

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin));
            Gizmos.DrawLine(new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax));
            Gizmos.DrawLine(new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax));
            Gizmos.DrawLine(new Vector2(r.xMin, r.yMax), new Vector2(r.xMin, r.yMin));
        }

        if (cameraLimits)
        {
            Bounds r = cameraLimits.bounds;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector2(r.min.x, r.min.y), new Vector2(r.max.x, r.min.y));
            Gizmos.DrawLine(new Vector2(r.max.x, r.min.y), new Vector2(r.max.x, r.max.y));
            Gizmos.DrawLine(new Vector2(r.max.x, r.max.y), new Vector2(r.min.x, r.max.y));
            Gizmos.DrawLine(new Vector2(r.min.x, r.max.y), new Vector2(r.min.x, r.min.y));
        }
    }
}
