using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberFaucetSequential : NumberFaucet {

	public Sprite cauldronNumberIcon;

	#region UserEditable
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.Add(LevelBuilder.inst.POCMheightButton);
		return els.ToArray();
	}
	public override void SetProperties(SimpleJSON.JSONClass N){
		// no properties to set!
	}
	public override void OnLevelBuilderObjectPlaced(){
		DestroyDuplicates<NumberFaucetSequential>();
	}

	public virtual void DestroyDuplicates<T> () where T : NumberFaucetSequential
	{
		//		// commented Debug.Log("T:");
		foreach(T t in FindObjectsOfType<T>()){
			if (t == this) continue;
			Destroy(t.gameObject);
		}

	}

	#endregion

	float genTimer2 = 1.5f;
	public int index = 1;
	public GameObject currentNumber;

	public override void Update(){
		if (LevelBuilder.inst.levelBuilderIsShowing) return; // transform.root.GetComponent<LevelMachineCauldronGroup>().puzzleFrozen) return;
		if (GenerateReady()){
			GenerateNumber(new Fraction(index,1));
			NumberInfo ni = dripNumber.GetComponent<NumberInfo>();
			ni.greaterThanCombine = 1;
			ni.lessThanCombine = 0; // no combinations!
			ni.childMeshRenderer.material = Utils.FadeMaterial();
			ni.childMeshRenderer.material.color = new Color(1,1,1,0.5f);

			dripNumber.AddComponent<CauldronNumber>();

			currentNumber = dripNumber;
		}
		if (dripNumber != null) DripFX();
		genTimer2 -= Time.deltaTime;
	}

	public override void  GenerateNumber(Fraction f){
		genTimer = interval;
		dripNumber = NumberManager.inst.CreateNumber(f,dripStartT.position,NumberShape.Schur);
		if (oneball) {
			dripNumber.GetComponent<NumberInfo>().SetFaucetRel(this);
			dripNumber.GetComponent<UserEditableObject>().isSerializeableForSceneInstance = false;
//			dripNumber.GetComponent<UserEditableObject>().isEphemeral = false;
		}
		generatedNumber = dripNumber;
		dripNumber.transform.localScale = Vector3.one * GameConfig.inst.numberScale;
		dripTime = 0;
		dripNumber.transform.parent = transform;
	}

	bool GenerateReady(){
		return genTimer2 < 0 && Resources.FindObjectsOfTypeAll<CauldronNumber>().Length == 0;
		// If the number was picked up and thrown into one of the cauldrons, 

	}

	public void OnNumberCollectedIntoCauldron(){
		index++;
//		Debug.Log("a num was collected into a cauldron! index:"+index);
		currentNumber = null;
		dripNumber = null;
	}

	public void Reset(){
		genTimer2 = 4;
		if (dripNumber) Destroy(dripNumber);
		if (currentNumber) Destroy(currentNumber);
		index = 1;
	}
}
