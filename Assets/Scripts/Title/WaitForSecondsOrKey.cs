using UnityEngine;

public class WaitForSecondsOrKey : CustomYieldInstruction
{
    private float waitTime;
    private float startTime;

    public WaitForSecondsOrKey(float time)
    {
        waitTime = time;
        startTime = Time.time;
    }

    // This property tells Unity when to stop yielding
    public override bool keepWaiting
    {
        get
        {
            // Return false if the wait time has passed or if any key has been pressed
            return (Time.time - startTime < waitTime) && !Input.anyKeyDown;
        }
    }
}
