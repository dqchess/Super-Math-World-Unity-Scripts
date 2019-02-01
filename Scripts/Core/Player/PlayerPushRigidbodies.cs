using UnityEngine;
using System.Collections;

public class PlayerPushRigidbodies : MonoBehaviour {


	public float startPower = 1;
	public float pushPower = 1; // duplicate (save orig value)
	
	
	
	void OnControllerColliderHit(ControllerColliderHit hit){
//		Debug.Log("push hit;"+hit.collider.name);
	  Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;
        
        if (hit.moveDirection.y < -0.3F)
            return;

//		if (body.GetComponent<AINumberSheep>()) {
//			GlobalVars.inst.Player.inst.transform.position += Vector3.Normalize(GlobalVars.inst.Player.inst.transform.position-body.transform.position)+Vector3.up;
//			return;
//		}
        
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
	}
}
