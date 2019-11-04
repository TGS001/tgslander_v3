using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FlightAI))]
public class FlightAIEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (Application.isPlaying) {
            if (GUILayout.Button("begin pathfinding")) {
                FlightAI ai = (FlightAI)target;
                ai.Pathfind(ai.motionTarget, 0);
            }
        }
    }
}
#endif

[RequireComponent(typeof(ThrustControl))]
public class FlightAI : MonoBehaviour {
    public enum FlightMode {
        verticalStay,
        freeStay,
        followPath,
        powerOff
    }

    public float pathingRadius = 1;
    public float arriveDistance = 0.5f;
    [Range(0, 1)]
    public float steeringCorrection = 0;
    public bool preferVertical = false;
    public FlightMode mode = FlightMode.verticalStay;
    public Vector2 motionTarget;
    public bool targetIsStartPosition = true;
    public float cruiseSpeed = 3;
    public bool avoidTerrain = true;
    [Range(0, 1)]
    public float angularThrottle = 0.5f;
    public bool arrived = false;
    public bool danger = false;
    public FlightPath path;
    public int pathingStage = 0;
    public bool pathing = false;
    public Vector2 pathingBarrier = Vector2.zero;
    ThrustControl control;
    float lastVError = 0;
    float nodeCruiseSpeed = float.MaxValue;
    public float nodeRadius = 0;
    int collideMask;
    Vector2 facingTarget;
    //Vector2 lastDeltaV;
    bool overrideFacing;
    FlightPathMap map;
    FlightPathNode currentNode;

    public void Face(Vector2 target) {
        overrideFacing = true;
        facingTarget = target;
    }

    public int clearanceMask {
        get {
            return collideMask;
        }
    }

    private void OnEnable() {
        map = FindObjectOfType<FlightPathMap>();
    }

    float speedLimit {
        get {
            return Mathf.Min(nodeCruiseSpeed, cruiseSpeed);
        }
    }

    public void Pathfind(Vector2 targetPoint, float speed, bool includeTargetPosition = true) {
        if (map) {
            ExecutePath(map.CreatePath(transform.position, targetPoint, pathingRadius, speed, includeTargetPosition));
        }
    }

    private void OnDrawGizmosSelected() {
        if (danger) {
            Gizmos.color = Color.red;
        } else if (Application.isPlaying && arrived) {
            Gizmos.color = Color.green;
        } else {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawWireSphere(transform.position, pathingRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, arriveDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, motionTarget);

        path.DrawGizmo(pathingStage - 1);

        if (pathingBarrier != Vector2.zero) {
            Vector2 barrierPos = motionTarget - pathingBarrier * (arriveDistance + control.linearBrakingDistance);
            Vector2 barrierRight = barrierPos + new Vector2(-pathingBarrier.y, pathingBarrier.x) * 4;
            Vector2 barrierLeft = barrierPos + new Vector2(pathingBarrier.y, -pathingBarrier.x) * 4;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(barrierRight, barrierLeft);
        }
    }

    // Use this for initialization
    void Start() {
        if (targetIsStartPosition) {
            motionTarget = transform.position;
            targetIsStartPosition = false;
        }
        control = GetComponent<ThrustControl>();
        collideMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
    }

    Vector2 Seek(Vector2 offset) {
        return offset.normalized * speedLimit;
    }

    Vector2 Approach(Vector2 offset) {
        float sd = control.linearBrakingDistance + arriveDistance;
        float distance = offset.magnitude;
        if (distance < sd) {
            return Vector2.zero;
        } else if (distance < sd * 2) {
            return Seek(offset) * ((distance - sd) / sd);
        }
        return Seek(offset);
    }

    public void Stay() {
        if (preferVertical) {
            mode = FlightMode.verticalStay;
        } else {
            mode = FlightMode.freeStay;
        }
        pathing = false;
    }

    public void Stay(Vector2 position) {
        motionTarget = position;
        Stay();
    }

    public void ExecutePath(FlightPath path) {
        if (path.nodes.Count > 0) {
            pathingBarrier = Vector2.zero;
            pathing = true;
            pathingStage = 0;
            this.path = path;
        }
    }

    void AdvancePathing() {
        if (pathing && (pathingStage == 0 || arrived)) {
            FlightPathNode nextNode;
            if (path.GetNode(pathingStage, out nextNode)) {
                motionTarget = nextNode.targetPosition;
                pathingBarrier = (motionTarget - (Vector2)transform.position).normalized;
                nodeRadius = nextNode.radius;
                mode = nextNode.mode;
                if (nextNode.cruiseSpeed > 0) {
                    nodeCruiseSpeed = nextNode.cruiseSpeed;
                } else {
                    nodeCruiseSpeed = cruiseSpeed;
                }

                pathingStage++;
                currentNode = nextNode;
            } else {
                pathingBarrier = Vector2.zero;
                pathing = false;
                nodeCruiseSpeed = float.MaxValue;
                nodeRadius = 0;
                pathingStage = 0;
                Stay();
            }
        }
    }

    void FixedUpdate() {

        Vector2 motionTargetOffset = motionTarget - (Vector2)transform.position;
        if (pathing) {
            //if (pathingStage == path.nodes.Count || pathingBarrier != Vector2.zero) {
            if (pathingBarrier != Vector2.zero) {
                float dotDistance = Vector2.Dot(pathingBarrier, motionTargetOffset);
                arrived = dotDistance < (control.linearBrakingDistance + arriveDistance * 1.3f);
                //arrived = motionTargetOffset.sqrMagnitude < (arriveDistance * arriveDistance * 1.3f);
            } else {
                arrived = motionTargetOffset.sqrMagnitude < (arriveDistance * arriveDistance * 1.3f);
            }
            AdvancePathing();
        } else {
            arrived = motionTargetOffset.sqrMagnitude < (arriveDistance * arriveDistance * 1.3f);
        }
        Vector2 velocity = control.velocity;
        Vector2 desiredVelocity = Vector2.zero;
        bool vertical = preferVertical;

        RaycastHit2D dangerProbe;
        if (avoidTerrain && mode != FlightMode.powerOff) {
            dangerProbe = Physics2D.CircleCast(transform.position, pathingRadius, velocity, control.linearBrakingDistance * 1.5f);
        } else {
            dangerProbe = new RaycastHit2D();
        }
        if (dangerProbe && dangerProbe.collider) {
            danger = true;
            //desiredVelocity = dangerProbe.normal * cruiseSpeed * 2;
            Geom.Line dangerLine = new Geom.Line(transform.position, motionTarget);
            desiredVelocity = ((dangerLine.Closest(dangerProbe.point) - dangerProbe.point) + dangerProbe.normal).normalized * cruiseSpeed;
        } else {
            danger = false;
            switch (mode) {
                case FlightMode.verticalStay:
                    desiredVelocity = Approach(motionTargetOffset);
                    vertical = true;
                    break;
                case FlightMode.freeStay:
                    desiredVelocity = Approach(motionTargetOffset);
                    vertical = arrived;
                    break;
                case FlightMode.followPath:

                    FlightPathNode lnode;
                    if (pathing) {
                        if (pathingStage > 1) {
                            lnode = path.nodes[pathingStage - 2];
                        } else {
                            lnode = new FlightPathNode(FlightMode.followPath, transform.position, 0, 0);
                        }
                        if (pathingStage > 0) {
                            motionTarget = path.nodes[pathingStage - 1].targetPosition;
                        }
                    } else {
                        lnode = new FlightPathNode(FlightMode.followPath, transform.position, 0, 0);
                    }
                        Debug.DrawLine(motionTarget, lnode.targetPosition, Color.green);
                        Geom.Line pathLine = new Geom.Line(lnode.targetPosition, motionTarget);
                        float prad = Mathf.Max(nodeRadius, pathingRadius + 1);
                        float maxError = prad - (pathingRadius);
                        Vector2 driftPosition = control.linearBrakingStop;
                        Vector2 lineClosest = pathLine.Closest(driftPosition);
                        float driftError = Vector2.Distance(lineClosest, driftPosition);
                        float det = (driftError - maxError) / (prad - maxError);//(driftError - maxError) / Mathf.Max(radError, 0.1f);
                        //Debug.DrawLine(transform.position, transform.position + Vector3.right + Vector3.up * det, Color.yellow);
                        //Debug.DrawLine(transform.position, transform.position + Vector3.right + Vector3.up, Color.red);
                        //Debug.DrawLine(transform.position, lineClosest, Color.white);
                        //Debug.DrawLine(transform.position, driftPosition, Color.black);
                        //Vector2 destOffset = Vector2.Lerp(motionTarget, lineClosest, det) - (Vector2)transform.position;
                        if (pathingStage == path.nodes.Count && path.noApproach == false) {
                            desiredVelocity = Vector2.Lerp(Approach(motionTargetOffset), Seek(lineClosest - driftPosition), det);
                        } else {
                            desiredVelocity = Vector2.Lerp(Seek(motionTargetOffset), Seek(lineClosest - driftPosition), det);
                        }
                        //Debug.DrawRay(transform.position, desiredVelocity, Color.green);
                    break;
                default:
                    pathing = false;
                    control.SetAngleControl(0, 0);
                    control.SetLinearControl(Vector2.zero, 0);
                    return;
            }
            //desiredVelocity *= Mathf.Clamp01(Vector2.Dot(velocity.normalized, desiredVelocity.normalized));
        }

        Vector2 deltaVelocity = desiredVelocity - velocity;
        float angle;
        if (overrideFacing) {
            Vector2 facingDelta = facingTarget - (Vector2)transform.position;
            angle = (Mathf.Rad2Deg * Mathf.Atan2(facingDelta.y, facingDelta.x)) - 90;
            overrideFacing = false;
        } else {
            if (!vertical) {
                angle = (Mathf.Rad2Deg * Mathf.Atan2(deltaVelocity.y, deltaVelocity.x)) - 90;
            } else {
                angle = 0;
            }
        }

        control.SetAngleControl(angle, angularThrottle);


        float vError = deltaVelocity.magnitude;
        float errorDerivative = vError - lastVError;
        control.SetLinearControl(deltaVelocity, vError + errorDerivative);
        lastVError = vError;
    }
}
