using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrouperButtonIndicator : MonoBehaviour {

	Color c;
	Text t;
	void Start(){
		t = GetComponent<Text>();
		c = t.color;
	}
	void OnEnable(){
		t = GetComponent<Text>();
		c = t.color;
		t.color = new Color(c.r,c.g,c.b,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.G) && !LevelBuilder.inst.AnySubMenusAreOpen()){
			t.color = new Color(c.r,c.g,c.b,1);
		}
		if (Input.GetKeyUp(KeyCode.G)){
			t.color = new Color(c.r,c.g,c.b,0);
		}
	}
}
