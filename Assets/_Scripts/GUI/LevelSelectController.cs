using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour {
    // This method should be avoided in favor of the safer method calls below in case scene names change
    public void SelectLevel(string name)
    {
        Debug.Log("selected level " + name);
        if (name == "DemoLevelSelect")
        {
            AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_LEVEL_SELECT);
        }
        else
        {
            AsyncLevelLoadController.LoadLevel(name);
			AudioManager.Instance.PlayMusicForScene(name);
		}
	}

    public void GoToStart()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_START);
		AudioManager.Instance.PlayMusicForScene(GGConst.SCENE_NAME_START);
    }

	public void GoToCredits()
	{
		AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_ABOUT);
		AudioManager.Instance.PlayMusicForScene(GGConst.SCENE_NAME_ABOUT);
	}

	public void GoToControls()
	{
		AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_ABOUT_CONTROLS);
		AudioManager.Instance.PlayMusicForScene(GGConst.SCENE_NAME_ABOUT);
	}

	public void GoToLevelSelect()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_LEVEL_SELECT);
		AudioManager.Instance.PlayMusicForScene(GGConst.SCENE_NAME_LEVEL_SELECT);
	}

	public void GoToWorkshop()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_WORKSHOP);
		AudioManager.Instance.PlayMusicForScene(GGConst.SCENE_NAME_WORKSHOP);
	}

	public void ExitGame()
    {
		if (GlobalGameManager.Instance != null)
		{
			GlobalGameManager.Instance.QuitGame();
		}
		else
		{
			Application.Quit();
		}
    }

}
