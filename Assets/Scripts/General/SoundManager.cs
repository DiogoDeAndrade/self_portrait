using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    [SerializeField] private AudioMixerGroup defaultMixerOutput;

    List<AudioSource> audioSources;

    public static SoundManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SoundManager>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Find all audio sources
        audioSources = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
        if (audioSources == null)
        {
            audioSources = new List<AudioSource>();
        }
    }

    private void _PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        var audioSource = GetSource();

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.outputAudioMixerGroup = defaultMixerOutput;

        audioSource.Play();
    }

    private AudioSource GetSource()
    {
        if (audioSources == null)
        {
            audioSources = new List<AudioSource>();
            return NewSource();
        }

        foreach (var source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return NewSource();
    }

    private AudioSource NewSource()
    {
        GameObject go = new GameObject();
        go.name = "Audio Source";
        go.transform.SetParent(transform);

        var audioSource = go.AddComponent<AudioSource>();

        audioSources.Add(audioSource);

        return audioSource;
    }

    static public void PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        instance._PlaySound(clip, volume, pitch);
    }
}