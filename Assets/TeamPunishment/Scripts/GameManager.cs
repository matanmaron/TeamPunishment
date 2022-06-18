using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TeamPunishment
{
    public class GameManager : MonoBehaviour
    {

        public Texture2D crosshair;
        public bool isAndroid = false;
        public bool isDemoMode = false;
        public bool isDemoRunning = false;
        DateTime lastClick = DateTime.Now;
        int MINUETS_TO_SHOW_DEMO = 3;
        public static GameManager instance;
        public bool LogRecords = true;

        const string ON = "on";
        const string OFF = "off";

        void Awake()
        {
            instance = this;
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
            if (PlayerPrefs.GetInt("logRecords", 1) == 0)
                LogRecords = false;
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
                        isDemoRunning = true;
                        VideoManager.instance.PlayDemo(StopDemo);
                    }
                }
            }
        }

        internal void StopDemo()
        {
            isDemoRunning = false;
            lastClick = DateTime.Now;
            Scenes.LoadMenu();
        }

        public void MuteLogRecords(bool isOn)
        {
            Debug.Log($"log mute is {(isOn ? OFF : ON)}");
            LogRecords = !isOn;
        }
    }

}