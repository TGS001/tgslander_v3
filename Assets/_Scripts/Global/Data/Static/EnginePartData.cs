using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnginePartData : LanderPartData
{
    public float thrust = 4;
    public Vector3 throttleFactor = new Vector3(1, 1, 1);
    public Vector3 flickerFactor = new Vector3(0, 0, 0);
    public Vector3 burstScale = new Vector3(1, 1, 1);
    public float burstSeconds = 0.5f;
    public float throttleDownSeconds = 0.5f;
    public float rotation = 1;
    public EnginePartData(string newPk) : base(newPk)
    {
    }

}
