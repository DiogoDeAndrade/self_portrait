using TMPro;
using UnityEngine;

public class Hearbeat : Minigame
{
    [SerializeField] private float              startDelay = 2.0f;
    [SerializeField] private Vector2Int         validRange = new Vector2Int(60, 90);
    [SerializeField] private TextMeshProUGUI    bpmText;
    [SerializeField] private ParticleSystem     heartPS;

    HeartTrail  ht;
    float[]     heartbeats;
    int         heartbeatsIndex;
    int         totalBeats = 0;
    float       timeToLoose = 0.0f;
    int         avgBpm = 0;

    protected override void Start()
    {
        base.Start();

        heartbeats = new float[8];
        heartbeatsIndex = 0;
        ht = GetComponentInChildren<HeartTrail>();

        Invoke(nameof(StartPlaying), startDelay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isPlaying)
        {
            if (avgBpm == 0)
            {
                bpmText.text = "";
            }
            return;
        }

        if (Input.GetButtonDown("Jump"))
        {
            ht.Pump();

            heartbeats[heartbeatsIndex] = Time.time;
            heartbeatsIndex = (heartbeatsIndex + 1) % heartbeats.Length;
            totalBeats++;
        }

        if (totalBeats > 2)
        {
            DisablePrompt();
        }

        if (totalBeats > 5)
        {
            // Get time of last heartbeat
            int i1 = GetPrevHeartbeatIndex(-1);
            float prevBeat = heartbeats[i1];

            if ((Time.time - prevBeat) > 2.0f)
            {
                bpmText.text = $"BPM: ---";
            }
            else
            {
                int i2 = GetPrevHeartbeatIndex(-2);

                float timeBetweenHeartbeats = prevBeat - heartbeats[i2];
                int bpm = ConvertTimeToBPM(timeBetweenHeartbeats);

                float avg = 0.0f;
                for (int i = 1; i < 5; i++)
                {
                    float tth = heartbeats[GetPrevHeartbeatIndex(-i)] - heartbeats[GetPrevHeartbeatIndex(-i-1)];
                    avg += tth;
                }
                avg /= 4.0f;
                
                avgBpm = ConvertTimeToBPM(avg);

                if (IsInRange(avgBpm))
                {
                    // Win!
                    Win();
                    heartPS.Play();
                }
                else
                {
                    timeToLoose += Time.deltaTime;
                }

                bpmText.text = $"BPM: {avgBpm}";
            }
        }
        else
        {
            timeToLoose += Time.deltaTime;
            bpmText.text = $"BPM: ---";
        }
        
        if (timeToLoose > 8.0f)
        {
            // Loose
            Loose();
        }
    }

    int GetPrevHeartbeatIndex(int delta)
    {
        int index = heartbeatsIndex;
        index += delta;
        while (index < 0) index += heartbeats.Length;
        while (index > heartbeats.Length) index -= heartbeats.Length;

        return index;
    }
    int ConvertTimeToBPM(float t)
    {
        return (int)(60.0f / t);
    }

    bool IsInRange(int bpm)
    {
        return (bpm >= validRange.x) && (bpm <= validRange.y);
    }
}
