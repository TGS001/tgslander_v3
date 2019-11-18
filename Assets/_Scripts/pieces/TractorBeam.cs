using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TractorBeam : MonoBehaviour {
    public float maxTowLength;
    public float maxTowForce;
    public SFX beamEffect;
    public SFX failBeamEffect;
    public ThrustControl fuelTank;
    public Life lifeTank;

    [SerializeField]
    [HideInInspector]
    Haulable target;
    [SerializeField]
    [HideInInspector]
    DistanceJoint2D joint;

    Rigidbody2D body;

    SFX beam;

    public bool connected {
        get {
            return (target != null && joint != null);
        }
    }

    private void Update() {
        if (target) {
            if (lifeTank) {
                float maxRequest = lifeTank.maxHitpoints - lifeTank.hitpoints;
                float pulledEnergy = target.PullEnergy(maxRequest);
                if (pulledEnergy > 0)
                    lifeTank.Afflict(-pulledEnergy);
            }

            if (fuelTank) {
                float maxRequest = fuelTank.maxFuel - fuelTank.fuel;
                float pulledFuel = target.PullFuel(maxRequest);
                if (pulledFuel > 0)
                    fuelTank.Refuel(pulledFuel);
            }
        }
    }

    public void Attach(Haulable target) {
        if (!connected) {
            Vector2 com = body.worldCenterOfMass;
            Vector2 tcom = target.body.worldCenterOfMass;
            float distance = (com - tcom).magnitude;
            if (distance < maxTowLength) {
                joint = gameObject.AddComponent<DistanceJoint2D>();
                joint.autoConfigureDistance = false;
                joint.autoConfigureConnectedAnchor = false;

                joint.anchor = body.centerOfMass;
                joint.connectedAnchor = target.GetComponent<Rigidbody2D>().centerOfMass;

                joint.maxDistanceOnly = true;
                joint.connectedBody = target.body;
                joint.distance = maxTowLength;

                joint.enableCollision = false;
                joint.breakForce = maxTowForce;
                joint.enabled = true;
                this.target = target;
                this.target.Grab(this);

                beam = Instantiate(beamEffect);
                beam.source = transform;
                beam.destination = target.transform;
            } else if (distance < maxTowLength * 2) {
                SFX failbeam = Instantiate(failBeamEffect);
                failbeam.source = transform;
                failbeam.destination = target.transform;
                failbeam.time = 0.3f;
            }
        }
    }

    public void Detach() {
        if (connected) {
            Destroy(joint);
            joint = null;
            target.Release();
            target = null;
            beam.Stop();
        }
    }

    void OnJointBreak2D(Joint2D brokenJoint) {
        beam.Stop();
        target.Release();
        target = null;
        joint = null;
    }

    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody2D>();
    }
}
