using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProjectileInfo))]
public class BulletControl : MonoBehaviour {
    [Tooltip("random launch angle")]
    public float scatter;
    [Tooltip("minimum launch speed")]
    public float launchSpeedMin = 1;
    [Tooltip("maximum launch speed")]
    public float launchSpeedMax = 2;
    [Tooltip("minimum range of the bullet")]
    public float rangeMin = 5;
    [Tooltip("maximum range of the bullet")]
    public float rangeMax = 6;
    [Tooltip("radius of the bullet")]
    public float bulletRadius = 0.1f;
    [Tooltip("proportion of gravity effect")]
    public float gravity = 1;
    [Tooltip("damage applied to object collided with")]
    public float directDamage = 10;
    [Tooltip("damage applied in area")]
    public float areaDamage = 10;
    [Tooltip("radius of explosion")]
    public float explodeRadius = 1;
    [Tooltip("special effect applied on impact")]
    public SFX impactFX;
    [Tooltip("special effect applied on explosion")]
    public SFX explodeFX;
    [Tooltip("special effect applied on range timeout")]
    public SFX dudFX;

    [HideInInspector]
    public ProjectileInfo info;
    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public float rangeRemaining;
    [HideInInspector]
    public int collisionMask;
    [HideInInspector]
    public Vector3 movePoint;

    void Start () {
        info = GetComponent<ProjectileInfo>();
        if (info.launchPoint) {
            transform.position = info.launchPoint.position;
            transform.rotation = info.launchPoint.rotation;
            collisionMask = ~(1 << info.launchPoint.gameObject.layer);
        } else {
            collisionMask = ~0;
        }
        if (scatter > 0) {
            transform.Rotate(Vector3.forward, Random.Range(-scatter, scatter));
        }
        Vector2 launch = ((Vector2)transform.forward).normalized * Random.Range(launchSpeedMin, launchSpeedMax);
        
        velocity = info.addVelocity + launch;
        
        rangeRemaining = Random.Range(rangeMin, rangeMax);
    }

    void Explode(bool isDud) {
        SFX emitFX;
        if (isDud) {
            if (dudFX) {
                emitFX = dudFX;
            } else {
                emitFX = explodeFX;
            }
        } else {
            emitFX = explodeFX;
        }
        if (emitFX) {
            SFX fx = SFX.Spawn(emitFX, movePoint);
            fx.size = explodeRadius;
            fx.magnitude = areaDamage;
            fx.velocity = velocity;
        }
        Destroy(gameObject);
    }

    private void Update() {
        transform.position = Vector2.MoveTowards(transform.position, movePoint, velocity.magnitude * Time.deltaTime);
    }

    void FixedUpdate () {
        float speed = Mathf.Min(velocity.magnitude * Time.fixedDeltaTime, rangeRemaining);
        Vector2 nvel = velocity.normalized;
        
        bool shouldExplode = false;
        bool isDud = false;
        if (speed > 0) {
            rangeRemaining -= speed;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, bulletRadius, nvel, speed, collisionMask);
            
            Vector3 displacement;
            if (hit && hit.collider) {
                displacement = (hit.fraction * (Vector3)nvel);
                movePoint = transform.position + displacement;
                if (impactFX) {
                    SFX fx = SFX.Spawn(impactFX, movePoint);
                    fx.normal = hit.normal;
                    fx.magnitude = directDamage;
                    Life.DoDamage(hit.collider.gameObject, directDamage, -hit.normal);
                }
                shouldExplode = true;
            } else {
                displacement = (speed * (Vector3)nvel);
                movePoint = transform.position + displacement;
            }
            transform.LookAt(movePoint);
        } else {
            shouldExplode = true;
            isDud = true;
        }
        velocity += Physics2D.gravity * Time.fixedDeltaTime;
        if (shouldExplode) {
            Explode(isDud);
        }
	}

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, bulletRadius);
    }
}
