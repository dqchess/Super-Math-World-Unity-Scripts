using UnityEngine;
using System.Collections;

public class AlwaysFaceSkyCamY : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion rot = transform.rotation;
		rot.eulerAngles = new Vector3(90,LevelBuilder.inst.camSky.transform.parent.rotation.eulerAngles.y+180,0);
		transform.rotation = rot;
	}
}
