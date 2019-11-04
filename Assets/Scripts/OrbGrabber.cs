using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectiveMarker))]
public class OrbGrabber : MonoBehaviour {
    public Transform overlapPoint;
    public string orbTag;
    public GameObject grabbedOrb;
    [SerializeField]
    [HideInInspector]
    bool grabbed = false;
    ObjectiveMarker marker;
    Collider2D[] colliders = new Collider2D[8];
    
	void Start () {
        marker = GetComponent<ObjectiveMarker>();
	}
	
	void FixedUpdate () {
		if (!grabbed) {
            int count = Physics2D.OverlapPointNonAlloc(overlapPoint.position, colliders);
            for (int i = 0; i < count; i++) {
                if (colliders[i].gameObject.tag == orbTag) {
                    grabbed = true;
                    marker.complete = true;
                    grabbedOrb.SetActive(true);
                    Destroy(colliders[i].gameObject);
                    break;
                }
            }
        }
	}
}
