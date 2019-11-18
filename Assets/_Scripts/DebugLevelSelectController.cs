using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLevelSelectController : MonoBehaviour {

	public void SelectLevel(string name)
    {
        Debug.Log("selected level " + name);
        AsyncLevelLoadController.LoadLevel(name);
    }
}
