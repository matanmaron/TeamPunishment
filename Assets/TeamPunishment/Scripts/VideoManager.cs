using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TeamPunishment
{
    public class VideoManager : MonoBehaviour
    {
        [SerializeField] GameObject VideoPrefab;
        [SerializeField] VideoClip Intro;
        [SerializeField] VideoClip Ferrum;
        [SerializeField] VideoClip Cibus;
        [SerializeField] VideoClip Ordo;
        [SerializeField] VideoClip Artem;
        [SerializeField] VideoClip End;
        [SerializeField] VideoClip Transition;
        [SerializeField] GameObject chatCanvas;

        public static VideoManager instance;

        private GameObject currentVideo;
        private string videoName;
        private List<Action> onVideoEndCallback = new List<Action>();

        void Awake()
        {
            instance = this;
        }

        private void OnEnterClick()
        {
            if (currentVideo != null)
            {
                if (videoName != "demo")
                {
                    GameManager.instance.SendAnalyticsEvent($"video-skip", "videoName", videoName);
                }
                OnVideoEnd(currentVideo.GetComponent<VideoPlayer>());
            }
        }

        private void PlayVideo(Action callback, VideoClip clip)
        {
            GameManager.instance.CanEsc = false;
            StartCoroutine(PlayVideoDelayed(callback, clip));
        }

        IEnumerator PlayVideoDelayed(Action callback, VideoClip clip)
        {
            yield return new WaitForSeconds(0.01f);
            AudioManager.instance.StopMusic();
            onVideoEndCallback.Add(callback);
            chatCanvas.SetActive(false);
            Debug.Log($"[PlayVideo]");
            currentVideo = Instantiate(VideoPrefab, transform);
            currentVideo.GetComponentInChildren<Button>().onClick.AddListener(OnEnterClick);
            VideoPlayer vid = currentVideo.GetComponent<VideoPlayer>();
            vid.clip = clip;
            vid.aspectRatio = VideoAspectRatio.FitInside;
            vid.targetCamera = Camera.main;
            vid.Play();
            vid.loopPointReached += OnVideoEnd;
        }

        public void PlayIntro(Action callback)
        {
            Debug.Log($"[PlayIntro]");
            videoName = "intro";
            PlayVideo(callback, Intro);
        }

        private void OnVideoEnd(VideoPlayer vid)
        {
            Debug.Log($"[OnVideoEnd]");
            vid.loopPointReached -= OnVideoEnd;
            Destroy(currentVideo);
            currentVideo = null;
            videoName = string.Empty;
            GameManager.instance.CanEsc = true;
            chatCanvas.SetActive(true);
            if (onVideoEndCallback.Count > 0)
            {
                Action callback = onVideoEndCallback[0];
                onVideoEndCallback.RemoveAt(0);
                callback?.Invoke();
            }
            AudioManager.instance.PlayMusic();
        }

        public void PlayFerrum(Action onStarVideoEnd)
        {
            Debug.Log($"[PlayFerrum]");
            videoName = "ferrum";
            PlayVideo(onStarVideoEnd, Ferrum);
        }

        public void PlayCibus(Action onStarVideoEnd)
        {
            Debug.Log($"[PlayCibus]");
            videoName = "cibus";
            PlayVideo(onStarVideoEnd, Cibus);
        }

        public void PlayOrdo(Action onStarVideoEnd)
        {
            Debug.Log($"[PlayOrdo]");
            videoName = "ordo";
            PlayVideo(onStarVideoEnd, Ordo);
        }

        public void PlayArtem(Action onStarVideoEnd)
        {
            Debug.Log($"[PlayArtem]");
            videoName = "artem";
            PlayVideo(onStarVideoEnd, Artem);
        }

        public void PlayEnd(Action onEndVideoEnd)
        {
            Debug.Log($"[PlayEnd]");
            videoName = "end";
            PlayVideo(onEndVideoEnd, End);
        }

        public void PlayTransition(Action onPlayTransitionEnd)
        {
            Debug.Log($"[PlayTransition]");
            videoName = "transition";
            PlayVideo(onPlayTransitionEnd, Transition);
        }

        public bool IsVideoPlaying()
        {
            return currentVideo != null;
        }
    }
}