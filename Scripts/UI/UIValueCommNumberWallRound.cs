using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class UIValueCommNumberWallRound : UIValueComm {

	//	UISetCurrentObjectProperties
	public Text radius;
	public Text degrees;
	public Text height;


	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
	public void OnEnable(){
		OnMenuOpened();
	}
	public void OnMenuOpened(){

		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		radius.text =  N[NumberWallCreatorRound.wallCreatorRoundKey][NumberWallCreatorRound.wallRadiusKey].Value; // hm.. Some keys appear to be in Json LevelLoader also. 
		degrees.text = N[NumberWallCreatorRound.wallCreatorRoundKey][NumberWallCreatorRound.wallDegreesKey].Value; // hm.. Some keys appear to be in Json LevelLoader also. 
		height.text = N[NumberWallCreatorRound.wallCreatorRoundKey][NumberWallCreatorRound.wallHeightKey].Value; // hm.. Some keys appear to be in Json LevelLoader also. 

	}




	public override void SetObjectProperties(){

		if (radius.text.Length == 0 || degrees.text.Length == 0 || height.text.Length == 0) return; 
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[NumberWallCreatorRound.wallCreatorRoundKey] = new SimpleJSON.JSONClass();
		N[NumberWallCreatorRound.wallCreatorRoundKey][NumberWallCreatorRound.wallRadiusKey] = radius.text; // hm.. Some keys appear to be in Json LevelLoader also. 
		N[NumberWallCreatorRound.wallCreatorRoundKey][NumberWallCreatorRound.wallDegreesKey] = Regex.Replace(degrees.text, "[^0-9]", ""); //degrees.text;
		N[NumberWallCreatorRound.wallCreatorRoundKey][NumberWallCreatorRound.wallHeightKey] = height.text; // hm.. Some keys appear to be in Json LevelLoader also. 

//		// commented Debug.Log("props:"+props);
		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
	}
}
