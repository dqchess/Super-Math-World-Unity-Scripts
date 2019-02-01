using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIValueComm_CycleObject : UIValueComm {

	public Text label;
	public void CycleObject(int i){
		if (LevelBuilder.inst.currentPiece){
			UEO_ObjectCycler[] cys = LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ObjectCycler>();
			foreach(UEO_ObjectCycler cy in cys){
				if (cy){
					cy.CycleObject(i);
					UpdateLabel();
				}
			}
		} 

	}

	public override void OnMenuOpened(){
		UpdateLabel();
	}
	void UpdateLabel(){
		if (LevelBuilder.inst.currentPiece){
			UEO_ObjectCycler cy = LevelBuilder.inst.currentPiece.GetComponent<UEO_ObjectCycler>();
			if (cy){
				label.text = cy.objectsToCycle[cy.objectIndex].name;
			}
		} 

	}


}
