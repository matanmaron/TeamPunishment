using UnityEngine;

namespace TeamPunishment
{
    public class CannonGame : MonoBehaviour
    {
        void Start()
        {
            if (GameManager.instance.isAndroid)
            {
                GameManager.instance.RemoveCRTEffect();
            }
        }
    }
}