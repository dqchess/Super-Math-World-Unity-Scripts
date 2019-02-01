using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIValueCommInteger : UIValueComm {

//	UISetCurrentObjectProperties
	public InputField numeratorInput;


	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.



	public override void OnMenuOpened(){
//		// commented Debug.Log("ui fraction setter opened");
		if (!LevelBuilder.inst.currentPiece) return;
		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
//		// commented Debug.Log("ui fractions getter got fraction:"+f.numerator+","+f.denominator);
		numeratorInput.text = f.numerator.ToString(); 
//		// commented Debug.Log("numerator text is now;"+numeratorInput.text);
//		denominatorInput.text = f.denominator.ToString();
	}

	public override void SetObjectProperties(){

		if (numeratorInput.text.Length == 0 || numeratorInput.text == "-") return;
		base.SetObjectProperties();
//		string den = denominatorInput.text.ToString();
//		// commented Debug.Log("den;"+den);
		Fraction userInputtedFraction = new Fraction(int.Parse(numeratorInput.text.ToString()),1);
		SimpleJSON.JSONClass N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,userInputtedFraction); // ConvertJsonToFraction(userInputtedFraction);
//		// commented Debug.Log("props:"+props);
		if (LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>()) {
			LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
		} else {
			// commented Debug.Log("tried to set frac, but no ueo!"+LevelBuilder.inst.currentPiece.name);
		}
	}
}
