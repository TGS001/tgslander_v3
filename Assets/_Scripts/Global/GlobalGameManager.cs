using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviourSingletonPersistent<GlobalGameManager>
{
    public LanderPieceSelector landerPartSelector;
    public ModularLander currentLanderModule;

    public enum EGameSceneType
    {
        Default,
        Start,
        Splash,
        Loading,
        DebugStart,
        LevelSelect,
        Workshop,
        GameplayLevel,
        GameplayResults
    }

    public EGameSceneType currentScene;
    public int levelNameSuffixDigits = 1;
    public int defaultFirstGameplayLevelToLoad = 1;

    public int levelsPerGameplaySession = 3;
    public List<int> defaultGameplayLevelSet = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public List<int> currentGameplayLevelSet = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private int nextLevelIndex = 0;
    public bool isInDebugMode = false;

    private GlobalDataManager gameDataManager;

    public override void Initialize()
    {
        base.Initialize();
        gameDataManager = GetComponent<GlobalDataManager>();
        if (GlobalDataManager.Instance.GetSelectedLanderData() == null)
        {
            GlobalDataManager.Instance.SetSelectedLander(GGConst.DATA_PK_DEFAULT_LANDER_NAME);
        }
    }

    public int GetCurrentLevelNum()
    {
        return nextLevelIndex;
    }

    public string GetLevelName(int levelnum)
    {
        string tempSuffix = levelnum.ToString();
        if (levelnum < 0)
        {
            Debug.LogError("GlobalGameManager.GetLevelName Error: Cannot have negative level numbers.");
        }
        if (levelNameSuffixDigits == 1)
        {
            if (levelnum < 10)
            {
                tempSuffix = levelnum.ToString();
            }
            else
            {
                Debug.LogError("GlobalGameManager.GetLevelName Error: Level number cannot have more digits than setting allows");
            }
        }
        else if (levelNameSuffixDigits == 2)
        {
            if (levelnum < 10)
            {
                //zero fill the number string
                tempSuffix = "0" + levelnum.ToString();
            }
            else if (levelnum < 100)
            {
                tempSuffix = levelnum.ToString();
            }
            else
            {
                Debug.LogError("GlobalGameManager.GetLevelName Error: Level number cannot have more digits than setting allows");
            }

        }
        else
        {
            Debug.LogWarning("GlobalGameManager.GetLevelName Error: level naming digits not matching hard-coded assumptions. May need to adjust code to handle.");
        }
        return GGConst.SCENE_NAME_LEVEL_PREFIX + tempSuffix;
    }

    public string GetSceneName(EGameSceneType sceneType, int levelNum = 0)
    {
        switch (sceneType)
        {
            case EGameSceneType.Start:
                return GGConst.SCENE_NAME_START;
            case EGameSceneType.DebugStart:
                return GGConst.SCENE_NAME_DEBUG_START;
            case EGameSceneType.Splash:
                return GGConst.SCENE_NAME_SPLASH;
            case EGameSceneType.Loading:
                return GGConst.SCENE_NAME_LOADING;
            case EGameSceneType.LevelSelect:
                return GGConst.SCENE_NAME_LEVEL_SELECT;
            case EGameSceneType.Workshop:
                return GGConst.SCENE_NAME_WORKSHOP;
            case EGameSceneType.GameplayLevel:
                string levelName = GetLevelName(levelNum);
                return levelName;
            default:
                return "";
        }
    }

    public void ChangeScene(EGameSceneType sceneType, int levelNum = 0)
    {
        string sceneName = GetSceneName(sceneType, levelNum);
        SceneManager.LoadScene(sceneName);
        currentScene = sceneType;
    }

    public void GoToGameplay()
    {
        int currentLevel = defaultFirstGameplayLevelToLoad;
        if (nextLevelIndex >= 0 && nextLevelIndex < currentGameplayLevelSet.Count)
        {
            currentLevel = currentGameplayLevelSet[nextLevelIndex];
            ChangeScene(EGameSceneType.GameplayLevel, currentLevel);
            nextLevelIndex++;
        }
        else
        {
            FinishedGameSession();
        }
    }

    public void FinishedGameSession()
    {
        nextLevelIndex = 0;
        ChangeScene(EGameSceneType.Start);

    }
    public void SetupCurrentLanderWithParts(ModularLander curLander)
    {
        currentLanderModule = curLander;
        if (landerPartSelector == null)
        {
            landerPartSelector = GameObject.FindObjectOfType<LanderPieceSelector>();
        }
        if (landerPartSelector != null && currentLanderModule != null)
        {
            landerPartSelector.SetupLander(currentLanderModule);
        }
    }
}
