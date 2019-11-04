using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueAction : AstronautAction {
    ModularLander lander;

	// Use this for initialization
	void Start () {
        Initialize();
        lander = FindObjectOfType<ModularLander>();
	}

    public override bool Possible()
    {
        return lander.Landed() && workSpace.col.OverlapPoint(lander.transform.position);
    }

    public override bool Complete(AstronautController astro)
    {
        return false;
    }

    public override int RolesRemaining()
    {
        return int.MaxValue;
    }

    public override int CompareAstronauts(AstronautController astro1, AstronautController astro2)
    {
        return Mathf.FloorToInt(Mathf.Abs(astro2.transform.position.x - lander.transform.position.x) - Mathf.Abs(astro1.transform.position.x - lander.transform.position.x));
    }

    public override void TickAction(AstronautController astro, out bool complete, out float delay)
    {
        base.TickAction(astro, out complete, out delay);
        if (!Possible())
        {
            complete = true;
            delay = 0;
            return;
        }
        Vector3 position = lander.transform.position;
        Vector3 astropos = astro.transform.position;
        float hdelta = (position.x - astropos.x);
        astro.Walk(hdelta);
        complete = false;
        delay = 1;
    }

    public override void FinishAction(AstronautController astro)
    {
        base.FinishAction(astro);
        astro.Walk(0);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
