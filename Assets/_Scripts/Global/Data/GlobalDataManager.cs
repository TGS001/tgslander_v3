using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GlobalDataManager : MonoBehaviourSingleton<GlobalDataManager> {
    public static bool useHumanReadableSaveFiles = true;
    private static bool _isDataLoading = true;
    public static bool isDataLoading { get { return _isDataLoading; } }
    private static bool _isGameDataLoaded = false;
    public static bool isGameDataLoaded { get { return _isGameDataLoaded; } }
    private static bool _isPlayerDataLoaded = false;
    public static bool isPlayerDataLoaded { get { return _isPlayerDataLoaded; } }
 
    public static bool EnumTryParse<E>(string enumVal, out E resOut) 
		where E : struct
	{
		var enumValFxd = enumVal.Replace(' ', '_');
		if (Enum.IsDefined(typeof(E), enumValFxd))
		{
		  resOut = (E)Enum.Parse(typeof(E), 
		     enumValFxd, true);
		  return true;
		}
		// ----------------------------------------
		foreach (var value in
		  Enum.GetNames(typeof (E)).Where(value => 
		      value.Equals(enumValFxd, 
		      StringComparison.OrdinalIgnoreCase)))
		{
		  resOut = (E)Enum.Parse(typeof(E), value);
		  return true;
		}
		resOut = default(E);
		return false;
	}

	private PersistentDataManager _playerDataManager;
	private StaticDataManager _gameDataManager;

	private string currentPlayerID = "";

    public GameData defaultGameData;

	public static int GetNextPersistentPK ()
	{
		int tempPk = PlayerPrefs.GetInt (GGConst.DATA_KEY_LAST_PERSISTENT_PK, 0);
		tempPk++;
		PlayerPrefs.SetInt (GGConst.DATA_KEY_LAST_PERSISTENT_PK, tempPk);
		return tempPk;
	}

    public static int GetNextStaticDataVersionNum()
    {
        int tempVersionNum = PlayerPrefs.GetInt(GGConst.DATA_KEY_LAST_STATIC_VERSION, 0);
        tempVersionNum++;
        PlayerPrefs.SetInt(GGConst.DATA_KEY_LAST_STATIC_VERSION, tempVersionNum);
        return tempVersionNum;
    }

    public void HardResetPlayerData()
    {
        PlayerPrefs.SetInt(GGConst.DATA_KEY_CURRENT_PLAYER_PK, 0);
        PlayerPrefs.SetInt(GGConst.DATA_KEY_LAST_PERSISTENT_PK, 0);
        //TODO: delete any json player data files from the local data dir
        // may not be necessary as the file will be overwritten, I think
    }

    public void HardResetGameData()
    {
        PlayerPrefs.SetInt(GGConst.DATA_KEY_LAST_STATIC_VERSION, 0);
        //TODO: delete any json game data files from the local data dir
        // may not be necessary as the file will be overwritten
    }

    void Start() {
		if (_playerDataManager == null) {
            _playerDataManager = new PersistentDataManager();
		}
        if (_gameDataManager == null)
        {
            _gameDataManager = new StaticDataManager();
        }
        //Uncomment the following lines to reset the game data or playerdata to 
        // default (hard-coded)
//			HardResetGameData ();
//			HardResetPlayerData();
        InitialLoad();
    }

    public void InitialLoad()
    {
        _isDataLoading = true;
        _isGameDataLoaded = false;

        //TODO: using internal global default game data in the globalGameManager prefab in editor
        // but this can be switched to load it from a file later
        //bool isLoadedFromFile = _gameDataManager.CreateOrLoadGameData();
        _gameDataManager.AssignGameData(defaultGameData);
        _isGameDataLoaded = true;
        _playerDataManager.CreateOrLoadPlayerData();
        Debug.Log("GlobalDataManager.InitialLoad: Game and Player Data Loaded. ");
        _isPlayerDataLoaded = true;
        _isDataLoading = false;
    }

    public LanderData GetSelectedLanderData()
    {
        if (_playerDataManager != null)
        {
            return _playerDataManager.GetSelectedLander();
        }
        else
        {
            return null;
        }
    }
    public void SetSelectedLander(string landerName)
    {
        if (!string.IsNullOrEmpty(landerName) && _playerDataManager != null)
        {
            _playerDataManager.SetSelectedLander(landerName);
        }
    }

    public void SetSelectedLander(LanderData curLander)
    {
        if (_playerDataManager != null)
        {
            _playerDataManager.SetSelectedLander(curLander);
        }
    }

    //   public ECharacterSelection GetSelectedCharacter()
    //   {
    //       if (_playerDataManager != null)
    //       {
    //           return _playerDataManager.GetDataEnum<ECharacterSelection>(
    //               GGConst.PLAYER_DATA_KEY_CHARACTER_SELECT);
    //       }
    //       else {
    //           return ECharacterSelection.PhoenixWright;
    //       }
    //}
    //// defaults to Phoenix Wright for now - remove default when in production
    //public void SetSelectedCharacter(ECharacterSelection newChar = ECharacterSelection.PhoenixWright) {
    //       if (_playerDataManager != null)
    //       {
    //           _playerDataManager.SetDataEnum<ECharacterSelection>(
    //           GGConst.PLAYER_DATA_KEY_CHARACTER_SELECT, newChar);
    //       }
    //}

    public int GetPlayerDataInt (string dataKey) {
		return _playerDataManager.GetDataInt (dataKey);
	}
	public float GetPlayerDataFloat (string dataKey) {
		return _playerDataManager.GetDataFloat (dataKey);
	}
	public string GetPlayerDataString (string dataKey) {
		return _playerDataManager.GetDataString (dataKey);
	}

	public void SetPlayerDataInt (string dataKey, int newValue) {
		_playerDataManager.SetDataInt (dataKey, newValue);
	}
	public void SetPlayerDataFloat (string dataKey, float newValue) {
		_playerDataManager.SetDataFloat (dataKey, newValue);
	}
	public void SetPlayerDataString (string dataKey, string newValue) {
		_playerDataManager.SetDataString (dataKey, newValue);
	} 

}
