using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] GameObject VideoPrefab;
    [SerializeField] VideoClip intro;
    [SerializeField] GameObject chatCanvas;

    public static VideoManager instance;

    private GameObject currentVideo;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) || Input.touchCount > 1)
        {
            if (currentVideo != null)
            {
                OnVideoEnd(currentVideo.GetComponent<VideoPlayer>());
            }
        }
    }

    public void PlayIntro()
    {
        chatCanvas.SetActive(false);
        Debug.Log($"[PlayIntro]");
        currentVideo = Instantiate(VideoPrefab, transform);
        var vid = currentVideo.GetComponent<VideoPlayer>();
        vid.clip = intro;
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
    }
}
