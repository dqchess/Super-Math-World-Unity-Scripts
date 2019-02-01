using UnityEngine;
using System.Collections;

public class LevelMachineRisingGate : MonoBehaviour {
	
	//	public Transform objToRotate;
	public int amountToRaiseUp = 25;
	public float speed=4f;
	bool needsRaise;
	bool started=false;
	float startY = 0;
	
	void StartMachine(){
		startY = transform.localPosition.y;
		if (started) return;
		started = true;
		needsRaise = true;
		GetComponent<AudioSource>().Play ();
	}
	
	
	float totalRot;
	void Update(){
		if (needsRaise){
			transform.position += Vector3.up*Time.deltaTime*speed;
			if (transform.localPosition.y >= startY + amountToRaiseUp){
				needsRaise=false;
			}
		}
	}
}