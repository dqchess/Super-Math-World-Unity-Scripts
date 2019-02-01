using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UEO_SimpleObject_NumberKnight : UserEditableObject {

//	public string objectName;

//	public int gridSnap = 10;
	public bool copy = true;
	public bool height = true;

	public AIMonsterManAnimated knight;

	#region UserEditable 
	public override SimpleJSON.JSONClass GetProperties(){
//		Debug.Log("Getting prop.");
		SimpleJSON.JSONClass N = base.GetProperties();
//		Debug.Log("N:"+N.ToString());
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,knight.GetFraction(), N);
		return N;
	}


//	/	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMFractionButton);
		els.Add(LevelBuilder.inst.POCMheightButton);
		els.Add(LevelBuilder.inst.POCMcopyButton);
		return els.ToArray();
	}


	public override void OnGameStarted(){
		base.OnGameStarted();
		knight.UnFreeze();


	}

	// upoffset 	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		knight.SetFraction(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N));
	}

	public override void OnLevelBuilderObjectPlaced(){
		
		base.OnLevelBuilderObjectPlaced();

		knight.Freeze();

	}

	#endregion
}
