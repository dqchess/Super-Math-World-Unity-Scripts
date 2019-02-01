using UnityEngine;
using System.Collections;

public class CollisionEnterBroadcaster : MonoBehaviour {

	public delegate void OnCollisionEnterDelegate(GameObject o,Collision hit);
	public OnCollisionEnterDelegate onCollisionEnterDelegate;
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter(Collision hit){
		if (null != onCollisionEnterDelegate) {
			onCollisionEnterDelegate(this.gameObject,hit);
		}
		else {
			Destroy(gameObject); // delegate obj was destroyed?
		}
//		OnCollisionEnterDetector();

	}

//	void OnCollisionEnterDetector(){
//	}
}
