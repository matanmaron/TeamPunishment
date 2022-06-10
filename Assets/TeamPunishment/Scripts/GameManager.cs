using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Texture2D crosshair;
    public bool isAndroid = false;
    public bool isDemoMode = false;
    bool d = false;
    bool e = false;
    bool m = false;

    public static GameManager instance;
    void Awake()
    {
        instance = this;
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
}
