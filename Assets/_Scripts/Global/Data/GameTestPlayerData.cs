using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTestPlayerData {
	// Testing data function
	public static void SetInitialPlayerDataForTesting (ref PlayerData testPlayerData)
	{
		if (testPlayerData != null) {
            if (testPlayerData.landerConfigs == null)
            {
                testPlayerData.landerConfigs = new List<LanderData>();
            }
            if (testPlayerData.landerConfigs.Count == 0)
            {
                LanderData defLander = new LanderData(GGConst.DATA_PK_DEFAULT_LANDER_PK, GGConst.DATA_PK_DEFAULT_LANDER_NAME);
                defLander.displayName = GGConst.DATA_PK_DEFAULT_LANDER_DNAME;
                defLander.partLandingGearPk = GGConst.DATA_PK_DEFAULT_LANDER_PART_LG_PK;
                defLander.partHullPk = GGConst.DATA_PK_DEFAULT_LANDER_PART_HULL_PK;
                defLander.partWeaponPk = GGConst.DATA_PK_DEFAULT_LANDER_PART_WEAPON_PK;
                defLander.partStrutPk = GGConst.DATA_PK_DEFAULT_LANDER_PART_STRUT_PK;
                defLander.partThrusterPk = GGConst.DATA_PK_DEFAULT_LANDER_PART_THRUSTER_PK;
                defLander.partEnginePk = GGConst.DATA_PK_DEFAULT_LANDER_PART_ENGINE_PK;
            }
        }
    }
}