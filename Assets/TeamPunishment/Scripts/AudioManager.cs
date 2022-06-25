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
        }

        private void Start()
        {
            if (GameManager.instance.IsMuteVoice)
            {
                SFX1.mute = true;
                SFX2.mute = true;
            }
            if (GameManager.instance.IsMuteMusic)
                Music.mute = true;
            Music.Play();
            Music.Pause();
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
            SFX2?.Stop();
        }

        public void MuteVoiceOver(bool state)
        {
            SFX1.mute = !state;
            SFX2.mute = !state;
        }

        public void MuteMusic(bool state)
        {
            Music.mute = !state;
        }
    }
}