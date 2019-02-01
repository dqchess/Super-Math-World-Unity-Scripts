using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectDestructorPlayerDistance : MonoBehaviour {

	public float destroyAfterSeconds = 14f;
	public float playerMinDist = 30f;
	// Use this for initialization
	void Start () {
		//an awkward way to ensure gems created in editor don't get destroyed.
		if (LevelBuilder.inst.levelBuilderIsShowing) Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(transform.position,Player.inst.transform.position) > playerMinDist){
			destroyAfterSeconds -= Time.deltaTime;
		}
		if (destroyAfterSeconds < 0){
			Destroy(gameObject);
		}
	}
}
