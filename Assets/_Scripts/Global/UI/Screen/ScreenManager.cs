using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviourSingleton<ScreenManager>
{
    private BaseScreen currentScreen;

    [Header("Helpers")]
    public Image blackBackground;
    public CanvasScaler canvasScaler;

    [Header("Screens")]
    public PauseScreen pauseScreen;
    public MainMenuScreen mainMenuScreen;
    public NewMatchScreen newMatchScreen;

    //Screen handling methods

    public void OpenPauseScreen()
    {
        ShowBlackBackground();

        if (pauseScreen != null)
        {
            currentScreen = pauseScreen;
            currentScreen.gameObject.SetActive(true);
            currentScreen.ScaleIn();
        }
    }

    //Helper handling methods

    public void ShowBlackBackground()
    {
        if (blackBackground != null)
        {
            blackBackground.gameObject.SetActive(true);
        }
    }

    public void HideBlackBackground()
    {
        if (blackBackground != null)
        {
            blackBackground.gameObject.SetActive(false);
        }
    }
}