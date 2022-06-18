using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class MobileCanvas : MonoBehaviour
    {
        public string localStarName;
        public TextboxElement textboxElement;

        public Transform StarFerrum;
        public Transform StarOrdo;
        public Text TextStarNone;

        public GameObject KickUIPanel;
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
            SetText();
            SetPlanets();
            KickUIPanel.SetActive(true);
        }

        private void SetText()
        {
            textboxElement.texts = new List<string>();
            textboxElement.texts.Add(@"The star in front of you is dealing with you in a long conflict. Several times he hit your convoy that brought the vaccines and destroyed a number of vaccines.");
            textboxElement.texts.Add(@"There will be two choices. One choice to bomb the planet (go to war) and another choice to avoid and continue to be moderate");
            textboxElement.voiceOvers = new List<AudioClip>();
            textboxElement.Init();
        }

        private void SetPlanets()
        {
            StarFerrum.GetComponent<OnStarClick>().Init(new List<int> { 200, -10, -20, -30, -50, -90 });
            StarOrdo.GetComponent<OnStarClick>().Init(new List<int> { 100, -10, -20, -25, -20, -25 });
        }
    }
}