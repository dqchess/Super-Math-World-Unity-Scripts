using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConveyerBeltSpacerNoCombineZone : MonoBehaviour {

	// We don't want objects "Stuck" in this area (waiting for spacer to finish operation) to combine.
	void OnTriggerEnter(Collider other){
		if (other.tag != "Player" && other.GetComponent<Rigidbody>()) {
			
			other.transform.parent = transform;
		}
	}
	void OnTriggerExit(Collider other){
		if (other.tag != "Player") {
			
			other.transform.parent = null;
		}
	}

	Transform GetFarthestAheadChild(){
		// Which child was closest to my "z" side?
		Transform farthestAhead = transform.GetChild(0);
		foreach(Transform t in transform){
			if (t.localPosition.z > farthestAhead.localPosition.z) {
				farthestAhead = t;
			}
		}
		return farthestAhead;
	}

	void Update(){
		if (transform.childCount > 1){
			Transform farthestAhead = GetFarthestAheadChild();
			PenalizeAllChildrenExcept(farthestAhead);

		}	
	}


	/*
	 * a
	 a
	 b
	 c
	 d
	 e
	 f
	 g

	i = 0, count = 8
	-2,-2
	-

	*/


	void PenalizeAllChildrenExcept(Transform farthestAhead){
		// If a group of children are in the spacer prep zone, make sure only one of them stays out in front to be "next in line" so that a group do not pass through the door at once.
		// In other words this will keep all children except 1 pushed away from the door that's about to open on the "spacer" conveyer.

		float x = -3.5f;
		float y = -1f;

		// start at x = -3.5f, y = -1

		int i=0;
		int j=0;
		int k =0;
		int tunnelWidth = 4;
		int tunnelHeight = 3;
		foreach(Transform t in transform){
			if (t == farthestAhead) continue;
			float spacing = 2.5f;
			Vector3 pos = new Vector3(x,y,0);

			pos += new Vector3(i*spacing,j*spacing,-k*spacing);

			t.localPosition = pos; // keep in "center" of staging area
			i += 1;
			if (i >= tunnelWidth){
				i = 0;
				j+=1;
			}
			if (j >= tunnelHeight){
				i=0;
				j=0;
				k+=1;
			}

		}

	}

}
