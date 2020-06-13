using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControlsImage : MonoBehaviour
{
	public enum EShowControlsUI
	{
		None,
		Small,
		Medium,
		Fullscreen
	}
	private EShowControlsUI _showControlsUI;
	public EShowControlsUI showControlsUI { get { return _showControlsUI; } set { _showControlsUI = value; AdjustView(); } }
	public Image controlsImage;
	public RectTransform imageRect;
	// Start is called before the first frame update
	void Start()
	{
		if (controlsImage != null)
		{
			AdjustView();
		}
	}

	protected void AdjustView()
	{
		if (imageRect != null)
		{
			switch (_showControlsUI)
			{
				case EShowControlsUI.Small:
					imageRect.SetHeight(Screen.height * 0.5f);
					break;
				case EShowControlsUI.Medium:
					imageRect.SetHeight(Screen.height * 0.8f);
					break;
				case EShowControlsUI.Fullscreen:
					imageRect.SetHeight(Screen.height);
					break;
				default: 
					break;
			}
		}
	}
}
