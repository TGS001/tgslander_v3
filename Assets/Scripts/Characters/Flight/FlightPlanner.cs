using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlightAI), typeof(ThrustControl))]
public class FlightPlanner : MonoBehaviour {
    public enum Plan {
        idle,
        followA,
        followALookatB,
        attackA,
        attackADefendB,
        moveA,
        moveALookatB,
        engineOff
    }
    FlightAI ai;
    ThrustControl control;
    FlightPathMap map;

    public Plan plan;
    public Transform A;
    public Transform B;
    public float followDistanceMin = 3;
    public float followDistanceMax = 6;
    public float defendRadius = 10;
    [Range(0, 90)]
    public float circlingAngle = 10;
    public bool ignoreVerticalPreference = false;
    int lastPlan = -1;
    FlightPathMap.Region lastRegion;
    Vector2 smoothVelocity;

    private void OnDrawGizmosSelected() {
        switch (plan) {
            case Plan.idle:
                break;
            case Plan.followA:
                break;
            case Plan.followALookatB:
                break;
            case Plan.attackA:
                break;
            case Plan.attackADefendB:
                if (B) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(B.position, defendRadius);
                }
                break;
            case Plan.moveA:
                break;
            case Plan.moveALookatB:
                break;
            case Plan.engineOff:
                break;
            default:
                break;
        }
    }

    private void OnEnable() {
        ai = GetComponent<FlightAI>();
        control = GetComponent<ThrustControl>();
        map = FindObjectOfType<FlightPathMap>();
    }

    bool PathwayCheck(Vector2 position) {
        Vector2 castDirection = position - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, ai.pathingRadius, castDirection, castDirection.magnitude);
        return (hit.collider == null);
    }

    bool VisibilityCheck(Vector2 position) {
        Vector2 castDirection = position - (Vector2)transform.position;
        Physics2D.Raycast(transform.position, castDirection, castDirection.magnitude);
        RaycastHit2D[] res = Physics2D.RaycastAll(transform.position, castDirection, castDirection.magnitude, ai.clearanceMask);
        foreach (RaycastHit2D hit in res) {
            if (hit.transform != transform.root) {
                return false;
            }
        }
        return true;
    }

    private bool VisibilityCheck(Transform a) {
        Vector2 castDirection = a.position - transform.position;
        Physics2D.Raycast(transform.position, castDirection, castDirection.magnitude);
        RaycastHit2D[] res = Physics2D.RaycastAll(transform.position, castDirection, castDirection.magnitude, ai.clearanceMask);
        foreach (RaycastHit2D hit in res) {
            if (hit.transform == a.root) {
                return true;
            } else {
                if (hit.transform != transform.root) {
                    return false;
                }
            }
        }
        return true;
    }

    void EndPlan(Plan plan) {
        lastRegion = null;
    }

    void StartPlan(Plan plan) {
        if (A == null) {
            if (plan != Plan.engineOff) {
                plan = Plan.idle;
            }
            return;
        }
        switch (plan) {
            case Plan.idle:
                ai.Stay();
                break;

            case Plan.followALookatB:
                if (B == null) {
                    plan = Plan.followA;
                }
                break;

            case Plan.attackADefendB:
                if (B == null) {
                    plan = Plan.attackA;
                }
                break;

            case Plan.moveALookatB:
                if (B == null) {
                    plan = Plan.moveA;
                }
                break;

            case Plan.engineOff:
                break;

            default:
                break;
        }
    }

    void TickPlan(Plan plan) {
        if (plan != Plan.engineOff && A == null) {
            plan = Plan.idle;
        }
        switch (plan) {
            case Plan.idle:
                break;
            case Plan.followA:
                TickFollowA();
                break;
            case Plan.followALookatB:
                TickFollowALookatB();
                break;
            case Plan.attackA:
                TickAttackA();
                break;
            case Plan.attackADefendB:
                TickAttackADefendB();
                break;
            case Plan.moveA:
                TickMoveA();
                break;
            case Plan.moveALookatB:
                TickMoveALookatB();
                break;
            case Plan.engineOff:
                break;
            default:
                break;
        }
    }

    public bool GoToPoint(Vector2 destination) {
        FlightPathMap.Region acr = map.ClosestRegion(destination);
        if (acr != lastRegion || !ai.pathing) {
            if (VisibilityCheck(destination)) {
                FlightPath path = new FlightPath();
                path.nodes = new List<FlightPathNode>();
                path.nodes.Add(new FlightPathNode(FlightAI.FlightMode.followPath, destination, 0, 0));
                ai.ExecutePath(path);
            } else {
                ai.Pathfind(destination, 0);
            }
            lastRegion = acr;
            return true;
        } else {
            ai.path.SetLastNodePosition(acr.PointInside(destination, ai.pathingRadius));
        }
        return false;
    }

    void Face(Vector2 position) {
        if (ai.preferVertical) {
            if (ignoreVerticalPreference) {
                ai.Face(position);
            }
        } else {
            ai.Face(position);
        }
    }

    void FaceIfVisible(Transform t) {
        if (VisibilityCheck(t)) {
            Face(t.position);
        }
    }

    private void TickFollowALookatB() {
        TickFollowA();
        FaceIfVisible(B);
    }

    private void TickAttackA() {
        Vector2 aofs = (transform.position - A.position);
        Vector2 rotator = new Vector2(-aofs.y, aofs.x).normalized;
        Debug.DrawRay(A.position, rotator, Color.yellow);
        Debug.DrawRay(A.position, control.smoothedAcceleration, Color.magenta);
        Debug.DrawRay(A.position, aofs, Color.white);
        if (Vector2.Dot(rotator, control.smoothedAcceleration) > 0) {
            aofs = Maths.rotatev2(aofs, circlingAngle);
        } else {
            aofs = Maths.rotatev2(aofs, -circlingAngle);
        }
        Debug.DrawRay(A.position, aofs, Color.black);
        float followDistance = Mathf.Clamp(aofs.magnitude, followDistanceMin, followDistanceMax);
        GoToPoint(aofs.normalized * (followDistance) + (Vector2)A.position);
        ai.path.noApproach = true;

        FaceIfVisible(A);

    }

    private void TickAttackADefendB() {
        float abd = Vector2.Distance(A.position, B.position);
        if (abd < defendRadius) {
            Vector2 aofs = (B.position - A.position);
            float followDistance = Mathf.Min(Mathf.Clamp(aofs.magnitude, followDistanceMin, followDistanceMax), abd/2);
            GoToPoint(aofs.normalized * (followDistance) + (Vector2)A.position);
            FaceIfVisible(A);
        } else {
            TickAttackA();
        }
    }

    private void TickMoveA() {
        GoToPoint(A.position);
    }

    private void TickMoveALookatB() {
        TickMoveA();
        FaceIfVisible(B);
    }

    private void TickFollowA() {
        Vector2 aofs = (transform.position - A.position);
        aofs.y *= 0.75f;
        float followDistance = Mathf.Clamp(aofs.magnitude, followDistanceMin, followDistanceMax);
        GoToPoint(aofs.normalized * (followDistance) + (Vector2)A.position);
    }

    private void FixedUpdate() {
        smoothVelocity = Vector2.Lerp(smoothVelocity, control.velocity, Time.fixedDeltaTime);
        if ((int)plan != lastPlan) {
            EndPlan((Plan)lastPlan);
            StartPlan(plan);
            lastPlan = (int)plan;
        }
        TickPlan(plan);
    }
}
