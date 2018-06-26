using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class GazeSpin : MonoBehaviour {

    // Use this for initialization

    private GazeAware gazeAware;

	void Start () {
        gazeAware = GetComponent<GazeAware>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gazeAware.HasGazeFocus)
        {
            transform.Rotate(Vector3.forward);
        }
	}
}
