using UnityEngine;
using System.Collections;

public class FollowTransform : MonoBehaviour {


	public Transform objToFollow;
	public bool keepRelativePosition = true;
	Vector3 relativePosStart;
	 float followSpeed = 10;

	// Use this for initialization
	void Start () {
		relativePosStart = transform.position - objToFollow.position;
//		// commented Debug.Log ("relativeStartPos:"+relativePosStart);
	}
	
	// Update is called once per frame
	void Update () {
//		float lerpSpeed = 3;
		if (!objToFollow) Destroy (this);
		Vector3 newPos = objToFollow.position + relativePosStart;
//		// commented Debug.Log("obj pos:"+objToFollow.position+", my target pos:"+newPos);
		transform.position = Vector3.Lerp (transform.position,newPos,Time.deltaTime * followSpeed);
	}
}
