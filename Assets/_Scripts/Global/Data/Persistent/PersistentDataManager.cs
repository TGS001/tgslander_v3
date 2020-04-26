using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PersistentDataManager {
	public enum EPlayerDataStorageType
	{
		None,
		PlayerPrefs,
		LocalSQLLiteDB,
		ServerDB
	}
    public EPlayerDataStorageType playerDataStorage = EPlayerDataStorageType.PlayerPrefs;

    private PlayerData _playerData = new PlayerData(GGConst.DATA_PK_DEFAULT_PLAYER_PK);

    private void SaveCurrentPlayerData()
    {
        if (_playerData != null)
        {
            SavePlayerData(_playerData);
        }
    }

    public void CreateOrLoadPlayerData()
    {
        int lastPlayerPk = PlayerPrefs.GetInt(GGConst.DATA_KEY_CURRENT_PLAYER_PK, 0);
        if (lastPlayerPk < 1)
        {
            _playerData = CreateNewPlayer();
            SavePlayerData(_playerData);
        }
        else
        {
            LoadPlayerData(lastPlayerPk);
        }
    }

    public void LoadPlayerData(int pk)
    {
        string fileName = "";
        if (GlobalDataManager.useHumanReadableSaveFiles)
        {
            fileName = GGConst.SAVE_FILE_NAME_PLAYER_DATA + pk.ToString() + GGConst.SAVE_FILE_EXT_JSON;
            _playerData = PlayerDataSerializer.JsonLoad(fileName);
        }
        else
        {
            fileName = GGConst.SAVE_FILE_NAME_PLAYER_DATA + pk.ToString() + GGConst.SAVE_FILE_EXT_BINARY;
            _playerData = PlayerDataSerializer.BinaryLoad(fileName);
        }
        if (_playerData == null)
        {
            _playerData = CreateNewPlayer();
            SavePlayerData(_playerData);
            if (_playerData != null)
            {
                Debug.LogWarning("Data Manager: Created new user data after failed to load existing player data from file: " + fileName + ". May indicate lost data.");
            }
            else
            {
                Debug.LogError("Data Manager: Could not create user data for exiting user after failed to load existing player data from file: " + fileName);
            }
        }
        else
        {
            Debug.Log("Data Manager: loaded existing player data from file: " + fileName);
        }
    }

    public PlayerData CreateNewPlayer()
    {
        int newPk = GlobalDataManager.GetNextPersistentPK();
        PlayerData tempPlayerData = new PlayerData(newPk);
        PlayerPrefs.SetInt(GGConst.DATA_KEY_CURRENT_PLAYER_PK, newPk);
        GameTestPlayerData.SetInitialPlayerDataForTesting(ref tempPlayerData);
        Debug.Log("Data Manager: created new player data");
        return tempPlayerData;
    }

    public string GetSelectedLanderPK()
    {
        if (_playerData != null)
        {
            return _playerData.m_selectedLander;
        }
        return GGConst.DATA_PK_DEFAULT_LANDER_NAME;
    }

    public void SetSelectedLander(string landerName)
    {
        if (_playerData != null)
        {
            _playerData.m_selectedLander = landerName;
            SaveCurrentPlayerData();
        }
    }
    public LanderData GetSelectedLander()
    {
        if (_playerData != null)
        {
            return _playerData.selectedLander;
        }
        return null;
    }

    public void SetSelectedLander(LanderData selLander)
    {
        if (selLander!= null)
        {
            for(int i = 0; i < _playerData.landerConfigs.Count; i++)
            {
                if (_playerData.landerConfigs[i].name == selLander.name)
                {
                    _playerData.landerConfigs[i] = selLander;
                    _playerData.m_selectedLander = selLander.name;
                    SaveCurrentPlayerData();
                    return;
                }
            }

            // if it gets here the item is not in the list so add it
            _playerData.landerConfigs.Add(selLander);
            _playerData.m_selectedLander = selLander.name;
            SaveCurrentPlayerData();
        }
    }

    protected void SavePlayerData(PlayerData data)
    {
        if (data != null)
        {
            string fileName = "";
            if (GlobalDataManager.useHumanReadableSaveFiles)
            {
                fileName = GGConst.SAVE_FILE_NAME_PLAYER_DATA + data.pk.ToString() + GGConst.SAVE_FILE_EXT_JSON;
                PlayerDataSerializer.JsonSave(data, fileName);
            }
            else
            {
                fileName = GGConst.SAVE_FILE_NAME_PLAYER_DATA + data.pk.ToString() + GGConst.SAVE_FILE_EXT_BINARY;
                PlayerDataSerializer.BinarySave(data, fileName);
            }
            Debug.Log("Data Manager: saved player data to file: " + fileName);
        }
        else
        {
            Debug.Log("Data Manager: could not save player data to file. Player Data null. ");
        }
    }



    public int GetDataInt (string dataKey) {
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			return PlayerPrefs.GetInt(dataKey, 0);
		}
		Debug.LogWarning ("PersistentDataManager.GetPlayerDataInt not implemented yet for type: "
            + playerDataStorage.ToString());
		return 0;

	}
	public float GetDataFloat (string dataKey) {
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			return PlayerPrefs.GetFloat(dataKey, 0f);
		}
		Debug.LogWarning ("PersistentDataManager.GetPlayerDataFloat not implemented yet for type: "
            + playerDataStorage.ToString());
		return 0f;
	}
	public string GetDataString (string dataKey) {
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			return PlayerPrefs.GetString(dataKey, "");
		}
		Debug.LogWarning ("PersistentDataManager.GetPlayerDataString not implemented yet for type: "
            + playerDataStorage.ToString());
		return "";
	}
	public E GetDataEnum<E>(string dataKey) 
		where E : struct {

		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			string tempStr = PlayerPrefs.GetString(dataKey, "");
			if (tempStr != null && tempStr != "") {
				E curEnum;
				if (GlobalDataManager.EnumTryParse<E> (tempStr, out curEnum)) {
					return curEnum;
				}
			} else {
				return default(E);
			}
		}
		Debug.LogWarning ("PersistentDataManager.GetPlayerDataEnum not implemented yet for type: "
            + playerDataStorage.ToString());
		return default(E);
	}

	public void SetDataInt (string dataKey, int newValue) {
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			PlayerPrefs.SetInt(dataKey, newValue);
			return;
		}
		Debug.LogWarning ("PersistentDataManager.SetPlayerDataInt not implemented yet for type: "
            + playerDataStorage.ToString());
	}
	public void SetDataFloat (string dataKey, float newValue) {
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			PlayerPrefs.SetFloat(dataKey, newValue);
			return;
		}
		Debug.LogWarning ("PersistentDataManager.SetPlayerDataFloat not implemented yet for type: "
            + playerDataStorage.ToString());
	}
	public void SetDataString (string dataKey, string newValue) {
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			PlayerPrefs.SetString(dataKey, newValue);
			return;
		}
		Debug.LogWarning ("PersistentDataManager.SetPlayerDataString not implemented yet for type: "
            + playerDataStorage.ToString());
	}
	public void SetDataEnum<E>(string dataKey, E newValue) 
		where E : struct {
		string dataStr = newValue.ToString ();
		if (playerDataStorage == EPlayerDataStorageType.PlayerPrefs) {
			PlayerPrefs.SetString(dataKey, dataStr);
			return;
		}
		Debug.LogWarning ("PersistentDataManager.SetPlayerDataString not implemented yet for type: "
            + playerDataStorage.ToString());
	}
}
