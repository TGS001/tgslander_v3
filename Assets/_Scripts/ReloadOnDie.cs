using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadOnDie : MonoBehaviour {
    public float delaySeconds = 2;
    public string level = "reload_same";

    IEnumerator loadLevel()
    {
        yield return new WaitForSeconds(delaySeconds);
        if (level.Equals("reload_same"))
        {
            AsyncLevelLoadController.LoadLevel(SceneManager.GetActiveScene().name);
        }
        else
        {
            AsyncLevelLoadController.LoadLevel(level);
        }
    }

    public void onDie()
    {
        StartCoroutine(loadLevel());
    }

    private void Start()
    {
        Life m = GetComponent<Life>();
        if (m != null)
        {
            m.Register(onDie);
        }
    }
}
