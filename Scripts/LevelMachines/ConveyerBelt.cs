using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyerBelt : MonoBehaviour {

	public float conveyerSpeed = 380f;
	List<Rigidbody> bodies = new List<Rigidbody>();
	public bool playWhoosh = false;
	public float playerFasterFactor = .01f;
	bool movingPlayer = false;
	public Transform outPosition;

	int framesToSkip = 15;
	int frames = 0;
	void Update(){
		if (movingPlayer){
//			float playerSpeedFactor = 1f * playerFasterFactor;

			FPSInputController.inst.motor.SetMomentum(transform.forward * conveyerSpeed * Time.deltaTime * playerFasterFactor);
//			FPSInputController.inst.motor.SetMomentum(transform.forward * conveyerSpeed * Time.deltaTime * playerFasterFactor);
//			FPSInputController.inst.motor.inputMoveDirection += transform.forward * conveyerSpeed * Time.deltaTime * playerFasterFactor;
//			SMW_FPSWE.inst.SetMomentum(transform.forward * conveyerSpeed * Time.deltaTime * playerFasterFactor);
			if (playWhoosh){ //"cannons" only
				FPSInputController.inst.motor.SuspendGravityForSeconds(2);
			}

//			FPSInputController.inst.motor.SetVelocity(transform.forward * playerFasterFactor * conveyerSpeed * Time.deltaTime);
//				FPSInputController.inst.motor.SuspendGravityForSeconds(2f);
//				SMW_FPSWE.inst.IgnoreGroundedForModMomentumForSeconds(0.3f);
//				SMW_FPSWE.inst.ModGravity(-20f);
//			}
		}
//		frames++;
//		if (frames > framesToSkip){
//			frames = 0;
//		} else {
//			return;
//		}
		List<Rigidbody> toDel = new List<Rigidbody>();
		foreach(Rigidbody rb in bodies){
			if (rb && rb.gameObject.activeSelf) {
				Vector3 localPosOfObject = transform.InverseTransformPoint(rb.transform.position);
//				float xOffset = localPosOfObject.x;
				bool targetHasOtherPlans = false;
				Animal an = rb.GetComponent<Animal>();
				if (an){
					if (an.target != null){
						if (an.target.transform){
							// Animals don't get conveyed towards the center of the conveyer belt, but they DO still get conveyed generally.
							targetHasOtherPlans = true;	
						}
					}
				} //
				int dir = (Mathf.Abs(localPosOfObject.x) > 0.05f && !targetHasOtherPlans) ? (localPosOfObject.x > 0 ? 1 : -1) : 0;// This dir is for rightforce
				Vector3 rightforce = -transform.right * dir * 50; // rightforce decides if the object should be nudged towards the "center" of the conveyer as its conveyed
				rb.velocity = (rightforce + transform.forward * conveyerSpeed) * Time.deltaTime; // AddForce(transform.forward*Time.deltaTime*conveyerSpeed);
			}
			else toDel.Add(rb);
		}
		foreach(Rigidbody rb in toDel){
			bodies.Remove(rb);
		}

	}



	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Rigidbody>() && other.GetComponent<NumberInfo>() ){
			if (bodies.Contains(other.GetComponent<Rigidbody>()) || other.GetComponent<GadgetTriggerHandler>()){ // exception for sword gadget!
				return;
			} else{
				bodies.Add(other.GetComponent<Rigidbody>());
			} 
//			other.transform.right = -transform.forward;
			if (playWhoosh){
				other.transform.position = (Random.insideUnitSphere * 0.2f) + outPosition.position;
				AudioManager.inst.PlayWhoosh(transform.position);
			}
		} else if (other.tag == "Player"){
			movingPlayer = true;
			if (playWhoosh){
				AudioManager.inst.PlayWhoosh(transform.position);
				other.transform.position = (Random.insideUnitSphere * 0.2f) + outPosition.position;
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<Rigidbody>()){
			if (bodies.Contains(other.GetComponent<Rigidbody>())){
				bodies.Remove(other.GetComponent<Rigidbody>());
			}
			//			other.transform.right = -transform.forward;
		} else if (other.tag == "Player"){
			movingPlayer = false;
		}
	}

}
