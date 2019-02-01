using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;


public class NumberHoopMultiply : NumberHoop {

	public Fraction frac = new Fraction(2,1);

	#region UserEditable 

	public override SimpleJSON.JSONClass GetProperties(){ 
		
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		return N;
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
		SetHoopFraction(f);
		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */
		//		// commented Debug.Log("props:"+props);
	}

	public override GameObject[] GetUIElementsToShow(){
		//		List<GameObject> elements = new List<GameObject>();
		//		elements.Add(LevelBuilder.inst.POCMFractionButton);
		return new GameObject[] {
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMintegerButton,
			LevelBuilder.inst.POCMheightButton
		};
		//		return elements.ToArray();
	}

	/* footpring was: (){
		return 1.4f;
	 */
	#endregion


	public override void Start(){
		base.Start();
//		frac = new Fraction(numerator,denominator);
		SetHoopText();
	}

	public override void SetHoopText(){
		// todo regex
		for (int i=0; i<10; i++){
			numberTextBack.text = numberTextBack.text.Replace(i.ToString(),"");
			numberTextFront.text = numberTextFront.text.Replace(i.ToString(),"");
		}

		numberTextBack.text = numberTextBack.text.Replace("-","");
		numberTextFront.text = numberTextFront.text.Replace("-","");
		if (!numberTextBack) return;
		if (!numberTextFront) return;

		// TODO: Use standard text setup as found in NumberInfo
		numberTextBack.text += frac.numerator.ToString();
		numberTextFront.text += frac.numerator.ToString();
//		Debug.Log("sethooptext, len:"+numberTextBack.text.Length);
		float s = 0.04f; // regular scale
		if (numberTextBack.text.Length <= 2){
			s = 0.04f;
		} else if (numberTextBack.text.Length == 3){
			s = 0.032f;
		} else if (numberTextBack.text.Length == 4){
			s = 0.027f;
		} else if (numberTextBack.text.Length > 4){
			s = 0.02f;
		}
		numberTextBack.transform.localScale = Vector3.one * s;
		numberTextFront.transform.localScale = Vector3.one * s;
	}

	public void SetHoopFraction(Fraction f){
		//		// commented Debug.Log ("yea?");
		frac = f;
		SetHoopText();
	}

	public override void UseHoop(GameObject obj, RecordPosition record, bool direction){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		base.UseHoop(obj,record,direction);
		NumberModifier.ModifyOperation nmf = (x => x);

		if (frac.numerator == -1 && frac.denominator == 1){
			nmf = (x => Fraction.Multiply(x, frac));
			currentEquation = " * " + frac;
		} else {

			if(direction) {
				nmf = (x => Fraction.Multiply(x, frac));
				currentEquation = " * " + frac;
			}
			else {
				nmf = (x => Fraction.Multiply(x, Fraction.Inverse(frac)));
				currentEquation = " / " + frac;
			}
		}

//		// commented Debug.Log("2");
		if (obj.tag=="Player") {
//			PlayerInventory.inst.ModifyInventoryItemnmf);
			Inventory.inst.ModifyInventoryItems(nmf);
		} else if (obj.GetComponent<NumberInfo>() != null && obj.GetComponent<ResourceNumber>() == null) {
			NumberInfo ni = obj.GetComponent<NumberInfo>();
			modifyOp = nmf;
//			// commented Debug.Log("3");
			ModifyNumber(ni);
		}

//		obj.SendMessage("OnNumberChanged",SendMessageOptions.DontRequireReceiver);

	}

	public override void PostModifyNumber (Fraction original, NumberInfo ni)
	{
		base.PostModifyNumber (original, ni);
		MonsterAIRevertNumber revert = ni.GetComponent<MonsterAIRevertNumber>();
		if(revert) { revert.SetNumber(ni.fraction); }
	}	

	public override Fraction GetModifiedFraction (Fraction original)
	{
		return modifyOp(original);
	}

	public override string GetEquation (Fraction original)
	{
		return original + " " + currentEquation + " = " + GetModifiedFraction(original);
	}
}






