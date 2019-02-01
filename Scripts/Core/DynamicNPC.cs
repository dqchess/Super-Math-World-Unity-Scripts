using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicNPC : PlaceableNPC {


	public CostumeController cc;


	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(CostumeController.beardColorKey)){
			// if it has one costume controller key, it has them all, so we just check one -- beard.
			cc.SetProperties(N);
		}
	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMModifyCharacterButton);
		els.Add(LevelBuilder.inst.POCMcopyButton);
		return els.ToArray();
	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N = cc.GetProperties(N);
		return N;
	}
}
