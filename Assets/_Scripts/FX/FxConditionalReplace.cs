using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxConditionalReplace : MonoBehaviour {
    
    public enum Condition {
        random,
        inWater
    }
    public SFX replacement;
    public Condition condition;
    [Range(0,1)]
    public float randomChance = 0.5f;
	
    public bool shouldReplace(Vector3 spawnPoint) {
        switch (condition) {
            case Condition.random:
                return Random.value < randomChance;
            case Condition.inWater:
                return Water.Submerged(spawnPoint);
            default:
                break;
        }
        return false;
    }
}
