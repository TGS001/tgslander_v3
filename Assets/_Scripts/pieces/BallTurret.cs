using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTurret : MonoBehaviour {
    public enum IdleBehavior {
        stop,
        recenter,
        wander,
        follow
    };
    [Tooltip("the direction the turret faces while at rest.")]
    public Vector3 localFacing = Vector3.forward;
    [Tooltip("the transform that the turret should look at.")]
    public Transform targetTransform;
    [Tooltip("the position the turret should look at when it has no target transform.")]
    public Vector3 target;
    [Tooltip("the maximum amount the turret can rotate, in degrees.")]
    [Range(5, 90)]
    public float maxAngle = 45;
    [Tooltip("the distance the turret can see its target, in world units.")]
    public float sightDistance = 6;
    [Tooltip("the speed the turret rotates, in degrees per second.")]
    public float turnSpeed = 45;
    [Tooltip("how the turret acts when its target is out of its field of view.")]
    public IdleBehavior idleBehavior;
    public float wanderSpeed = 1;
    [Tooltip("the amount in degrees that the turret jitters while aiming")]
    [Range(0, 10)]
    public float waver;
    [Tooltip("the amount in degrees that the turret jitters while idling")]
    [Range(0, 10)]
    public float idleWaver;

    float wanderY;

    public Vector3 targetPosition {
        get {
            if (targetTransform == null) {
                return target;
            }
            return targetTransform.position;
        }
    }

    private void OnValidate() {
        localFacing.Normalize();
    }

    bool ParentPositionVisible(Vector3 localpos) {
        Vector3 offsetPos = (localpos - transform.localPosition);
        //Debug.DrawRay(transform.parent.position, transform.parent.TransformDirection(offsetPos), Color.black);
        //Debug.DrawRay(transform.parent.position, transform.parent.TransformDirection(localFacing), Color.green);
        //debugcos = Vector3.Dot(offsetPos.normalized, localFacing);
        //debugtcos = Mathf.Cos(Mathf.Deg2Rad * maxAngle);
        return Vector3.Dot(offsetPos.normalized, localFacing) > Mathf.Cos(Mathf.Deg2Rad * maxAngle) && offsetPos.magnitude < sightDistance;
    }

    public bool PositionVisible(Vector3 pos) {
        return ParentPositionVisible(transform.parent.InverseTransformPoint(pos));
    }

    private void OnDrawGizmosSelected() {


        Vector3 pitchAxis = Vector3.Cross(transform.parent.up, localFacing);
        Vector3 yawAxis = Vector3.Cross(localFacing, pitchAxis);

        Vector3 yawLeft = Quaternion.AngleAxis(-maxAngle, yawAxis) * localFacing * sightDistance;
        Vector3 yawRight = Quaternion.AngleAxis(maxAngle, yawAxis) * localFacing * sightDistance;

        Vector3 pitchUp = Quaternion.AngleAxis(-maxAngle, pitchAxis) * localFacing * sightDistance;
        Vector3 pitchDown = Quaternion.AngleAxis(maxAngle, pitchAxis) * localFacing * sightDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, pitchUp);
        Gizmos.DrawRay(transform.position, pitchDown);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, yawLeft);
        Gizmos.DrawRay(transform.position, yawRight);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * sightDistance);

        Vector3 tpos = targetPosition;
        if (PositionVisible(tpos)) {
            Gizmos.color = Color.cyan;
        } else {
            Gizmos.color = Color.yellow;
        }
        Gizmos.DrawLine(transform.position, tpos);
    }

    Quaternion idlequat;

    private void Start() {
        Vector3 lookPos = transform.parent.TransformDirection(localFacing);
        wanderY = Random.value * 1000;
        idlequat = Quaternion.identity;
    }

    // Update is called once per frame
    void Update() {
        Vector3 tpos = targetPosition;
        if (PositionVisible(tpos)) {
            Quaternion wq = Quaternion.Euler(Mathf.PerlinNoise(Time.time * wanderSpeed, wanderY) * waver, Mathf.PerlinNoise(wanderY, Time.time * wanderSpeed) * waver, 0);
            Quaternion lookr = Quaternion.LookRotation(tpos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookr, turnSpeed * Time.deltaTime);
        } else {
            //Gizmos.color = Color.yellow;
            Quaternion wq = Quaternion.Euler(Random.Range(-1, 1) * idleWaver, Random.Range(-1, 1) * idleWaver, 0);
            switch (idleBehavior) {
                case IdleBehavior.stop:
                    break;
                case IdleBehavior.recenter: {
                        Quaternion lookr = Quaternion.LookRotation(localFacing);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookr, turnSpeed * Time.deltaTime);
                    }
                    break;
                case IdleBehavior.wander:
                    break;
                case IdleBehavior.follow:
                    break;
                default:
                    break;
            }
        }
    }
}
