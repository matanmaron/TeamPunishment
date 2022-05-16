using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Text txt;
    static readonly Color32 on = new Color32(204, 53, 150, 255);
    static readonly Color32 off = new Color32(250, 227, 195, 255);

    void Start()
    {
        txt = GetComponentInChildren<Text>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        txt.color = on;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        txt.color = off;
    }
}