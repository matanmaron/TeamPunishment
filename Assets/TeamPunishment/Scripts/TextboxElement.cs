using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class TextboxElement : MonoBehaviour
    {
        [HideInInspector] public List<string> texts;
        [HideInInspector] public List<AudioClip> voiceOvers;
        Text textElement;
        Button button;
        int index = 0;

        private void Awake()
        {
            button = GetComponent<Button>();
            textElement = GetComponentInChildren<Text>();
        }

        void Start()
        {
            button.onClick.AddListener(OnButton);
            textElement.text = texts[index];
        }

        public void Init()
        {
            index = 0;
            if (textElement != null && texts != null)
            {
                textElement.text = texts[index];
            }
            if (voiceOvers.Count > index)
            {
                AudioManager.instance.PlayVoiceOver(voiceOvers[index]);
            }
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
            AudioManager.instance.StopVoiceOver();
        }

        private void OnButton()
        {
            if (texts.Count == 1)
                return;

            index++;
            if (index > texts.Count - 1)
            {
                index = 0;
            }
            textElement.text = texts[index];
            if (voiceOvers.Count > index)
            {
                AudioManager.instance.PlayVoiceOver(voiceOvers[index]);
            }
        }
    }
}