using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class ScoresPanel : MonoBehaviour
    {
        [SerializeField] ScoreOption OptionNone;
        [SerializeField] ScoreOption OptionFerru;
        [SerializeField] ScoreOption OptionCibus;
        [SerializeField] ScoreOption OptionOrdo;
        [SerializeField] ScoreOption OptionArtem;

        Image winnerImage = null;

        public void Init(Dictionary<Stars, float> votes, Stars winner)
        {
            Debug.Log($"winner is {winner}");
            OptionNone.Init(Mathf.RoundToInt(votes[Stars.None]));
            OptionFerru.Init(Mathf.RoundToInt(votes[Stars.Ferrum]));
            OptionCibus.Init(Mathf.RoundToInt(votes[Stars.Cibus]));
            OptionOrdo.Init(Mathf.RoundToInt(votes[Stars.Ordo]));
            OptionArtem.Init(Mathf.RoundToInt(votes[Stars.Artem]));
            FlashWinner(winner);
        }

        private void FlashWinner(Stars winner)
        {
            switch (winner)
            {
                case Stars.None:
                    SelectWinner(OptionNone.GetImage());
                    break;
                case Stars.Ferrum:
                    SelectWinner(OptionFerru.GetImage());
                    break;
                case Stars.Cibus:
                    SelectWinner(OptionCibus.GetImage());
                    break;
                case Stars.Ordo:
                    SelectWinner(OptionOrdo.GetImage());
                    break;
                case Stars.Artem:
                    SelectWinner(OptionArtem.GetImage());
                    break;
                default:
                    Debug.LogError("no image!");
                    break;
            }
        }

        private void Update()
        {
            if (winnerImage != null)
            {
                winnerImage.color = new Color(winnerImage.color.r, winnerImage.color.g, winnerImage.color.b, Mathf.PingPong(Time.time, 1));
            }
        }

        private void SelectWinner(Image img)
        {
            Debug.Log(img.sprite.name);
            winnerImage = img;
            winnerImage.color = Color.yellow;
        }

        private void OnDisable()
        {
            winnerImage.color = Color.white;
            winnerImage.color = new Color(winnerImage.color.r, winnerImage.color.g, winnerImage.color.b, 1f);
            winnerImage = null;
        }
    }
}
