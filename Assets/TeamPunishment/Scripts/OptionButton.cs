using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image indicator;
    [SerializeField] Image state;

    [SerializeField] Sprite StateOn;
    [SerializeField] Sprite StateOff;
    [SerializeField] Sprite IndicatorOn;
    [SerializeField] Sprite IndicatorOff;

    private bool currentState = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeState();
    }

    public void ChangeState()
    {
        currentState = !currentState;
        if (currentState)
        {
            indicator.sprite = IndicatorOn;
            state.sprite = StateOn;
        }
        else
        {
            indicator.sprite = IndicatorOff;
            state.sprite = StateOff;
        }
    }
}
