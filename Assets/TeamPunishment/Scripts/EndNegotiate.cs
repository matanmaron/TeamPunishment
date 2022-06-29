using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class EndNegotiate : MonoBehaviour
    {
        [SerializeField] Text endText;
        [SerializeField] Button button;
        [SerializeField] GameObject black;

        void Start()
        {
            if (Random.Range(0, 2) == 0)
            {
                endText.text = @"Both of you have chosen peace and decided to negotiate. 
The settlement will have an equal amount of vaccines to each planet, 
which means each of you  will lose 50% of his civilians. ";
            }
            else
            {
                endText.text = @"You chose to negotiate, 
while your opponent chose to fight - He demolished you and now you are dead ! ";
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