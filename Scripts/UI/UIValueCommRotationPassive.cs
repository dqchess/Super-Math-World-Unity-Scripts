using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValueCommRotationPassive : UIValueComm {

	public Text x;
	public Text y;
	public Text z;

	public override void OnMenuOpened(){
		if (LevelBuilder.inst.currentPiece){
			Vector3 r = LevelBuilder.inst.currentPiece.transform.rotation.eulerAngles;
			x.text = Mathf.RoundToInt(r.x).ToString();
			y.text = Mathf.RoundToInt(r.y).ToString();
			z.text = Mathf.RoundToInt(r.z).ToString();
		}
	}
}
