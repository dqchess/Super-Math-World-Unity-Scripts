using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGemCollector : MonoBehaviour {


	public LayerMask gemLayerMask;

	float distToAttract = 10f;
	float sqrDistToGrab = 6f;
	float attractSpeed = 8f;
	// Update is called once per frame
	void Update () {
		foreach(Collider c in Physics.OverlapSphere(transform.position,distToAttract,gemLayerMask)){
			PickUppableObjectGem gem = c.gameObject.GetComponent<PickUppableObjectGem>();
			if (gem){
				float sqrDistToPlayer = Vector3.SqrMagnitude(gem.transform.position-Player.inst.transform.position);
//				if (Random.value > 0.1f){
//					// every 10 frames stop it from spinning
//					gem.GetComponent<Rigidbody>().velocity = Vector3.zero;
//				}
				Vector3 playerDir = Vector3.Normalize(Player.inst.transform.position - gem.transform.position);
				gem.GetComponent<Rigidbody>().velocity = (playerDir + Vector3.up*.1f) * attractSpeed;
				if (sqrDistToPlayer < sqrDistToGrab){
					gem.PickupGem();
				}


			}
		}
	}
}
