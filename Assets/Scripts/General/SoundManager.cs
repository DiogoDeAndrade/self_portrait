using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    [SerializeField] private AudioMixerGroup defaultMixerOutput;
    [SerializeField] private AudioMixerGroup musicMixerOutput;

    List<AudioSource> audioSources;
    AudioSource       musicSource;

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

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;           
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
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
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = defaultMixerOutput;

        audioSource.Play();
    }

    private void _FadeIn(AudioClip clip, float volume = 1.0f)
    {
        if (musicSource != null)
        {
            if (musicSource.clip == clip) return;

            StartCoroutine(FadeOutCR(musicSource, 0.5f));
        }
        if (clip)
        {
            musicSource = GetSource();

            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.pitch = 1.0f;
            musicSource.loop = true;
            musicSource.outputAudioMixerGroup = musicMixerOutput;

            musicSource.Play();
        }
    }

    private IEnumerator FadeOutCR(AudioSource source, float duration)
    {
        while (source.volume > 0.0f)
        {
            source.volume = Mathf.Clamp01(source.volume - Time.deltaTime / duration);
            yield return null;
        }

        source.Stop();
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

    static public void FadeIn(AudioClip clip, float volume = 1.0f)
    {
        instance._FadeIn(clip, volume);
    }
}