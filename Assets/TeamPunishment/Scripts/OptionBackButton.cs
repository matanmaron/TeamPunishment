using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionBackButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image buttonImg;

    [SerializeField] Sprite standart;
    [SerializeField] Sprite hover;
    [SerializeField] Sprite clicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImg.sprite = clicked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImg.sprite = hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImg.sprite = standart;
    }
}
