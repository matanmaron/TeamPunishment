using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] GameObject VideoPrefab;
    [SerializeField] VideoClip intro;
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

    public void PlayIntro(Action callback)
    {
        onVideoEndCallback = callback;
        chatCanvas.SetActive(false);
        Debug.Log($"[PlayIntro]");
        currentVideo = Instantiate(VideoPrefab, transform);
        currentVideo.GetComponentInChildren<Button>().onClick.AddListener(OnEnterClick);
        var vid = currentVideo.GetComponent<VideoPlayer>();
        vid.clip = intro;
        vid.aspectRatio = VideoAspectRatio.FitInside;
        vid.targetCamera = Camera.main;
        vid.Play();
        vid.loopPointReached += OnVideoEnd;
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
}
