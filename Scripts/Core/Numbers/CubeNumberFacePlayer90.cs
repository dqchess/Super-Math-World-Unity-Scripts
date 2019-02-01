using UnityEngine;
using System.Collections;

public class CubeNumberFacePlayer90 : MonoBehaviour {

	public bool debug=false;
	int framesToSkip = 50;
	int frameSkip;	
	Vector3[] cubeCorners = new Vector3[4];
	Transform target;
	// Use this for initialization
	void Start () {
		frameSkip = gameObject.GetInstanceID() % framesToSkip;
		target = Camera.main.transform;
	}


	// Update is called once per frame
	void Update () {
		
		if (LevelBuilder.inst.levelBuilderIsShowing) {
			frameSkip = 0;
			return;
		}
//		if (transform.parent == NumberPool.inst) {
//			frameSkip = 100; // lol
//			return;
//		}
		frameSkip--;
		if(frameSkip > 0) { return; }

//		#if UNITY_EDITOR
//		if (GameObject.FindWithTag("MovieCamera")) {
////			target = GameObject.FindWithTag("MovieCamera").transform;
//			FaceObjectOnce(GameObject.FindWithTag("MovieCamera").transform);
//		} 
//		#endif
		frameSkip = framesToSkip;
		FacePlayerOnce();

	}

	public void FaceObjectOnce(Transform t, bool sticky=false){
		if (sticky) frameSkip = 1000;
		target = t;
		Vector3 playerDir = t.position - transform.position;
		float playerXZdist = Mathf.Sqrt(Mathf.Pow(playerDir.x,2)+Mathf.Pow(playerDir.z,2));
//		Debug.Log("xzdist;"+playerXZdist);
		if (false){ // fuck it' let's just always leave a number on top of the cube.
//		if (playerXZdist < 35f && playerDir.y > 9.5f){// && playerDir.y * playerDir.y > Utils.FlattenVector(playerDir).sqrMagnitude){
			Quaternion rot = new Quaternion();
			rot.eulerAngles = new Vector3(315,90,270);
			transform.localRotation = rot;
			//			cubeCorners[0] = transform.position + transform.right; //+ transform.forward;
			//			cubeCorners[1] = transform.position + transform.forward; //- transform.right;
			//			cubeCorners[2] = transform.position - transform.right; //- transform.forward;
			//			cubeCorners[3] = transform.position - transform.forward; // +transform.right;
			//			Vector3 closest = transform.position;
			//			foreach(Vector3 v in cubeCorners){
			//				float lastClosest = Vector3.SqrMagnitude(Camera.main.transform.position - (closest));
			//				if (lastClosest > 10000) return;
			//				float newClosest = Vector3.SqrMagnitude(Camera.main.transform.position - (v));
			//				//			if (debug) // commented Debug.Log ("last, new:"+lastClosest+", "+newClosest);
			//				if ( lastClosest > newClosest){
			//					closest = v;
			//				}
			//			}
			//			transform.LookAt(closest,transform.parent.up);
		} else {
			Quaternion rot = new Quaternion();
			rot.eulerAngles = new Vector3(0,45,0);
			transform.localRotation = rot;
			cubeCorners[0] = transform.position + transform.right; //+ transform.forward;
			cubeCorners[1] = transform.position + transform.forward; //- transform.right;
			cubeCorners[2] = transform.position - transform.right; //- transform.forward;
			cubeCorners[3] = transform.position - transform.forward; // +transform.right;
			Vector3 closest = transform.position;
			foreach(Vector3 v in cubeCorners){
				float lastClosest = Vector3.SqrMagnitude(target.position - (closest));
				if (lastClosest > 50000) return;
				float newClosest = Vector3.SqrMagnitude(target.position - (v));
				//			if (debug) // commented Debug.Log ("last, new:"+lastClosest+", "+newClosest);
				if ( lastClosest > newClosest){
					closest = v;
				}
			}
			transform.LookAt(closest,transform.parent.up);
		}

	}

	public void FacePlayerOnce(){
		FaceObjectOnce(Camera.main.transform);

		// would quats be faster?
//		Vector3 dirToPlayer = Vector3.Normalize(Player.inst.transform.position - transform.position);
//		float mag = Quaternion.FromToRotation(transform.forward + transform.right,dirToPlayer).eulerAngles.y);
//		Quaternion lookRot = Quaternion.LookRotation(dirToPlayer);
//		lookRot.eulerAngles = new Vector3(lookRot.x,Mathf.RoundToInt(lookRot.y / 90)*90,lookRot.z);
//
//		// commented Debug.Log("mag:"+mag);
//		Quaternion rot = transform.localRotation; // will be a multiple of 90n + 45
//		rot.eulerAngles.y += 90 * Mathf.RoundToInt(mag / 90);
//
//		Quaternion.
	}


}
