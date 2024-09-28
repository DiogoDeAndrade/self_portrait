using TMPro;
using OkapiKit;
using UnityEngine;
using Mono.Cecil;

public class Teacher : Minigame
{
    [SerializeField, Header("Music")] private float              startDelay = 2.0f;
    [SerializeField] private TeacherPlayer      teacherPlayer;
    [SerializeField] private BoxCollider2D      cameraLimits;
    [SerializeField] private ParticleSystem     effectPS;

    MovementPlatformer teacherPlayerMovement;

    protected override void Start()
    {
        base.Start();

        if (minigameCamera)
        {
            var cameraFollow = minigameCamera.GetComponent<CameraFollow>();
            if (cameraFollow)
            {
                cameraFollow.targetObject = teacherPlayer.transform;
                cameraFollow.cameraLimits = cameraLimits;
            }
        }

        teacherPlayerMovement = teacherPlayer.GetComponent<OkapiKit.MovementPlatformer>();

        Invoke(nameof(StartPlaying), startDelay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isPlaying)
        {
            teacherPlayerMovement.enabled = false;
            return;
        }
        else
        {
            teacherPlayerMovement.enabled = true;
        }

        var students = FindObjectsByType<Student>(FindObjectsSortMode.None);
        bool allSmart = true;
        foreach (var student in students)
        {
            if (!student.smart)
            {
                allSmart = false;
                break;
            }
        }
        if (allSmart)
        {
            effectPS.Play();
            Win();
        }

        if (playTime > 1.0f)
        {
            DisablePrompt();
        }
    }
}
