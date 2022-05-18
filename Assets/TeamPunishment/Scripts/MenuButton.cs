using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Text txt;
    static readonly Color32 on = new Color32(204, 53, 150, 255);
    static readonly Color32 off = new Color32(250, 227, 195, 255);

    void Start()
    {
        txt = GetComponentInChildren<Text>();
        txt.color = off;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        txt.color = on;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        txt.color = off;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        txt.color = off;
    }
}
