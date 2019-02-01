using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIValueCommNumberTowerHeight : UIValueComm {

	public Text towerHeight;

	public override void OnMenuOpened(){
		if (LevelBuilder.inst.currentPiece){
			AINumberTower tow = LevelBuilder.inst.currentPiece.GetComponentInChildren<AINumberTower>();
			if (tow){
				towerHeight.text = tow.towerHeightMax.ToString();
			}
		}
	}

	public void ModTowerHeight(int amount){
		if (LevelBuilder.inst.currentPiece){
			AINumberTower tow = LevelBuilder.inst.currentPiece.GetComponentInChildren<AINumberTower>();
			if (tow){
				towerHeight.text = tow.SetMaxTowerHeight(tow.towerHeightMax+amount).ToString();
				tow.BuildTower(0.11f,false); // note this only builds ONE tower, which works because the TowerGroup object only has ONE tower in it! IF you add more towers this will only modify one of them.
//				towerHeight.text = tow.towerHeightMax.ToString();
			}
		}
	}
}
