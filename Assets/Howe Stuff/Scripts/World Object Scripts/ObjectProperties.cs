using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectProperties : MonoBehaviour
{

    // Properties of objects that can be edited in the inspector
    public bool isHaulable = true;

    // Private properties of objects
    private bool isTethered = false;
    private GameObject ship = GameObject.Find("Lander");
    private Transform objectPosition;
    private Rigidbody2D objectRigibody;

    public void tetherMovement()
    {
        Vector3 shipPosition = ship.GetComponent<Transform>().position;

        float distanceFromShip = Vector3.Distance(objectPosition.position, shipPosition);
        float distanceFromRadius = distanceFromShip - ship.GetComponent<ShipControl>().tetherLength;


        if (distanceFromRadius > 0)
        {
            Vector2 relativePosition = (Vector2)shipPosition - (Vector2)objectPosition.position;
            relativePosition = relativePosition.normalized;

            objectRigibody.AddForce(relativePosition * ship.GetComponent<ShipControl>().tetherStrength);
        }


    }



    // Start is called before the first frame update
    void Start()
    {
        objectPosition = GetComponent<Transform>();
        objectRigibody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTethered)
        {
        }

    }
}
