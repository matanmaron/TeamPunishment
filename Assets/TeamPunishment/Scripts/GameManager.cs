using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.IO;
using UnityEngine.UI;

namespace TeamPunishment
{
    public class GameManager : MonoBehaviour
    {
        public GameObject volumeCanvas;
        public Button volumeUp;
        public Button volumeDown;
        public float VolumeMargin = 0;
        public Text VolumeText;
        private Coroutine textCoroutine = null;

        public Texture2D crosshair;
        public bool isAndroid = false;
        public bool isDemoMode = false;
        public bool isDemoRunning = false;
        DateTime lastClick = DateTime.Now;
        public bool IsMuteMusic;
        public bool IsMuteVoice;
        public bool IsMuteLogs;
        public bool CanEsc = true;
#if UNITY_EDITOR
        double MINUETS_TO_SHOW_DEMO = 1;
#else
        double MINUETS_TO_SHOW_DEMO = 10; //DO NOT CHANGE !
#endif

        public static GameManager instance;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
#if UNITY_EDITOR
            isAndroid = false;
#endif
#if UNITY_IOS || UNITY_ANDROID
            isAndroid = true; //NEVER CHANGE!
#endif
            CheckDemoMode();
        }

        private void CheckDemoMode()
        {
            DontDestroyOnLoad(gameObject);
            var path = Path.Combine(Application.streamingAssetsPath, "demo");
            Debug.Log(path);
            if (Directory.Exists(path))
            {
                isDemoMode = true;
            }
#if UNITY_EDITOR
            isDemoMode = true;
#endif
        }

        void Start()
        {
            Cursor.SetCursor(crosshair, Vector2.zero, CursorMode.Auto);
            if (!isDemoMode)
            {
                volumeCanvas.SetActive(false);
            }
            volumeUp.onClick.AddListener(VolumeUp);
            volumeDown.onClick.AddListener(VolumeDown);
        }

        private void Update()
        {
            if (CanEsc && Input.GetKeyUp(KeyCode.Escape))
            {
                SendAnalyticsEvent($"quit");
                var chat = FindObjectOfType<ChatUI>();
                if (chat != null)
                {
                    chat.Disconnect();
                }
                Scenes.LoadMenu();
            }
            if (isDemoMode)
            {
                if (!isDemoRunning)
                {
                    if (Input.anyKey)
                    {
                        lastClick = DateTime.Now;
                    }
                    if (DateTime.Now > lastClick.AddMinutes(MINUETS_TO_SHOW_DEMO))
                    {
                        Debug.Log("DEMO START");
                        volumeCanvas.SetActive(false);
                        var chat = FindObjectOfType<ChatUI>();
                        if (chat != null)
                        {
                            chat.Disconnect();
                        }
                        Scenes.LoadDemo();
                        isDemoRunning = true;
                    }
                }
            }
        }

        public void StopDemo()
        {
            volumeCanvas.SetActive(true);
            VolumeMargin = 0.5f;
            isDemoRunning = false;
            lastClick = DateTime.Now;
        }

        public void MuteLogRecords(bool state)
        {
            IsMuteLogs = !state;
        }

        public void SendAnalyticsEvent(string eName, string pKey, object pValue)
        {
#if UNITY_EDITOR
            return;
#endif
            if (IsMuteLogs)
            {
                return;
            }
            AnalyticsResult res = Analytics.CustomEvent(eName, 
                new Dictionary<string, object>
                {
                    { pKey, pValue }
                }) ;
            Debug.Log($"[SendAnalyticsEvent] - {res} ({eName})");
        }

        public void SendAnalyticsEvent(string eName)
        {
#if UNITY_EDITOR
            return;
#endif
            if (IsMuteLogs)
            {
                return;
            }
            AnalyticsResult res = Analytics.CustomEvent(eName);
            Debug.Log($"[SendAnalyticsEvent] - {res} ({eName})");
        }

        public void RemoveCRTEffect()
        {
            Camera.main.GetComponent<FlareLayer>().enabled = false;
            Camera.main.GetComponent<RetroTVFX.CRTEffect>().enabled = false;
        }

        private void VolumeUp()
        {
            if (VolumeMargin < 1)
            {
                VolumeMargin += 0.1f;
                VolumeMargin = (float)Math.Round(VolumeMargin, 1);
                AudioManager.instance.SetVolumeMargin(VolumeMargin);
                VideoManager.instance.SetVolumeMargin(VolumeMargin);
            }
            ShowText();
        }

        private void VolumeDown()
        {
            if (VolumeMargin > 0.1f)
            {
                VolumeMargin -= 0.1f;
                VolumeMargin = (float)Math.Round(VolumeMargin, 1);
                AudioManager.instance.SetVolumeMargin(VolumeMargin);
                VideoManager.instance.SetVolumeMargin(VolumeMargin);
            }
            ShowText();
        }

        IEnumerator RemoveText()
        {
            yield return new WaitForSeconds(1);
            VolumeText.text = string.Empty;
        }

        private void ShowText()
        {
            Debug.Log($"Volume Margin: {VolumeMargin}");
            VolumeText.text = $"Volume: {Mathf.RoundToInt(VolumeMargin*10)}/10";
            if (textCoroutine != null)
            {
                StopCoroutine(textCoroutine);
            }
            textCoroutine = StartCoroutine(RemoveText());
        }
    }

}