using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressKeyDialogue : MonoBehaviour {



	public static PressKeyDialogue inst;
	public GameObject dialogue;
	public Image squareKey;
	public Image wideKey;

	public void SetInstance(){
		inst = this;
	}

	KeyCode keyToPress;
	public Text instructions;
	public Text key;
	public Text function;

	public void Display (string keyToPressS, string t) {
		Player.inst.FreezePlayer("key display");
		switch(keyToPressS){
			case "w":
			keyToPress = KeyCode.W;
			break;
			case "e":
			keyToPress = KeyCode.E;
			break;
			case "shift":
			keyToPress = KeyCode.LeftShift;
			break;
			case "space":
			keyToPress = KeyCode.Space;
			break;
		default:break;
		}
		if (keyToPressS.Length == 1){
			// for length 1 keys use the square key, e.g.  "press W"
			squareKey.gameObject.SetActive(true);
			wideKey.gameObject.SetActive(false);
		} else {
			// for length >1 keys use the wide rectangle key, e.g.  "press SHIFT"
			wideKey.gameObject.SetActive(true);
			squareKey.gameObject.SetActive(false);
		}
		if (keyToPress == KeyCode.LeftShift){
			instructions.text = "Hold key:";
		} else {
			instructions.text = "Press key:";
		}
		key.text = keyToPressS;
		function.text = "[ " + t + " ]";

		Show();
	}

	bool showing = false;
	bool fading = false;
	void Update(){
		if (showing){
			if (Input.GetKeyDown(keyToPress)){
				Fadeout();
			}
		}
		if (fading) {
			bool finishedFading = false;

			float fadeSpeed = 3f;
			foreach(Text i in GetComponentsInChildren<Text>()){
				Color targetColor = new Color(i.color.r,i.color.g,i.color.b,0);
				i.color = Color.Lerp (i.color,targetColor,Time.deltaTime * fadeSpeed);
			}
			foreach(Image i in GetComponentsInChildren<Image>()){
				Color targetColor = new Color(i.color.r,i.color.g,i.color.b,0);
				i.color = Color.Lerp (i.color,targetColor,Time.deltaTime * fadeSpeed);
				if (Mathf.Abs(i.color.a-0) < .01f){
					finishedFading = true;
				}
			}
			if (finishedFading){
				fading = false;
				Hide();
			}

		}
	}

	void Show (){
		showing = true;
		fading = false;
		dialogue.SetActive(true);
		foreach(Image i in GetComponentsInChildren<Image>()){
			Color targetColor = new Color(i.color.r,i.color.g,i.color.b,0.85f);
			i.color = targetColor;
		}
		foreach(Text i in GetComponentsInChildren<Text>()){
			Color targetColor = new Color(i.color.r,i.color.g,i.color.b,1);
			i.color = targetColor;
		}
	}
	void Hide (){
		dialogue.SetActive(false);
	}

	void Fadeout(){
		Player.inst.UnfreezePlayer("fade out key");
		fading = true;
	}
}
