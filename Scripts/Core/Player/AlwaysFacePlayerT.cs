using UnityEngine;
using System.Collections;

public class AlwaysFacePlayerT : MonoBehaviour {
	
	public Transform target;
	// Use this for initialization
	void Start () {
//		Destroy(this);
		target = Player.inst.transform;
	}
	
	int frameskip = 0;
	public int framesToSkip = 4;
	// Update is called once per frame
	void LateUpdate () {
		if (frameskip < framesToSkip) { 
			frameskip ++;
			return; 
		}


		frameskip = 0;
		#if UNITY_EDITOR
		target = GetTarget();
		LookOnce();
		return;
		#endif

		if (target) {
			LookOnce();

		} else {
			target = GetTarget();
		}
	}

	Transform GetTarget(){
		#if UNITY_EDITOR
		if (GameObject.FindWithTag("MovieCamera")) {
			return GameObject.FindWithTag("MovieCamera").transform;
		}
		#endif
//		// commented Debug.Log("cam main transF:"+Camera.main.transform);
		return Player.inst.transform;
	}
	public void LookOnce(){
		if (target){
			transform.LookAt (target);
		}
	}
//	bool TrySleep(){
//		return false;
//	}
}
