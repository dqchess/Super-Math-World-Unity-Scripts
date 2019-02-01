using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCountMaxEnforcer : MonoBehaviour {

	// This script uses a trigger to make sure numbers that enter its collider do not combine each other.

	List<NumberInfo> numbers = new List<NumberInfo>();
	public int max = 12;

	void OnTriggerEnter(Collider other){
		if (ValidNumber(other)){
			numbers.Add(other.GetComponent<NumberInfo>());
		}
		EnforceMax();
	}

	void EnforceMax(){
		RemoveChildrenWhoAreDisabled();
		if (numbers.Count > max){
			if (numbers[0] && numbers[0].gameObject){
				Destroy(numbers[0].gameObject);
				numbers.RemoveAt(0);
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (ValidNumber(other)){
			if (numbers.Contains(other.GetComponent<NumberInfo>())){
				numbers.Remove(other.GetComponent<NumberInfo>());
			}
		}

	}


	bool ValidNumber(Collider other){
		return other.GetComponent<NumberInfo>() && !other.GetComponent<MonsterAIBase>() && !other.GetComponent<Animal>();
	}

	float t = 0;
	void Update(){
		t -= Time.deltaTime;
		if (t<0){
			t = 2;
			RemoveChildrenWhoAreDisabled();
		}
//		if (Utils.IntervalElapsed(2f)){
//
//		}
	}

	void RemoveChildrenWhoAreDisabled(){
		// In case the player picked up a child, make sure we aren't keeping it as a parent.
		List<NumberInfo> toRemove = new List<NumberInfo>();
		foreach(NumberInfo ni in numbers){
			if ((ni && !ni.gameObject.activeSelf) || !ni || !ni.gameObject){
				toRemove.Add(ni);	
			}
		}
		foreach(NumberInfo n in toRemove){
			if (numbers.Contains(n)){
				numbers.Remove(n);
			}
		}
	}


}
