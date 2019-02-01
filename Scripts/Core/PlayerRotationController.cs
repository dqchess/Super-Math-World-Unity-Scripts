using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationController : MonoBehaviour {

	// This script makes the character face in the direction its moving, unless you fire a gadget in which case rotation snaps back to "forwards".
	public Transform localRotObject; // needed for keeping track of relative local rotations since the player body is not a child of us 

	bool MovingLeft(){
		return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
	}

	bool MovingUp(){
		return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
	}

	bool MovingDown(){
		return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
	}

	bool MovingRight(){
		return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
	}
	void LateUpdate(){
		Transform r = PlayerCostumeController.inst.curCharInfo.root;
		if (Player.frozen){ // vehicles, etc
			if (PlayerVehicleController.inst.currentVehicle){
				Quaternion vehicleRot = Utils.FlattenRotation(PlayerVehicleController.inst.currentVehicle.transform.rotation);
				Player.inst.transform.rotation = vehicleRot;
				r.rotation = vehicleRot;
//				Debug.Log("player frozen vehcile");
			} else {
//				Debug.Log("player frozen no vehcile");
			}
//			r.rotation = Quaternion.identity;
			return;
		} else {
//			Debug.Log("player not froze at all.");
		}
		float targetYrotation = -1;
//		if (Input.GetAxis("Horizontal") < -.5f){
		if (MovingLeft()){
			if (MovingUp()){
				targetYrotation = -45f;

			} else if (MovingDown()){
				targetYrotation = -135f;
			} else {
				targetYrotation = -90f;
			}
		} else if (MovingRight()){
			if (MovingUp()){
				targetYrotation = 45f;
			} else if (MovingDown()){
				targetYrotation = 135f;
			} else {
				targetYrotation = 90f;
			}
		} else if (MovingDown()){
			targetYrotation = -180f;	
		} else if (MovingUp()){
			targetYrotation = 0;
		}

		if (Input.GetMouseButton(0)){
			// snap to zero rot for firing gadgets
			localRotObject.localRotation = Quaternion.identity;
			r.rotation = localRotObject.rotation;
		} else if (targetYrotation != -1){
//			Debug.Log("targety:"+targetYrotation);
			float rotSpeed = 20f;
			Quaternion rot = new Quaternion();
			float curY = Mathf.MoveTowardsAngle(localRotObject.localRotation.eulerAngles.y,targetYrotation,rotSpeed);
			int sign = Camera.main.transform.forward.y > 0 ? -1 : 1; // facing up or down?
			if (Input.GetAxis("Vertical") < 0) sign *= -1; // flip sign if we're swimming backwards.
			float lessFactor = 0.6f; // don't rotate all the way, it's just for fx
			float targetXrotation = Vector3.Angle(Camera.main.transform.forward,Utils.FlattenVector(Camera.main.transform.forward)) * sign * lessFactor;
			float rotSpeedPitch = 0.9f; // slower than left-right yaw
			float curX = 0;
			if (PlayerUnderwaterController.inst.playerUnderwater){ // face char up or down if swimming
				curX = Mathf.MoveTowardsAngle(localRotObject.localRotation.eulerAngles.x,targetXrotation,rotSpeedPitch);
			}
			rot.eulerAngles = new Vector3(curX,curY,0);
			if (PlayerUnderwaterController.inst.playerUnderwater){ // face char up or down if swimming
//				rot.eulerAngles +=  new Vector3(Vector3.Angle(Camera.main.transform.forward,Utils.FlattenVector(Camera.main.transform.forward)),0,0) * sign;

				// the following was an attempt to lerp that rotation but it didn't work.
//				int sign = Camera.main.transform.forward.y > 0 ? -1 : 1; // facing up or down?
//				if (Input.GetAxis("Vertical") < 0) sign *= -1; // flip sign if we're swimming backwards.
//				float targetAngleX = Vector3.Angle(Camera.main.transform.forward,Utils.FlattenVector(Camera.main.transform.forward)) * sign;
//				float actualX = r.localEulerAngles.x;
//				float pitchSpeed = 1f; // how fast to lerp player to new x rot?
//				float deltaX = Mathf.Lerp(actualX,targetAngleX,Time.deltaTime * pitchSpeed);
//				rot.eulerAngles += new Vector3(deltaX,0,0);
			}
			localRotObject.localRotation = rot; // match our child.localrotobject's local rotation to the desired direction
			r.rotation = localRotObject.rotation; // match global rotation of character object to global rotation of our child.localrotobject

		}

	}
}
