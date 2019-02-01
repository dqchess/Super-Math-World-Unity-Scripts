using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetBoomerangFlyingObject : MonoBehaviour {

	List<Collider> touched = new List<Collider>();

	Quaternion originalRotation;
	float rotateSpeed = 550;
	Vector3 thrownDirection;

	float timeThrown = 0;
	float speed = 32;
	float totalTimetoFly = 0f;


	public void Init( float t){
		totalTimetoFly = t;
		timeThrown = Time.time;
		thrownDirection = Player.inst.transform.forward;
	}

	void Update(){

		Move();
		Spin();
		DestroyAfterFlytimeElapsed();

	}

	void Move (){
		Vector3 dir = Vector3.zero;
		if (Time.time < timeThrown + totalTimetoFly / 2f){
			// AWAY from player
			dir = thrownDirection;

			transform.position += dir * Time.deltaTime * speed;
		} else if (Time.time < timeThrown + totalTimetoFly / 1.5f) {
			// BACK to player
			Vector3 dirToPlayer = Vector3.Normalize(Player.inst.transform.position - transform.position);
			dir = dirToPlayer;

			transform.position += dir * Time.deltaTime * speed;
		} else {
			float lerpSpeed = 3;
			transform.position = Vector3.Slerp(transform.position,Player.inst.transform.position,Time.deltaTime * lerpSpeed);

		}
		
		StayAboveGround();

	}

	void StayAboveGround(){
		RaycastHit hit;
		float minDistToGround = 3.5f;
		if (Physics.Raycast(transform.position,Vector3.down,out hit,minDistToGround, FindObjectOfType<SceneLayerMasks>().terrainOnly)){
			transform.position += (minDistToGround - hit.distance) * Vector3.up; // Vector3.up * Time.deltaTime;
//			// commented Debug.Log ("HIT A SHIT");
		} else {
//			// commented Debug.Log ("DIDNT HIT SHIT");
		}
	}


	void Spin(){
		Quaternion newRot = transform.rotation;
		newRot.eulerAngles += new Vector3(0,0,Time.deltaTime * rotateSpeed);
		transform.rotation = newRot;
	}

	void DestroyAfterFlytimeElapsed(){
		if (Time.time > timeThrown + totalTimetoFly) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni && ni.enabled){
			if (ni.fraction.numerator <= 0) return;
			if (ni.fraction.numerator > 4096) return;
			EffectsManager.inst.StartAnimationSequence(ni);

		}
	}
}
