using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {

	public GameObject backboard;

	public static DebugText inst;
	public Text debug;
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		Hide();
	}

	// typewriter vars

	public void Display(string s){
		debug.text = s;
		backboard.SetActive(true);
	}

	public void Hide(){
		debug.text = "";
		backboard.SetActive(false);	
	}

}
