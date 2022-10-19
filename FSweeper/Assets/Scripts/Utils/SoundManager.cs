using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZnoKunG.Utils
{
    public class SoundManager : MonoBehaviour
    {
        private Sound[] sounds;
        private GameObject soundGameObjectParent;
        public static SoundManager Instance;

        private Dictionary<Sound, float> soundTimerDictionary;
        private void Awake()
        {
            Instance = this;
            sounds = GameAssets.Instance.sounds;
            soundTimerDictionary = new Dictionary<Sound, float>();
            soundGameObjectParent = new GameObject("SoundGameObject");
            foreach (Sound sound in sounds)
            {
                GameObject soundGameObject = new GameObject(sound.name, typeof(AudioSource));
                DontDestroyOnLoad(soundGameObject);
                soundGameObject.transform.parent = soundGameObjectParent.transform;
                sound.audioSource = soundGameObject.GetComponent<AudioSource>();
                sound.audioSource.clip = sound.audioClip;

                sound.audioSource.volume = sound.volume;
                sound.audioSource.pitch = sound.pitch;
                sound.audioSource.loop = sound.loop;
                soundTimerDictionary[sound] = sound.cooldown;
            }
        }

        public void PlayOneShot(string name)
        {
            Sound sound = Array.Find(sounds, sound => sound.name == name);
            if (sound != null) {
                if (sound.hasCoolDown) {
                    PlaySoundPeriodic(sound);
                } else {
                    Play(sound);
                }
            } else {
                Debug.Log("Sound " + sound.name + " not found!");
            }
        }

        private void Play(Sound sound)
        {
            sound.audioSource.Play();
        }

        public void PlaySoundPeriodic(Sound sound)
        {
            soundTimerDictionary[sound] -= Time.deltaTime;
            if (soundTimerDictionary[sound] < 0)
            {
                Play(sound);
                soundTimerDictionary[sound] = sound.cooldown;
            }
        }
    }
}
