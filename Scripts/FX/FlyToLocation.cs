using UnityEngine;
using System.Collections;

public class FlyToLocation : MonoBehaviour {

	public Transform d; 
	float pushForce = 5f;
	// Use this for initialization
	void Start () {
		pushForce = Random.Range (2,6);
		d=GameObject.Find ("BalloonDestination").transform;
		if (!d) { Destroy (this); return; }
		if (gameObject.GetComponent<MoveAlongAxis>()) Destroy (gameObject.GetComponent<MoveAlongAxis>());
//		if (!rigidbody) gameObject.AddComponent<Rigidbody>();
//		rigidbody.isKinematic=false;
//		rigidbody.useGravity=false;
//		rigidbody.freezeRotation=true;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 dirForce = Vector3.Normalize (d.position-transform.position);
//		dirForce = new Vector3(dirForce.x,0,dirForce.z);
//		rigidbody.AddForce (dirForce*pushForce*Time.deltaTime);
		if (Vector3.Distance(d.position,transform.position) > 10f) transform.position += dirForce*pushForce*Time.deltaTime;


		if (Vector3.Distance (Player.inst.transform.position,transform.position) < 20){
			Destroy (this);
			MoveAlongAxis m = transform.root.gameObject.AddComponent<MoveAlongAxis>();
			m.useLocalDir=false;
			m.dir = Random.insideUnitSphere + (Vector3.up / 2f) * Random.Range(2,6) * 100;
			m.speed=1;
		}
	}
}
