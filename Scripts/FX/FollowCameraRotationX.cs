using UnityEngine;
using System.Collections;

public class FollowCameraRotationX : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	bool follow=true;
	void LateUpdate () {
		if (Input.GetMouseButtonDown(1)) follow=false;
		if (Input.GetMouseButtonUp(1)) follow=true;
		if (follow) {
			transform.forward = Camera.main.transform.forward;
//			Quaternion rot = new Quaternion();
//			rot.eulerAngles = new Vector3(Camera.main.transform.rotation.eulerAngles.x,0,0);
//			transform.localRotation = rot;
		}

	}
}
