using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

namespace TeamPunishment
{
    public class GameManager : MonoBehaviour
    {
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
        double MINUETS_TO_SHOW_DEMO = 10;
#else
        double MINUETS_TO_SHOW_DEMO = 10;
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
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Cursor.SetCursor(crosshair, Vector2.zero, CursorMode.Auto);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape) && CanEsc)
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
    }

}