using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanMeyerCube : MonoBehaviour {

	public Vector3 indexedPosition;


	public GameObject cubeOnObject; // the actual cube that the player will see, 
	public GameObject cubeOffObject; // visible representation of the cube being absent, such as an outline, editor visible only (player can't see)
	public bool cubeActive {
		get {
			return !!cubeOnObject && cubeOnObject.activeSelf;
		}
	}

	public void TurnCubeOn(bool active){
		AudioManager.inst.PlayClick2();
		cubeOnObject.SetActive(true);
		cubeOffObject.SetActive(false);
		GetComponent<Collider>().enabled = active;
	}
	public void TurnCubeOff(bool active){
		AudioManager.inst.PlayClick3();
		cubeOnObject.SetActive(false);
		cubeOffObject.SetActive(true);
		GetComponent<Collider>().enabled = !active;
	}

	void OnCollisionEnter(Collision hit){
		
		NumberInfo ni = hit.collider.GetComponent<NumberInfo>();
		if (ni && !ni.GetComponent<ResourceNumber>()){
			transform.root.GetComponent<LevelMachine_DanMeyerCubes>().TrySolve(ni);
		}
	}

}
