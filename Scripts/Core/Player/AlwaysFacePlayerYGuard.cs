using UnityEngine;
using System.Collections;

public class AlwaysFaceCameraYGuard : MonoBehaviour {
	
	float defaultY;
	float jumpTime = 0;
	bool jumping = false;
	float rotateAmount = 0;

	
	Transform target;
	// Use this for initialization
	void Start () {
		defaultY = transform.position.y;
//		Destroy(this);
		CheckTarget();
		
		//else // commented Debug.Log("I didn't find player: "+name);
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (transform.localRotation.x != 0 || transform.localRotation.z != 0){
//			Quaternion newRot=Quaternion.identity;
//			newRot.eulerAngles = new Vector3(0,transform.localRotation.eulerAngles.y,0);
//			transform.rotation = newRot; // stand up straight.
		}
		if (target) {
			Transform pt = transform; //.parent.transform;
			
			//transform.rotation = Quaternion.LookRotation(target.position-transform.position,Vector3.up);
			Vector3 fxz = pt.forward;
			fxz.y = 0;
			fxz.Normalize();
			
			Vector3 playerOff = (target.position - pt.position);
			playerOff.y = 0;
			playerOff.Normalize();
			
			
			rotateAmount = Vector3.Angle(fxz, playerOff);
			float dir = Vector3.Dot(pt.right, playerOff);
			if(Mathf.Abs(rotateAmount) > 30 && !jumping) {
				jumping = true;	
			}
			
			if(jumping) {
				jumpTime += Time.deltaTime*2;
				if(jumpTime >= 1) { jumpTime = 0; jumping = false; }
				pt.position = new Vector3(pt.position.x, defaultY + Mathf.Sin(jumpTime * 3.14159f) * 4, pt.position.z);
				pt.RotateAround(pt.position, Vector3.up, (dir > 0 ? 70 : -70) * Time.deltaTime);

			}
		}
		else CheckTarget();
		
	}
	
	void CheckTarget(){
		if (!target) {
			if (Camera.main)
				target = Camera.main.transform;
		}
	}
//	// commented Debug.Log("got targeT?"+target)

}
