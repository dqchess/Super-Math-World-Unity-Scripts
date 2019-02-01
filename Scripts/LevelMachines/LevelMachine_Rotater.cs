using UnityEngine;
using System.Collections;

public class LevelMachine_Rotater : MonoBehaviour {

//	public Transform objToRotate;
	public int degToRotate;
	public float rotSpeed=25f;
//	float finalY=0;
	bool needsRotate;
	bool started=false;
	public Vector3 axis = Vector3.up;
	Quaternion newRot = new Quaternion();


	public void StartMachine(){
		if (started) return;
		started = true;
		Quaternion r = transform.localRotation;
//		finalY = r.eulerAngles.y+degToRotate;
		needsRotate = true;
		newRot.eulerAngles = transform.localEulerAngles + axis * degToRotate; // new Vector3(0,degToRotate,0);
	}


	float totalRot;
	void Update(){
		if (needsRotate){
//			transform.Rotate(new Vector3(0,1,0),Time.deltaTime/(float)degToRotate,Space.Self);
			float dr = Time.deltaTime*rotSpeed;
			transform.Rotate(axis,dr,Space.Self);
			totalRot += dr;
			if (totalRot >= degToRotate && degToRotate != 0) needsRotate=false;

		}
	}
}
