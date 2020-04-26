using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : BaseScreen
{        
    public Button quitMatchButton;
    public Button sfxButton;
    public Button bgmButton;
    public Button resumeButton;
    public Toggle twoStepsToggle;
    public Toggle threeStepsToggle;

    public override void OnClose()
    {
        //Do Nothing
    }

    public override void OnOpen()
    {
        Time.timeScale = GGConst.TIME_SCALE_PAUSE;
    }

    public override void OnCompleteOpen()
    {
        //Do Nothing
    }

    public override void OnCompleteClose()
    {
        ScreenManager.Instance.HideBlackBackground();
        Time.timeScale = GGConst.TIME_SCALE_RESUME;
    }
}