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
#if UNITY_EDITOR
        double MINUETS_TO_SHOW_DEMO = 0.2;
#else
        double MINUETS_TO_SHOW_DEMO = 3;
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
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
#if UNITY_EDITOR
            isAndroid = false;
#endif
#if UNITY_IOS || UNITY_ANDROID
            isAndroid = true; //NEVER CHANGE!
#endif
            Cursor.SetCursor(crosshair, Vector2.zero, CursorMode.Auto);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
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
            if (IsMuteLogs)
            {
                return;
            }
            AnalyticsResult res = Analytics.CustomEvent(eName);
            Debug.Log($"[SendAnalyticsEvent] - {res} ({eName})");
        }
    }

}