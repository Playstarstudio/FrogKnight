using UnityEngine;

public class SoundFXManager : MonoBehaviour
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

    public void PlayFXClip(AudioClip audioClip, Transform spawntransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity); //spawn in game object to host the audio source
        audioSource.clip = audioClip; //assign the audioclip
        audioSource.volume = volume; //ensure volume is set appropriately
        audioSource.Play(); //play sound
        float clipLength = audioSource.clip.length; //get length of sound clip
        Destroy(audioSource.gameObject, clipLength); //destroy the sound clip
    }

    public void PlayRandomFXClip(AudioClip[] audioClip, Transform spawntransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);//get random index
        AudioSource audioSource = Instantiate(soundFXObject, spawntransform.position, Quaternion.identity); //spawn in game object to host the audio source
        audioSource.clip = audioClip[rand]; //assign the audioclip
        audioSource.volume = volume; //ensure volume is set appropriately
        audioSource.Play(); //play sound
        float clipLength = audioSource.clip.length; //get length of sound clip
        Destroy(audioSource.gameObject, clipLength); //destroy the sound clip
    }
}
