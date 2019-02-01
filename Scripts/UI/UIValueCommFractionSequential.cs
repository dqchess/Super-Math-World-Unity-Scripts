using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class UIValueCommFractionSequential : UIValueComm {

////	UISetCurrentObjectProperties
//	public InputField fracOrigNumerator;
//	public InputField fracOrigDenominator;
//	public InputField fracSeqANumerator;
//	public InputField fracSeqADenominator;
//	public InputField fracSeqBNumerator;
//	public InputField fracSeqBDenominator;
//	public InputField totalSequenceLength;
//
//
//	// When the menu is activated, pull the fraction property information from the current object and populate the inputs accordingly.
//
//	public override void OnMenuOpened(){
////		// commented Debug.Log("ui fraction setter opened");
//		if (!LevelBuilder.inst.currentPiece) return;
//		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
//		Fraction fracOrig = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
//		Fraction fracA = Fraction.Add(fracOrig,JsonUtil.ConvertJsonToFraction(JsonLevelLoader.inst.fractionSeqAKey,N)); 
//		Fraction fracB = Fraction.Add(fracA,JsonUtil.ConvertJsonToFraction(JsonLevelLoader.inst.fractionSeqBKey,N));
//		// commented Debug.Log("orig, a, b;"+fracOrig+","+fracA+","+fracB);
//		fracOrigNumerator.text = fracOrig.numerator.ToString(); 
//		fracOrigDenominator.text = fracOrig.denominator.ToString();
//		fracSeqANumerator.text = fracA.numerator.ToString();
//		fracSeqADenominator.text = fracA.denominator.ToString();
//		fracSeqBNumerator.text = fracB.numerator.ToString();
//		fracSeqBDenominator.text = fracB.denominator.ToString(); // sorry for poor naming.
//		if (N.GetKeys().Contains(JsonLevelLoader.inst.fracSequenceLengthKey)) totalSequenceLength.text = N[JsonLevelLoader.inst.fracSequenceLengthKey].AsInt.ToString();
//		else totalSequenceLength.text = "0";
//		// note that in json we're storing the DELTA between the orig frac and fracs b and c, but the user sees the rESULTING fractions a, b, c in a sequence.
//		// for example user enters 1/2, 2/2, 3/2 into the input for origfrac, fracseqa, fracseqb. JSON stores 1/2 as the origfrac, 1/2 as the fracseqA, 1/2 as the fracseqB
//		// and numberstructurecreator uses 1/2 as the increment for each successive GetFractionFromIndex(i) for both i and i%2
//	
//
//	}
//
//	public override void SetObjectProperties(){
//		// User enters the first three numbers of a fraction sequence, A B C, and a limit, N, to cycle.
//		// Save B-A as fracSeqStepA
//		// Save C-B as fracSeqStepB
//		// save N as fractionLimit property.
//		if (fracOrigNumerator.text.Length == 0 
//			|| fracOrigNumerator.text == "-" // shouldn't happen.. 
//			|| fracOrigDenominator.text.Length == 0 
//			|| fracSeqANumerator.text.Length == 0
//			|| fracSeqANumerator.text == "-"
//			|| fracSeqADenominator.text.Length == 0
//			|| fracSeqBNumerator.text.Length == 0
//			|| fracSeqBNumerator.text == "-"
//			|| fracSeqBDenominator.text.Length == 0
//			|| totalSequenceLength.text.Length == 0
//		) return;
//
//
//		// I am sorry for this ugliness
//		// converts user inputted sequence into a starting index and 2 deltas for json
//		// user inputs 1, 5, -2 -- json stores 1, 4, -6 (the second two are deltas)
//		Fraction origFrac = new Fraction(MathUtils.IntParse(fracOrigNumerator.text),MathUtils.IntParse(fracOrigDenominator.text));
//		Fraction secondFractionInSequence = new Fraction(MathUtils.IntParse(fracSeqANumerator.text),MathUtils.IntParse(fracSeqADenominator.text));
//		Fraction thirdFractionInSequence = new Fraction(MathUtils.IntParse(fracSeqBNumerator.text),MathUtils.IntParse(fracSeqBDenominator.text));
//		Fraction fraqSeqADelta = Fraction.Subtract(secondFractionInSequence,origFrac);
//		Fraction fracSeqBDelta = Fraction.Subtract(thirdFractionInSequence,secondFractionInSequence);
//		SimpleJSON.JSONClass N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,origFrac); 
//		N = JsonUtil.ConvertFractionToJson(JsonLevelLoader.inst.fractionSeqAKey,fraqSeqADelta,N); // I am sorry for this ugliness
//		N = JsonUtil.ConvertFractionToJson(JsonLevelLoader.inst.fractionSeqBKey,fracSeqBDelta,N); // I am sorry for this ugliness
//		N[JsonLevelLoader.inst.fracSequenceLengthKey].AsInt = MathUtils.IntParse(totalSequenceLength.text);
//		if (LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>()) {
//			LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
//		} else {
//			// commented Debug.Log("tried to set frac, but no ueo!"+LevelBuilder.inst.currentPiece.name);
//		}
//
////		LevelBuilder.inst.SetSequentialFractionBackboardColor(fraqSeqADelta.numerator,fracSeqBDelta.numerator);
//
//	}
//
//	public void CancelSequence(){
//		fracSeqANumerator.text = fracOrigNumerator.text;
//		fracSeqADenominator.text = fracOrigDenominator.text;
//		fracSeqBNumerator.text = fracOrigNumerator.text;
//		fracSeqBDenominator.text = fracOrigDenominator.text;
//		totalSequenceLength.text = "0";
//		SetObjectProperties();
//	}
}
