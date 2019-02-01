using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UEO_SimpleObject_LooseSphere : UserEditableObject {

//	public string objectName;
	public NumberInfo looseSphere;
	#region UserEditable 
	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();// new SimpleJSON.JSONClass();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,looseSphere.fraction,N);

		return N;

	}

	public override void OnLevelBuilderObjectCreated(){
		
		base.OnLevelBuilderObjectCreated();
//		if (rotationOffset){
//			Quaternion rot = transform.rotation;
//			rot.eulerAngles = new Vector3(rot.eulerAngles.x + .04f,rot.eulerAngles.y + .04f,rot.eulerAngles.z + .04f); // offset for zfighting
//			transform.rotation = rot;
//		}
	}

//	public override void OnLevelBuilderObjectPlaced(){
//	}

//	/	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> ui = new List<GameObject>();
		ui.AddRange(base.GetUIElementsToShow());
		ui.Add(LevelBuilder.inst.POCMcopyButton);
		ui.Add(LevelBuilder.inst.POCMheightButton);
		ui.Add(LevelBuilder.inst.POCMFractionButton);
		return ui.ToArray();

	}
		
	public override void OnGameStarted(){
		base.OnGameStarted();
		looseSphere.OnGameStarted();
	}



	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);

		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
		looseSphere.SetNumber(f);



	}



	#endregion
}
