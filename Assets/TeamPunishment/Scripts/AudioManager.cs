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

        const string ON = "on";
        const string OFF = "off";

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (PlayerPrefs.GetInt("voiceOver", 1) == 0)
            {
                SFX1.mute = true;
                SFX2.mute = true;
            }
            if (PlayerPrefs.GetInt("music", 1) == 0)
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

        public void MuteVoiceOver(bool isOn)
        {
            Debug.Log($"voice is {(isOn ? OFF : ON)}");
            SFX1.mute = !isOn;
            SFX2.mute = !isOn;
        }

        public void MuteMusic(bool isOn)
        {
            Debug.Log($"music mute is {(isOn ? OFF : ON)}");
            Music.mute = !isOn;
        }
    }
}