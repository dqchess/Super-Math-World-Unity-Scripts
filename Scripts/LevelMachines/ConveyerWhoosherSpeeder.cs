using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhooshingObjectInfo {
	// for getting info on objects being moved frame by frame
	public Vector3 position = Vector3.zero;
	public bool endArcThreshholdReached = false;
	public WhooshingObjectInfo(Vector3 _position, bool _endArcThreshholdReached) {
		position = _position;
		endArcThreshholdReached = _endArcThreshholdReached;
	}
}

public class ConveyerWhoosherSpeeder : MonoBehaviour {

	public static string speedKey = "Speed";

	public LayerMask layerMask;
	public float speed = 10f;
	public float downForce = 0.5f;
//	public float playerFasterFactor = .01f;
	public Transform outPosition;
	public LineRenderer lr;
	public int vCount = 100;
	public float fadeFactor = 1.7f; // how much initial speed should be lost by the end of the travel

	Vector3[] positions;
	void Start(){
		positions = new Vector3[vCount];
//		lr
//		lr.positionCount = vCount;
		SetLinePositions();
	}
	void Update(){
		SetLinePositions();
		if (numberWasObstructing){
//			Debug.Log("num obstructing!");
			if (!obstructingNumber) {
				// it was destroyed!
				numberWasObstructing = false;
				obstructingNumber = null;
				GetEndArcThreshhold();
			}

		}
	}

	void SetLinePositions(){
		for(int i=0;i<positions.Length;i++){
			if (i == 0) {
				positions[i] = transform.position;
			} else {
//				float fade = GetFade(i);
				//				Debug.Log("fade for :"+i+":"+fade);
				positions[i] = GetPositionAtIndex(i);

			}
		}
		//		Debug.Log("setpos:"+positions[4]);
		lr.SetPositions(positions);
	}

	float GetFade(float i){
		return (1 - ((float)i/(float)vCount)*fadeFactor);
	}
		

	public float maxTime = 50f; // should travel entire arc in this time
	public WhooshingObjectInfo GetWhooshingObjectPositionAtTime(float t){
		float i = (t / maxTime)*positions.Length;
		return new WhooshingObjectInfo(GetPositionAtIndex(i),i>GetEndArcThreshhold());
//		Vector3 speederForce = transform.forward * speed * GetFade(i) * (i+1);
//		Vector3 gravityForce = Vector3.down * downForce*Mathf.Pow(i,2);
//		Vector3 newPos = transform.position + speederForce + gravityForce; // transform.forward * i;
//		return newPos;


	}

//	Vector3 GetPositionAtIndex(float i) {
		
//	}

	bool numberWasObstructing = false;
	NumberInfo obstructingNumber = null;
	float endArcThreshhold;
	float GetEndArcThreshhold(){
		// pre-calculate where the whooshing object wil colllide and end its journey.
		float maxDistToColliderBeforeEndArc = 0.1f;
		RaycastHit hit = new RaycastHit();
		for(int i=0;i<vCount-1;i++){
			Vector3 currentDirection = (GetPositionAtIndex(i+1)-GetPositionAtIndex(i)); // tangent to the arc at this point

			Ray ray = new Ray(GetPositionAtIndex(i),currentDirection.normalized);
			if (Physics.Raycast(ray,out hit, Mathf.Max(currentDirection.magnitude,maxDistToColliderBeforeEndArc),layerMask)){
				if (!hit.collider.GetComponent<WhooshingObject>() && !hit.collider.GetComponent<NumberHoop>()) 	{
					NumberInfo ni = hit.collider.GetComponent<NumberInfo>();
					if (ni){
						// path is obstructed by a number which may die at any time. Check periodically if it dies, then if it does, Recalculate the arc.
						numberWasObstructing = true;
						obstructingNumber = ni;
//						Debug.Log("NEW num obstructing!");
					} else {
//						Debug.Log("Nothing obstructing!");
					}
//					Debug.Log("collide i :"+i+", hit col:"+hit.collider.name);
					return i; // don't collide with other objects being whooshed!
				}
			}
		}

		return vCount; // no colliisons
	}
		
	Vector3 GetPositionAtIndex(float i){
		Vector3 speederForce = transform.forward * speed * GetFade(i) * (i+1);
		Vector3 gravityForce = Vector3.down * downForce*Mathf.Pow(i,2);
		Vector3 newPos = transform.position + speederForce + gravityForce; // transform.forward * i;
		return newPos;
	}


	bool ObjectIsWhooshable(Collider other){
		return (other.GetComponent<Rigidbody>() && other.GetComponent<NumberInfo>()) || other.GetComponent<Player>();
	}

	void OnTriggerEnter(Collider other){
		endArcThreshhold = GetEndArcThreshhold();
		if (ObjectIsWhooshable(other)){
//			Debug.Log("add whoosh");
			other.transform.position = transform.position;
			// Add a component that will afffect the position of the object whooshing.
			WhooshingObject wo = other.GetComponent<WhooshingObject>();
			if (wo) Destroy(wo); // supercede previous whoosh
			wo = other.gameObject.AddComponent<WhooshingObject>();
			wo.whoosher = this;
			AudioManager.inst.PlayWhoosh(transform.position);

		}
	}


}
