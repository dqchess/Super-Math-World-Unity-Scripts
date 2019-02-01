using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GadgetUserEditable : UserEditableObject {

	public Gadget gadgetRef;

	#region UserEditable 
	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();
		N = gadgetRef.GetProperties(N);
		return gadgetRef.GetProperties(N);
	}


	public override void OnGameStarted(){
		gadgetRef.OnGameStarted();

		base.OnGameStarted();
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		gadgetRef.SetProperties(N);
	}	
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.AddRange(gadgetRef.GetUIElementsToShow());
		return els.ToArray();

	}


	#endregion
}
