using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData : StaticData {
	public int versionNum = 1;
    public List<LevelData> gameLevels;
    public List<LandingGearPartData> landingGearParts;
    public List<HullPartData> hullParts;
    public List<WeaponPartData> weaponParts;
    public List<StrutPartData> strutParts;
    public List<ThrusterPartData> thrusterParts;
    public List<EnginePartData> engineParts;

    public GameData(string newPk) : base(newPk)
    {
        gameLevels = new List<LevelData>();
        landingGearParts = new List<LandingGearPartData>();
        hullParts = new List<HullPartData>();
        weaponParts = new List<WeaponPartData>();
        strutParts = new List<StrutPartData>();
        thrusterParts = new List<ThrusterPartData>();
        engineParts = new List<EnginePartData>();
    }

    public List<T> GetList<T>() where T : StaticData {
        
        if (typeof(T) == typeof(LevelData)) { 
			return gameLevels as List<T>;
        }
        else if (typeof(T) == typeof(LandingGearPartData))
        {
            return landingGearParts as List<T>;
        }
        else if (typeof(T) == typeof(HullPartData))
        {
            return hullParts as List<T>;
        }
        else if (typeof(T) == typeof(WeaponPartData))
        {
            return weaponParts as List<T>;
        }
        else if (typeof(T) == typeof(StrutPartData))
        {
            return strutParts as List<T>;
        }
        else if (typeof(T) == typeof(ThrusterPartData))
        {
            return thrusterParts as List<T>;
        }
        else if (typeof(T) == typeof(EnginePartData))
        {
            return engineParts as List<T>;
        }
        return null;
	}
}
