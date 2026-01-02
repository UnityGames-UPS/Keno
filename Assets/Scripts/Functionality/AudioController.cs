using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AudioClip> AudioClips;
    public AudioSource BgAudioSource;
    public AudioSource MainAudioSource;
    public AudioSource ButtonaudioSource;
    public AudioSource KenoAudioSource;
    void Start()
    {
        BgAudioSource.Play();
    }

    void Update()
    {

    }
    public void PlayMainAudio(int index)
    {
        if (index < 0 || index >= AudioClips.Count)
        {
            Debug.LogWarning("Audio index out of range: " + index);
            return;
        }
        MainAudioSource.Stop(); // Stop any currently playing audio
        MainAudioSource.clip = AudioClips[index];
        MainAudioSource.Play();
    }
    public void PlayButtonAudio()
    {
        if (ButtonaudioSource != null && AudioClips.Count > 0)
        {
            ButtonaudioSource.clip = AudioClips[0]; // Assuming the first clip is for button sounds
            ButtonaudioSource.Play();
        }
    }

    public void PlayKenoAudio(int index)
    {
        if (KenoAudioSource != null )
        {
            KenoAudioSource.clip = AudioClips[index]; // Assuming the second clip is for Keno sounds
            KenoAudioSource.Play();
        }
    }

    public void StopMainAudio()
    {
        if (MainAudioSource.isPlaying)
        {
            MainAudioSource.Stop();
        }
    }

    // public void ToggleAllSound(bool isOn)
    // {
    //     if (isOn)
    //     {
    //         if (!BgAudioSource.isPlaying && BgAudioSource.clip != null)
    //             BgAudioSource.Play();

    //         // Resume main audio if it had a clip
    //         if (MainAudioSource.clip != null && !MainAudioSource.isPlaying)
    //             MainAudioSource.Play();

    //         // Resume button audio if needed
    //         if (ButtonaudioSource.clip != null && !ButtonaudioSource.isPlaying)
    //             ButtonaudioSource.Play();
    //     }
    //     else
    //     {
    //         // Stop all sounds
    //         BgAudioSource.Stop();
    //         MainAudioSource.Stop();
    //         ButtonaudioSource.Stop();
    //     }
    // }
    public void ToggleBgSound(bool isOn)
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

    public void ToggleMainSound(bool isOn)
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
