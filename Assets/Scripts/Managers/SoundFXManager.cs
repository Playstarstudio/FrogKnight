using UnityEngine;

public class SoundFXManager : MonoBehaviour //Singleton that controls all sound FX in the game
{
    public static SoundFXManager instance;
    public AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayFXClip(AudioClip audioClip, Transform spawntransform, float volume) //Plays the incoming sound clip at the set volume
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity); //Spawn in game object to host the audio source
        audioSource.clip = audioClip; //Assign the audioclip
        audioSource.volume = volume; //Ensure volume is set appropriately
        audioSource.Play(); //Play sound
        float clipLength = audioSource.clip.length; //Get length of sound clip
        Destroy(audioSource.gameObject, clipLength); //Destroy the sound clip
    }

    public void PlayRandomFXClip(AudioClip[] audioClip, Transform spawntransform, float volume, string tag) //Plays a random sound clip from an array at the set volume
    {
        int rand = Random.Range(0, audioClip.Length);//Get random index
        AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity); //Spawn in game object to host the audio source
        audioSource.clip = audioClip[rand]; //Assign the audioclip
        audioSource.volume = volume; //Ensure volume is set appropriately
        audioSource.Play(); //Play sound
        float clipLength = audioSource.clip.length; //Get length of sound clip
        Destroy(audioSource.gameObject, clipLength); //Destroy the sound clip
    }
}
