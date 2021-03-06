using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

namespace TeamPunishment
{
    public class OnStarClick : MonoBehaviour
    {
        [SerializeField] ButtonClickedEvent OnButtonClickEnd;
        [SerializeField] List<Sprite> planetStates = new List<Sprite>();
        List<int> residents = new List<int>();
        [SerializeField] Text info;
        [SerializeField] string TextForInfo;
        [SerializeField] MoveUp deathTextPrefab;
        [SerializeField] Transform DeathPosition;
        [SerializeField] GameObject popup;
        [SerializeField] Button popupOK;
        [SerializeField] Text popupText;
        [SerializeField] string popupTextString;
        [SerializeField] Stars planetToHurt;
        bool hurt = true;
        Image planetImage;
        int counter = 0;
        int currentResidents = 0;
        bool canClick;
        bool firstClick;

        private void Awake()
        {
            planetImage = GetComponent<Image>();
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(OnStar);
        }

        public void Init(List<int> _residents)
        {
            firstClick = true;
            residents = _residents;
            canClick = true;
            counter = 0;
            currentResidents = _residents[counter];
            info.text = TextForInfo.Replace("XX", _residents[counter].ToString());
            if (planetImage == null)
            {
                return;
            }
            planetImage.sprite = planetStates[counter];
            planetImage.SetNativeSize();
        }

        private void OnStar()
        {
            if (!canClick)
            {
                return;
            }
            StartCoroutine(clickTimer());
            if (firstClick)
            {
                popupOK.onClick.RemoveAllListeners();
                popupText.text = popupTextString;
                popupOK.onClick.AddListener(() =>
                {
                    firstClick  = false;
                    TakeDown();
                });
                popup.SetActive(true);
                return;
            }
            TakeDown();
        }

        public void TakeDown(bool fromPlanet = false)
        {
            if (!fromPlanet)
            {
                if (hurt)
                {
                    ChatUI.instance.HurtPlanet(planetToHurt);
                }
                hurt = !hurt;
                AudioManager.instance.PlayStarsExsplosion();
            }
            counter++;
            if (counter == 6)
            {
                OnButtonClickEnd?.Invoke();
                return;
            }
            planetImage.sprite = planetStates[counter];
            planetImage.SetNativeSize();
            currentResidents += residents[counter];
            info.text = TextForInfo.Replace("XX", currentResidents.ToString());
            MoveUp go = Instantiate(deathTextPrefab, DeathPosition);
            go.Init(residents[counter]);

        }

        IEnumerator clickTimer()
        {
            canClick = false;
            yield return new WaitForSeconds(0.5f);
            canClick = true;
        }
    }
}