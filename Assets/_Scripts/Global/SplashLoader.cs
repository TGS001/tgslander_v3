using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashLoader : MonoBehaviour {
	public float loadTimeMin = 2.0f;
	public GlobalGameManager.EGameSceneType nextScene = GlobalGameManager.EGameSceneType.Start;
    public float debugHoldTimeMin = 1.0f;
    public int debugTouches = 2;
    public GlobalGameManager.EGameSceneType debugScene = GlobalGameManager.EGameSceneType.DebugStart;

    private float loadTimer = 0f;

    private float debugTouchTimer = 0f;
    private bool canDebug = true;
    void Start () {
		loadTimer = loadTimeMin;
        debugTouchTimer = debugHoldTimeMin;

    }

    // Update is called once per frame
    void Update()
    {
        if (canDebug)
        {
            bool isDebugTouching = Input.touches.Length >= debugTouches;
#if UNITY_EDITOR
            isDebugTouching = Input.GetMouseButton(0) || Input.GetMouseButton(1);
#endif
            if (isDebugTouching)
            {
                debugTouchTimer -= Time.deltaTime;
                if (debugTouchTimer <= 0f)
                {
                    GlobalGameManager.Instance.isInDebugMode = true;
                    GlobalGameManager.Instance.ChangeScene(debugScene);
                }
            }
            else
            {
                debugTouchTimer = debugHoldTimeMin;
                CheckLoadTime();
            }
        }
        else
        {
            CheckLoadTime();
        }
    }

    private void CheckLoadTime()
    {
        loadTimer -= Time.deltaTime;
        if (loadTimer <= 0f)
        {
            GlobalGameManager.Instance.ChangeScene(nextScene);
        }
    }
}
