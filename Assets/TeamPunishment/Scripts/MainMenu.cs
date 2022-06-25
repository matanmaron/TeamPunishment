using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace TeamPunishment
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Button btnStart;
        [SerializeField] Button btnOption;
        [SerializeField] Button btnCredits;
        [SerializeField] Button btnExit;
        [SerializeField] Button btnBackOption;
        [SerializeField] Button btnBackCredits;
        [SerializeField] GameObject menuPanel;
        [SerializeField] GameObject optionPanel;
        [SerializeField] GameObject creditsPanel;
        [SerializeField] Image qrCodeImage;

        int imgTry = 0;
        const string URL = @"https://drive.google.com/uc?export=download&id=18ftMMDTJjuZI4K8E3mmd55yXqCx7sV_-";

        private void Start()
        {
            if (Application.isBatchMode)
            {
                Scenes.LoadStandartGame();
                return;
            }
            btnStart.onClick.AddListener(OnMenuStart);
            btnOption.onClick.AddListener(OnMenuOption);
            btnCredits.onClick.AddListener(OnMenuCredits);
            btnExit.onClick.AddListener(OnMenuExit);
            btnBackOption.onClick.AddListener(OnBack);
            btnBackCredits.onClick.AddListener(OnBack);
            AudioManager.instance.PlayMusic();
            var path = Path.Combine(Application.streamingAssetsPath, "demo");
            Debug.Log(path);
#if UNITY_EDITOR
            if (true)
#else
            if (Directory.Exists(path) || GameManager.instance.isDemoMode)
#endif
            {
                Debug.Log("demo detected");
                ShowDemoMenu();
            }
        }

        private void OnMenuCredits()
        {
            menuPanel.SetActive(false);
            creditsPanel.SetActive(true);
        }

        private void OnBack()
        {
            creditsPanel.SetActive(false);
            optionPanel.SetActive(false);
            menuPanel.SetActive(true);
        }

        private void OnMenuExit()
        {
            Application.Quit();
        }

        private void OnMenuOption()
        {
            menuPanel.SetActive(false);
            optionPanel.SetActive(true);
        }

        private void OnMenuStart()
        {
            if (GameManager.instance.isAndroid)
            {
                Scenes.LoadMobileInfo();
            }
            else
            {
                GameManager.instance.SendAnalyticsEvent("start");
                Scenes.LoadStandartGame();
            }
        }

        private void OnDestroy()
        {
            btnStart.onClick.RemoveAllListeners();
            btnOption.onClick.RemoveAllListeners();
            btnExit.onClick.RemoveAllListeners();
            btnBackOption.onClick.RemoveAllListeners();
            btnBackCredits.onClick.RemoveAllListeners();
            btnCredits.onClick.RemoveAllListeners();
        }

        private void ShowDemoMenu()
        {
            GameManager.instance.isDemoMode = true;
            btnExit.gameObject.SetActive(false);
            qrCodeImage.gameObject.SetActive(true);
            StartCoroutine(GetImage(qrCodeImage));
        }

        IEnumerator GetImage(Image img)
        {
            Debug.Log("GetImage");
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(URL))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"GetImage ERROR {imgTry}");
                    imgTry++;
                    if (imgTry > 3)
                    {
                        Debug.Log("GetImage FINAL ERROR");
                        qrCodeImage.gameObject.SetActive(false);
                        yield break;
                    }
                    StartCoroutine(GetImage(qrCodeImage));
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    var bytes = texture.EncodeToPNG();
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
                    img.overrideSprite = sprite;
                    Debug.Log("GetImage OK");
                }
            }
        }
    }
}