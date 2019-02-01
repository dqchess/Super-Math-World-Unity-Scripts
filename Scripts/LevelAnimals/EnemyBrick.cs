using UnityEngine;
using System.Collections;

public class EnemyBrick : MonoBehaviour {
	
	
	void Start(){
//		gameObject.GetComponent<NumberInfo>().numberIsAlive=true;
//		gameObject.tag = "Monster";
		// Add a box collider for extra destruction!
		
//		BoxCollider bc = gameObject.AddComponent<BoxCollider>()  as BoxCollider;
//		SphereCollider sc = gameObject.GetComponent<SphereCollider>()  as SphereCollider;
//		if (sc && bc) {
//			bc.size *= sc.radius*2.05f;
//			bc.isTrigger=true;
//		} else {
//			name +=" bc sphere box";
//			// commented Debug.Log(name+" did not have box collider and sphere collider for some reason.");
//		}
	}
//	public void OnChangeSign(){
//		
//		if (!gameObject.GetComponent<NumberInfo>().isFlippable){
//			// commented Debug.Log("Tried to flip but number was too loyal.");
//			return;
//		}
//		if (gameObject.GetComponent<NumberAttackPlayer>()) {
//			Destroy(gameObject.GetComponent<NumberAttackPlayer>());
//		}
//		
////		// commented Debug.Log("sign changed.");
//		if (transform.parent) transform.parent=null; // unparent from enemy
//		
//		if (!rigidbody) gameObject.AddComponent<Rigidbody>();
//		
//		if (GetComponent<HingeJoint>()) Destroy(GetComponent<HingeJoint>());
//		if (GetComponent<FixedJoint>()) Destroy(GetComponent<FixedJoint>());
//		
//		
//		rigidbody.useGravity=true;
//		rigidbody.isKinematic=false;
//		
//		NumberInfo ni = GetComponent<NumberInfo>();
//		ni.comb=Combineability.inverseOnly;
//		ni.pickupFlag=true;
//		gameObject.tag="Number";
//		gameObject.layer=8; // regular!
//		Destroy(this);
////		ni.SetStability(false); // it will disappear!
////		ni.SetInstabilityTimer(7); // in seven seconds!!!
//		// OMG you guys!!
//	}
	
	void OnCollisionEnter(Collision hit){
		if (hit.collider.tag == "Player"){
			gameObject.SendMessage("OnPlayerTouch",SendMessageOptions.DontRequireReceiver);
//			PlayerHealthManager.PlayerTakeDamage();
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag=="Player"){
//			PlayerHealthManager.PlayerTakeDamage();
		}
	}

	void ReturningToPool() {
		Destroy (this);
	}
	
	
}
