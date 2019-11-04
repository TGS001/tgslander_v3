using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuoyancyEffector2D))]
public class Water : MonoBehaviour {
    public class Interaction
    {
        public Transform t = null;
        public Collider2D collider = null;
        public Rigidbody2D body = null;
        bool _isInWater = false;
        
        public bool isInWater
        {
            get
            {
                return _isInWater;
            }
            set
            {
                /*
                if (collider != null && _isInWater != value)
                {
                    if (value)
                    {
                        collider.SendMessageUpwards("OnEnterWater", SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        collider.SendMessageUpwards("OnExitWater", SendMessageOptions.DontRequireReceiver);
                    }
                }
                */
                _isInWater = value;
            }
        }

        public bool submerged {
            get {
                return Water.Submerged(t.TransformPoint(collider.offset));
            }
        }
    }
    public Transform displayMesh;
    public SFX entrySplash;
    public SFX exitSplash;
    public float minVelocity;

    static List<Water> waters = new List<Water>();
    static List<Interaction> interactions = new List<Interaction>();
    List<Collider2D> waterColliders = new List<Collider2D>();
    BuoyancyEffector2D water;
    float meshZ = 0;

    static Vector3 ToSurfacePoint(Vector2 point)
    {
        float y = float.MaxValue;
        float z = 0;
        float diff = float.MaxValue;
        foreach (Water w in waters) {
            float cy = w.transform.position.y + w.water.surfaceLevel;
            float ndiff = Mathf.Abs(point.y - cy);
            if (ndiff < diff) {
                diff = ndiff;
                y = cy;
                z = w.meshZ;
            }
        }
        return new Vector3(point.x, y, z);
    }

    bool IsPointBelowSurface(Vector2 point)
    {
        bool res = false;
        foreach (Collider2D c in waterColliders)
        {
            if (c.OverlapPoint(point))
            {
                res = true;
                break;
            }
        }
        if (res)
        {
            return point.y < (water.surfaceLevel + water.transform.position.y);
        }
        return false;
    }

    bool IsColliderBelowSurface(Collider2D coll)
    {
        return (IsPointBelowSurface(coll.transform.TransformPoint(coll.offset)));
    }

    void AddInteraction(Collider2D coll)
    {
        Interaction ia = new Interaction();
        ia.t = coll.transform;
        ia.collider = coll;
        ia.body = coll.attachedRigidbody;
        ia.isInWater = IsColliderBelowSurface(coll);
        interactions.Add(ia);
        //Debug.Log(interactions.Count + " interactions");
    }

    void RemoveInteraction(Collider2D coll)
    {
        foreach (Interaction ia in interactions)
        {
            if (ia.collider.Equals(coll))
            {
                interactions.Remove(ia);
                break;
            }
        }
        //Debug.Log(interactions.Count + " interactions");
    }

	void Start () {
        water = GetComponent<BuoyancyEffector2D>();
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders)
        {
            if (c.usedByEffector)
                waterColliders.Add(c);
        }
        if (displayMesh)
        {
            meshZ = displayMesh.transform.position.z;
        }
	}
	
	void Update () {

        for (int j = 0; j < interactions.Count; )
        {
            Interaction i = interactions[j];
            if (i.collider == null)
            {
                interactions.RemoveAt(j);
                continue;
            }
            bool inWater = i.submerged;
            if (inWater && !i.isInWater)
            {
                DoEntrySplash(i);
            }
            else if (!inWater && i.isInWater)
            {
                DoExitSplash(i);
            }
            i.isInWater = inWater;
            j++;
        }
	}

    void DoEntrySplash(Interaction action)
    {
        Vector2 velocity = Vector2.down;
        float mag = 1;
        if (action.body != null)
        {
            velocity = action.body.GetRelativePointVelocity(action.collider.offset);
            mag = velocity.magnitude;
            if (mag < minVelocity)
            {
                return;
            }
        }
        if (entrySplash != null)
        {
            SFX fx = Instantiate(entrySplash);
            fx.position = ToSurfacePoint(action.collider.transform.TransformPoint(action.collider.offset));
            fx.size = action.collider.bounds.size.magnitude * Maths.ir2conv;
            fx.magnitude = mag;
            fx.velocity = velocity;
            fx.normal = Vector2.up;
        }
    }

    void DoExitSplash(Interaction action)
    {
        Vector2 velocity = Vector2.down;
        float mag = 1;
        if (action.body != null)
        {
            velocity = action.body.GetRelativePointVelocity(action.collider.offset);
            mag = velocity.magnitude;
            if (mag < minVelocity)
            {
                return;
            }
        }
        if (exitSplash != null)
        {
            SFX fx = Instantiate(exitSplash);
            fx.position = ToSurfacePoint(action.collider.transform.TransformPoint(action.collider.offset));
            fx.size = action.collider.bounds.size.magnitude * Maths.ir2conv;
            fx.magnitude = mag;
            fx.velocity = velocity;
            fx.normal = Vector2.up;
        }
    }

    private void OnEnable()
    {
        waters.Add(this);
    }

    private void OnDisable()
    {
        waters.Remove(this);
    }

    public static bool Submerged(Vector2 point)
    {
        bool res = false;
        foreach (Water w in waters)
        {
            res = w.IsPointBelowSurface(point);
            if (res)
            {
                break;
            }
        }
        return res;
    }

    static bool IsInteractable(Collider2D col, bool excludeDouble = true) {
        if (excludeDouble) {
            foreach (Interaction i in interactions) {
                if (i.collider.Equals(col)) {
                    return false;
                }
            }
        }
        foreach (Water w in waters) {
            foreach (Collider2D c in w.waterColliders) {
                if (col.IsTouching(c)) {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInteractable(collision)) {
            AddInteraction(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsInteractable(collision, false)) {
            RemoveInteraction(collision);
        }
    }
}
