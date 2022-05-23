using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] GameObject VideoPrefab;
    [SerializeField] VideoClip Intro;
    [SerializeField] VideoClip Ferrum;
    [SerializeField] VideoClip Cibus;
    [SerializeField] VideoClip Ordo;
    [SerializeField] VideoClip Artem;
    [SerializeField] GameObject chatCanvas;

    public static VideoManager instance;

    private GameObject currentVideo;
    private Action onVideoEndCallback;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            OnEnterClick();
        }
    }

    private void OnEnterClick()
    {
        if (currentVideo != null)
        {
            OnVideoEnd(currentVideo.GetComponent<VideoPlayer>());
        }
    }

    private void PlayVideo(Action callback, VideoClip clip)
    {
        onVideoEndCallback = callback;
        chatCanvas.SetActive(false);
        Debug.Log($"[PlayVideo]");
        currentVideo = Instantiate(VideoPrefab, transform);
        currentVideo.GetComponentInChildren<Button>().onClick.AddListener(OnEnterClick);
        var vid = currentVideo.GetComponent<VideoPlayer>();
        vid.clip = clip;
        vid.aspectRatio = VideoAspectRatio.FitInside;
        vid.targetCamera = Camera.main;
        vid.Play();
        vid.loopPointReached += OnVideoEnd;
    }

    public void PlayIntro(Action callback)
    {
        Debug.Log($"[PlayIntro]");
        PlayVideo(callback, Intro);
    }

    private void OnVideoEnd(VideoPlayer vid)
    {
        Debug.Log($"[OnVideoEnd]");
        vid.loopPointReached -= OnVideoEnd;
        Destroy(currentVideo);
        currentVideo = null;
        chatCanvas.SetActive(true);
        onVideoEndCallback?.Invoke();
        onVideoEndCallback = null;
    }

    public void PlayFerrum(Action onStarVideoEnd)
    {
        Debug.Log($"[PlayFerrum]");
        PlayVideo(onStarVideoEnd, Ferrum);
    }

    public void PlayCibus(Action onStarVideoEnd)
    {
        Debug.Log($"[PlayCibus]");
        PlayVideo(onStarVideoEnd, Cibus);
    }

    public void PlayOrdo(Action onStarVideoEnd)
    {
        Debug.Log($"[PlayOrdo]");
        PlayVideo(onStarVideoEnd, Ordo);
    }

    public void PlayArtem(Action onStarVideoEnd)
    {
        Debug.Log($"[PlayArtem]");
        PlayVideo(onStarVideoEnd, Artem);
    }
}
