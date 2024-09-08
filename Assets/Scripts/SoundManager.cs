using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;
    private static AudioSource audioSource;
    private static SoundLibrary soundLibrary;

    private void Awake()
    {
        // Ensure that this is the only instance
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            soundLibrary = GetComponent<SoundLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string soundName)
    {
        // Find a clip from our library and then if found, play it!
        AudioClip clip = soundLibrary.getRandomClip(soundName);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
