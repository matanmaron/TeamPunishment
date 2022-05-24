using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class OnStarClick : MonoBehaviour
{
    [SerializeField] ButtonClickedEvent OnButtonClickEnd;
    [SerializeField] List<Sprite> planetStates = new List<Sprite>();
    Image planetImage;
    int counter = 0;

    private void Start()
    {
        planetImage = GetComponent<Image>();
        var btn = GetComponent<Button>();
        Init();
        btn.onClick.AddListener(OnStar);
    }

    public void Init()
    {
        if (planetImage == null)
        {
            return;
        }
        counter = 0;
        planetImage.sprite = planetStates[counter];
        planetImage.SetNativeSize();
    }

    private void OnStar()
    {
        counter++;
        if (counter == 6)
        {
            OnButtonClickEnd?.Invoke();
            return;
        }
        planetImage.sprite = planetStates[counter];
        planetImage.SetNativeSize();
    }
}
