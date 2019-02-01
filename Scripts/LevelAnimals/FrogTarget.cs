using UnityEngine;
using System.Collections;

public class FrogTarget : MonoBehaviour {

	void Awake(){
		if (GetComponent<MultiblasterBullet>()){
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb) {
				rb.drag = 10f;
				rb.velocity = rb.velocity.normalized;
			}
		}
	}
	// A short lived script to help frogs seek different targets.
	public void DestroyAfterSeconds(float s){
		Destroy(this,s);
	}
//	float t = 3f;
//	void Update () {
//		t -= Time.deltaTime;
//		if (t<0) Destroy(this);
//	}
}
