using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
//using System.Text.RegularExpressions;


public class NumberHoopMultiplySphere : NumberHoop {

	public Fraction frac = new Fraction(2,1);
	public GameObject triggerObject;
	List<HoopText> texts = new List<HoopText>();
	bool dividesVisible = true;

	#region UserEditable 

	public override void Start(){
		base.Start();
		SetHoopText();
		texts.AddRange(GetComponentsInChildren<HoopText>());

		HideDivides();
	}


	public override SimpleJSON.JSONClass GetProperties(){ 
		
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		return N;
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)){
			Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
			SetHoopFraction(f);
		}
		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */
		//		// commented Debug.Log("props:"+props);
	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> elements = new List<GameObject>();
		elements.AddRange(base.GetUIElementsToShow());

		return elements.ToArray();
	}

	/* footpring was: (){
		return 1.4f;
	 */
	#endregion




	public override void SetHoopText(){
		// todo regex
		foreach(HoopText ht in GetComponentsInChildren<HoopText>()){
			Text t = ht.GetComponent<Text>();
			for (int i=0; i<10; i++){
				t.text = t.text.Replace(i.ToString(),"");

			}
			
			t.text = t.text.Replace("-","");
			t.text = t.text.Replace("/","");

			// TODO: Use standard text setup as found in NumberInfo
//			Debug.Log("t:"+t.text);
			t.text += frac.numerator.ToString();
			if (frac.denominator != 1) {
				string fractionbit = "/"+frac.denominator.ToString();
				t.text += fractionbit;
			}

//			Debug.Log("t.txt;"+frac.ToString());


			float s = 0.04f; // regular scale
			if (t.text.Length <= 2){
				s = 0.04f;
			} else if (t.text.Length == 3){
				s = 0.032f;
			} else if (t.text.Length == 4){
				s = 0.027f;
			} else if (t.text.Length > 4){
				s = 0.02f;
			}
			t.transform.localScale = Vector3.one * s;
		}
	}

	public void SetHoopFraction(Fraction f){
		//		// commented Debug.Log ("yea?");
		frac = f;
		SetHoopText();
	}

	bool ObjectIsCloseToSurface(GameObject o){
		float distToObj = Vector3.Distance(o.transform.position,triggerObject.transform.position);
		float radiusDist = triggerObject.transform.lossyScale.x/2f;
//		Debug.Log("dist:"+distToObj+", rad;"+radiusDist);
		return Mathf.Abs(distToObj - radiusDist) < 2f;
	}

	public override void UseHoop(GameObject obj, RecordPosition record, bool direction){
		if (!ObjectIsCloseToSurface(obj)){
			// Prevents objects from getting flipped when being taken out of inventory, or created, INSIDE the sphere.
			return;
		}
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

	void ShowDivides(){
		foreach(HoopText ht in texts){
			if (ht.side == HoopTextSide.Front){
				ht.GetComponent<Text>().enabled = false;
			} else {
				ht.GetComponent<Text>().enabled = true;
			}
		}
		dividesVisible = true;
	}
	void HideDivides(){
		foreach(HoopText ht in texts){
			if (ht.side == HoopTextSide.Front){
				ht.GetComponent<Text>().enabled = true;
			} else {
				ht.GetComponent<Text>().enabled = false;
			}

		}
		dividesVisible = false;
	}
	public void OnTriggerEnterO(GameObject o){
		if (o.GetComponent<Player>() && !dividesVisible){
			ShowDivides();
		}
		UseHoop(o,o.GetComponent<RecordPosition>(),true);
	}

	public void OnTriggerExitO(GameObject o){
		if (o.GetComponent<Player>() && dividesVisible){
			HideDivides();
		}
		UseHoop(o,o.GetComponent<RecordPosition>(),false);
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






