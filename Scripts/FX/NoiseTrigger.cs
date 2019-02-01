using UnityEngine;
using System.Collections;

public class NoiseTrigger : MonoBehaviour {


	public bool oneShot = true;

	void OnTriggerEnter(Collider other){
		// commented Debug.Log("col:"+other);
		if (LevelBuilder.inst.levelBuilderIsShowing) {
			// commented Debug.Log("shown");
			return;
		}
		if (other.tag == "Player"){
			// commented Debug.Log("play");
			GetComponent<AudioSource>().Play();
			if (oneShot) Destroy(this);
		}else {
			// commented Debug.Log("hit:"+other);
		}
	}


}
