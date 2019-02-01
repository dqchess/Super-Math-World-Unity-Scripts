using UnityEngine;
using System.Collections;

public class UICameraEditorToggle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
//		if (Input.GetKeyDown(KeyCode.U)){
//			GetComponent<Camera>().enabled = !GetComponent<Camera>().enabled;
//		}
		#endif
	
	}
}
