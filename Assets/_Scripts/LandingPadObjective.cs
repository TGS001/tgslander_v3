﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectiveMarker))]
public class LandingPadObjective : MonoBehaviour {
    public GameObject baseGroup;
    public GameObject workingGroup;
    public GameObject scoreGroup;
    public float requiredTime = 1;
    public float fuelSupply = 500;
    public float energySupply = 50;
    ObjectiveMarker marker;
    float timer = 0;
    bool landed = false;

	// Use this for initialization
	void Start () {
        marker = GetComponent<ObjectiveMarker>();
        Base();
	}

    void Base()
    {
        baseGroup.SetActive(true);
        workingGroup.SetActive(false);
        scoreGroup.SetActive(false);
    }

    void Score()
    {
        baseGroup.SetActive(false);
        workingGroup.SetActive(false);
        scoreGroup.SetActive(true);
    }

    void Working()
    {
        baseGroup.SetActive(false);
        workingGroup.SetActive(true);
        scoreGroup.SetActive(false);
    }

    private void Update()
    {
        if (landed)
        {
            timer += Time.deltaTime;
            if (timer > requiredTime)
            {
                marker.complete = true;
                Score();
                if (fuelSupply > 0)
                {
                    if (PlaySessionControl.player != null)
                    {
                        ThrustControl control = PlaySessionControl.player.GetComponent<ThrustControl>();
                        control.Refuel(fuelSupply);
                    }
                }
                if (energySupply > 0)
                {
                    if (PlaySessionControl.player != null)
                    {
                        Life.DoHeal(PlaySessionControl.player.gameObject, energySupply);
                    }
                }
                enabled = false;

            }
        }
    }

    public void OnLandingEnter()
    {
        landed = true;
        if (marker.complete == false)
        {
            Working();
        }
    }

    public void OnLandingExit()
    {
        landed = false;
        timer = 0;
        if (marker.complete == false)
        {
            Base();
        }
    }
}
