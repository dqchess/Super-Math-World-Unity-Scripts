using UnityEngine;
using System.Collections;

public class StayAtToonWaterSurface : MonoBehaviour {


	public float offset = 0.2f;
	// Use this for initialization

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		float newY = 40;
		newY = UltimateToonWaterC.inst.getHeightByPos(new Vector2(transform.position.x,transform.position.z)) + offset;
		transform.position = new Vector3(transform.position.x,newY,transform.position.z);
	}
}
