using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class QrCode : MonoBehaviour
    {
        Image qrCodeImage;
        int imgTry = 0;
        const string URL = @"https://drive.google.com/uc?export=download&id=18ftMMDTJjuZI4K8E3mmd55yXqCx7sV_-";

        void Start()
        {
            qrCodeImage = GetComponent<Image>();
            if (!GameManager.instance.isDemoMode)
            {
                gameObject.SetActive(false);
                return;
            }
            Debug.Log("qr on");
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