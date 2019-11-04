using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAim : MonoBehaviour {
    public Transform yaw;
    public Transform pitch;
    public Transform target;

    public float closeAngle = 10;
    public float yawSpeed = 45;
    public float pitchSpeed = 45;

    private float _fireCos = 0;
    private GunController[] myGuns;

    public float fireCos {
        set {
            _fireCos = value;
            myGuns = GetComponentsInChildren<GunController>();
        }
    }

    // Update is called once per frame
    void Update() {
        if (target != null) {
            Vector3 forward = pitch.forward;
            Vector3 targetOffset = (target.position - pitch.position);
            Vector3 targetForward = targetOffset.normalized;
            Vector3 yel = yaw.localEulerAngles;
            Vector3 pel = pitch.localEulerAngles;

            Vector3 yawLocal;
            if (yaw.parent) {
                yawLocal = yaw.parent.InverseTransformPoint(target.position);
            } else {
                yawLocal = target.position;
            }

            float yawTarget = Mathf.Atan2(yawLocal.x, yawLocal.z) * Mathf.Rad2Deg;
            yel.y = Mathf.MoveTowardsAngle(yel.y, yawTarget, Time.deltaTime * yawSpeed);
            yaw.localEulerAngles = yel;

            Vector3 pitchLocal;
            if (pitch.parent) {
                pitchLocal = pitch.parent.InverseTransformPoint(target.position);
            } else {
                pitchLocal = target.position;
            }

            float pitchTarget = Mathf.Atan2(-pitchLocal.y, pitchLocal.z) * Mathf.Rad2Deg;
            pel.x = Mathf.MoveTowardsAngle(pel.x, pitchTarget, Time.deltaTime * pitchSpeed);
            pitch.localEulerAngles = pel;

            if ((myGuns != null) && (myGuns.Length > 0)) {
                float offsetCosine = Vector3.Dot(pitch.forward, targetForward);
                if (offsetCosine > _fireCos) {
                    foreach (GunController gun in myGuns) {
                        gun.Fire(target.position, target);
                    }
                }
            }
        }
    }
}
