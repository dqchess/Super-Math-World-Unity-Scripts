using UnityEngine;
using System.Collections;

public class RotateBasedOnSpeed : MonoBehaviour {

	Rotate r;
	RecordPosition rp;
	public float minRotateSpeed = 30;
	public float maxRotateSpeed = 360;
	public float deltaPosPerFrameToAchieveMaxRotation = 1f; // approx 30 m/s
	void Start(){
		r = GetComponent<Rotate>();
		rp = GetComponent<RecordPosition>();
	}

	void Update(){
		float deltaPos = Vector3.Magnitude(rp.lastPosition-rp.nowPosition);
		float deltaSpeedRange = maxRotateSpeed - minRotateSpeed;
		r.rotateSpeed = deltaPos / deltaPosPerFrameToAchieveMaxRotation * deltaSpeedRange + minRotateSpeed;
	}
}
