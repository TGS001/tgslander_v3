using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    [Range(0, 1)]
    public float damageReductionPercent = 0.5f;
    public PhysicsMaterial2D material;
    public GameObject shieldNode;
    public SFX impactEffect;
    public bool shieldOn;
    [SerializeField]
    float rad;
    public float radius
    {
        get
        {
            return rad;
        }
        set
        {
            SetRadius(value);
        }
    }
    
    MeshRenderer graphic;

    

    float shieldPing = 0;
    float shieldUp = 0;
    float shieldThickness = 0;

    public bool on
    {
        get
        {
            return shieldOn;
        }
        set
        {
            shieldOn = value;
            UpdateShieldStatus();
        }
    }

    void UpdateShieldStatus()
    {
        if (shieldNode != null)
        {
            CircleCollider2D collider = shieldNode.GetComponent<CircleCollider2D>();
            if (shieldOn)
            {
                if (collider == null)
                {
                    collider = shieldNode.AddComponent<CircleCollider2D>();
                    collider.radius = 0.5f;
                    collider.sharedMaterial = material;
                }
            }
            else
            {
                if (collider != null)
                {
                    Destroy(collider);
                    collider = null;
                }
            }
        }
    }

    Life life;

    private void OnEnable()
    {
        life = GetComponent<Life>();
        life.Register(OnAfflict);
        if (shieldNode != null)
        {
            graphic = shieldNode.GetComponent<MeshRenderer>();
        }
        UpdateShieldStatus();
    }

    private void OnDisable()
    {
        life.Unregister(OnAfflict);
    }

    void OnAfflict(ref Life.Damage damage)
    {
        if (shieldOn)
        {
            if (damage.amount > 0)
            {
                damage.multiplier -= damageReductionPercent;
            }
            shieldPing = Mathf.Clamp(damage.finalAmount / (life.maxHitpoints * 0.6f), shieldPing, 1);
            if (damage.normal.sqrMagnitude > 0.01f)
            {
                SFX fx = SFX.Spawn(impactEffect, shieldNode.transform.position);
                fx.size = rad;
                fx.magnitude = Mathf.Clamp(damage.finalAmount / (life.maxHitpoints * 0.2f), 0.2f, 1);
                fx.normal = damage.normal;
                fx.source = shieldNode.transform;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        if (on)
        {
            Debug.DrawRay(transform.position, transform.up, Color.red);
        }
        if (graphic != null)
        {
            Material mat = graphic.material;
            mat.SetFloat("_Ping", shieldPing);
            mat.SetFloat("_ShieldUp", shieldUp);
            mat.SetFloat("_ShieldThickness", shieldThickness);
            shieldPing = Mathf.MoveTowards(shieldPing, 0, Time.deltaTime * 10);
            shieldThickness = Mathf.MoveTowards(shieldThickness, Mathf.Clamp01(life.percent + 0.3f), Time.deltaTime);
            if (shieldOn)
            {
                shieldUp = Mathf.MoveTowards(shieldUp, 1, Time.deltaTime * 2);
            }
            else
            {
                shieldUp = Mathf.MoveTowards(shieldUp, 0, Time.deltaTime * 2);
            }
        }
    }

    private void Start() {
        SetRadius(rad);
        UpdateShieldStatus();
    }

    internal void SetRadius(float newRad)
    {
        rad = newRad;
        shieldNode.transform.localScale = Vector3.one * 2 * rad;
    }
}
