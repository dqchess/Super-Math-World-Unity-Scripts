using UnityEngine;
using System.Collections;

public class GUITopLeftAlign : MonoBehaviour {
	
	public Camera guicam;
	
	// Use this for initialization
	void Start () {
		Vector3 anchor = guicam.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
		transform.position = new Vector3(anchor.x, anchor.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
