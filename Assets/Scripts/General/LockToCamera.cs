using UnityEngine;

public class LockToCamera : MonoBehaviour
{
    Transform   minigameCameraTransform;
    Vector3     cameraBasePos;
    Vector3     objBasePos;

    void Start()
    {
        var minigameCamera = MinigameManager.minigameCamera;
        if (minigameCamera == null)
        {
            // Find it by name (just used for testing)
            minigameCamera = GameObject.Find("MinigameCamera").GetComponent<Camera>();
        }

        minigameCameraTransform = minigameCamera.transform;

        cameraBasePos = minigameCameraTransform.position;
        objBasePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = minigameCameraTransform.position - cameraBasePos;

        transform.position = objBasePos + delta;
    }
}
