using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamPunishment
{
    public class ScoresPanel : MonoBehaviour
    {
        [SerializeField] ScoreOption OptionNone;
        [SerializeField] ScoreOption OptionFerru;
        [SerializeField] ScoreOption OptionCibus;
        [SerializeField] ScoreOption OptionOrdo;
        [SerializeField] ScoreOption OptionArtem;

        public void Init(Dictionary<Stars, float> votes, int winner)
        {
            OptionNone.Init(Mathf.RoundToInt(votes[Stars.None]));
            OptionFerru.Init(Mathf.RoundToInt(votes[Stars.Ferrum]));
            OptionCibus.Init(Mathf.RoundToInt(votes[Stars.Cibus]));
            OptionOrdo.Init(Mathf.RoundToInt(votes[Stars.Ordo]));
            OptionArtem.Init(Mathf.RoundToInt(votes[Stars.Artem]));
        }
    }
}
