using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIValueCommNumberWallSquare : UIValueComm {

	//	UISetCurrentObjectProperties
	public Text wallSizeX;
	public Text wallSizeY;
	public Text wallSizeZ;


	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
	public void OnEnable(){
		OnMenuOpened();
	}
	public void OnMenuOpened(){
//		// commented Debug.Log("setter opened");
		if (LevelBuilder.inst.currentPiece == null) return;
//		string props = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();

		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		wallSizeX.text = N[NumberWallCreatorSquare.wallCreatorSquareKey][NumberWallCreatorSquare.wallXkey].Value; 
		wallSizeY.text = N[NumberWallCreatorSquare.wallCreatorSquareKey][NumberWallCreatorSquare.wallYkey].Value; 
		wallSizeZ.text = N[NumberWallCreatorSquare.wallCreatorSquareKey][NumberWallCreatorSquare.wallZkey].Value; 
//		// commented Debug.Log("text set");
	}

	public override void SetObjectProperties(){

	
		if (wallSizeX.text.Length == 0 || wallSizeY.text.Length == 0 || wallSizeZ.text.Length == 0) return; 
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[NumberWallCreatorSquare.wallCreatorSquareKey] = new SimpleJSON.JSONClass(); // is it needed to initiate these sub-node clases? I think so
		N[NumberWallCreatorSquare.wallCreatorSquareKey][NumberWallCreatorSquare.wallXkey] = wallSizeX.text; 
		N[NumberWallCreatorSquare.wallCreatorSquareKey][NumberWallCreatorSquare.wallYkey] = wallSizeY.text;
		N[NumberWallCreatorSquare.wallCreatorSquareKey][NumberWallCreatorSquare.wallZkey] = wallSizeZ.text; // hm.. duplicated in many diff numwall objs

//		// commented Debug.Log("props:"+props);
		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
	}
}
