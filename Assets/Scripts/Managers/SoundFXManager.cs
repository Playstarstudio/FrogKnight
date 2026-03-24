using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour //Singleton that controls all sound FX in the game
{
    public static SoundFXManager instance;
    public AudioSource soundFXObject;

    [Header("List Enums")]
    [HideInInspector] public int footstepList = 5;
    [HideInInspector] public int impactList = 5;
    [HideInInspector] public int deathList = 5;

    public enum SoundType
    {
        Generic,
        Footstep,
        Impact,
        Death
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void TriggerFXClip(AudioClip[] audioClip, Transform spawntransform, float volume, SoundType tag) //Plays a random sound clip from an array at the set volume
    {
        switch(tag){ //Switches between the different sound cases
            case SoundType.Generic:
                AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity); //Spawn in game object to host the audio source
                audioSource.clip = audioClip[Random.Range(0, audioClip.Length)]; //Assign the audioclip
                audioSource.volume = volume; //Ensure volume is set appropriately
                audioSource.Play(); //Play sound
                Destroy(audioSource.gameObject, audioSource.clip.length); //Destroy the sound clip after it is done playing
                break;

            case SoundType.Footstep:
                PlayFXClip(audioClip, spawntransform, volume, footstepList);
                break;

            case SoundType.Impact:
                PlayFXClip(audioClip, spawntransform, volume, impactList);
                break;

            case SoundType.Death:
                PlayFXClip(audioClip, spawntransform, volume, deathList);
                break;

            default:
                Debug.LogError("Received an unknown sound type: " + tag);
                break;
        }
    }

    public void PlayFXClip(AudioClip[] audioClip, Transform spawntransform, float volume, int list) //Plays the incoming sound clip at the set volume
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity); //Spawn in game object to host the audio source
        list--;
        audioSource.clip = audioClip[Random.Range(0, audioClip.Length)]; //Assign the audioclip
        audioSource.volume = volume; //Ensure volume is set appropriately
        audioSource.Play(); //Play sound
        Destroy(audioSource.gameObject, audioSource.clip.length); //Destroy the sound clip after it is done playing
        list++;
        Mathf.Clamp(list, 0, 5);
    }
}
