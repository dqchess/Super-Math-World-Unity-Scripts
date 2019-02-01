using UnityEngine;
using System.Collections;

public class ToggleGate : MonoBehaviour {

	bool moving=false;
	Vector3 targetPos;
	public Transform openT;
	public Transform closedT;
	public float gateSpeed=1.5f;
	bool gateIsOpen=false;

	void Start(){


	}

	void Update(){
		if (moving){
//			// commented Debug.Log ("we're moving.");
			transform.position = Vector3.Lerp (transform.position,targetPos,Time.deltaTime*gateSpeed);
			if (Vector3.Distance(transform.position,targetPos)<.2f){
//				// commented Debug.Log ("done.");
				transform.position = targetPos;
				moving=false;
				AudioManager.inst.PlayClunk(transform.position);
			}
		} 
	}


	public void OpenGate(){
		moving = true;
		AudioManager.inst.PlayGateOpen(transform.position);
		targetPos = openT.position;
		gateIsOpen = true;
//		// commented Debug.Log ("target pos open gate. hi?");
	}

	public void CloseGate(){
		moving = true;
		AudioManager.inst.PlayGateOpen(transform.position);
		targetPos = closedT.position;
		gateIsOpen = false;
	}

	public void ToggleGateOpen(){
//		// commented Debug.Log ("toggle.");
		if (gateIsOpen){
			CloseGate ();
		} else {
			OpenGate ();
		}
	}
}
