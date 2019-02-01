using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour {

	public Text userAndLevelInfo;
	public static InGameHUD inst;
	public GameObject crosshairs;
	public void SetInstance(){
		inst = this;
	}

	public void SetUserAndLevelInfoText(string user, string level){
		string t = "Logged in as: "+user+", on level: "+level;
		userAndLevelInfo.text = t;
		LevelBuilder.inst.levelTitle.text = t;
	}

	public void Show(){
		foreach(Transform t in transform){
			t.gameObject.SetActive(true);
		}
	}

	public void Hide(){
		foreach(Transform t in transform){
			t.gameObject.SetActive(false);
		}
	}



}
