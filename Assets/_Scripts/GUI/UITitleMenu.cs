using UnityEngine;
using UnityEngine.UI;

public class UITitleMenu : MonoBehaviour
{
	public Text titleTextControl;
	public Image backgroundImage;
	public Text companyName;
	public bool hasDynamicBackgound = false;
	// Start is called before the first frame update
	void Start()
	{
		if (hasDynamicBackgound)
		{
			if (titleTextControl != null)
			{
				titleTextControl.gameObject.SetActive(false);
			}
			if (backgroundImage != null)
			{
				backgroundImage.gameObject.SetActive(false);
			}
		}
		if (AudioManager.Instance != null && !AudioManager.Instance.isMusicPlaying())
		{
			AudioManager.Instance.PlayMusicForScene(GGConst.SCENE_NAME_START);
		}
	}
}
