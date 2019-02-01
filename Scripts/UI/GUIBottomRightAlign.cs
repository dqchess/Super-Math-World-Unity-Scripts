using UnityEngine;
using System.Collections;

public class GUIBottomRightAlign : MonoBehaviour {
	
	public Camera guicam;
	
	// Use this for initialization
	void Start () {
		guicam = GameObject.Find ("CamGUI").GetComponent<Camera>();
		Vector3 anchor = guicam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
		transform.position = new Vector3(anchor.x, anchor.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
