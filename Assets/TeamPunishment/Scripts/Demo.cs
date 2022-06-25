using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] Button button;
        void Start()
        {
            if (Application.isBatchMode)
            {
                Scenes.LoadStandartGame();
                return;
            }
            button.onClick.AddListener(onButton);
        }

        private void Update()
        {
            if (Input.anyKey)
            {
                onButton();
            }
        }

        private void onButton()
        {
            GameManager.instance.StopDemo();
            Scenes.LoadMenu();
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}