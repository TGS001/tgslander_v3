using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

enum ControlMode
{
    CM_TOUCH = 1,
    CM_KEYBOARD,
    CM_GAMEPAD
}

public class ModularLander : MonoBehaviour
{

    // Creating the variables for the parts of the lander (imported from other scripts)
    public HullLabel hull;
    public WingLabel wing;
    public ThrusterLabel thruster;
    public StrutLabel strut;
    public EngineLabel engine;
    public WeaponLabel weapon;
    public ShieldLabel shield;

    public Transform root;

    public float longTouchSeconds = 1.5f;
    public float longTouchVisibleDelay = 0.5f;

    [Range(0, 1)]
    public float longTouchScreenSpace = 0.05f;

    [Range(0, 1)]
    public float motionDeadzoneScreenSpace = 0.1f;

    [Range(0, 1)]
    public float motionMaximumScreenspace = 0.3f;
    Vector2 touchStart;
    Vector2 touchCurrent;
    bool touchIsMotion = false;
    bool isTouch = false;
    bool isLongTouch = false;
    float touchStartTime = 0.0f;

    [SerializeField]
    [HideInInspector]
    int landedCount = 0;
    [SerializeField]
    [HideInInspector]
    private LandingGearEffector[] landingGear;

    private GunController[] guns;

    float longTouchPixels = 0;
    float motionDeadzonePixels = 0;
    float motionMaximumPixels = 0;

    private ThrustControl thrusters;
    private Rigidbody2D body;
    private bool expanded = false;

    private TractorBeam tractorBeam;

    private float lastAngleTarget = 0;

    private Life life;
    private ControlMode control;

    private Shield shieldControl;


    // The class that connects the tractor beam to an object if it can
    void ConnectBeam(Vector3 point)
    {
        if (!tractorBeam.connected)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(point);
            Haulable target = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                target = colliders[i].GetComponent<Haulable>();
                if (target != null)
                {
                    tractorBeam.Attach(target);
                    break;
                }
            }
        }
    }

    // The class that detaches the tractor beam
    void BreakBeam()
    {
        tractorBeam.Detach();
    }


    void Fire(Vector3 point)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(point);
        Transform target = null;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (Alliance.IsPartOf(colliders[i].gameObject, AllyGroup.Enemy))
            {
                target = colliders[i].transform;
                break;
            }
        }
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].Fire(point, target);
        }
        //if (guns[currentGun].Fire(point, target)) {
        //currentGun = (currentGun + 1) % guns.Length;
        //}
    }

    // Has the lander landed
    public bool Landed()
    {
        return landedCount > 0;
    }

    GameObject landedSurface = null;


    void LandingListener(bool touch, GameObject surface)
    {
        if (touch)
        {
            landedCount += 1;
            if (landedSurface == null)
            {
                landedSurface = surface;
                landedSurface.BroadcastMessage("OnLandingEnter", SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            landedCount -= 1;
            if (landedCount == 0)
            {
                landedSurface.BroadcastMessage("OnLandingExit", SendMessageOptions.DontRequireReceiver);
                landedSurface = null;
            }
        }

    }

    void expandNode(ExpansionNode node, GameObject part)
    {
        GameObject xo = Instantiate(part);
        xo.transform.position = node.transform.position;
        xo.transform.rotation = node.transform.rotation;

        for (int i = 0; i < xo.transform.childCount; i++)
        {
            Transform t = xo.transform.GetChild(i);
            t.SetParent(node.transform);
        }
        DestroyImmediate(xo);
    }

    void expand(LanderPieceSelector pieceSelector = null)
    {
        if (!expanded)
        {
            ExpansionNode[] nodes = root.GetComponentsInChildren<ExpansionNode>();
            int lastNodeCount = 1;
            int lastFinishedCount;

            while (lastNodeCount > 0)
            {
                lastNodeCount = 0;
                lastFinishedCount = 0;
                foreach (ExpansionNode node in nodes)
                {
                    if (!node.Finished())
                    {
                        switch (node.expansion)
                        {
                            case ExpansionType.Engine:
                                if (pieceSelector != null && pieceSelector.engine != null)
                                {
                                    if (engine != null)
                                    {
                                        Destroy(engine.gameObject);
                                    }
                                    expandNode(node, pieceSelector.engine.gameObject);
                                }
                                else if (engine != null)
                                {
                                    expandNode(node, engine.gameObject);
                                }
                                break;
                            case ExpansionType.Hull:
                                if (pieceSelector != null && pieceSelector.hull != null)
                                {
                                    if (hull != null)
                                    {
                                        Destroy(hull.gameObject);
                                    }
                                    expandNode(node, pieceSelector.hull.gameObject);
                                }
                                else if (hull != null)
                                {
                                    expandNode(node, hull.gameObject);
                                }
                                break;
                            case ExpansionType.Shield:
                                if (shield != null)
                                {
                                    expandNode(node, shield.gameObject);
                                }
                                break;
                            case ExpansionType.Strut:
                                if (strut != null)
                                {
                                    expandNode(node, strut.gameObject);
                                }
                                break;
                            case ExpansionType.Thruster:
                                if (thruster != null)
                                {
                                    expandNode(node, thruster.gameObject);
                                }
                                break;
                            case ExpansionType.Weapon:
                                if (weapon != null)
                                {
                                    expandNode(node, weapon.gameObject);
                                }
                                break;
                            case ExpansionType.Wing:
                                if (wing != null)
                                {
                                    expandNode(node, wing.gameObject);
                                }
                                break;
                        }
                        node.Finish();
                        lastNodeCount++;
                    }
                    else
                    {
                        lastFinishedCount++;
                    }
                }
                nodes = root.GetComponentsInChildren<ExpansionNode>();
            }
            expanded = true;
        }
    }

    public void Clear()
    {
        Initialize();
        while (root.childCount > 0)
        {
            foreach (Transform child in root)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        expanded = false;
        ExpansionNode rn = root.GetComponent<ExpansionNode>();
        rn.Reset();
        
        if (shieldControl != null)
        {
            shieldControl.SetRadius(1);
        }
    }

    public void Generate(LanderPieceSelector pieceSelector = null)
    {
        Clear();
        Vector3 startScale = root.localScale;
        root.localScale = Vector3.one;
        expand(pieceSelector);
        root.localScale = startScale;
    }

    public void ReSetup(LanderPieceSelector pieceSelector)
    {
        Vector3 startScale = root.localScale;
        root.localScale = Vector3.one;
        expand(pieceSelector);
        root.localScale = startScale;
    }

    void OnMortalDie()
    {
        Destroy(gameObject);
        PlaySessionControl.Lose(null, "YOU DIED", "Your lander was destroyed.\n Watch your shield guage!");
    }

    //When starting the game
    void Initialize()
    {

        // Getting the parts of the ship
        body = GetComponent<Rigidbody2D>();
        tractorBeam = GetComponent<TractorBeam>();
        thrusters = GetComponent<ThrustControl>();
        engine = GetComponent<EngineLabel>();
        shieldControl = GetComponent<Shield>();
        landingGear = root.GetComponentsInChildren<LandingGearEffector>();
        guns = root.GetComponentsInChildren<GunController>();
        life = GetComponent<Life>();

        foreach (LandingGearEffector gear in landingGear)
        {
            gear.listener = LandingListener;
        }

        if (shieldControl)
        {
            Collider2D[] colliders = root.GetComponentsInChildren<Collider2D>();
            float radius = 0;
            Vector3 position = transform.position;
            foreach (Collider2D collider in colliders)
            {
                Bounds b = collider.bounds;
                float maxy = b.max.y - position.y;
                float maxx = b.max.x - position.x;
                float miny = b.min.y - position.y;
                float minx = b.min.x - position.x;
                float nradv = Mathf.Max(Mathf.Abs(miny), Mathf.Abs(maxy));
                float nradh = Mathf.Max(Mathf.Abs(minx), Mathf.Abs(maxx));
                float nrad = Mathf.Max(nradv, nradh) * 12;
                if (nrad > radius)
                {
                    radius = nrad;
                }
            }
            shieldControl.SetRadius(radius * 0.1f);
        }
    }

    // Use this for initialization
    void Start()
    {
        PlaySessionControl.player = this;
        //if (GlobalGameManager.Instance != null)
        //{
        //    GlobalGameManager.Instance.SetupCurrentLanderWithParts(this);
        //}
        //else
        //{
            Initialize();
        //}

        // Setting the control scheme
#if UNITY_IOS || UNITY_ANDROID
      control = ControlMode.CM_TOUCH;
#else
        control = ControlMode.CM_KEYBOARD;
#endif
    }


    // Update is called once per frame. Every frame get the controller input
    void Update()
    {
        if (PlaySessionControl.PlayerControllable()) {
            switch (control) {
                case ControlMode.CM_KEYBOARD:
                    if (Input.GetButtonDown("Debug Switch Controls")) {
                        control = ControlMode.CM_TOUCH;
                    }
                    DoKeyboardControls();
                    break;
                case ControlMode.CM_TOUCH:
                    if (Input.GetButtonDown("Debug Switch Controls")) {
                        control = ControlMode.CM_KEYBOARD;
                    }
                    DoTouchControls();
                    break;
                case ControlMode.CM_GAMEPAD:
                    DoGamepadControls();
                    break;
            }
        }
//        Debug.Log(GetComponent<Rigidbody2D>().angularVelocity);
    }

    // With the controller input, create an update that relies on time rather than frame count
    void FixedUpdate()
    {
        if (life.percent > 0.3 && !Water.Submerged(transform.position))
        {
            Vector2 co = Maths.tov2(transform.position);
            Vector2 vn = body.velocity.normalized;
            Vector2 cu = Maths.tov2(transform.up);
            float vd = Vector2.Dot(vn, cu);
            float castRadius = shieldControl.radius * 0.75f;
            float castDistance = shieldControl.radius * 0.5f;
            int castMask = Physics2D.GetLayerCollisionMask(14);
            Debug.DrawRay(transform.position, Maths.tov3(vn * 3), Color.red);
            Debug.DrawRay(transform.position, Maths.tov3(cu * 3), Color.blue);
            bool inside1 = false;
            bool inside2 = false;
            if (!shieldControl.on || body.velocity.magnitude < 2)
            {
                if (vd < 0)
                {
                    Vector2 checkpoint = co + (cu * -castDistance);
                    Color cres = Color.blue;
                    if (Physics2D.OverlapCircle(checkpoint, castRadius, castMask))
                    {
                        cres = Color.red;
                        inside1 = true;
                    }
                    Debug.DrawLine(
                       Maths.tov3(checkpoint + new Vector2(castRadius, 0)),
                       Maths.tov3(checkpoint + new Vector2(-castRadius, 0)), cres);
                    Debug.DrawLine(
                       Maths.tov3(checkpoint + new Vector2(0, castRadius)),
                       Maths.tov3(checkpoint + new Vector2(0, -castRadius)), cres);
                }

                if (!shieldControl.on)
                {
                    Vector2 checkpoint = co;
                    Color cres = Color.blue;
                    if (Physics2D.OverlapCircle(checkpoint, shieldControl.radius, castMask))
                    {
                        cres = Color.red;
                        inside2 = true;
                    }
                    Debug.DrawLine(
                       Maths.tov3(checkpoint + new Vector2(shieldControl.radius, 0)),
                       Maths.tov3(checkpoint + new Vector2(-shieldControl.radius, 0)), cres);
                    Debug.DrawLine(
                       Maths.tov3(checkpoint + new Vector2(0, shieldControl.radius)),
                       Maths.tov3(checkpoint + new Vector2(0, -shieldControl.radius)), cres);
                }

                if (shieldControl.on)
                {
                    if (inside1)
                    {
                        shieldControl.on = false;
                    }
                }
                else
                {
                    if (!(inside1 || inside2))
                    {
                        shieldControl.on = true;
                    }
                }
            }
        }
        else
        {
            shieldControl.on = false;
        }
    }

    private void SetShield(bool enabled)
    {
        Debug.Log("setting shield: " + enabled);
        shieldControl.on = enabled;
    }

    void touchBegin(Vector2 touchPosition, float longTouch, float motionDeadzone, float motionMaximum)
    {
        touchStart = touchPosition;
        touchCurrent = touchPosition;
        touchIsMotion = false;
        touchStartTime = Time.time;
        isTouch = true;
    }

    void touchEnd(Vector2 touchPosition, float longTouch, float motionDeadzone, float motionMaximum)
    {
        //placeholder: rotate and fire on short press, inventory on long press
        if (!touchIsMotion)
        {
            if (isLongTouch)
            {
                if (tractorBeam.connected)
                {
                    BreakBeam();
                }
                else
                {
                    ConnectBeam(Camera.main.ScreenToWorldPoint(touchPosition));
                }
            }
            else
            {
                Fire(Camera.main.ScreenToWorldPoint(touchPosition));
            }
        }
        touchIsMotion = false;
        isTouch = false;
        isLongTouch = false;
    }

    void touchCancel(Vector2 touchPosition, float longTouch, float motionDeadzone, float motionMaximum)
    {
        touchIsMotion = false;
        isTouch = false;
        isLongTouch = false;
    }

    void touchMove(Vector2 touchPosition, float longTouch, float motionDeadzone, float motionMaximum)
    {
        touchCurrent = touchPosition;
        if (touchIsMotion)
        {
        }
        else
        {
            if ((touchStart - touchPosition).magnitude > longTouch)
            {
                touchIsMotion = true;
            }
        }
    }

    void touchStationary(Vector2 touchPosition, float longTouch, float motionDeadzone, float motionMaximum)
    {
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        
        Vector2 relativeVelocity = coll.relativeVelocity;
        float proportion = 1.0f / coll.contacts.Length;
        Vector3 normals = Vector3.zero;
        float damageTotal = 0;

        for (int i = 0; i < coll.contacts.Length; i++)
        {
            ContactPoint2D c = coll.contacts[i];
            float dot = Vector2.Dot(relativeVelocity, c.normal);
            Debug.DrawRay(c.point, c.normal, Color.blue, 1);
            Debug.DrawRay(c.point, relativeVelocity, Color.red, 1);
            damageTotal += dot * 10;
            normals += (Vector3)c.normal;
        }
        life.Afflict((damageTotal * proportion), 1, normals.normalized);
    }

    void DoKeyboardControls()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        bool translate = Input.GetButton("Translate");
        bool forward = Input.GetButton("Forward");
        bool backward = Input.GetButton("Backward");
        bool left = Input.GetButton("Left");
        bool right = Input.GetButton("Right");
        bool weapon = Input.GetMouseButtonDown(0);
        bool tool = Input.GetMouseButtonDown(1);
        if (translate)
        {
            Vector2 direction = Vector2.zero;
            if (forward ^ backward)
            {
                if (forward)
                {
                    if (GetComponent<Rigidbody2D>().velocity.y <= thrusters.maxVerticalSpeed)
                    {
                        direction.y = 1;
                    }
                    Debug.Log(thrusters.velocity.y);
                }
                else
                {
                    direction.y = -1;
                }
            }
            if (left ^ right)
            {
                if (right && GetComponent<Rigidbody2D>().velocity.x <= thrusters.maxHorizantalSpeed)
                {
                    direction.x = 1;
                }
                else if (GetComponent<Rigidbody2D>().velocity.x >= -thrusters.maxHorizantalSpeed)
                {
                    direction.x = -1;
                }
            }
            thrusters.SetLinearControl(direction, direction.sqrMagnitude, 0);
        }
        else
        {
            if (left ^ right)
            {
                if (right)
                {
                    thrusters.SetAngleControl(body.rotation - 90, 1);
                }
                else
                {
                    thrusters.SetAngleControl(body.rotation + 90, 1);
                }
            }
            else
            {
                thrusters.SetAngleControl(body.rotation, 0.5f);
            }
            if (forward)
            {
                thrusters.SetLinearControl(transform.up, 0, 1);
//                Debug.Log(thrusters.velocity.y);
            }
            else
            {
                thrusters.SetLinearControl(transform.up, 0, 0);
            }
        }
        if (tool)
        {
            if (tractorBeam.connected)
            {
                BreakBeam();
            }
            else
            {
                ConnectBeam(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        if (weapon)
        {
            Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    void DoGamepadControls()
    {
        Vector2 axis = new Vector2(Input.GetAxis("L_XAxis_1"), -Input.GetAxis("L_YAxis_1"));
        float throttle = Input.GetAxis("TriggersL_1");
        float mag = axis.magnitude;
        float linearThrottle = 0;
        if (mag > 0.2f)
        {
            thrusters.SetAngleControl((Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg) - 90, 1);
            linearThrottle = (mag - 0.2f) * 1.25f;
        }
        else
        {
            thrusters.SetAngleControl(body.rotation, 0.5f);
        }
        thrusters.SetLinearControl(axis, linearThrottle, throttle);
    }

    void DoTouchControls()
    {
        float pixelBasis = Mathf.Min(Screen.width, Screen.height);
        longTouchPixels = longTouchScreenSpace * pixelBasis;
        motionDeadzonePixels = motionDeadzoneScreenSpace * pixelBasis;
        motionMaximumPixels = motionMaximumScreenspace * pixelBasis;
        float motionBandPixels = motionMaximumPixels - motionDeadzonePixels;

#if UNITY_IOS || UNITY_ANDROID
      if (Input.touchCount > 0) {

      Touch t = Input.GetTouch (0);
      switch (t.phase) {
      case TouchPhase.Began:
      touchBegin (t.position, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
      break;
      case TouchPhase.Canceled:
      touchCancel (t.position, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
      break;
      case TouchPhase.Ended:
      touchEnd (t.position, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
      break;
      case TouchPhase.Moved:
      touchMove (t.position, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
      break;
      case TouchPhase.Stationary:
      touchStationary (t.position, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
      break;
      }
      }
#else


        if (Input.GetMouseButtonDown(0))
        {
            touchBegin(Input.mousePosition, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
        }
        else if (Input.GetMouseButton(0))
        {
            touchMove(Input.mousePosition, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            touchEnd(Input.mousePosition, longTouchPixels, motionDeadzonePixels, motionMaximumPixels);
        }
#endif

        isLongTouch = (isTouch && (Time.time - touchStartTime) > longTouchSeconds);

        float angleTarget = 0;
        float angleThrottle = 0;
        float thrustPercent = 0;
        Vector2 offset = (touchCurrent - touchStart);
        if (touchIsMotion)
        {
            angleThrottle = 1;
            angleTarget = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg + 90;
            lastAngleTarget = angleTarget;
            thrustPercent = Mathf.Max(0, Mathf.Min((offset.magnitude - motionDeadzonePixels) / motionBandPixels, 1));
        }

        thrusters.SetLinearControl(-offset, thrustPercent);
        if (!Landed())
        {
            if (angleThrottle > 0)
            {
                thrusters.SetAngleControl(angleTarget, angleThrottle);
            }
            else
            {
                thrusters.SetAngleControl(lastAngleTarget, 0.5f);
            }
        }
        else
        {
            thrusters.SetAngleControl(angleTarget, angleThrottle * 0.5f);
        }
    }

    void OnGUI()
    {
        if (isTouch)
        {
            Vector2 gtc = new Vector2(touchCurrent.x, Screen.height - touchCurrent.y);
            Vector2 gts = new Vector2(touchStart.x, Screen.height - touchStart.y);
            Vector2 touchNormal = gtc - gts;
            touchNormal.Normalize();
            if (touchIsMotion)
            {
                Drawing.DrawCircle(gts, (int)motionDeadzonePixels, Color.yellow, 2, 3);
                Drawing.DrawCircle(gts, (int)motionMaximumPixels, Color.red, 2, 3);
                Drawing.DrawLine(gts, gtc, Color.cyan, 1, false);
            }
            else
            {
                if (isLongTouch)
                {
                    Drawing.DrawCircle(gts, (int)longTouchPixels, Color.green, 2, 3);
                }
                else
                {
                    Drawing.DrawCircle(gts, (int)longTouchPixels, Color.blue, 2, 3);
                    //Drawing.DrawLine(gts, gtc, Color.cyan, 1, false);
                    float percent = Mathf.Clamp01((Time.time - (touchStartTime + longTouchVisibleDelay)) / (longTouchSeconds - longTouchVisibleDelay));
                    if (percent > 0)
                    {
                        Drawing.DrawCircle(gts, (int)(longTouchPixels * percent), Color.green, 2, 3);
                    }
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ModularLander))]
public class MLEditor : Editor
{
    void OnSceneGUI()
    {

    }

    public override void OnInspectorGUI()
    {
        ModularLander targ = (ModularLander)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            targ.Generate();
        }
        if (GUILayout.Button("Reset"))
        {
            targ.Clear();
        }
    }
}
#endif
