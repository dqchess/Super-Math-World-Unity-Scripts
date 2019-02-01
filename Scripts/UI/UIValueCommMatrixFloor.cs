using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIValueCommMatrixFloor : UIValueComm {


	//	UISetCurrentObjectProperties
	public Text wallSizeX;
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
		wallSizeX.text = N[JsonUtil.dimensionsKey][JsonUtil.sizeXkey].Value;
		wallSizeZ.text = N[JsonUtil.dimensionsKey][JsonUtil.sizeZkey].Value;
	}

	public override void SetObjectProperties(){


		if (wallSizeX.text.Length == 0  || wallSizeZ.text.Length == 0) return; 
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
//		N[NumberWallCreatorSquare.wallCreatorSquareKey] = new SimpleJSON.JSONClass(); // is it needed to initiate these sub-node clases? I think so
		N[JsonUtil.dimensionsKey][JsonUtil.sizeXkey] = wallSizeX.text; // hm.. Some keys appear to be in JsonLevelLoader also. 
		N[JsonUtil.dimensionsKey][JsonUtil.sizeZkey] = wallSizeZ.text; // hm.. Some keys appear to be in JsonLevelLoader also. 

		//		// commented Debug.Log("props:"+props);
		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
	}
}
