using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDataManager {

    private  GameData _gameData;

    public T GetDataByPk<T>(string curPk) where T : StaticData
    {
        if (!GlobalDataManager.isGameDataLoaded)
        {
            Debug.LogWarning("StaticDataManager.GetDataByPk<" + typeof(T).Name + "> Warning: Game Data not finished loading. Try again later.");
            return null;
        }
        if (_gameData == null)
        {
            Debug.LogError("StaticDataManager.GetDataByPk<" + typeof(T).Name + "> Error: no Game Data loaded");
            return null;
        }
        List<T> curList = _gameData.GetList<T>();
        if (curList != null)
        {
            for (int i = 0; i < curList.Count; i++)
            {
                if (curList[i].pk == curPk)
                {
                    return curList[i] as T;
                }
            }
            Debug.LogError("StaticDataManager.GetDataByPk<" + typeof(T).Name + "> Error: Could not find item in sub list with matching pk (" + curPk + ") for this type in Game Data");
        }
        else
        {
            Debug.LogError("StaticDataManager.GetDataByPk<" + typeof(T).Name + "> Error: Could not find sub list for this type in Game Data");
        }
        
        return null;
    }

    public bool CreateOrLoadGameData()
    {
        int lastVersion = PlayerPrefs.GetInt(GGConst.DATA_KEY_LAST_STATIC_VERSION, 0);
        if (lastVersion < 1)
        {
            // if no existing game static data, use hardcoded data
            _gameData = GameTestGameData.CreateNewGameData();
            // this will only be needed for initial setup while developing:
            GameTestGameData.SaveGameData(_gameData);
            return false;
        }
        else
        {
            return LoadGameData(lastVersion);
        }

    }

    public void AssignGameData(GameData extGameData)
    {
        if (_gameData == null)
        {
            _gameData = extGameData;
        }
    }

    public bool LoadGameData(int version)
    { /*
        string fileName = "";
        if (GlobalDataManager.useHumanReadableSaveFiles)
        {
            fileName = CJConst.SAVE_FILE_NAME_GAME_DATA + GGConst.SAVE_FILE_EXT_JSON;
            _gameData = GameDataSerializer.JsonLoadFromResourceTextFile(fileName);
            if (_gameData == null)
            {
                _gameData = GameDataSerializer.JsonLoad(fileName);
                if (_gameData != null)
                {
                    Debug.LogWarning("StaticDataManager: could not loaded existing game data from Resources file but was able to load from save data location: " + fileName);
                }
            }
            else
            {
                Debug.Log("StaticDataManager: loaded existing game data from Resources file: " + fileName);
            }
        }
        else
        {
            fileName = GGConst.SAVE_FILE_NAME_GAME_DATA + GGConst.SAVE_FILE_EXT_BINARY;
            _gameData = GameDataSerializer.BinaryLoad(fileName);
        }
        if (_gameData == null)
        {
            _gameData = CJTestGameData.CreateNewGameData();
            CJTestGameData.SaveGameData(_gameData);
            if (_gameData != null)
            {
                Debug.LogWarning("StaticDataManager: Created new game data after failed to load existing game data from file: " + fileName + ". May indicate lost data.");
            }
            else
            {
                Debug.LogError("StaticDataManager: Could not create game data after failed to load existing game data from file: " + fileName);
            }
            return false;
        }
        else
        {
            if (version == _gameData.versionNum)
            {
                Debug.Log("StaticDataManager: successfully loaded existing game data matching version: " + version);
            }
            else
            {
                Debug.LogWarning("StaticDataManager: loaded existing game data from file: " + fileName
                    + " but version number mismatch found: expected (" + version.ToString()
                    + "), loaded (" + _gameData.versionNum.ToString() + ")"
                );
            }
            return true;
        }
        */
        return false;
    }

}
