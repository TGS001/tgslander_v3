using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ThrustControl : MonoBehaviour
{
    public Slider displaySlider;
    public float maxFuel = 100;
    public float fuel = 100;
    public float engineAngle = 5;
    private bool auto = true;
    public float maxHorizantalSpeed = 10;
    public float maxVerticalSpeed = 10;
    private RCSPod[] pods;
    private Engine[] engines;
    private Rigidbody2D body;
    public float turnForce;
    public float linearForce;
    public float engineForce;
    private float engineCos;

    Vector2 cVector = Vector2.up;
    Vector2[] cVSmooth = new Vector2[10];
    int cvsi = 0;
    float cvThrottle = 0;
    float mainEngine = 0;
    float cAngle = 0;
    float caThrottle = 0;
    float retainedThrottle = 0;

    public bool isThrusting { get { return mainEngine > 0f; } }

    Vector2 lastVelocity;
    Vector2 _smoothedAcceleration;
    public Vector2 smoothedAcceleration {
        get {
            return _smoothedAcceleration;
        }
    }

    public Vector2 smoothCVector {
        get {
            Vector2 sv = new Vector2();
            for (int i = 0; i < cVSmooth.Length; i++) {
                sv += cVSmooth[i];
            }
            return sv / cVSmooth.Length;
        }

        set {
            cVSmooth[cvsi] = value;
            cvsi = (cvsi + 1) % cVSmooth.Length;
        }
    }

    public Vector2 velocity {
        get {
            return body.velocity;
        }

        set {
            body.velocity = value;
        }
    }

    public float angle {
        get {
            return body.rotation;
        }
        set {
            body.rotation = value;
        }
    }

    public float LinearBrakingDistanceFor(Vector2 vel) {
        float gravityAcceleration = Physics2D.gravity.magnitude * -Vector2.Dot(vel.normalized, Physics.gravity.normalized);
        float engineAcceleration = linearForce / body.mass;
        float sspeed = vel.sqrMagnitude;
        return sspeed / (2 * (engineAcceleration + gravityAcceleration));
    }

    public float EngineBrakingDistanceFor(Vector2 vel) {
        float gravityAcceleration = Physics2D.gravity.magnitude * -Vector2.Dot(vel.normalized, Physics.gravity.normalized);
        float engineAcceleration = engineForce / body.mass;
        float sspeed= vel.sqrMagnitude;
        return sspeed / (2 * (engineAcceleration + gravityAcceleration));
    }

    public float engineBrakingDistance {
        get {
            return EngineBrakingDistanceFor(body.velocity);
        }
    }

    public float linearBrakingDistance {
        get {
            return LinearBrakingDistanceFor(body.velocity);
        }
    }

    public Vector2 linearBrakingStop {
        get {
            Vector2 vel = body.velocity.normalized;
            float gravityAcceleration = Physics2D.gravity.magnitude * -Vector2.Dot(vel.normalized, Physics.gravity.normalized);
            float engineAcceleration = linearForce / body.mass;
            float sspeed = body.velocity.sqrMagnitude;
            return (Vector2)transform.position + (vel * sspeed / (2 * (engineAcceleration + gravityAcceleration)));
        }
    }

    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            float ebd = engineBrakingDistance;
            if (Vector2.Dot(body.velocity.normalized, Physics.gravity.normalized) > 0) {
                Gizmos.color = Color.red;
            } else {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireSphere(transform.position, linearBrakingDistance);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere((Vector2)transform.position + cVector * cvThrottle,  cvThrottle);
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawWireSphere(transform.position, linearBrakingDistance);
        }
    }

    public float fuelPercent
    {
        get
        {
            if (maxFuel == 0)
            {
                return 1;
            }
            return fuel / maxFuel;
        }
    }

    

    public void Refuel(float amount)
    {
        fuel = Mathf.Min(fuel + amount, maxFuel);
        if (displaySlider != null)
        {
            displaySlider.value = fuelPercent;
        }
    }

    float ConsumeFuel(float amount)
    {
        if (maxFuel == 0)
        {
            return 1;
        }
        amount *= 0.2f;
        if (fuel > 0)
        {
            if (amount > fuel)
            {
                float frac = fuel / amount;
                fuel = 0;
                if (displaySlider != null)
                {
                    displaySlider.value = fuelPercent;
                }
                return frac;
            }
            fuel -= amount;
            if (displaySlider != null)
            {
                displaySlider.value = fuelPercent;
            }
            return 1;
        }
        return 0;
    }

    public void SetLinearControl(Vector2 desired, float throttle)
    {
                
                cVector = desired.normalized;
                cvThrottle = Mathf.Clamp01(throttle);
                mainEngine = cvThrottle;
                auto = true;
    }

    public void SetLinearControl(Vector2 desired, float rcsThrottle, float mainThrottle)
    {
        Vector2Int tooFast = isTooFast();
        Vector2 forward = transform.up;
        Vector2 newDesire;
        if (tooFast == Vector2Int.zero)
        {
            cVector = desired.normalized;
        }
        else if (tooFast.x == 1)
            if (tooFast.y == 0 || (tooFast.y == 1 && forward.y <= 0) || (tooFast.y == -1 && forward.y >= 0))
            {
                newDesire = new Vector2(0,desired.y);
                cVector = newDesire.normalized;
            }
            else if (tooFast.x == -1)
            {
                if (tooFast.y == 0 || (tooFast.y == 1 && forward.y <= 0) || (tooFast.y == -1 && forward.y >= 0))
                {
                    newDesire = new Vector2(desired.x, 0);
                    cVector = newDesire.normalized;
                }
            }
        cvThrottle = Mathf.Clamp01(rcsThrottle);
        mainEngine = Mathf.Clamp01(mainThrottle);
        auto = false;
    }

    public void SetAngleControl(float angle, float throttle)
    {
        cAngle = angle;
        caThrottle = Mathf.Clamp01(throttle);
    }

    
    public Vector2Int isTooFast()
    {
    /* Asking if the ship is going too fast. If it is here is the coding
       -1 = too fast negative
       0  = not too fast
       1  = too fast positive */
       
        Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
        Vector2Int tooFast = new Vector2Int(0, 0);
        if (velocity.x > maxHorizantalSpeed) tooFast.x = 1;
        else if (velocity.x < -maxHorizantalSpeed) tooFast.x = -1;
        if (velocity.y > maxVerticalSpeed) tooFast.y = 1;
        else if (velocity.y < -maxVerticalSpeed) tooFast.y = -1;
        return tooFast;
    }

    void Start()
    {
        pods = GetComponentsInChildren<RCSPod>();
        engines = GetComponentsInChildren<Engine>();
        body = GetComponent<Rigidbody2D>();
        if (pods.Length > 0) {
            turnForce = pods[0].turnSpeed * pods.Length;
            linearForce = pods[0].linearForce * pods.Length;
        }
        if (engines.Length > 0) {
            engineForce = engines[0].thrust * engines.Length;
        }
        engineCos = Mathf.Cos(Mathf.Deg2Rad * engineAngle);
        //Debug.Log("engine cos " + engineCos);
    }

    void Update()
    {
        if (retainedThrottle > 0)
        {
            foreach (Engine e in engines)
            {
                e.doThrust(retainedThrottle);
            }
        }
    }

    void FixedUpdate()
    {
        Vector2Int tooFast = isTooFast();
        smoothCVector = cVector;

        _smoothedAcceleration = Vector2.Lerp(smoothedAcceleration, (velocity - lastVelocity) / Time.deltaTime, Time.deltaTime * 10);
        lastVelocity = velocity;
        if (body != null && (cvThrottle > 0 || caThrottle > 0))
        {
            //float adiff = Mathf.DeltaAngle(body.rotation, cAngle);
            //float pd = (adiff * Maths.root2 - body.angularVelocity) * 10;
            //pd = Mathf.Min(Mathf.Abs(pd), turnForce) * Mathf.Sign(pd);
            //body.AddTorque(pd * Time.fixedDeltaTime);
            float rcsThrottle = cvThrottle;
            Vector2 forward = transform.up;
            Vector2 forwardX = new Vector2(transform.up.x, 0);
            Vector2 forwardY = new Vector2(0, transform.up.y);
            forward.Normalize();
            float ecos = Vector2.Dot(forward, cVector);
            //Debug.Log("ENGIEN " + ecos);
            retainedThrottle = 0;
            if (engineForce > 0 && ((auto && ecos >= engineCos) || (!auto && mainEngine > 0)))
            {
                rcsThrottle = 0;
                float force = engineForce * mainEngine;
                float fuelmod = ConsumeFuel(force);


                
                body.AddForce(forward * force * fuelmod);
                retainedThrottle = mainEngine * fuelmod;
            }
            else if (rcsThrottle > 0)
            {
                float force = linearForce * rcsThrottle;
                float fuelmod = ConsumeFuel(force);
                body.AddForce(cVector * force * fuelmod);
            }
            
            //Vector2 cvec = Vector2.MoveTowards(rcVector, smoothCVector, Time.fixedDeltaTime * 4);
            //rcVector = cvec;
            if (caThrottle > 0)
            {
                float adiff = Mathf.DeltaAngle(body.rotation, cAngle);
                float angularTarget = turnForce * caThrottle * Mathf.Sign(adiff);
                if (Mathf.Abs(adiff) < Mathf.Abs(angularTarget * Time.fixedDeltaTime))
                {
                    angularTarget = adiff;
                }
                body.angularVelocity += (angularTarget - body.angularVelocity) * 0.1f;
                float pd = angularTarget;
                Vector2 rotationCenter = body.worldCenterOfMass;
                //float pd = (adiff * Maths.root2 - body.angularVelocity) * 10;
                //pd = Mathf.Min(Mathf.Abs(pd), turnForce) * Mathf.Sign(pd);
                //body.AddTorque(pd * Time.fixedDeltaTime);
                //body.angularVelocity += pd * Time.deltaTime;
                Vector3 targetn = new Vector3(0, 0, -pd * Time.fixedDeltaTime);
                float turnThrottle = (Mathf.Abs(pd) / turnForce);
                
                foreach (RCSPod pod in pods)
                {
                    if (Mathf.Approximately(turnThrottle, 0))
                    {
                        pod.thrust(smoothCVector, rcsThrottle);
                    }
                    else if (rcsThrottle > 0)
                    {
                        Vector3 podNormal = (Maths.tov2(pod.transform.position) - rotationCenter).normalized;
                        Vector3 podCross = Vector3.Cross(podNormal, targetn);// / (turnSpeed);
                        pod.thrust(podCross + (Vector3)(smoothCVector * rcsThrottle * linearForce), (caThrottle * turnThrottle + rcsThrottle * 2) * 0.5f);
                    }
                    else
                    {
                        Vector3 podNormal = (Maths.tov2(pod.transform.position) - rotationCenter).normalized;
                        Vector3 podCross = Vector3.Cross(podNormal, targetn);// / (turnSpeed);
                        pod.thrust(podCross, caThrottle * turnThrottle);
                    }
                }
            }
            else if (rcsThrottle > 0)
            {
                foreach (RCSPod pod in pods)
                {
                    pod.thrust(cVector, rcsThrottle);
                }
            }
        }
    }
}
