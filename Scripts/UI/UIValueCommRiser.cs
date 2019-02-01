using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIValueCommRiser : UIValueComm {

	//	UISetCurrentObjectProperties
	public InputField numerator;
	public InputField denominator;


	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
	public void OnEnable(){
		OnMenuOpened();
	}
	public void OnMenuOpened(){

		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		// commented Debug.Log("N.tostr:"+N.ToString());
		if (N.GetKeys().Contains(NumberRiser.heightScaleKey)){
			numerator.text = Fraction.GetFractionFromFloat(N[NumberRiser.heightScaleKey].AsFloat).numerator.ToString();
			denominator.text = Fraction.GetFractionFromFloat(N[NumberRiser.heightScaleKey].AsFloat).denominator.ToString();
		}

	}




	public override void SetObjectProperties(){
		
		if (numerator.text.Length == 0 || denominator.text.Length == 0 || numerator.text == "-") return; 
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[NumberRiser.heightScaleKey].AsFloat = (new Fraction(MathUtils.IntParse(numerator.text),MathUtils.IntParse(denominator.text))).GetAsFloat();
		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);

	}
}
