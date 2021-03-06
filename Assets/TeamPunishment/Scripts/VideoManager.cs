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
        string Intro = "Intro.mp4";
        string Ferrum = "Ferrum Stage.mp4";
        string Cibus = "Cibus Stage.mp4";
        string Ordo = "OrdoStage.mp4";
        string Artem = "Artheem Stage.mp4";
        string End = "EndGame.mp4";
        string Transition = "Transition.mp4";
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

        private void PlayVideo(Action callback, string clip)
        {
            StartCoroutine(PlayVideoDelayed(callback, clip));
        }

        IEnumerator PlayVideoDelayed(Action callback, string clip)
        {
            yield return new WaitForSeconds(0.01f);
            AudioManager.instance.StopMusic();
            onVideoEndCallback.Add(callback);
            chatCanvas.SetActive(false);
            Debug.Log($"[PlayVideo]");
            currentVideo = Instantiate(VideoPrefab, transform);
            currentVideo.GetComponentInChildren<Button>().onClick.AddListener(OnEnterClick);
#if !UNITY_EDITOR
            if (GameManager.instance.isDemoMode)
            {
                currentVideo.GetComponentInChildren<Button>().gameObject.SetActive(false);
            }
#endif
            VideoPlayer vid = currentVideo.GetComponent<VideoPlayer>();
            vid.SetDirectAudioVolume(0,GameManager.instance.VolumeMargin);
           
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, clip);
            vid.source = VideoSource.Url;
            vid.url = filePath;

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

        public void SetVolumeMargin(float margin)
        {
            if (currentVideo != null)
            {
                currentVideo.GetComponent<VideoPlayer>().SetDirectAudioVolume(0, margin);
            }
        }
    }
}