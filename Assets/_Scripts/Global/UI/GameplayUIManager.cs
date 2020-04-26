using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviourSingleton<GameplayUIManager>
{
	public GameplayMenuManager menuManager;
//	public PopupGameplay menuPopup;
	public HUDGameplay gameplayHUD;
	public GameObject canvasObj;
	public Canvas uiGameplayCanvas;
	public AudioSource menuAudioSrc;
	public GameObject backGroundFade;

	void Awake () {
		SetInstanceInternally (this);
		if (menuManager == null) {
			menuManager = GameObject.FindObjectOfType<GameplayMenuManager> ();
			if (menuManager != null) {
//				menuPopup = menuManager.gameplayMenu;
			}
		}
		if (canvasObj == null) {
			canvasObj = GameObject.FindGameObjectWithTag (GGConst.TAG_UI_GAMEPLAY_CANVAS);
		}
		if (uiGameplayCanvas == null && canvasObj != null) {
			uiGameplayCanvas = canvasObj.GetComponent<Canvas> ();
		}
		if (menuAudioSrc == null && canvasObj != null) {
			menuAudioSrc = canvasObj.GetComponent<AudioSource> ();
		}
	}

	void Start () {
		//if (menuPopup == null) {
		//	menuPopup = GameObject.FindObjectOfType<PopupGameplay> ();
		//}
		if (gameplayHUD == null) {
			gameplayHUD = GameObject.FindObjectOfType<HUDGameplay> ();
		}
	}
}
