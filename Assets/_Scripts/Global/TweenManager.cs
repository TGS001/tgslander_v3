using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TweenManager : MonoBehaviourSingleton<TweenManager>
{
    //SCALE

    public void ScaleIn(GameObject obj, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
    {
        LeanTween.scale(obj, Vector3.one, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
    }
    //
    public void ScaleOut(GameObject obj, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
    {
        LeanTween.scale(obj, Vector3.zero, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    }

    public void ScaleIn(RectTransform rect, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
    {
        LeanTween.scale(rect, Vector3.one, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
    }

    public void ScaleOut(RectTransform rect, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
    {
        LeanTween.scale(rect, Vector3.zero, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    }

    //SLIDE

    //public void SlideX(GameObject obj, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
    //{
    //    LeanTween.moveX(obj, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    //}

    //public void SlideX(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
    //{
    //    LeanTween.moveX(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
    //}

	//Additional Tweening methods using LeanTween:
	public void ResetTweening()
	{
		LeanTween.cancelAll();
	}

	public void Alpha(RectTransform rect, float to, float time, bool unscaledTime = true, float delay = 0, Action onComplete = null)
	{
		LeanTween.alpha(rect, to, time).setDelay(delay).setIgnoreTimeScale(unscaledTime).setOnComplete(onComplete);
	}

	public void ScaleIn(GameObject obj, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(obj, Vector3.one, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
	}

	public void ScaleOut(GameObject obj, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(obj, Vector3.zero, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
	}

	public void ScaleIn(RectTransform rect, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(rect, Vector3.one, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack().setOnStart(onStart);
	}

	public void ScaleTo(RectTransform rect, Vector3 to, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInOutBack().setOnStart(onStart);
	}

	public void ScaleOut(RectTransform rect, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(rect, Vector3.zero, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
	}

	public void ScalePingPong(RectTransform rect, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(rect, new Vector3(1.1f, 1.1f, 1.1f), time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setLoopPingPong();
	}

	public void ScalePunch(RectTransform rect, Vector3 target, float time, bool unscaledTime = true, Action onStart = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.scale(rect, target, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEasePunch();
	}

	public void ValueTransition(float from, float to, float time, bool unscaledTime = true, System.Action onStart = null, System.Action<float> onUpdate = null, Action onComplete = null, float delay = 0)
	{
		LeanTween.value(from, to, time).setOnStart(onStart).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setOnUpdate(onUpdate);
	}

	//SLIDE - Animated movements in one axis

	public void SlideX(GameObject obj, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveX(obj, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
	}

	public void SlideX(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveX(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
	}

	public void SlideXLinear(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveX(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime);
	}

	public void SlideY(GameObject obj, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveY(obj, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
	}

	public void SlideY(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveY(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInBack();
	}

	public void SlideYLinear(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveY(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime);
	}

	public void SlideYExpoQuad(GameObject obj, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveY(obj, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInOutExpo();
	}

	public void SlideYExpoQuad(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveY(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseInOutExpo();
	}

	public void SlideTo(GameObject obj, Vector3 to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.move(obj, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
	}

	public void SlideTo(RectTransform rect, Vector3 to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.move(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutBack();
	}

	public void MoveOutRight(RectTransform rect, float to, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveX(rect, to, 0).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime);
	}

	public void MoveY(RectTransform rect, float to, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		LeanTween.moveY(rect, to, time).setOnComplete(onComplete).setDelay(delay).setIgnoreTimeScale(unscaledTime).setEaseOutExpo();
	}

	//ALPHA FADING
	public void Fade(GameObject obj, float fadeTo, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		if (obj != null)
		{
			LeanTween.alpha(obj, fadeTo, time).setDelay(delay).setOnComplete(onComplete).setIgnoreTimeScale(unscaledTime);
		}
	}

	public void Fade(RectTransform rect, float fadeTo, float time, bool unscaledTime = true, Action onComplete = null, float delay = 0)
	{
		if (rect != null)
		{
			LeanTween.alpha(rect, fadeTo, time).setDelay(delay).setOnComplete(onComplete).setIgnoreTimeScale(unscaledTime);
		}
	}

	public void Spin(RectTransform rect, float time, bool unscaledtime = true, float delay = 0)
	{
		if (rect != null)
		{
			LeanTween.rotateAround(rect, new Vector3(0, 0, 1), 360, time).setDelay(delay).setRepeat(-1).setIgnoreTimeScale(unscaledtime);
		}
	}

	public void Rotate180Y(RectTransform rect, float time, bool unscaledtime = true, float delay = 0)
	{
		if (rect != null)
		{
			LeanTween.rotateAround(rect, new Vector3(0, 1, 0), 180, time).setDelay(delay).setIgnoreTimeScale(unscaledtime);
		}
	}

	public void Rotate(RectTransform rect, float time, float y, Action onComplete, bool unscaledtime = true, float delay = 0)
	{
		if (rect != null)
		{
			LeanTween.rotateAround(rect, new Vector3(0, 1, 0), y, time).setDelay(delay).setIgnoreTimeScale(unscaledtime).setOnComplete(onComplete);
		}
	}
}