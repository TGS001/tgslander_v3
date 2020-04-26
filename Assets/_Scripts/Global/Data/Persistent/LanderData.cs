using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanderData : PersistentData
{
    public string name = "DefaultLander";
    public string displayName = "Default Lander";
    public string partLandingGearPk;
    public string partHullPk;
    public string partWeaponPk;
    public string partStrutPk;
    public string partThrusterPk;
    public string partEnginePk;


    public LanderData(int newPk, string newName) : base(newPk)
    {
        pk = newPk;
        name = newName;
    }
}
