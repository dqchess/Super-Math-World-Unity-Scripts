using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class UIValueComm_ScaleManipulator : UIValueComm {

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
		if (N.GetKeys().Contains(UEO_ScaleManipulator.key)){
			wallSizeX.text = N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyX].Value; // hm.. Some keys appear to be in JsonLevelLoader also. 
			wallSizeY.text = N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyY].Value; 
			wallSizeZ.text = N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyZ].Value; 
		}
//		// commented Debug.Log("text set");
	}


	public override void SetObjectProperties(){


		if (wallSizeX.text.Length == 0 || wallSizeY.text.Length == 0 || wallSizeZ.text.Length == 0) return; 
		base.SetObjectProperties();
//		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[NumberWallCreatorSquare.wallCreatorSquareKey] = new SimpleJSON.JSONClass(); // is it needed to initiate these sub-node clases? I think so
		N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyX] = wallSizeX.text; 
		N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyY] = wallSizeY.text;
		N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyZ] = wallSizeZ.text;

//		// commented Debug.Log("props:"+N.ToString());
		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
	}


//	bool moddingInt = false;
//	float modIntinterval = 0;
//	int modIntAmount = 0;
//	void Update(){
//		if (moddingInt){
//			if (modIntinterval < 0){
//				modIntinterval = 0.2f;
//				AddToIntegerText(modIntAmount);
//			}
//		}
//	}
//
//	public void IntegerTextMouseDown(int amount){
//		moddingInt = true;
//		modIntinterval = 0.5f;
//		AddToIntegerText(amount);
//		modIntAmount  = amount;
//	}
//
//	public void IntegerTextMouseUp(){
//		moddingInt = false;
//	}
}
