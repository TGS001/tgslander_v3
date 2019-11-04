using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AstronautWorkspace))]
public class AstronautAction : MonoBehaviour
{
    [Range(0, 100)]
    public float priority = 50;
    public bool interruptable = true;
    public int maxRoles = 0;
    public string[] tags;
    int actors;
    protected AstronautWorkspace workSpace;

    public static int CompareActions(AstronautAction action1, AstronautAction action2)
    {
        float res = action2.priority - action1.priority;
        
        if (res == 0)
        {
            return 0;
        }
        if (res > 0)
        {
            return 1;
        }
        return -1;
    }

    protected void Initialize()
    {
        workSpace = GetComponent<AstronautWorkspace>();
        workSpace.AddAction(this);
    }

    private void Start()
    {
        Initialize();
    }

    public bool MatchesTags(AstronautController astro)
    {
        if (tags == null || tags.Length == 0)
        {
            return true;
        }
        foreach (string s in tags)
        {
            if (astro.CompareTag(s))
            {
                return true;
            }
        }
        return false;
    }

    virtual public int CompareAstronauts(AstronautController astro1, AstronautController astro2)
    {
        return 0;
    }

    virtual public bool Possible()
    {
        return false;
    }

    virtual public bool Complete(AstronautController astro)
    {
        return false;
    }

    virtual public int RolesRemaining()
    {
        return maxRoles - actors;
    }

    virtual public void StartAction(AstronautController astro)
    {
        actors += 1;
    }

    virtual public void TickAction(AstronautController astro, out bool complete, out float delay)
    {
        complete = true;
        delay = 0;
    }

    virtual public void FinishAction(AstronautController astro)
    {
        actors -= 1;
    }
}
