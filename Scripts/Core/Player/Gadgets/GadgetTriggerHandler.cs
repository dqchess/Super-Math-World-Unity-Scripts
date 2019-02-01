using UnityEngine;
using System.Collections;

public class GadgetTriggerHandler : MonoBehaviour {

	void OnTriggerEnter(Collider other){
//		// commented Debug.Log ("Trigger enter:"+other.name);
		PlayerGadgetController.inst.GetCurrentGadget().GadgetOnTriggerEnter(other);
	}
}
