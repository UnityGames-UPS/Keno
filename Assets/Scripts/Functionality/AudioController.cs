using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> AudioClips;
    [SerializeField] private AudioSource BgAudioSource;
    [SerializeField] private AudioSource MainAudioSource;
    [SerializeField] private AudioSource ButtonaudioSource;
    [SerializeField] private AudioSource KenoAudioSource;

    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();
    private bool isForceMuted = false;
    private List<AudioSource> AllSources => new List<AudioSource> { BgAudioSource, MainAudioSource, ButtonaudioSource, KenoAudioSource };

    private void Start()
    {
        BgAudioSource.Play();
    }

    private void OnApplicationFocus(bool focus)
    {
        SetMuteAll(!focus);
    }

    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;
        isForceMuted = forceMute;

        foreach (var source in AllSources)
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
            }
        }
    }

    internal void PlayMainAudio(int index)
    {
        if (index < 0 || index >= AudioClips.Count)
        {
            Debug.LogWarning("Audio index out of range: " + index);
            return;
        }
        MainAudioSource.Stop(); 
        MainAudioSource.clip = AudioClips[index];
        MainAudioSource.Play();
    }
    internal void PlayButtonAudio()
    {
        if (ButtonaudioSource != null && AudioClips.Count > 0)
        {
            ButtonaudioSource.clip = AudioClips[2];
            ButtonaudioSource.Play();
        }
    }

    internal void PlayKenoAudio(int index)
    {
        if (KenoAudioSource != null )
        {
            KenoAudioSource.clip = AudioClips[index];
            KenoAudioSource.Play();
        }
    }

    internal void StopMainAudio()
    {
        if (MainAudioSource.isPlaying)
        {
            MainAudioSource.Stop();
        }
    }
    internal void ToggleBgSound(bool isOn)
    {
        isForceMuted = false;
        if (isOn)
        {
            BgAudioSource.Play();
            BgAudioSource.mute = false;
        }
        else
        {
            BgAudioSource.mute = true;
        }
    }

    internal void ToggleMainSound(bool isOn)
    {
        isForceMuted = false;
        if (isOn)
        {

            MainAudioSource.mute = false;
            ButtonaudioSource.mute = false;
            KenoAudioSource.mute = false;
        }
        else
        {
            MainAudioSource.mute = true;
            ButtonaudioSource.mute = true;
            KenoAudioSource.mute = true;
        }
    }
}
