using UnityEngine;
using System.Collections;

public class AlwaysFaceCameraY : MonoBehaviour {
	
	void Update(){
//		Quaternion rot = transform.rotation;
		Vector3 dirToPlayer = Utils.FlattenVector(Camera.main.transform.position - transform.position);
		transform.rotation = Quaternion.LookRotation(dirToPlayer);
	}

}
