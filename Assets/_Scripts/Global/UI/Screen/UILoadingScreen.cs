using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
	public float minFirstTimeDelay = 5f;
	public UIControlsImage controlsUI;
    // Start is called before the first frame update
    void Start()
    {
        if (controlsUI != null)
		{

		}
    }

    // Update is called once per frame
    public void ShowControlsUI(UIControlsImage.EShowControlsUI showUI)
    {
		if (controlsUI != null)
		{
			controlsUI.showControlsUI = showUI;
		}
	}
}
