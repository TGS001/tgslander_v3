using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileInfo))]
public class LaserBeam : MonoBehaviour {
    public Transform destGraphic;
    public LineRenderer beam;
    public SFX damageFX;
    public SFX extinguishFX;
    public float onSeconds;
    public int queryFrameSkip;
    public float damage;
    public float effectSize;
    ProjectileInfo info;
    float destroyTime;
    int frameSkip = 0;
    int traceMask;

    private void Start() {
        info = GetComponent<ProjectileInfo>();
        if (info.launchPoint) {
            traceMask = ~(1 << info.launchPoint.gameObject.layer);
        }

        if (info.launchPoint) {
            transform.SetParent(info.launchPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        destroyTime = Time.time + onSeconds;
        if (destGraphic) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, float.MaxValue, traceMask);
            if (hit && hit.collider) {
                Vector3 hitpoint = new Vector3(hit.point.x, hit.point.y, transform.position.z);
                destGraphic.position = hitpoint;
            } else {
                Vector3 hitpoint = transform.position + transform.forward * 100;
                destGraphic.position = hitpoint;
            }
            destGraphic.gameObject.SetActive(true);
        }
    }

    private void FixedUpdate() {
        if (frameSkip-- == 0) {
            frameSkip = queryFrameSkip;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, float.MaxValue, traceMask);
            if (hit && hit.collider) {
                Life l = hit.collider.GetComponentInParent<Life>();
                if (l && !Alliance.IsPartOf(hit.collider.gameObject, info.faction)) {
                    float damagePerSecond = damage / onSeconds;
                    float outputDamage = damagePerSecond * Time.fixedDeltaTime * (queryFrameSkip + 1);
                    l.Afflict(outputDamage, 1, -hit.normal);
                    if (damageFX) {
                        SFX fx = SFX.Spawn(damageFX, hit.point);
                        fx.normal = hit.normal;
                        fx.size = effectSize;
                        fx.magnitude = damagePerSecond;
                    }
                }
            }
        }
        if (Time.time > destroyTime) {
            if (extinguishFX) {
                SFX fx = SFX.Spawn(extinguishFX, transform.position);
                fx.source = info.launchPoint;
                fx.normal = transform.forward;
                fx.size = effectSize;
                fx.magnitude = damage/onSeconds;
            }
            Destroy(gameObject);
        }
    }

    private void LateUpdate() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward, float.MaxValue, traceMask);
        if (hit && hit.collider) {
            Vector3 hitpoint = new Vector3(hit.point.x, hit.point.y, transform.position.z);
            if (destGraphic)
                destGraphic.position = hitpoint;
            if (beam) {
                beam.SetPosition(0, transform.position);
                beam.SetPosition(1, hitpoint);
            }
        } else {
            Vector3 hitpoint = transform.position + transform.forward * 100;
            if (destGraphic)
                destGraphic.position = hitpoint;
            if (beam) {
                beam.SetPosition(0, transform.position);
                beam.SetPosition(1, hitpoint);
            }
        }
    }
}
