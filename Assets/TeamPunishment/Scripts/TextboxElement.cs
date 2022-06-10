using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class TextboxElement : MonoBehaviour
    {
        [HideInInspector] public List<string> texts;
        Text textElement;
        Button button;
        int index = 0;

        void Start()
        {
            button = GetComponent<Button>();
            textElement = GetComponentInChildren<Text>();
            button.onClick.AddListener(OnButton);
            textElement.text = texts[index];
        }

        public void Init()
        {
            index = 0;
            textElement.text = texts[index];
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        private void OnButton()
        {
            index++;
            if (index > texts.Count - 1)
            {
                index = 0;
            }
            textElement.text = texts[index];
        }
    }
}