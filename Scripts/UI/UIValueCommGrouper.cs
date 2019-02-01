using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValueCommGrouper : UIValueComm {

	public Button groupNow;
	public Button ungroupNow;

	public override void OnMenuOpened(){
		if (LevelBuilder.inst.currentPiece && LevelBuilder.inst.currentPiece.GetComponent<UEO_DraggingParent>()){
			if (LevelBuilder.inst.currentPiece.GetComponent<UEO_DraggingParent>().groupedState == GroupedState.Grouped) {
				ShowUngroupNowButton();
			} else {
				ShowGroupNowButton();
			}
		}
	}

	void ShowGroupNowButton(){
		groupNow.gameObject.SetActive(true);
		ungroupNow.gameObject.SetActive(false);
	}

	void ShowUngroupNowButton(){
		groupNow.gameObject.SetActive(false);
		ungroupNow.gameObject.SetActive(true);	
	}



	public void GroupNow(){
		List<UserEditableObject> ueos = new List<UserEditableObject>();
		foreach(Transform t in LevelBuilder.inst.currentPiece.transform){
			ueos.Add(t.GetComponent<UserEditableObject>());
		}
		LevelBuilderGroupManager.inst.MakeGroup(ueos);
		ShowUngroupNowButton();
	}

	public void UngroupNow(){
		List<UserEditableObject> ueos = new List<UserEditableObject>();
		foreach(Transform t in LevelBuilder.inst.currentPiece.transform){
			ueos.Add(t.GetComponent<UserEditableObject>());
		}
		LevelBuilderGroupManager.inst.Ungroup(ueos);
		ShowGroupNowButton();
		LevelBuilder.inst.UserFinishedPlacingObject();
	}
}
