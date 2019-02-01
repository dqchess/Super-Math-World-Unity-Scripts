using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCube : MonoBehaviour {


	void OnTriggerEnter(Collider other){
		AudioManager.inst.WaterSplash1(other.transform.position,1f);
		EffectsManager.inst.WaterSplash(other.transform.position);
		if (other.GetComponent<Animal>()){
			other.GetComponent<Animal>().SetUnderwater(true);
		} else if (other.transform.root.GetComponentInChildren<Vehicle>()){
			other.transform.root.GetComponentInChildren<Vehicle>().SetUnderwater(true);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<Animal>()){
			other.GetComponent<Animal>().SetUnderwater(false);
		}
	}
}
