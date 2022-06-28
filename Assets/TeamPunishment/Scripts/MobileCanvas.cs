using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class MobileCanvas : MonoBehaviour
    {
        public string localStarName;
        public TextboxElement textboxElement;

        public Button StarFerrum;
        public Button StarOrdo;
        public Text TextStarNone;

        public GameObject KickUIPanel;
        public GameObject Black;

        void Start()
        {
            localStarName = Stars.Ordo.ToString();
            VideoManager.instance.PlayIntro(OnIntroEnd);
            
        }

        private void OnIntroEnd()
        {
            VideoManager.instance.PlayOrdo(OnOrdoEnd);
        }

        private void OnOrdoEnd()
        {
            StartGame();
        }

        private void StartGame()
        {
            GameManager.instance.SendAnalyticsEvent("mobile-start");
            SetText();
            SetPlanets();
            KickUIPanel.SetActive(true);
        }

        private void SetText()
        {
            textboxElement.texts = new List<string>();
            textboxElement.texts.Add(@"The star in front of you is dealing with you in a long conflict. Several times he hit your convoy that brought the vaccines and destroyed a number of vaccines.");
            textboxElement.texts.Add(@"There will be two choices. One choice to bomb the planet (go to war) and another choice to avoid and continue to be moderate");
            textboxElement.texts.Add(@"* If both planets choose to fight, The vehicles will be destroyed and both will lose 75% of their population");
            textboxElement.texts.Add(@"* If only one planet chooses to fight, this planet will possess all the vaccines, while the other one will lose all of its citizens");
            textboxElement.texts.Add(@"* If both planets choose to negotiate, the vaccine amount will split equally and each of them will lose 50% of their population");
            textboxElement.voiceOvers = new List<AudioClip>();
            textboxElement.Init();
        }

        private void SetPlanets()
        {
            StarFerrum.onClick.AddListener(OnAttack);
            StarOrdo.onClick.AddListener(OnNegotiate);
        }

        private void OnAttack()
        {
            GameManager.instance.SendAnalyticsEvent("mobile-attack");
            Scenes.LoadMobileGame();
        }

        private void OnNegotiate()
        {
            Black.SetActive(true);
            GameManager.instance.SendAnalyticsEvent("mobile-negotiate");
            VideoManager.instance.PlayEnd(() => Scenes.LoadMenu());
        }

        private void OnDestroy()
        {
            StarFerrum.onClick.RemoveAllListeners();
            StarOrdo.onClick.RemoveAllListeners();
        }
    }
}