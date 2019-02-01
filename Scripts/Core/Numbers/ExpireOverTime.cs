using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpireOverTime : MonoBehaviour {

	// This script is added to objects we want to disappear after not being used for a long time (e.g. numers from faucets) that are also not near the player (don't surprise player by destroying objects)
	int expireTime = 100;
	float careDistance = 50f;
	void Update(){
		if (gameObject.activeSelf && Utils.IntervalElapsed(1)){ // every second, only while active
			if (Vector3.Distance(Player.inst.transform.position,transform.position) > careDistance){
				expireTime -= 1;
				if (expireTime < 0){
					Destroy(gameObject);
				}
			}
		}
	}
}
