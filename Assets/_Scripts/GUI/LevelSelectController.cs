using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour {
    public static string LEVEL_NAME_LEVEL_SELECT = "LevelSelect";
    public static string LEVEL_NAME_WORKSHOP = "LanderWorkShop";
    public static string LEVEL_NAME_START = "Game_Title_Screen";

    public void SelectLevel(string name)
    {
        Debug.Log("selected level " + name);
        if (name == "DemoLevelSelect")
        {
            AsyncLevelLoadController.LoadLevel(LEVEL_NAME_LEVEL_SELECT);
        }
        else
        {
            AsyncLevelLoadController.LoadLevel(name);
        }
    }

    public void GoToStart()
    {
        AsyncLevelLoadController.LoadLevel(LEVEL_NAME_START);
    }

    public void GoToLevelSelect()
    {
        AsyncLevelLoadController.LoadLevel(LEVEL_NAME_LEVEL_SELECT);
    }

    public void GoToWorkshop()
    {
        AsyncLevelLoadController.LoadLevel(LEVEL_NAME_WORKSHOP);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
