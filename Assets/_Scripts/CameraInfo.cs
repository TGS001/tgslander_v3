using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraInfo : MonoBehaviour {
    Camera cam;
	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        int buffers = cam.commandBufferCount;
        foreach (CameraEvent e in Enum.GetValues(typeof(CameraEvent)))
        {
            CommandBuffer[] buffer = cam.GetCommandBuffers(e);
            Debug.Log(e.ToString() + ": " + buffer.Length + " command buffers");
            foreach (CommandBuffer b in buffer)
            {
                Debug.Log("\t" + b.ToString());
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
