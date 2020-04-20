using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTestGameData {
    public static GameData CreateNewGameData()
    {
        GameData tempData = new GameData(GGConst.DATA_PK_DEFAULT_GAME_ROOT);
        tempData.versionNum = GlobalDataManager.GetNextStaticDataVersionNum();
        SetInitialGameDataForTesting(ref tempData);
        Debug.Log("GameTestGameData: created new game data holder");
        return tempData;
    }

    public static void SetInitialGameDataForTesting(ref GameData testData)
    {
        if (testData != null)
        {
            //TODO: add other initial game data classes and create an initial record for each

        }
    }
    // this is only for initial setup of the data file and should never be called at runtime in the released game
    // Game Data is static at runtime, should eventually come from the server and never need to be saved locally
    // using editor only define to make sure
    public static void SaveGameData(GameData data)
    {
#if UNITY_EDITOR
        if (data != null)
        {
            string fileName = "";
            if (GlobalDataManager.useHumanReadableSaveFiles)
            {
                fileName = GGConst.SAVE_FILE_NAME_GAME_DATA + GGConst.SAVE_FILE_EXT_JSON;
                GameDataSerializer.JsonSave(data, fileName);
            }
            else
            {
                fileName = GGConst.SAVE_FILE_NAME_GAME_DATA + GGConst.SAVE_FILE_EXT_BINARY;
                GameDataSerializer.BinarySave(data, fileName);
            }

            Debug.Log("Data Manager: saved game data to file: " + fileName);
        }
        else
        {
            Debug.Log("Data Manager: could not save game data to file. Game Data null. ");
        }
#endif
    }
}
