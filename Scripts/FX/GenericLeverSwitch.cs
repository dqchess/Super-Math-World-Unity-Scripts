using UnityEngine;
using System.Collections;

public class GenericLeverSwitch : ObjectMessenger {

	public Transform leftPosition;
	public Transform rightPosition;
	public Transform target;
	public GameObject leverObject;
//	public bool constrainReuse = false; // wait for object to give OK before flipping lever
//	public string booleanRestraintFunction; // e.g. public void CanFlipLever() { if (finishemdoving) return true; else return false }
//	public GameObject restraintFunctionObject; // e.g. the object with CanFlipLever bool method

	bool moving = false;
	float leverPullRange = 7;

	void Start(){
		//		target = leftPosition;
	}

	public void FlipLever(){
		SendMessage();
		moving = true;
//		// commented Debug.Log("movee true");
		AudioManager.inst.PlayDoorLever(transform.position);
		target = target == leftPosition ? rightPosition : leftPosition;
	}

	void Update(){
		
		if (moving){
			float lerpSpeed = 2;
			leverObject.transform.rotation = Quaternion.Lerp(leverObject.transform.rotation,target.rotation,Time.deltaTime * lerpSpeed);
			if (Vector3.Angle(target.forward,leverObject.transform.forward) < 1){
				leverObject.transform.rotation = target.rotation;
				moving = false;
//				// commented Debug.Log("movee false");
			}
			//			if (Vector3.Angle(
		} else {
			if (Input.GetKeyDown(KeyCode.F) && Vector3.Distance(Player.inst.transform.position,transform.position) < leverPullRange && !moving){
				FlipLever();
			}
		}
	}
}
