using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
	public float rotationSpeed;


	// Update is called once per frame
	void Update () {
		if(Time.timeScale != 0){
			transform.Rotate (0, Time.deltaTime * rotationSpeed, 0);	
		}
	}
}
