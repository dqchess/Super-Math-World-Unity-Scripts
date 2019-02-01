using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class UIValueComm_ScaleManipulatorMultiple : UIValueComm {

	//	UISetCurrentObjectProperties


	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
	public void OnEnable(){
		OnMenuOpened();
	}
	public void OnMenuOpened(){

		if (LevelBuilder.inst.currentPiece == null) return;

	}


	public void IncreaseScaleX(int s){
		foreach(UEO_ScaleManipulator sc in LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ScaleManipulator>()){
			sc.IncreaseCubeSizeX(s);
		}
	}

	public void IncreaseScaleY(int s){
		foreach(UEO_ScaleManipulator sc in LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ScaleManipulator>()){
			sc.IncreaseCubeSizeY(s);
		}
	}

	public void IncreaseScaleZ(int s){
		foreach(UEO_ScaleManipulator sc in LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ScaleManipulator>()){
			sc.IncreaseCubeSizeZ(s);
		}
	}

	public void IncreaseScaleAll(int s){
		
		foreach(UEO_ScaleManipulator sc in LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ScaleManipulator>()){
//			Vector3 origScale = sc.transform.localScale;
//			Bounds origBounds = Utils.boundsOfChildren(Utils.GetTransformJustAboveRoot(sc.transform));
			sc.IncreaseCubeSizeX(s);
			sc.IncreaseCubeSizeY(s);
			sc.IncreaseCubeSizeZ(s);
//			Bounds newBounds = Utils.boundsOfChildren(Utils.GetTransformJustAboveRoot(sc.transform));
////			Vector3 newScale = sc.transform.localScale;
//
//			Transform t = Utils.GetTransformJustAboveRoot(sc.transform); // the ueo root of this object, but not of the whole group (hence "just above root")
//			float deltaScale = Vector3.Magnitude(newBounds.extents)/Vector3.Magnitude(origBounds.extents);
//			int direction = newBounds.extents.magnitude > origBounds.extents.magnitude ? -1 : 1; // moving in or out depending on if we got bigger or smaller. Smallerified objects move in towrads the center.
//			// Note that this will move all objects in tandem with respect to the parent object
//			// note that the parent object is, by definition (ScaleManipulatorMultiple is only available in game when), the parent is a "dragging parent" in LevelBuilder so it is ia collection of single objects
//			Vector3 toCenter = (t.parent.position - t.position); // direction from this object to the center of the group
//
////			Debug.Log("dirtowards:"+dirTowardsCenter+", deltascale:"+deltaScale);
//			t.position += 1f/deltaScale * toCenter/4f * direction;// * origBounds.extents.magnitude/2f;
		}

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
