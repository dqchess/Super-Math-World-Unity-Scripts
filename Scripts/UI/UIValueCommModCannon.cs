using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class UIValueCommModCannon : UIValueComm {

	public InputField speed;



	override public void OnMenuOpened() {
		if (LevelBuilder.inst.currentPiece){
			ConveyerWhoosherSpeeder cannon = LevelBuilder.inst.currentPiece.GetComponentInChildren<ConveyerWhoosherSpeeder>();
			if (cannon){
				speed.text = Mathf.RoundToInt(cannon.speed).ToString();
			} else {
				Debug.LogError("no cannon on"+LevelBuilder.inst.currentPiece.name);
			}
		}
	}

	public void UpdateCannonSpeed(){
		if (speed.text != ""){
			ConveyerWhoosherSpeeder cannon = LevelBuilder.inst.currentPiece.GetComponentInChildren<ConveyerWhoosherSpeeder>();
			if (cannon){
				cannon.speed = MathUtils.IntParse(speed.text);
			} else {
				Debug.LogError("no cannon on"+LevelBuilder.inst.currentPiece.name);
			}
		}
	}
}
