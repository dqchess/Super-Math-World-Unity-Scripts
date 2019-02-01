using UnityEngine;
using System.Collections;

public class PlayerMapDirectionHelperLineRendererObject : MonoBehaviour {
	

	public Transform destination;
	LineRenderer lineRenderer;
	int maxVerts = 100;
	bool initialized = false;
	public void Init(Transform t){
		initialized = true;
		destination = t;
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(maxVerts);
//		DrawLaser();
	}

	Vector3 dirToTarget;
	float stepLength = 4;
	void DrawLaser(){
		dirToTarget = Vector3.Normalize(destination.position - Player.inst.transform.position);
		Vector3 lastPos= Player.inst.transform.position + dirToTarget * stepLength; // 1 is arbitrary, let's get over the head though.
		lineRenderer.SetPosition(0,lastPos);

		for (int i=1;i<maxVerts; i++){
			lastPos = GetNextPosition(lastPos);
			lineRenderer.SetPosition(i,lastPos);
		}

		// I want it to ..
		/*
		 * Start at player on ground
		 * Move towards destination along the ground (easiest curve within 90 degrees of direction to)
		 * So,
		 * Get P0 of player..
		 * to get P1, move DIRTOTARGET * SEGLEGNTH + VECTOR3.UP * SEGLENGTH then raycast DOWN for all the fans..
		 * Take the fan that had the Raycast hit distance that was closest to the one I was already standing on (take the least steep path for the very next step.)
		 * 
		 * */

	}

	Vector3 GetNextPosition(Vector3 lastPos){

		Vector3 newPos = lastPos + dirToTarget * stepLength;
		RaycastHit hit;
		if (Physics.Raycast(newPos,Vector3.up,out hit, 20)){
			newPos += Vector3.up * hit.distance;
		} else if (Physics.Raycast(newPos,Vector3.down,out hit, 20)){
			newPos -= Vector3.up * hit.distance;
		}
		newPos += Vector3.up*2;
		return newPos;
	}

//	Vector3 GetNextPosition(Vector3 lastPos){
//		float segLength = 5;
//		Vector3 dir = Vector3.Normalize(destination.position - lastPos);
//		Vector3 yOffset = Vector3.up * 20;
//		float distToGroundFromLastPos = DistToGround(lastPos + yOffset);
//		float minDiff = Mathf.Infinity;
//		Vector3 optimalDir = Vector3.zero;
//		Vector3[] dirs = new Vector3[11];
//		for(int i=0; i<11; i++){
//			dirs[i] = Vector3.Normalize(Quaternion.AngleAxis((i-5)*8f, Vector3.up) * dir) * segLength; // FAN
//			float distToGround = DistToGround(lastPos + dirs[i] + yOffset);
////			// commented Debug.Log ("dist to ground for [ "+i+" ]:"+distToGround);
//			if (distToGround < minDiff){
//				minDiff = distToGround;
//				optimalDir = dirs[i];
//			}
//		}
//		return optimalDir;
//	}

//	float DistToGround(Vector3 s){
//		// commented Debug.Log ("asking for a frind:"+s);
//		return s.y - GetFloorPosition(s).y;
//	}
//
//	Vector3 GetFloorPosition(Vector3 s){
//		RaycastHit hit;
//		if (Physics.Raycast(s,Vector3.down,out hit,50f)){
//			return hit.point;
//		}
//		// commented Debug.Log ("uh oh didn't hit terrain!");
//		return new Vector3(0,0,101);
//	}
//	
	
	// Update is called once per frame
	void Update () {
		if (initialized){
			DrawLaser ();
		}
	}


//	void OnDrawGizmos(){
//		Gizmos.color=Color.yellow;
//		Gizmos.DrawSphere(transform.position,3);
//		for (int i=0;i<laserPoints.Length;i++){
//			
//			if (i!=0){ // ignore first node
//				Gizmos.color=Color.blue;
//				Gizmos.DrawLine(laserPoints[i-1].position,laserPoints[i].position);
//			}
//			Gizmos.color=Color.red;
//			Gizmos.DrawSphere(laserPoints[i].position,.5f);
//		}
//	}
}
