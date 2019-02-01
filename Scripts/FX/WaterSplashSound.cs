using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterSplashSound : MonoBehaviour {

	Dictionary<Collider,float> recentlyCollided = new Dictionary<Collider,float>();
	// Use this for initialization
	void Start () {
		gameObject.isStatic=true;
	}
	
	// Update is called once per frame
	float timer = 0;
	void Update () {
		timer -= Time.deltaTime;
		runningInWaterTimer -= Time.deltaTime;
		List<Collider> toRemove = new List<Collider>();
		foreach(KeyValuePair<Collider,float> kvp in recentlyCollided){
			if (Time.time - kvp.Value > 2f){
				toRemove.Add (kvp.Key);
			}
		}
		foreach(Collider c in toRemove){
			recentlyCollided.Remove(c);
		}
	}


	void OnTriggerEnter(Collider other){
		
//		// commented Debug.Log("hi: splashing "+other.name);
		if (other.CompareTag("Player")){
			if (timer < 0){
				timer = Random.Range (2,5f);
				AudioManager.inst.WaterSplash1(other.transform.position);
				Instantiate(EffectsManager.inst.waterSplash,other.transform.position,Quaternion.identity);
			}
			return;
		} 


		// Not player / other objs
		if (recentlyCollided.ContainsKey(other) || other.GetComponentInParent<Vehicle>()) return;
		AudioManager.inst.WaterSplash1(other.transform.position);
		Instantiate(EffectsManager.inst.waterSplash,other.transform.position,Quaternion.identity);
		recentlyCollided.Add (other,Time.time);
//		if (other.GetComponent<Rigidbody>()) {
//				other.GetComponent<Rigidbody>().AddForce(other.transform.forward*200); // WHAT?
//		}
	}

	float runningInWaterTimer = 0;
	void OnTriggerStay(Collider other){
//		// commented Debug.Log("stay:"+other.name);
		if (runningInWaterTimer < 0){
			runningInWaterTimer = Random.Range(.4f,.6f);
			if (other.CompareTag("Player")){
//				// commented Debug.Log("player");
				if (!PlayerUnderwaterController.inst.playerUnderwater && FPSInputController.inst.motor.inputMoveDirection.magnitude > 10 && !Player.frozen){
//					// commented Debug.Log("mag;"+ FPSInputController.inst.motor.inputMoveDirection.magnitude);
					AudioManager.inst.WaterSplash1(other.transform.position,1f);
					EffectsManager.inst.WaterSplash(other.transform.position);

//					// commented Debug.Log("player running");
				} 
			}
		}
	}
}
