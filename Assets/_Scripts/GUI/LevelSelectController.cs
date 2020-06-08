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
			if (name.StartsWith(GGConst.SCENE_NAME_TUTORIAL_LEVEL_PREFIX))
			{
				if (name.EndsWith("02") || name.EndsWith("04"))
				{
					AudioManager.Instance.PlayMusic(AudioManager.EMusicClip.Music04_KeysOne);
				}
			}
			else if (name.StartsWith(GGConst.SCENE_NAME_GAME_LEVEL_PREFIX))
			{
				if (name.EndsWith("01"))
				{
					AudioManager.Instance.PlayMusic(AudioManager.EMusicClip.Music04_KeysOne);
				}
				else if (name.EndsWith("08"))
				{
					AudioManager.Instance.PlayMusic(AudioManager.EMusicClip.Music07_One);
				}
			}
		}
	}

    public void GoToStart()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_START);
		AudioManager.Instance.PlayMusic(AudioManager.EMusicClip.Music01_GotToBe);
    }

    public void GoToLevelSelect()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_LEVEL_SELECT);
		AudioManager.Instance.PlayMusic(AudioManager.EMusicClip.Music03_SixYearAgo);
	}

	public void GoToWorkshop()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_WORKSHOP);
		AudioManager.Instance.PlayMusic(AudioManager.EMusicClip.Music03_SixYearAgo);
	}

	public void ExitGame()
    {
        Application.Quit();
    }

}
