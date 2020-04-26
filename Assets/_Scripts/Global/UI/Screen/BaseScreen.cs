using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseScreen : MonoBehaviour
{
    [Header("Neighbour Screens")]
    public BaseScreen nextScreen;
    public BaseScreen previousScreen;
            
    [Header("Properties")]
    public string screenTitle;

    [Header("UI Elements")]
    public RectTransform rectTransform;
    public Image panel;
    public Text titleText;        

    public abstract void OnOpen();
    public abstract void OnClose();
    public abstract void OnCompleteOpen();
    public abstract void OnCompleteClose();

    private float screenWidth;
    private float screenHeight;

    protected virtual void Start()
    {
        if (titleText != null)
        {
            titleText.text = screenTitle;
        }

        screenWidth = ScreenManager.Instance.canvasScaler.referenceResolution.x;
        screenHeight = ScreenManager.Instance.canvasScaler.referenceResolution.y;
    }

    public void ScaleIn()
    {
        OnOpen();
        TweenManager.Instance.ScaleIn(rectTransform, GGConst.TIME_SCALE_SCREEN, true, OnCompleteOpen);
    }

    public void ScaleOut()
    {
        OnClose();
        TweenManager.Instance.ScaleOut(rectTransform, GGConst.TIME_SCALE_SCREEN, true, OnCompleteClose);
    }

    public void SlideInLeft()
    {
        OnOpen();
        TweenManager.Instance.SlideX(rectTransform, screenWidth, GGConst.TIME_SLIDE_SCREEN, true, OnCompleteOpen);
    }

    public void SlideOutLeft()
    {
        OnClose();
        TweenManager.Instance.SlideX(rectTransform, -screenWidth, GGConst.TIME_SLIDE_SCREEN, true, OnCompleteClose);
    }

    public void SlideInRight()
    {
        OnOpen();
        TweenManager.Instance.SlideX(rectTransform, -screenWidth, GGConst.TIME_SLIDE_SCREEN, true, OnCompleteOpen);
    }

    public void SlideOutRight()
    {
        OnClose();
        TweenManager.Instance.SlideX(rectTransform, Screen.width, GGConst.TIME_SLIDE_SCREEN, true, OnCompleteClose);
    }

    public void Open()
    {
        OnOpen();
    }

    public void Close()
    {
        OnClose();
    }

    public void GoToNext()
    {
        Close();

        if (nextScreen != null)
        {
            nextScreen.Open();
        } 
    }

    public void GoToPrevious()
    {
        Close();

        if (previousScreen != null)
        {
            previousScreen.Open();
        }
    }
}