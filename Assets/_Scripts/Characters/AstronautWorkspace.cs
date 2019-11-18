using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AstronautWorkspace : MonoBehaviour
{
    public float frequency = 8;
    internal BoxCollider2D col;

    [ReadOnly]
    public List<AstronautController> astronauts;
    protected int astronautLayer;

    private List<AstronautAction> actions = new List<AstronautAction>();
    private List<AstronautController> freeAstronauts = new List<AstronautController>();

    private float left
    {
        get
        {
            return col.bounds.min.x;
        }
    }

    internal void AddAction(AstronautAction astronautAction)
    {
        actions.Add(astronautAction);
        actions.Sort(AstronautAction.CompareActions);
    }

    private float right
    {
        get
        {
            return col.bounds.max.x;
        }
    }

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        astronautLayer = LayerMask.NameToLayer("Astronauts");
        InvokeRepeating("Assign", 0, 1/Mathf.Max(frequency, 0.01f));
    }

    void Assign()
    {
        Debug.DrawRay(transform.position, Vector2.up);
        for (int i = 0; i < astronauts.Count;) {
            if (astronauts[i] == null) {
                astronauts.RemoveAt(i);
                continue;
            }
            i++;
        }
        freeAstronauts.Clear();
        foreach (AstronautController astro in astronauts)
        {
            if (astro.interruptable)
            {
                freeAstronauts.Add(astro);
            }
        }
        foreach (AstronautAction action in actions)
        {
            if (action.Possible() && action.RolesRemaining() > 0)
            {
                freeAstronauts.Sort(action.CompareAstronauts);
                foreach (AstronautController astro in freeAstronauts)
                {
                    if (astro.action != action &&
                        astro.interruptable &&
                        astro.priority < action.priority &&
                        action.MatchesTags(astro) &&
                        !action.Complete(astro))
                    {
                        astro.SetAction(action);
                        if (action.RolesRemaining() == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    bool AstronautIsNull(AstronautController astro)
    {
        return (astro == null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == astronautLayer)
        {
            AstronautController con = other.GetComponent<AstronautController>();
            if (con != null)
            {
                astronauts.Add(con);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == astronautLayer)
        {
            AstronautController con = other.GetComponent<AstronautController>();
            if (con != null)
            {
                con.SetAction(null);
                astronauts.Remove(con);
            }
        }
    }
}
