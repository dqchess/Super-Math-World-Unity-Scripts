using UnityEngine;
using System.Collections;

public class SometimesFacePlayer : MonoBehaviour {


	public Transform target;
	// Use this for initialization
	float minSqrDistToRoll = 300; // closer than this, do not allow number to roll and not face player.
	NumberInfo ni;
	void Start () {
		target = Camera.main.transform;
		FacePlayerOnce();
		ni = transform.root.GetComponentInChildren<NumberInfo>();
	}


	void OnPlayerThrow(){
		// commented Debug.Log ("thrown");
		timer = 1.3f;
	}
	
	// Update is called once per frame
	float timer=0;
	float lookDuration=0;

	void Update () {
		if (Time.timeScale == 0) return;
		timer -= Time.deltaTime;
		lookDuration -= Time.deltaTime;

//		if (timer < 0){
		#if UNITY_EDITOR
		if (GameObject.FindWithTag("MovieCamera")) {
			target = GameObject.FindWithTag("MovieCamera").transform;

		} 
		#endif

//			timer = .1f;
		if (ni){
			Rigidbody rb = transform.root.GetComponentInChildren<Rigidbody>();
			if (rb){
				if (rb.angularVelocity.magnitude > 5 && Vector3.SqrMagnitude(transform.position - Player.inst.transform.position) > minSqrDistToRoll) return; // allow number to roll if rolling fast and far away from player.
				else	FacePlayerOnce();
			} else {
				FacePlayerOnce();	
			}
		} else {
			FacePlayerOnce();
		}
	}

	public void FacePlayerOnce(){
		transform.LookAt(target);
//		// commented Debug.Log ("faced player!");
//		transform.rotation = Quaternion.LookRotation(Vector3.Normalize(Camera.main.transform.position-transform.position));
	}

	public void FaceObjectOnce(Transform t){
		transform.LookAt(t);
	}
}
