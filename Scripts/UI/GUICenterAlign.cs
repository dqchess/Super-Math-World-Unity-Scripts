using UnityEngine;
using System.Collections;

public class GUICenterAlign : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = FindObjectOfType<CamGUI>().GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width/2f,Screen.height/2f,transform.localPosition.z));
	}

}
