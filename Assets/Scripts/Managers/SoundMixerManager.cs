using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour //Sets sound volume based on sound settings
{
    public AudioMixer audioMixer;

    public void SetMasterVolume(float level) //Note: the code converts linear to log for decibels
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level)*20f);
    } 
    public void SetSoundFXVolume(float level)
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level)*20f);
    }
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level)*20f);
    }
}
