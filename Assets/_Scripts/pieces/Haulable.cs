using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Haulable : MonoBehaviour {
    public SFX fuelBeamEffect;
    public SFX energyBeamEffect;
    public SFX depleteEffect;
    public float fuel = 0;
    public float energy = 0;
    public float fuelRate = 50;
    public float energyRate = 10;
    public bool destroyOnEmpty = false;
    TractorBeam beam;
    SFX fuelBeam;
    SFX energyBeam;

    public Rigidbody2D body {
        get {
            return _body;
        }
    }

    public bool energyEmpty {
        get {
            return (energy <= 0);
        }
    }

    public bool fuelEmpty {
        get {
            return (fuel <= 0);
        }
    }

    public bool empty {
        get {
            return energyEmpty && fuelEmpty;
        }
    }
    Rigidbody2D _body;
    // Use this for initialization
    void Start() {
        _body = GetComponent<Rigidbody2D>();
    }

    void DoDeplete() {
        if (empty) {
            if (depleteEffect) {
                SFX fx = SFX.Spawn(depleteEffect, transform.position);
                fx.size = Maths.CalculateSize(this);
                depleteEffect = null;
                fx.magnitude = 1;
            }
            if (destroyOnEmpty) {
                Destroy(gameObject);
            }
        }
    }

    public float PullEnergy(float request) {
        DoDeplete();
        if (!energyEmpty) {
            if (request > float.Epsilon) {
                float drainedEnergy =
                    Mathf.Max(Mathf.MoveTowards(energy, 0, Time.deltaTime * energyRate),
                    energy - request);
                float pulledEnergy = energy - drainedEnergy;
                energy = drainedEnergy;
                if (energyBeam) {
                    if (energyEmpty || pulledEnergy <= 0) {
                        energyBeam.Stop();
                        energyBeam = null;
                    }
                } else {
                    if (pulledEnergy > 0) {
                        energyBeam = Instantiate(energyBeamEffect);
                        energyBeam.source = transform;
                        energyBeam.destination = beam.transform;
                    }
                }
                return pulledEnergy;
            } else {
                if (energyBeam) {
                    energyBeam.Stop();
                    energyBeam = null;
                }
            }
        } else {
            if (energyBeam) {
                energyBeam.Stop();
                energyBeam = null;
            }
        }
        return 0;
    }

    public float PullFuel(float request) {
        DoDeplete();
        if (!fuelEmpty) {
            if (request > float.Epsilon) {
                float drainedFuel =
                    Mathf.Max(Mathf.MoveTowards(fuel, 0, Time.deltaTime * fuelRate),
                    fuel - request);
                float pulledFuel = fuel - drainedFuel;
                if (pulledFuel > 0) {
                    fuel = drainedFuel;
                }

                if (fuelBeam) {
                    if (fuelEmpty || pulledFuel <= 0) {
                        fuelBeam.Stop();
                        fuelBeam = null;
                    }
                } else {
                    if (pulledFuel > 0) {
                        fuelBeam = Instantiate(fuelBeamEffect);
                        fuelBeam.source = transform;
                        fuelBeam.destination = beam.transform;
                    }
                }
                return pulledFuel;
            } else {
                if (fuelBeam) {
                    fuelBeam.Stop();
                    fuelBeam = null;
                }
            }
        } else {
            if (fuelBeam) {
                fuelBeam.Stop();
                fuelBeam = null;
            }
        }
        return 0;
    }

    public void Grab(TractorBeam beam) {
        this.beam = beam;
    }

    public void Release() {
        this.beam = null;
        if (fuelBeam) {
            fuelBeam.Stop();
            fuelBeam = null;
        }
        if (energyBeam) {
            energyBeam.Stop();
            energyBeam = null;
        }
    }
}
