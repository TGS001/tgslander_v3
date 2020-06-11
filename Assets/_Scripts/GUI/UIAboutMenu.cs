using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAboutMenu : MonoBehaviour
{
	public Text versionText;
    // Start is called before the first frame update
    void Start()
    {
        if (versionText != null)
		{
			versionText.text = GGConst.GAME_VERSION_PREFIX + Application.version;
		}
    }
}
