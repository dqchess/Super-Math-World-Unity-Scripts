using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class UIValueCommFractionRandom : UIValueComm {

	//	UISetCurrentObjectProperties
	public InputField numeratorInputLower; // fraction will be randomized between these two values.
	public InputField denominatorInputLower;
	public InputField numeratorInputUpper; // fraction will be randomized between these two values.
	public InputField denominatorInputUpper;

	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.



	public override void OnMenuOpened(){
		//		// commented Debug.Log("ui fraction setter opened");
		if (!LevelBuilder.inst.currentPiece) return;
		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		Fraction lowerFrac = JsonUtil.ConvertJsonToFraction(NumberWallCreatorSquareRandom.fractionLowerKey,N);
		Fraction upperFrac = JsonUtil.ConvertJsonToFraction(NumberWallCreatorSquareRandom.fractionUpperKey,N);
	
		numeratorInputLower.text = lowerFrac.numerator.ToString(); 
		denominatorInputLower.text = lowerFrac.denominator.ToString();
		numeratorInputUpper.text = upperFrac.numerator.ToString(); 
		denominatorInputUpper.text = upperFrac.denominator.ToString();
	}

	public override void SetObjectProperties(){
		//		// commented Debug.Log("numerator text is now2;"+numeratorInput.text);
		//		// commented Debug.Log("ui fraction setter setting props.");
		//
		//		// commented Debug.Log("num, den:"+numeratorInput.text+","+denominatorInput.text);
		if (numeratorInputLower.text.Length == 0 || denominatorInputLower.text.Length == 0 || numeratorInputLower.text == "-"
			|| numeratorInputUpper.text.Length == 0 || denominatorInputUpper.text.Length == 0 || numeratorInputUpper.text == "-") return;
		base.SetObjectProperties();
		//		// commented Debug.Log("den;"+den);
		Fraction lowerFrac = new Fraction(int.Parse(numeratorInputLower.text.ToString()),int.Parse(denominatorInputLower.text.ToString()));
		Fraction upperFrac = new Fraction(int.Parse(numeratorInputUpper.text.ToString()),int.Parse(denominatorInputUpper.text.ToString()));
		SimpleJSON.JSONClass N = JsonUtil.ConvertFractionToJson(NumberWallCreatorSquareRandom.fractionLowerKey,lowerFrac);
		N = JsonUtil.ConvertFractionToJson(NumberWallCreatorSquareRandom.fractionUpperKey,upperFrac,N);

		if (LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>()) {
			LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
		} else {
			// commented Debug.Log("tried to set frac, but no ueo!"+LevelBuilder.inst.currentPiece.name);
		}
	}

}
