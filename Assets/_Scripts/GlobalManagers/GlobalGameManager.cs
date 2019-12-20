using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : MonoBehaviourSingletonPersistent<GlobalGameManager>
{
    public LanderPieceSelector landerPartSelector;
    public ModularLander currentLanderModule;

    public void SetupCurrentLanderWithParts(ModularLander curLander)
    {
        currentLanderModule = curLander;
        if (landerPartSelector == null)
        {
            landerPartSelector = GameObject.FindObjectOfType<LanderPieceSelector>();
        }
        if (landerPartSelector != null && currentLanderModule != null)
        {
            landerPartSelector.SetupLander(currentLanderModule);
        }
    }
}
