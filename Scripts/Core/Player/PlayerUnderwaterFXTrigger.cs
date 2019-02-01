using UnityEngine;
using System.Collections;

public class PlayerUnderwaterFXTrigger : MonoBehaviour {

//	MeshFilter m;
//
//	void Start(){
//		m = GetComponent<MeshFilter>();
//	}
//	void OnTriggerEnter(Collider other){
//		// commented Debug.Log ("Trig");
//		if (other.CompareTag("Player")){
//			active = true;
//			// commented Debug.Log ("Trig Player");
//		}
//	}
//
//	void OnTriggerExit(Collider other){
//		if (other.CompareTag("Player")){
//			active = false;
//		}
//	}
//
//	bool active = false;
//	// Update is called once per frame
//	void Update () {
//		if (active){
//			BaryCentricDistance closestPointCalculator = new BaryCentricDistance(m);
//			BaryCentricDistance.Result result = closestPointCalculator.GetClosestTriangleAndPoint(Camera.main.transform.position);
//			Vector3 closest = result.closestPoint;
//			if (closest.y > Camera.main.transform.position.y) {
//				// commented Debug.Log ("UNDER");
//			} else {
//				// commented Debug.Log ("OVER");
//			}
//		}
//	}
}
