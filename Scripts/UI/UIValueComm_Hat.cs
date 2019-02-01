using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIValueComm_Hat : UIValueComm {


	public override void OnMenuOpened(){
//		//		// commented Debug.Log("ui fraction setter opened");
//		if (!LevelBuilder.inst.currentPiece) return;
//		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
//		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
//
//
	}


	public void CycleNumber(int i){
		
		if (LevelBuilder.inst.currentPiece.GetComponent<PlayerHatPickup>()) {
			LevelBuilder.inst.currentPiece.GetComponent<PlayerHatPickup>().CycleHat(i);
		}
	}

	public void CycleColor(int i){
		if (LevelBuilder.inst.currentPiece.GetComponent<PlayerHatPickup>()) {
			LevelBuilder.inst.currentPiece.GetComponent<PlayerHatPickup>().CycleColor(i);
		}
	}

}
