﻿using System.Collections;
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
        }
    }

    public void GoToStart()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_START);
    }

    public void GoToLevelSelect()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_LEVEL_SELECT);
    }

    public void GoToWorkshop()
    {
        AsyncLevelLoadController.LoadLevel(GGConst.SCENE_NAME_WORKSHOP);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
