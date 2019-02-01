using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SheepTriggerGroup : UEO_SimpleObject {

	public SheepTrigger sheepTrigger;


	#region UserEditable 
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> ui = new List<GameObject>();
		ui.AddRange(base.GetUIElementsToShow());
		if (height) ui.Add(LevelBuilder.inst.POCMheightButton);
		ui.Add(LevelBuilder.inst.POCMFractionButton);

		return ui.ToArray();
	}

	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,sheepTrigger.sheepNeeded,N);
//		WebGLComm.inst.Debug("Sheep trigger get prop:"+N.ToString());
		return N;
	}
	public override void SetProperties(SimpleJSON.JSONClass N){
//		WebGLComm.inst.Debug("Sheep trigger set prop:"+N.ToString());
		base.SetProperties(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)){
			sheepTrigger.sheepNeeded = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
		}
	}

	public override void OnGameStarted(){
		base.OnGameStarted();
		sheepTrigger.OnGameStarted();
	}

	#endregion
}
