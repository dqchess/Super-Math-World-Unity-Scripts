using UnityEngine;
using System.Collections;

public class PlayerUnderwaterTrigger : MonoBehaviour {

	PlayerUnderwaterController puc;

	void Start(){
		puc = PlayerUnderwaterController.inst;

	}

	MeshFilter closestWaterMesh;
	void OnTriggerEnter(Collider other){
//		// commented Debug.Log("		other name:"+other.gameObject.name);
		if (other.tag == "Player"){
			
		} else if (other.GetComponent<Animal>()){
			other.GetComponent<Animal>().SetUnderwater(true);
		} else if (other.transform.root.GetComponentInChildren<Vehicle>()){
			other.transform.root.GetComponentInChildren<Vehicle>().SetUnderwater(true);
		}
	}




	MeshFilter GetClosestWaterMesh(){
		float closestDist = Mathf.Infinity;
		MeshFilter m = null;
		foreach(PlayerUnderwaterFXTrigger p in FindObjectsOfType<PlayerUnderwaterFXTrigger>()){
			if (Vector3.SqrMagnitude(Player.inst.transform.position - p.transform.position) < closestDist){
				m = p.GetComponent<MeshFilter>();
			}
		}
		return m;
	}

	bool exitingWater = false; // treadingWater
	float exitY = 0;
	float deltaYNeededToExitFully=1f;//4.5f;
	void OnTriggerExit(Collider other){
		if (other.tag == "Player"){

		}  else if (other.GetComponent<Animal>()){
			other.GetComponent<Animal>().SetUnderwater(false);
		}else if (other.transform.root.GetComponentInChildren<Vehicle>()){
			other.transform.root.GetComponentInChildren<Vehicle>().SetUnderwater(false);
		}
	}

	bool under = false;
	void Update(){

	}
}
