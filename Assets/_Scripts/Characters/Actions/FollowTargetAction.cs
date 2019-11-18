using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowTargetAction : AstronautAction
{
    public Transform follow;
    public float closeDistance = 0.5f;

    public override bool Possible()
    {
        return workSpace.col.OverlapPoint(follow.position);
    }

    public override bool Complete(AstronautController astro)
    {
        return Mathf.Abs(astro.transform.position.x - follow.position.x) < closeDistance;
    }

    public override int CompareAstronauts(AstronautController astro1, AstronautController astro2)
    {
        return Mathf.FloorToInt(Mathf.Abs(astro2.transform.position.x - follow.position.x) - Mathf.Abs(astro1.transform.position.x - follow.position.x));
    }

    override public void TickAction(AstronautController astro, out bool complete, out float delay)
    {
        base.TickAction(astro, out complete, out delay);
        if (follow == null || !Possible() || Complete(astro))
        {
            complete = true;
            delay = 0;
            return;
        }
        Vector3 friendlyPosition = follow.position;
        Vector3 astroPosition = astro.transform.position;
        float hdelta = (friendlyPosition.x - astroPosition.x);
        if (Mathf.Abs(hdelta) < closeDistance)
        {
            astro.Walk(0);
            complete = true;
            delay = 0;
            return;
        }

        astro.Walk(hdelta);
        complete = false;
        delay = 0.2f;
    }

    override public void FinishAction(AstronautController astro)
    {
        base.FinishAction(astro);
        astro.Walk(0);
    }
}
