using UnityEngine;
using System.Collections;

public class FollowTransformXZ : MonoBehaviour {


	public Transform objToFollow;

	void Update () {
		transform.position = new Vector3(objToFollow.position.x,transform.position.y,objToFollow.position.z);//Vector3.Lerp (transform.position,newPos,Time.deltaTime * followSpeed);
	}
}
