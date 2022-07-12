using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioSource Music;
        [SerializeField] AudioSource SFX1;
        [SerializeField] AudioSource SFX2;

        [SerializeField] List<AudioClip> StarsExsplosion;
        int currentExsplosion = 0;

        public static AudioManager instance;

        void Awake()
        {
            instance = this;
            Music.Play();
            Music.Pause();
        }


        private void Start()
        {
            if (GameManager.instance.IsMuteVoice)
            {
                SFX2.mute = true;
            }
            if (GameManager.instance.IsMuteMusic)
            {
                Music.mute = true;
            }
            SFX1.volume = GameManager.instance.VolumeMargin;
            SFX2.volume = GameManager.instance.VolumeMargin;
        }

        public void PlayMusic()
        {
            Debug.Log("[AudioManager] - PlayMusic");
            Music.UnPause();
        }

        public void StopMusic()
        {
            Debug.Log("[AudioManager] - StopMusic");
            Music.Pause();
        }

        private void PlaySFX(AudioClip clip, AudioSource sfxChannel)
        {
            sfxChannel.clip = clip;
            sfxChannel.Play();
        }

        public void PlayStarsExsplosion()
        {
            PlaySFX(StarsExsplosion[currentExsplosion], SFX1);
            currentExsplosion++;
            if (currentExsplosion > StarsExsplosion.Count - 1)
            {
                currentExsplosion = 0;
            }
        }

        public void PlayVoiceOver(AudioClip clip)
        {
            PlaySFX(clip, SFX2);
        }

        public void StopVoiceOver()
        {
            try
            {
                SFX2?.Stop();
            }
            catch(Exception ex)
            {

            }
        }

        public void MuteVoiceOver(bool state)
        {
            SFX2.mute = !state;
        }

        public void MuteMusic(bool state)
        {
            Music.mute = !state;
        }

        public void SetVolumeMargin(float margin)
        {
            SFX1.volume = margin;
            SFX2.volume = margin;
        }
    }
}