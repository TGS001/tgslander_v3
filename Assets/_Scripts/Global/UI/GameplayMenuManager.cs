using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMenuManager : MonoBehaviour {
	//public PopupGameplay gameplayMenu;
	public GameObject panelTapToStart;
	public GameObject TextTapToStart;
	public GameObject panelTutorial;


	public void InitializeSettings () {
		if (TextTapToStart == null) {
			if (panelTapToStart != null) {
				Transform tempTrans =
					panelTapToStart.transform.Find ("TxtTapToStart");
				if (tempTrans != null) {
					TextTapToStart = tempTrans.gameObject;
				}
			}
		}

	}

	void Start() {
		InitializeSettings ();
	}

	public void HandleMenu(bool isShow) {
		//gameplayMenu.gameObject.SetActive (isShow);
		if (GameplayUIManager.Instance != null &&
		    GameplayUIManager.Instance.gameplayHUD != null) {
			GameplayUIManager.Instance.gameplayHUD.HandleHUD (!isShow);
		}

	}

	public void HandleTapToStart(bool isShow) {
		if (panelTapToStart != null) {
			panelTapToStart.gameObject.SetActive (isShow);
		}
		if (GameplayUIManager.Instance != null && 
			GameplayUIManager.Instance.gameplayHUD != null) {
			GameplayUIManager.Instance.gameplayHUD.HandleHUD(!isShow);
		}
	}

}
