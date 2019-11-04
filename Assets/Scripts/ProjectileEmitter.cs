using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEmitter : MonoBehaviour
{
    public ProjectileInfo projectile;
    public SFX launchEffect;
    Rigidbody2D body;
    AllyGroup faction;

    // Use this for initialization
    void Start()
    {
        body = GetComponentInParent<Rigidbody2D>();
        faction = Alliance.GetAlliance(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire(Vector3 target, Transform t)
    {
        if (projectile)
        {
            ProjectileInfo bullet = Instantiate(projectile);
            if (body != null)
            {
                bullet.addVelocity = body.velocity;
            }
            bullet.launchPoint = transform;
            bullet.target = t;
            bullet.destination = target;
            bullet.faction = faction;
        }
        if (launchEffect)
        {
            SFX effect = SFX.Spawn(launchEffect, transform.position);
            effect.source = transform;
            effect.normal = transform.forward;
        }
    }
}
