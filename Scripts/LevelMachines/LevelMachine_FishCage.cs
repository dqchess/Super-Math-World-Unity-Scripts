using UnityEngine;
using System.Collections;

public class LevelMachine_FishCage : MonoBehaviour {

	Vector3 startPos;
	Vector3 upPos;
	Vector3 targetPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		upPos = transform.position + Vector3.up * 55;
		targetPos = startPos;

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.N)){
			targetPos = upPos;
		} else if (Input.GetKeyDown (KeyCode.M)){
			targetPos = startPos;
		}
		float moveSpeed = 0.1f;
		transform.position = Vector3.Lerp(transform.position,targetPos,Time.deltaTime * moveSpeed);
	}
}
