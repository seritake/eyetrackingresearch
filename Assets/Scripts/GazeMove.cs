using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class GazeMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GazePoint gazePoint = Tobii.Gaming.TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid)
        {
            transform.position = gazePoint.Screen;
            print(gazePoint.Screen);
        }
	}
}
