using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{

    // Creating the variables that can be accessed via the inspector
    public float maxXVelocity = 5.0f;
    public float maxYVelocity = 5.0f;
    public float thrusterPower = 5.0f;
    public float emergencyBoostMultiplier = 1f;

    public float rotationMaxSpeed = 1.0f;
    public float rotationAcceleration = 1.0f;
    public float rotationDeceleration = 1.0f;
    public bool discreteMaxVelocity = true;
    public float torque = 1f;

    public float tetherStrength = 1.0f;

    // Creating the variables that will house the player inputs
    private float thrustInput;
    private float rotationInput;
    private bool translationInput;
    private float gridInputX;
    private float gridInputY;
    private bool tetherInput;

    // Creating misc variables
    private Vector2 appliedThrust;
    private float rotationVelocity;
    private int frameCount;
    private bool tetherAttached = false;
    public float tetherLength = 5f;
    private Camera cam;


    // Creating variables to hold info from other parts of the game object
    Rigidbody2D ship;
    Transform orientation;


    // A method to apply thrust as long as the ship is under max speed. If not the ship will only apply forces in a way that will not exceed max speeds
    public void applyThrust()
    {
        // Use this if using discrete max velocities
        if (discreteMaxVelocity)
        {
            if (orientation.up.x >= 0 && ship.velocity.x <= maxXVelocity || orientation.up.x < 0 && ship.velocity.x >= -maxXVelocity)
            {
                if (orientation.up.y >= 0 && ship.velocity.y <= maxYVelocity || orientation.up.y < 0 && ship.velocity.y >= -maxYVelocity)
                {
                    appliedThrust = orientation.up * thrusterPower;

                    // An emergency boost for if you approach the ground too quick
                    if (orientation.up.y >= 0 && ship.velocity.y < -maxYVelocity)
                    {
                        appliedThrust.y = appliedThrust.y * (1 - ship.velocity.y - maxYVelocity);
                    }
                }
                else
                {
                    appliedThrust.Set(orientation.up.x * thrusterPower, 0);
                }
            }
            else if (orientation.up.y >= 0 && ship.velocity.y <= maxYVelocity || orientation.up.y < 0 && ship.velocity.y >= -maxYVelocity)
            {
                appliedThrust.Set(0, orientation.up.y * thrusterPower);
            }
            else
            {
                appliedThrust = Vector2.zero;
            }

            ship.AddForce(appliedThrust);
        }
        
        // Use this is using one max velocity
        /*else
        {
            appliedThrust = orientation.up * thrusterPower;
            ship.AddForce(appliedThrust);
            if (ship.velocity.magnitude > maxVelocity)
            {
                ship.velocity = ship.velocity.normalized * maxVelocity;
            }
        }*/
    }


    // A method to apply thrust when the shift button is pressed down
    public void applyGridThrust()
    {
        gridInputX = Input.GetAxisRaw("Horizontal");
        gridInputY = Input.GetAxisRaw("Vertical");
        Vector2 gridThrust = Vector2.zero;

        if (Mathf.Abs(ship.velocity.x) <= maxXVelocity)
        {
            if(Mathf.Sign(ship.velocity.x) != Mathf.Sign(gridInputX))
            {
                gridThrust.x = (gridInputX * thrusterPower);
                Debug.Log("X_Input");
            }
        }

        if (Mathf.Abs(ship.velocity.y) <= maxXVelocity)
        {
            if (Mathf.Sign(ship.velocity.y) != Mathf.Sign(gridInputY))
            {
                gridThrust.y = (gridInputY * thrusterPower);
            }
        }

        ship.AddForce(gridThrust);
    }

    /*An old method that applies the rotation to the object
    public void applyRotation()
    {
        if (rotationInput != 0)
        {
            if (rotationVelocity <= rotationMaxSpeed && rotationVelocity >= -rotationMaxSpeed)
            {
                rotationVelocity = rotationVelocity - rotationInput * rotationAcceleration * Time.fixedDeltaTime;
            }
        }
        else if (rotationVelocity != 0)
        {
            rotationVelocity = rotationVelocity - Mathf.Sign(rotationVelocity) * rotationDeceleration * Time.fixedDeltaTime;
        }
        ship.SetRotation(ship.rotation + rotationVelocity);
    }
    */

    // The new method of apply a rotational velocity to the ship
    public void applyTorque()
    {
        if (Input.GetButton("Horizontal"))
        {
            if (Mathf.Abs(ship.angularVelocity) <= rotationMaxSpeed || Mathf.Sign(ship.angularVelocity) == Input.GetAxisRaw("Horizontal"))
            {
                ship.AddTorque(-Input.GetAxisRaw("Horizontal") * torque);
            }
            
        }
        else
        {
            ship.AddTorque(ship.angularVelocity * -rotationDeceleration);
        }
    }

    public void applyTether()
    {

        Vector3 aimDirection = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection2D = aimDirection;

        if (!tetherAttached)
        {
            
            RaycastHit2D hit = Physics2D.Raycast(ship.position, aimDirection2D, tetherLength);
            Debug.Log(hit.collider);

        }
    }


    // Start is called before the first frame update
    void Start()
    {
        ship = GetComponent<Rigidbody2D>();
        orientation = GetComponent<Transform>();
        cam = Camera.main;
        rotationVelocity = 0f;
        frameCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Getting the player inputs
        thrustInput = Input.GetAxisRaw("Forward");
        rotationInput = Input.GetAxisRaw("Horizontal");
        translationInput = Input.GetButton("Translate");
        tetherInput = Input.GetMouseButtonDown(1);
    }

    // Update is called over fixed interval of time
    private void FixedUpdate()
    {


        if (!translationInput)
        {
            // Give the ship thrust if it is under the speed limit
            if (thrustInput > 0)
            {
                applyThrust();
            }

            // Apply rotation to the ship
            applyTorque();
        }
        /*else
        {
            Debug.Log("Grid");
            applyGridThrust();
        }*/

        // If the right mouse button is clicked 
        if (tetherInput)
        {
            applyTether();
            //Debug.Log("TetherTried");
        }


    }
}
