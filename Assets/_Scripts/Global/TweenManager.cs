using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenManager : MonoBehaviourSingleton<TweenManager>
{
    //SCALE

    public void ScaleIn(GameObject obj, float time, bool unscaledTime = true, System.Action onComplete = null, float delay = 0)
    {
//        LeanTween.scale(obj, Vector3.one, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
    }
    //
    public void ScaleOut(GameObject obj, float time, bool unscaledTime = true, System.Action onComplete = null, float delay = 0)
    {
//        LeanTween.scale(obj, Vector3.zero, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    }

    public void ScaleIn(RectTransform rect, float time, bool unscaledTime = true, System.Action onComplete = null, float delay = 0)
    {
//        LeanTween.scale(rect, Vector3.one, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
    }

    public void ScaleOut(RectTransform rect, float time, bool unscaledTime = true, System.Action onComplete = null, float delay = 0)
    {
//        LeanTween.scale(rect, Vector3.zero, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    }

    //SLIDE

    public void SlideX(GameObject obj, float to, float time, bool unscaledTime = true, System.Action onComplete = null, float delay = 0)
    {
//        LeanTween.moveX(obj, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    }

    public void SlideX(RectTransform rect, float to, float time, bool unscaledTime = true, System.Action onComplete = null, float delay = 0)
    {
//        LeanTween.moveX(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    }
}