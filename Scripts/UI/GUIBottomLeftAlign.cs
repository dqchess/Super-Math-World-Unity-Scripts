using UnityEngine;
using System.Collections;

public class GUIBottomLeftAlign : MonoBehaviour {
	
	public Camera guicam;
	
	// Use this for initialization
	void Start () {
		Vector3 anchor = FindObjectOfType<CamGUI>().GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 0));
		transform.position = new Vector3(anchor.x, anchor.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
