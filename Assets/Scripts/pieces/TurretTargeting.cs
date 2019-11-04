using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargeting : MonoBehaviour
{
    public string targetTag = "Player";
    public float maxRange = 10;
    public float fireAngle = 5;
    private TurretAim[] turretAims;
    private Transform target;

    int groundMask;
    // Use this for initialization
    void Start()
    {
        turretAims = GetComponentsInChildren<TurretAim>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");
        if (fireAngle > 0)
        {
            float fireCos = Mathf.Cos(Mathf.Deg2Rad * fireAngle);
            foreach (TurretAim turret in turretAims)
            {
                turret.fireCos = fireCos;
            }
        }
    }

    public void AimAt(Transform t)
    {
        target = t;
        foreach (TurretAim turret in turretAims)
        {
            turret.target = t;
        }
    }

    public void StopAim()
    {
        if (target != null)
        {
            target = null;
            foreach (TurretAim turret in turretAims)
            {
                turret.target = null;
            }
        }
    }

    private void FixedUpdate()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        GameObject best = null;
        float bestDistance = maxRange;
        float distance;
        foreach (GameObject cur in targets)
        {
            distance = Vector2.Distance(cur.transform.position, transform.position);
            if (distance < bestDistance)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, cur.transform.position - transform.position, distance, groundMask);
                if (hit.collider == null)
                {

                    bestDistance = distance;
                    best = cur;
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                }
                
            }
        }
        if (best != null)
        {
            AimAt(best.transform);
        }
        else
        {
            StopAim();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
