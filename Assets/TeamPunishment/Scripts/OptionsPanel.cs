using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class OptionsPanel : MonoBehaviour
    {
        [SerializeField] OptionButton voiceOver;
        [SerializeField] OptionButton music;
        [SerializeField] OptionButton logRecords;

        void Start()
        {
            if (GameManager.instance.isDemoMode)
            {
                PlayerPrefs.SetInt("voiceOver", 1);
                PlayerPrefs.SetInt("music", 1);
                PlayerPrefs.SetInt("logRecords", 1);
            }
            voiceOver.SetStartState(PlayerPrefs.GetInt("voiceOver", 1));
            music.SetStartState(PlayerPrefs.GetInt("music", 1));
            logRecords.SetStartState(PlayerPrefs.GetInt("logRecords", 1));
            voiceOver.OnStateChanged += OnVoiceOver;
            music.OnStateChanged += OnMusic;
            logRecords.OnStateChanged += OnLogRecords;
        }

        private void OnVoiceOver(bool state)
        {
            PlayerPrefs.SetInt("voiceOver", state ? 1 : 0);
            AudioManager.instance.MuteVoiceOver(state);
        }

        private void OnMusic(bool state)
        {
            PlayerPrefs.SetInt("music", state ? 1 : 0);
            AudioManager.instance.MuteMusic(state);
        }

        private void OnLogRecords(bool state)
        {
            PlayerPrefs.SetInt("logRecords", state ? 1 : 0);
            GameManager.instance.MuteLogRecords(state);
        }

    }
}