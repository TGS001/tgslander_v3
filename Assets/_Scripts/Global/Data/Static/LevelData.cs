using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData : StaticData
{
    public enum EDifficulty
    {
        Easy,
        Medium,
        Hard
    }
    
    public string sceneName = "Level00";
    public EDifficulty difficulty = EDifficulty.Easy;
    public LevelData(string newPk, string newSceneName) : base(newPk)
    {
        sceneName = newSceneName;
    }
}
