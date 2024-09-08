using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    // Library of sound effects
    private Dictionary<string, List<AudioClip>> soundDictionary;
    // For use in the inspector
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;

    private void Awake()
    {
        IntializeDictionary();
    }

    private void IntializeDictionary()
    {
        // Create a dictionary from our list of sound effects
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup group in soundEffectGroups)
        {
            soundDictionary[group.name] = group.clips;
        }
    }

    public AudioClip getRandomClip(string name)
    {
        if (soundDictionary.ContainsKey(name))
        {
            List<AudioClip> clips = soundDictionary[name];
            if (clips.Count > 0)
            {
                return clips[UnityEngine.Random.Range(0, clips.Count)];
            }
        }
        return null;
    }

    [System.Serializable]
    public struct SoundEffectGroup
    {
        public string name;
        public List<AudioClip> clips;
    }
}