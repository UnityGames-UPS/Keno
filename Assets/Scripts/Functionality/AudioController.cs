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
    private void Start()
    {
        BgAudioSource.Play();
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
