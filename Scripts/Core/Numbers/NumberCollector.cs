using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCollector : MonoBehaviour {

	// This script uses a trigger to make sure numbers that enter its collider do not combine each other.



	void OnTriggerEnter(Collider other){
		if (ValidNumber(other)){
			other.transform.parent = transform.root;
		}
	}

	void OnTriggerExit(Collider other){
		if (ValidNumber(other) && other.transform.parent == transform){
			other.transform.parent = null;
		}
	}

	bool ValidNumber(Collider other){
		return other.GetComponent<NumberInfo>() && !other.GetComponent<MonsterAIBase>() && !other.GetComponent<Animal>();
	}

	void Update(){
		if (Utils.IntervalElapsed(2f)){
			RemoveChildrenWhoAreDisabled();

		}
	}

	void RemoveChildrenWhoAreDisabled(){
		// In case the player picked up a child, make sure we aren't keeping it as a parent.
		List<Transform> toRemove = new List<Transform>();
		foreach(Transform t in transform.parent){
			NumberInfo ni = t.GetComponent<NumberInfo>();
			if (ni && !ni.gameObject.activeSelf){
				toRemove.Add(ni.transform);	
			}
		}
		foreach(Transform t in toRemove){
			t.parent = null;
		}
	}


}
