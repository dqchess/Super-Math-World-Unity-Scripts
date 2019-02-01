using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleJSON;

public class UIValueCommCharacterSpeech : UIValueComm {

	//	UISetCurrentObjectProperties
	public InputField speech;

	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
	public void OnEnable(){
		OnMenuOpened();
	}
	public void OnMenuOpened(){
//		// commented Debug.Log("setter opened");
		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		if (N.GetKeys().Contains(PlaceableNPC.speechKey)){
//			// commented Debug.Log("got n and st:"+N.ToString());
			string s = Utils.FakeToRealQuotes(N[PlaceableNPC.speechKey].Value); // hm.. Some keys appear to be in Json LevelLoader also. 

			speech.text = s;
		} else {
		}
	}




	public override void SetObjectProperties(){

		if (speech.text.Length == 0) return; 
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[PlaceableNPC.speechKey] = speech.text; // hm.. Some keys appear to be in Json LevelLoader also. 
//		// commented Debug.Log("props:"+props+" on:"+);
		if (LevelBuilder.inst.currentPiece){
			LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
		} else {
//			// commented Debug.Log("whoops, tried to set object properties for speech without a current.");
		}
	}

	void OnDisable(){
		SetObjectProperties();
	}
}
