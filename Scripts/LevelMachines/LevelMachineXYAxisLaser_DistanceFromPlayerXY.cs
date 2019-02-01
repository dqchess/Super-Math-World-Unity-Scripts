using UnityEngine;
using System.Collections;

public class LevelMachineXYAxisLaser_DistanceFromPlayerXY : MonoBehaviour {

	public float targetDistance = 50;
	public Transform globalBoundsLowerZ;
	public Transform globalBoundsUpperZ;
	float margin = 0.3f;
	public float lerpSpeed = 2;
	void Update(){
		float distToPlayerZ = transform.position.z - Player.inst.transform.position.z;
//		// commented Debug.Log ("my Z:"+transform.position.z+", player Z:"+Player.inst.transform.position.z);
		if ((Mathf.Abs (distToPlayerZ - targetDistance)) > margin && WithinBounds()){
			float newZ = Player.inst.transform.position.z + targetDistance;
			Vector3 targetPos = new Vector3(transform.position.x,transform.position.y,newZ);
			transform.position = Vector3.Lerp(transform.position,targetPos,Time.deltaTime * lerpSpeed);
		}
	}

	bool WithinBounds(){
		return (transform.position.z > globalBoundsLowerZ.position.z && transform.position.z < globalBoundsUpperZ.position.z);
	}
}
