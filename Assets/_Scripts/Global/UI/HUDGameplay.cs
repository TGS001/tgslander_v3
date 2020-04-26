using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDGameplay : MonoBehaviour {
	public void Initialize()
	{
	}

	void Start () {
		Initialize ();

    }

	public void ShowMenu() {
		/*if (GameplayUIManager.Instance != null &&
			GameplayUIManager.Instance.menuManager != null) {
			GameplayUIManager.Instance.menuManager.HandleMenu (true);
		}*/

        if(ScreenManager.Instance != null)
        {
            ScreenManager.Instance.OpenPauseScreen();
        }
	}

	public void HandleHUD(bool isShow)
	{
		HandleBackgroundOverlay (!isShow);
	}

	public void HandleBackgroundOverlay(bool showBackgroundPanel) {
	}

}
