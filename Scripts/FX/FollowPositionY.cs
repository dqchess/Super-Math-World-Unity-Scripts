using UnityEngine;
using System.Collections;

public class FollowPositionY : MonoBehaviour {

	// This odd script is specifically to eliminate weird left-right (local x axis) jitter motion. The jitter was inherent in the Mixamo Mascot animation and couln't be erased, 
	// so we maintain a steady X position for the head while following the animation's Y and Z positions. This eliminates X axis motion for the head.

	public Transform bobbingT;
	Vector3 startPos;
	Vector3 offset;
	Vector3 startPosLocal;
	Vector3 startOffsetLocal;

	// Use this for initialization
	void Start () {
		startPos = bobbingT.transform.position - Player.inst.transform.position;
//		startPosLocal = transform.localPosition;
//		offset = transform.localPosition - startPos;
		startOffsetLocal = transform.position - Player.inst.transform.position;
		// how far was the bobbing transform from the player root at start?
//		offset = transform.localPosition.y - targetY.localPosition.y - GlobalVars.inst.Player.inst.transform.position.y;

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 nowPos = bobbingT.transform.position - Player.inst.transform.position;
		Vector3 deltaPos = nowPos - startPos;
		transform.position = Player.inst.transform.position + startOffsetLocal + new Vector3(0,deltaPos.y,deltaPos.z);//new Vector3(transform.localPosition.x,startPosLocal.y + deltaPos.y,startPosLocal.z + deltaPos.z);
		// what is the delta between the bobbing transform from layer root at start and now?


//		float deltaY = targetY.transform.position.y - GlobalVars.inst.Player.inst.transform.position.y;
//		transform.localPosition = new Vector3(transform.localPosition.x,deltaY+offset,transform.localPosition.z);
//		// commented Debug.Log ("target y :" +targetY.transform.localPosition.y);
	}
}
