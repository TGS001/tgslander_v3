using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh), typeof(Animator), typeof(Rigidbody2D))]
public class ScoreNote : MonoBehaviour {
    public string message;
    public int points;
    public Color positiveColor;
    public Color neutralColor;
    public Color negativeColor;
    public float displayTime;
    public float springDistance = 6;
    public float springHz = 0.14f;
    public float springDampingRatio = 0.5f;
    TextMesh text;
    Animator anim;
    float timer;
    int state;

	// Use this for initialization
	void Start () {
        text = GetComponent<TextMesh>();
        anim = GetComponent<Animator>();
        timer = Time.time + displayTime;
        state = 0;
        if (points < 0) {
            text.color = negativeColor;
            text.text = message + "\n" + points;
        } else if (points > 0) {
            text.color = positiveColor;
            text.text = message + "\n+" + points;
        } else {
            text.color = neutralColor;
            text.text = message;
        }

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Rigidbody2D camBody = Camera.main.GetComponent<Rigidbody2D>();
        if (body && camBody) {
            SpringJoint2D spring = gameObject.AddComponent<SpringJoint2D>();
            spring.autoConfigureConnectedAnchor = false;
            spring.autoConfigureDistance = false;
            spring.distance = springDistance;
            spring.dampingRatio = springDampingRatio;
            spring.frequency = springHz;
            spring.connectedBody = camBody;
        }
        
    }

    public void EndNote() {
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case 0:
                if (Time.time > timer) {
                    anim.SetBool("done", true);
                    state = 1;
                }
                break;
            case 1:
                break;
            default:
                break;
        }
    }
}
