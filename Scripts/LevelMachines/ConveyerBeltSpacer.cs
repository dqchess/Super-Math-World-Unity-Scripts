using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyerBeltSpacer : MonoBehaviour {

	List<GameObject> objectsInSpacer = new List<GameObject>();
	public GameObject spacerIn;

	void OnTriggerEnter(Collider other){
		if (other.tag != "Player") {
//			Debug.Log("in spacer;"+other.name);
			if (!objectsInSpacer.Contains(other.gameObject)){
				objectsInSpacer.Add(other.gameObject);
			}
			foreach(Transform t in spacerIn.transform){
				t.GetComponent<Collider>().enabled = true;
				t.GetComponent<Renderer>().enabled = true;
			}
			other.transform.parent = transform;
		}
	}

	void OnTriggerExit(Collider other){
		if (other.tag != "Player"){
//			Debug.Log("out spacer;"+other.name);
			objectsInSpacer.Remove(other.gameObject);
			foreach(Transform t in spacerIn.transform){
				t.GetComponent<Collider>().enabled = false;
				t.GetComponent<Renderer>().enabled = false;
			}
			other.transform.parent = null;
		}
	}

}
