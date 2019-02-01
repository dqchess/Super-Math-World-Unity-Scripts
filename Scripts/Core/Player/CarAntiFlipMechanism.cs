using UnityEngine;
using System.Collections;

public class CarAntiFlipMechanism : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	float flippedTimer =0;
	void Update () {
		if (Vector3.Angle(transform.up,Vector3.up) > 70){
			flippedTimer += Time.deltaTime;
		} else {
			flippedTimer = 0;
		}
		if (flippedTimer > 2){
			flippedTimer = 0;
			Quaternion newRot = transform.rotation;
			newRot.eulerAngles = new Vector3(0,newRot.y,0);
			transform.rotation = newRot;
			transform.position += Vector3.one * 5; // so that rotation doesn't let you go up thru terrain.
		}
	}
}
