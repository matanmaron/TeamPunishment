using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class OnStarClick : MonoBehaviour
{
    [SerializeField] ButtonClickedEvent OnButtonClickEnd;
    [SerializeField] List<Sprite> planetStates = new List<Sprite>();
    [SerializeField] List<int> residents = new List<int>();
    [SerializeField] Text info;
    [SerializeField] string TextForInfo;
    [SerializeField] MoveUp deathTextPrefab;
    Image planetImage;
    int counter = 0;
    int currentResidents = 0;

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
        currentResidents = residents[counter];
        counter = 0;
        planetImage.sprite = planetStates[counter];
        planetImage.SetNativeSize();
        info.text = TextForInfo.Replace("XX", residents[counter].ToString());
    }

    private void OnStar()
    {
        AudioManager.instance.PlayStarsExsplosion();
        counter++;
        if (counter == 6)
        {
            OnButtonClickEnd?.Invoke();
            return;
        }
        planetImage.sprite = planetStates[counter];
        planetImage.SetNativeSize();
        currentResidents += residents[counter];
        info.text = TextForInfo.Replace("XX", currentResidents.ToString());
        MoveUp go = Instantiate(deathTextPrefab, transform);
        go.Init(residents[counter]);
    }
}
