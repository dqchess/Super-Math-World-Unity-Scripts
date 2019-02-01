using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValueCommModCharacter : UIValueComm {


//	public override void OnMenuOpened(){
//		base.OnMenuOpened();
//		Randomize();
//	}
//
	public Text labelHeadColor;
	public Text labelBodyStyle;
	public Text labelBodyColor;
	public Text labelHairStyle;
	public Text labelHairColor;
	public Text labelBeardStyle;
	public Text labelBeardColor;

	CostumeController cc {
		get {
			if (LevelBuilder.inst.currentPiece && LevelBuilder.inst.currentPiece.GetComponent<DynamicNPC>()){
				return LevelBuilder.inst.currentPiece.GetComponent<DynamicNPC>().cc;
			} else return null;
		}
	}


	// a little thick but organized and effective. Costume Controller doesn't need to know it's being set, just the gameobject's materials etc are being set or activated
	// When the object is serialized during a sAVE event, CostumeController looks at what objects are active and what color they are and reports that, which is then serialized into JSON for saving;

	public void CycleHeadColor(int c){
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.headColorKey].AsInt + c;
		if (i < 0) i = cc.allMaterials.Length - 1;
		i %= cc.allMaterials.Length;
		N[CostumeController.headColorKey].AsInt = i;
		cc.SetProperties(N);
		labelHeadColor.text = Utils.NiceName(cc.allMaterials[i].name);
//		LevelBuilderMessager.inst.Display("Head color: "+cc.allMaterials[i].name);
	}

	public void CycleBodyStyle(int c) {
//		Debug.Log("c:"+c);
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.bodyStyleKey].AsInt + c;
		if (i < 0) i = cc.characters.Length - 1;
		i %= cc.characters.Length;
		N[CostumeController.bodyStyleKey].AsInt = i;
		cc.SetProperties(N);
//		LevelBuilderMessager.inst.Display("Body style: "+i);
		labelBodyStyle.text = i.ToString();
	}

	public void CycleBodyColor(int c){
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.bodyColorKey].AsInt + c;
		if (i < 0) i = cc.allMaterials.Length - 1;
		i %= cc.allMaterials.Length;
		N[CostumeController.bodyColorKey].AsInt = i;
		cc.SetProperties(N);
//		LevelBuilderMessager.inst.Display("Body color: "+cc.allMaterials[i].name);
		labelBodyColor.text = Utils.NiceName(cc.allMaterials[i].name);
	}

	public void CycleHairStyle(int c){
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.hairStyleKey].AsInt + c;
		if (i < 0) i = cc.hair.Length - 1;
		i %= cc.hair.Length;
		N[CostumeController.hairStyleKey].AsInt = i;
		cc.SetProperties(N);
//		LevelBuilderMessager.inst.Display("Hair style: "+cc.hair[i].name);
		labelHairStyle.text = cc.hair[i].name;
	}

	public void CycleHairColor(int c){
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.hairColorKey].AsInt + c;
		if (i < 0) i = cc.allMaterials.Length - 1;
		i %= cc.allMaterials.Length;
		N[CostumeController.hairColorKey].AsInt = i;
		cc.SetProperties(N);
//		LevelBuilderMessager.inst.Display("Hair color: "+cc.allMaterials[i].name);
		labelHairColor.text = Utils.NiceName(cc.allMaterials[i].name);
	}

	public void CycleBeardStyle(int c){
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.beardStyleKey].AsInt + c;
		if (i < 0) i = cc.beards.Length - 1;
		i %= cc.beards.Length;
		N[CostumeController.beardStyleKey].AsInt = i;
		cc.SetProperties(N);
//		LevelBuilderMessager.inst.Display("Beard style: "+cc.beards[i].name);
		labelBeardStyle.text = cc.beards[i].name;
	}

	public void CycleBeardColor(int c){
		Debug.Log("c;"+c);
		SimpleJSON.JSONClass N = cc.GetProperties();
		int i = N[CostumeController.beardColorKey].AsInt + c;
		Debug.Log("i 1:"+i);
		if (i < 0) i = cc.allMaterials.Length - 1;
		i %= cc.allMaterials.Length;
		N[CostumeController.beardColorKey].AsInt = i;
		cc.SetProperties(N);
		Debug.Log("i 2:"+i);
//		LevelBuilderMessager.inst.Display("Beard color: "+cc.allMaterials[i].name);
		labelBeardColor.text = Utils.NiceName(cc.allMaterials[i].name);
	}

	public void Randomize(){
		CycleBeardColor(Random.Range(0,10));
		CycleBeardStyle(Random.Range(0,10));
		CycleHairColor(Random.Range(0,10));
		CycleHairStyle(Random.Range(0,10));
		CycleBodyColor(Random.Range(0,10));
		CycleBodyStyle(Random.Range(0,10));
		CycleHeadColor(Random.Range(0,10));

	}
}
