using System;
using System.Collections;
using System.Collections.Generic;
using TeamPunishment;
using UnityEngine;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class ScoreOption : MonoBehaviour
    {
        [SerializeField] Image number0;
        [SerializeField] Image number;
        [SerializeField] Image number100;
        [SerializeField] Image bar4;
        [SerializeField] Image bar3;
        [SerializeField] Image bar2;
        [SerializeField] Image bar1;
        [SerializeField] Sprite num25;
        [SerializeField] Sprite num50;
        [SerializeField] Sprite num75;
        [SerializeField] Sprite barEnabled;
        [SerializeField] Sprite barDisabled;

        public void Init(int vote)
        {
            SetAll();
            if (vote == 0)
            {
                return;
            }
            if (vote > 0)
            {
                number0.gameObject.SetActive(false);
                number.gameObject.SetActive(true);
                number.sprite = num25;
                bar1.sprite = barEnabled;
            }
            if (vote > 1)
            {
                number.sprite = num50;
                bar2.sprite = barEnabled;
            }
            if (vote > 2)
            {
                number.sprite = num75;
                bar3.sprite = barEnabled;
            }
            if (vote > 3)
            {
                number.gameObject.SetActive(false);
                number100.gameObject.SetActive(true);
                bar4.sprite = barEnabled;
            }
        }

        private void SetAll()
        {
            number0.gameObject.SetActive(true);
            number.gameObject.SetActive(false);
            number100.gameObject.SetActive(false);
            bar1.sprite = barDisabled;
            bar2.sprite = barDisabled;
            bar3.sprite = barDisabled;
            bar4.sprite = barDisabled;
        }

    }
}