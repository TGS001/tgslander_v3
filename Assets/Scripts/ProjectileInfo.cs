using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alliance))]
public class ProjectileInfo : MonoBehaviour {
    public Transform target;
    public Vector3 destination;
    public Vector3 origin;
    public float launchAngle;
    public Transform launchPoint;
    public Vector2 addVelocity;
    public AllyGroup faction = AllyGroup.None;


    void Start() {
        if (target != null) {
            destination = target.position;
        }

        if (launchPoint != null) {
            gameObject.layer = launchPoint.gameObject.layer;
            Vector2 launchVector = ((Vector2)launchPoint.forward).normalized;
            launchAngle = Mathf.Atan2(launchVector.y, launchVector.x) * Mathf.Rad2Deg;
            Alliance a = GetComponent<Alliance>();
            if (a) {
                a.faction = faction;
            }
            origin = launchPoint.position;
        }
    }
}
