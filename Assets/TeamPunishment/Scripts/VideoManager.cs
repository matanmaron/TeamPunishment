using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    [SerializeField] GameObject VideoPrefab;
    [SerializeField] VideoClip intro;
    
    public static VideoManager instance;

    private GameObject currentVideo;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) || Input.touchCount > 1)
        {
            if (currentVideo != null)
            {
                OnVideoEnd(currentVideo.GetComponent<VideoPlayer>());
            }
        }
    }

    public void PlayIntro()
    {
        Debug.Log($"[PlayIntro]");
        currentVideo = Instantiate(VideoPrefab, transform);
        var vid = currentVideo.GetComponent<VideoPlayer>();
        vid.clip = intro;
        vid.Play();
        vid.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vid)
    {
        Debug.Log($"[OnVideoEnd]");
        vid.loopPointReached -= OnVideoEnd;
        Destroy(currentVideo);
        currentVideo = null;
    }
}
