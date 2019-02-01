using UnityEngine;
using System.Collections;


public class PlayerRestrictPickupAfterThrow : MonoBehaviour {




	bool waitForPlayerDistance = false;
	float startDist = 0;
	float additionalDistancePlayerMustBeBeforePickupReEnabled = 5;
	int origLayer;

	void Start(){
		origLayer = gameObject.layer;
	}

	void OnPlayerThrow(){
		waitForPlayerDistance = true;
		startDist = Vector3.Magnitude(transform.position - Player.inst.transform.position);
		gameObject.layer = LayerMask.NameToLayer("DontCollideWithPlayer");
		// commented Debug.Log("this one");
		// commented Debug.Log ("my new layer:"+gameObject.layer);
	}

	void Update(){
		if (waitForPlayerDistance){
			float nowDist = Vector3.Magnitude(transform.position - Player.inst.transform.position);
			if (nowDist > startDist + additionalDistancePlayerMustBeBeforePickupReEnabled){
				waitForPlayerDistance = false;
				gameObject.layer = origLayer;
				// commented Debug.Log ("my back layer:"+gameObject.layer);
			}
		}
	}
}
