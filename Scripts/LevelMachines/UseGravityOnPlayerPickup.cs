using UnityEngine;
using System.Collections;

public class UseGravityOnPlayerPickup : MonoBehaviour {

	void Start(){
		PlayerGadgetController.inst.gadgetThrow.onPlayerCollect += Collected;
	}

	public void Collected(GameObject o){
		if (gameObject == o){
			PlayerGadgetController.inst.gadgetThrow.onPlayerCollect -= Collected;
			if (GetComponent<Rigidbody>()){
				GetComponent<Rigidbody>().useGravity = true;

			}
			Destroy(this);
		}
	}

	void OnDestroy(){
		PlayerGadgetController.inst.gadgetThrow.onPlayerCollect -= Collected;
	}
}
