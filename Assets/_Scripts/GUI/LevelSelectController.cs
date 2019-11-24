using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectController : MonoBehaviour {

	public void SelectLevel(string name)
    {
        Debug.Log("selected level " + name);
        AsyncLevelLoadController.LoadLevel(name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
