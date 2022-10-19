using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    public bool loop;
    [Tooltip("Cooldown of Sound in second")]
    public bool hasCoolDown;
    public float cooldown;

    [HideInInspector]
    public AudioSource audioSource;
}
