using UnityEngine;
using System.Collections;


public class LevelMachineMoveCollidersToParent : MonoBehaviour {




	void OnTriggerEnter(Collider other){
		if (other.GetComponent<NumberInfo>()){
			other.transform.parent = transform.parent;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.GetComponent<NumberInfo>()){
			other.transform.parent = null;
		}
	}

}
