using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryPreventCombine : MonoBehaviour {


	void OnDisable(){
		// when player collects, obj is disabled, so this is how we detect it
		//		Debug.Log("disb");
		Destroy(this);
	}

	float destructTimer = 1.5f;

	void Update () {
		destructTimer -= Time.deltaTime;
		if (destructTimer < 0){
			Destroy(this);
		}
	}
}
