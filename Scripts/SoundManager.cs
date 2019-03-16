using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] GameObject[] areas;
    [SerializeField] AudioClip[] musicClips;
    Dictionary<AudioSource, float> timeToKillAudioSource;

    GameObject currentArea = null;

    private void Awake()
    {
        timeToKillAudioSource = new Dictionary<AudioSource, float>();
    }

    private void Update()
    {
        List<AudioSource> toKill = new List<AudioSource>();
        foreach (AudioSource source in timeToKillAudioSource.Keys)
        {
            if (Time.time > timeToKillAudioSource[source])
            {
                toKill.Add(source);
            }
        }
        foreach (AudioSource source in toKill)
        {
            timeToKillAudioSource.Remove(source);
            Destroy(source);
        }
    }

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = loop;
        source.dopplerLevel = 0.0f;
        timeToKillAudioSource[source] = Time.time + clip.length * (loop ? 100.0f : 1.0f);
        source.Play();
    }

    public void PlayMusic(GameObject area)
    {
        if (currentArea == area)
        {
            return;
        }
        currentArea = area;
        // Find the appropriate clip
        AudioClip clip = null;
        for (int i = 0; i < areas.Length; i++)
        {
            if (areas[i] == area)
            {
                clip = musicClips[i];
            }
        }

        // Stop other music
        StopMusic();

        // Play music
        PlaySound(clip, true);
    }

    public void StopMusic()
    {
        AudioSource musicSource = null;
        foreach (AudioSource source in timeToKillAudioSource.Keys)
        {
            foreach (AudioClip clip in musicClips)
            {
                if (source.clip == clip)
                {
                    musicSource = source;
                }
            }
        }
        if (musicSource != null)
        {
            timeToKillAudioSource.Remove(musicSource);
            Destroy(musicSource);
        }
    }
}
