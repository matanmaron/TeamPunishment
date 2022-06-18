using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class KickedOut : MonoBehaviour
    {
        [SerializeField] Button button;
        void Start()
        {
            button.onClick.AddListener(onButton);
        }

        private void onButton()
        {
            Scenes.LoadMenu();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}