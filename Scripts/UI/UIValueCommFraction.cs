using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIValueCommFraction : UIValueComm {

	// This allows the user to input (and view) the object's current fraction
	public bool allowZero = false;
	public InputField numeratorInput;
	public InputField denominatorInput;

	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.

	public override void OnMenuOpened(){
//		// commented Debug.Log("ui fraction setter opened");
		if (!LevelBuilder.inst.currentPiece) return;
		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
//		// commented Debug.Log("ui fractions getter got fraction:"+f.numerator+","+f.denominator);
		numeratorInput.text = f.numerator.ToString(); 
//		// commented Debug.Log("numerator text is now;"+numeratorInput.text);
		denominatorInput.text = f.denominator.ToString();
	}

	public override void SetObjectProperties(){
//		// commented Debug.Log("numerator text is now2;"+numeratorInput.text);
//		// commented Debug.Log("ui fraction setter setting props.");
//
//		// commented Debug.Log("num, den:"+numeratorInput.text+","+denominatorInput.text);
		if (numeratorInput.text.Length == 0 
			|| denominatorInput.text.Length == 0 
			|| numeratorInput.text == "-" 
			|| denominatorInput.text.Contains("-") 
			|| (denominatorInput.text.Contains("0") && MathUtils.IntParse(denominatorInput.text) == 0) 
			||  ((numeratorInput.text.Contains("0") && MathUtils.IntParse(numeratorInput.text) == 0)) && !allowZero) 
			return;
//		UIRestrictInputCustom restrict = numeratorInput.GetComponent<UIRestrictInputCustom>();
//		restrict.RestrictInput(numeratorInput);
//		restrict = denominatorInput.GetComponent<UIRestrictInputCustom>();
//		restrict.RestrictInput(denominatorInput);
//		Debug.Log("numerator:"+numeratorInput.text);
		base.SetObjectProperties();
		string den = denominatorInput.text.ToString();
//		// commented Debug.Log("den;"+den);
		Fraction userInputtedFraction = new Fraction(int.Parse(numeratorInput.text.ToString()),int.Parse(den));
		SimpleJSON.JSONClass N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,userInputtedFraction); // ConvertJsonToFraction(userInputtedFraction);
//		// commented Debug.Log("props:"+props);
		if (LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>()) {
			LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
		} else {
			// commented Debug.Log("tried to set frac, but no ueo!"+LevelBuilder.inst.currentPiece.name);
		}
	}
}
