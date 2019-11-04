using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileInfo))]
public class MissileControl : MonoBehaviour {
    public float launchSpeed;
    public float hangtime;
    public float hangtimeVariance;
    public float fueltime;
    public float fueltimeVariance;
    public float burnoutTime;
    public float burnoutTimeVariance;
    public float steeringForce;
    public float thrusterForce;
    public float maxSpeed;
    public float closeRadius;
    public float explodeRadius;
    public float collideRadius;
    [Range(0, 1)]
    public float wanderAmount;
    public float wanderSpeed;
    public int splashDamage;
    public int directDamageBonus;
    public Transform nozzle;
    public SFX explosionEffect;
    public SFX impactEffect;
    public SFX dudEffect;
    public SFX fuseTrailEffect;
    public SFX jetFireEffect;
    public SFX burnoutEffect;

    RaycastHit2D[] moveResults = new RaycastHit2D[4];
    ProjectileInfo info;
    [SerializeField]
    [HideInInspector]
    float cutout = 0;
    [SerializeField]
    [HideInInspector]
    int stage = 0;
    [SerializeField]
    [HideInInspector]
    float wanderAngle = 0;

    Vector2 vel;

    void Start() {
        info = GetComponent<ProjectileInfo>();
        if ((stage == cutout) && (stage == 0)) {
            if (!nozzle) {
                nozzle = transform;
            }
            transform.position = info.origin;
            Vector2 launchDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * info.launchAngle), Mathf.Sin(Mathf.Deg2Rad * info.launchAngle));
            transform.LookAt(transform.TransformPoint(launchDirection));
            vel = launchDirection.normalized * launchSpeed + info.addVelocity;
            cutout = hangtime + Random.Range(-hangtimeVariance, hangtimeVariance);
            if (fuseTrailEffect) {
                SFX trail = SFX.Spawn(fuseTrailEffect, nozzle.position);
                trail.source = nozzle;
                trail.time = cutout;
            }
            wanderAngle = Random.Range(0, Mathf.PI * 2);
        }
    }

    void Dud() {
        if (dudEffect) {
            SFX fx = SFX.Spawn(dudEffect, transform.position);
            fx.magnitude = 1;
            fx.size = Mathf.Max(collideRadius, 1);
            fx.faction = info.faction;
        }
        Destroy(gameObject);
    }

    void Explode() {
        if (explosionEffect) {
            SFX fx = SFX.Spawn(explosionEffect, transform.position);
            fx.size = explodeRadius;
            fx.magnitude = splashDamage;
            fx.faction = info.faction;
        }
        Destroy(gameObject);
    }

    void DoMove(Vector2 amount) {
        int count = Physics2D.CircleCastNonAlloc(transform.position, collideRadius, amount, moveResults, amount.magnitude);
        RaycastHit2D result = moveResults[0];
        bool didHit = false;


        for (int i = 0; i < count; i++) {
            RaycastHit2D hit = moveResults[i];
            if (!Alliance.IsPartOf(hit.collider.gameObject, info.faction)) {
                result = hit;
                didHit = true;
                break;
            }
        }
        Vector3 position = transform.position;
        if (didHit) {
            transform.position = transform.position + Maths.tov3(amount * result.fraction);
            if (impactEffect) {
                SFX fx = SFX.Spawn(impactEffect, transform.position);
                fx.faction = info.faction;
                fx.magnitude = directDamageBonus;
                fx.size = 1;
                fx.normal = result.normal;
            }
            Life.DoDamage(result.collider.gameObject, directDamageBonus, (Vector2)result.collider.transform.position - result.point);
            Explode();
        } else {
            transform.position = transform.position + Maths.tov3(amount);
        }
    }

    void Update() {

        cutout -= Time.deltaTime;
        if (cutout <= 0) {
            //end of stage
            switch (stage) {
                case 0:
                    //hangtime ends
                    cutout = fueltime + Random.Range(-fueltimeVariance, fueltimeVariance);
                    if (jetFireEffect) {
                        SFX jet = SFX.Spawn(jetFireEffect, nozzle.position);
                        jet.source = nozzle;
                        jet.time = cutout;
                        jet.faction = info.faction;
                    }
                    break;

                case 1:
                    //fueltime ends
                    cutout = burnoutTime + Random.Range(-burnoutTimeVariance, burnoutTimeVariance);
                    if (burnoutEffect) {
                        SFX jet = SFX.Spawn(burnoutEffect, nozzle.position);
                        jet.source = nozzle;
                        jet.time = cutout;
                        jet.faction = info.faction;
                    }
                    break;

                default:
                    Dud();
                    break;
            }
            stage++;
        }

        Vector2 position = transform.position;
        if (stage == 1) {
            Vector2 targetPosition;
            if (info.target != null) {
                targetPosition = (Vector2)(info.target.position);
            } else {
                targetPosition = (Vector2)(info.destination);
            }
            Vector2 targetVector = (targetPosition - position);

            Debug.DrawRay(position, vel, Color.red);
            Debug.DrawLine(position, targetPosition, Color.cyan);

            Vector2 desiredVector;
            Vector2 wanderVector = (((Vector2)transform.forward) + (new Vector2(Mathf.Cos(wanderAngle), Mathf.Sin(wanderAngle)) * wanderAmount));
            wanderAngle += Random.Range(-wanderSpeed, wanderSpeed) * Time.deltaTime;

            desiredVector = targetVector.normalized + wanderVector.normalized;

            float angle = Mathf.Atan2(transform.forward.y, transform.forward.x) * Mathf.Rad2Deg;
            float desiredAngle = Mathf.Atan2(desiredVector.y, desiredVector.x) * Mathf.Rad2Deg;
            angle = Mathf.MoveTowardsAngle(angle, desiredAngle, Time.deltaTime * steeringForce);

            float mag = vel.magnitude;
            vel.x = Mathf.Cos(angle * Mathf.Deg2Rad) * mag;
            vel.y = Mathf.Sin(angle * Mathf.Deg2Rad) * mag;
            vel += vel.normalized * thrusterForce * Time.deltaTime;
        } else {
            vel += (Physics2D.gravity * Time.deltaTime * 3);
        }

        vel = Vector2.ClampMagnitude(vel, maxSpeed);

        transform.LookAt(position + vel);
        DoMove(vel * Time.deltaTime);
    }

}
