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
            if (GameManager.instance.IsMuteVoice)
            {
                voiceOver.SetStartState(false);
            }
            if (GameManager.instance.IsMuteMusic)
            {
                music.SetStartState(false);
            }
            if (GameManager.instance.IsMuteLogs)
            {
                logRecords.SetStartState(false);
            }
            voiceOver.OnStateChanged += OnVoiceOver;
            music.OnStateChanged += OnMusic;
            logRecords.OnStateChanged += OnLogRecords;
        }

        private void OnVoiceOver(bool state)
        {
            GameManager.instance.IsMuteVoice = !state;
            AudioManager.instance.MuteVoiceOver(state);
        }

        private void OnMusic(bool state)
        {
            GameManager.instance.IsMuteMusic = !state;
            AudioManager.instance.MuteMusic(state);
        }

        private void OnLogRecords(bool state)
        {
            GameManager.instance.MuteLogRecords(state);
        }

    }
}