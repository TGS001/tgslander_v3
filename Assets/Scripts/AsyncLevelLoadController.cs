using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLevelLoadController : MonoBehaviour {
    public string levelName;
    public string loadingSceneName = "LoadingScreen";
    static bool isLoading = false;

    public void onSceneLoaded(Scene to, LoadSceneMode mode)
    {
        Debug.Log("Loaded " + to.name + " as " + mode.ToString());
        if (to.name.Equals(loadingSceneName))
        {
            StartCoroutine(loadAsync(levelName));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= onSceneLoaded;
    }

    IEnumerator loadAsync(string level)
    {
        yield return null;
        yield return SceneManager.LoadSceneAsync(level);
        Destroy(gameObject);
        isLoading = false;
        
    }

	public bool loadLevel()
    {
        if (!isLoading)
        {
            isLoading = true;
            DontDestroyOnLoad(gameObject);
            Time.timeScale = 1;
            SceneManager.LoadScene(loadingSceneName);
            
        }
        else
        {
            return false;
        }
        return true;
    }

    public static bool LoadLevel(string levelName, string loadingSceneName = "LoadingScreen")
    {
        if (isLoading)
        {
            return false;
        }
        Debug.Log("trying to load level " + levelName);
        GameObject llo = new GameObject("level loader");
        AsyncLevelLoadController con = llo.AddComponent<AsyncLevelLoadController>();
        con.levelName = levelName;
        con.loadingSceneName = loadingSceneName;
        con.loadLevel();
        return true;
    }

    internal static bool ReloadLevel(string loadingSceneName = "LoadingScreen")
    {
        if (isLoading)
        {
            return false;
        }
        /*
        ModularLander lander = FindObjectOfType<ModularLander>();
        if (lander != null)
        {
            GameObject lio = new GameObject("perts");
            DemoPieceSelector selector = lio.AddComponent<DemoPieceSelector>();
        }
        */

        string levelName = SceneManager.GetActiveScene().name;
        Debug.Log("trying to load level " + levelName);
        GameObject llo = new GameObject("level loader");
        AsyncLevelLoadController con = llo.AddComponent<AsyncLevelLoadController>();
        con.levelName = levelName;
        con.loadingSceneName = loadingSceneName;
        con.loadLevel();
        return true;
    }
}
