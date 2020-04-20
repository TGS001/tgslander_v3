using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : PersistentData {
    //[System.Serializable]
    //public class LanderSelectionClass: StringSerializableEnum<GlobalDataManager.ECharacterSelection> { }

    //    public LanderSelectionClass m_selectedLander;
    public string m_selectedLander;

    public List<LanderData> landerConfigs;

    public LanderData selectedLander
    {
        get
        {
            if (!string.IsNullOrEmpty(m_selectedLander))
            {
                foreach (LanderData curLander in landerConfigs)
                {
                    if (m_selectedLander == curLander.name)
                    {
                        return curLander;
                    }
                }
            }
            return null;
        }
    }

    public PlayerData(int newPk) : base(newPk) {
        if (newPk == 0)
        {
            pk = GGConst.DATA_PK_DEFAULT_PLAYER_PK;
            m_selectedLander = GGConst.DATA_PK_DEFAULT_LANDER_NAME;
        }
        else
        {
            pk = newPk;
        }
	}
}
