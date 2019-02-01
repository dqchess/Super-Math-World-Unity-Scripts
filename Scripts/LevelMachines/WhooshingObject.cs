using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhooshingObject : MonoBehaviour {

	public ConveyerWhoosherSpeeder whoosher;
	float time = 0;
	float maxTime = 15f; // no whoosher should last long
	bool isPlayer = false;
	float upOffset = 0f;
	void Start(){
		if (GetComponent<Player>()){
			isPlayer = true;
			upOffset = -1f;
		}
	}

	void Update(){
		time += Time.deltaTime;
		if (isPlayer) {
			MascotAnimatorController.inst.SetGrounded(false);

		}
		if (time > maxTime) {
			Die();
		}
		if (whoosher) { 
			WhooshingObjectInfo wi = whoosher.GetWhooshingObjectPositionAtTime(time);
			if (wi.endArcThreshholdReached) {
//				Debug.Log("reached");
				Die();
				return;
			}
//			if (isPlayer){
////				FPSInputController.inst.motor.SuspendGravityForSeconds(.1f);
////				transform.position = wi.position + Vector3.up * upOffset;
////				foreach(Collider c in Physics.OverlapSphere(wi.position,0.5f)){
////					// one minute I was colliding with the "Pillar pad" object in mid-flight, the next I wasn't
////					// Literally no clue how a single collider mesh can suddenly stop working in 100% of cases when this script nor the collision mesh have changed
////					// so now we do this ridiculous expensive spherecast every frame to detect hits for these meshes that won't be detected by OnControllerColliderHit
////					if (c.isTrigger) continue;
////					if (c.GetComponent<Player>()) continue;
//////					Debug.Log("sphc:"+c.name);
////					CollidedWith(c);
////					break;
////				}
//			} else {
				transform.position = wi.position + Vector3.up * upOffset;
//			}
		} else {
			if (time > 1) {
				// no whoosher, what happened? eh well destroy
				Die();
			}
		}
	}


//	void OnControllerColliderHit(ControllerColliderHit h){
//		Debug.Log("whoosher hit;"+h.collider.name);
//		CollidedWith(h.collider);
//	}
//	void OnCollisionEnter(Collision hit){
//		CollidedWith(hit.collider);
//	}

	void CollidedWith(Collider c){
		if (c && c.gameObject.GetComponent<WhooshingObject>()){
			// don't break for other whooshing objets you hit midair.
			return; 
		} else {
			Die();
		}
	}
	void Die(){
		if (isPlayer) FPSInputController.inst.motor.SetVelocity(Vector3.zero);
		else if (GetComponent<Rigidbody>()) {
			WhooshingObjectInfo wi1 = whoosher.GetWhooshingObjectPositionAtTime(time-1);
			WhooshingObjectInfo wi2 = whoosher.GetWhooshingObjectPositionAtTime(time);
			GetComponent<Rigidbody>().velocity = wi2.position - wi1.position;
		}
		Destroy(this);
	}
}
