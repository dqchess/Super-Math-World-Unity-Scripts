using UnityEngine;
using System.Collections;


public class ConveyerBeltSwitch : MonoBehaviour {


	public Transform leftPosition;
	public Transform rightPosition;
	public Transform target;
	public GameObject conveyer;

	bool needsMove = false;
	void Start(){
//		target = leftPosition;
	}

	public void FlipSwitch(){
//		AudioManager.inst.PlayDoorLever(transform.position);
		needsMove = true;
		target = target == leftPosition ? rightPosition : leftPosition;
	}

	void Update(){
		if (needsMove){
			float lerpSpeed = 2;
			conveyer.transform.rotation = Quaternion.Lerp(conveyer.transform.rotation,target.rotation,Time.deltaTime * lerpSpeed);
			if (Vector3.Angle(target.forward,conveyer.transform.forward) < 1){
				conveyer.transform.rotation = target.rotation;
				needsMove = false;
			}
//			if (Vector3.Angle(
		}
	}
}
