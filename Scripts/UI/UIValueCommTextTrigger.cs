using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleJSON;

public class UIValueCommTextTrigger : UIValueComm {

	//	UISetCurrentObjectProperties
	public InputField speech;

	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
	public void OnEnable(){
		OnMenuOpened();
	}
	public void OnMenuOpened(){
//		// commented Debug.Log("setter opened");
		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		if (N.GetKeys().Contains(PlayerNowMessageTrigger.textTriggerKey)){
//			// commented Debug.Log("got n and st:"+N.ToString());
			string s = N[PlayerNowMessageTrigger.textTriggerKey].Value; // hm.. Some keys appear to be in JsonLevelLoader also. 
//			s = Regex.Unescape(s);
			s = Utils.FakeToRealQuotes(s);
			speech.text = s;
		} else {
//			string keylist="";
//			N.GetKeys
//			foreach(string s in N.GetKeys()){
//				// commented Debug.Log("key:L"+s);
////				// commented Debug.Log("kvkp:"+kvp.Key+","+kvp.Value.ToString());
//			}
//			// commented Debug.Log("missed key:"+PlaceableNPC.speechKey+" in :"+N.ToString());
		}
	}




	public override void SetObjectProperties(){

		if (speech.text.Length == 0) return; 
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[PlayerNowMessageTrigger.textTriggerKey] = speech.text; // hm.. Some keys appear to be in JsonLevelLoader also. 
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
