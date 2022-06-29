using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class EndAttack : MonoBehaviour
    {
        [SerializeField] Text endText;
        [SerializeField] Button button;
        [SerializeField] GameObject black;

        void Start()
        {
            if (Random.Range(0, 2) == 0)
            {
                endText.text = @"Both of you have chosen to eliminate and you have started a war. 
As a result 75% of your civilians are dead!";
            }
            else
            {
                endText.text = @"You chose to fight the other planet, 
while he was ready to negotiate. 
You have destroyed all of his resources.";
            }
            button.onClick.AddListener(onButton);
        }

        private void onButton()
        {
            black.SetActive(true);
            VideoManager.instance.PlayEnd(() => Scenes.LoadMenu());
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}