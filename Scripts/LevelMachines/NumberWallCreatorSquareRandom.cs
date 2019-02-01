using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class NumberWallCreatorSquareRandom : NumberWallCreatorSquare {

	#region usereditable

	public static string fractionLowerKey = "fractionLowerKey";
	public static string fractionUpperKey = "fractionUpperKey";
	public Fraction lowerFrac = new Fraction(1,1);
	public Fraction upperFrac = new Fraction(9,1);

	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] {
			LevelBuilder.inst.POCMRandomFractionButton,
			LevelBuilder.inst.placedObjectContextMenuNumberWallSizeButton,
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMheightButton
		};
	}

	public override SimpleJSON.JSONClass GetProperties(){ 

		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(fractionLowerKey,lowerFrac,N);
		N = JsonUtil.ConvertFractionToJson(fractionUpperKey,upperFrac,N);
//		N[JsonLevelLoader.inst.fracSequenceLengthKey].AsInt = maxFracSeqSteps;
		N[wallCreatorSquareKey][wallXkey] = wallX.ToString();
		N[wallCreatorSquareKey][wallYkey] = wallY.ToString();
		N[wallCreatorSquareKey][wallZkey] = wallZ.ToString();
		return N;
	}



	public override void SetProperties(SimpleJSON.JSONClass N){
//		// commented Debug.Log("set prop on obj:"+name+", prop:"+N.ToString());
//		if (N.GetKeys().Contains(wallCreatorSquareKey)) SetPropertiesSize(N);
//
//		// move following to base.setprop?
		SetPropertiesFraction(N);
		base.SetProperties(N);

	}

	void SetPropertiesFraction(SimpleJSON.JSONClass N){
		if (N.GetKeys().Contains(fractionLowerKey)) lowerFrac = JsonUtil.ConvertJsonToFraction(fractionLowerKey,N);
		if (N.GetKeys().Contains(fractionUpperKey)) upperFrac = JsonUtil.ConvertJsonToFraction(fractionUpperKey,N);

		foreach(NumberInfo ni  in GetComponentsInChildren<NumberInfo>()){
			Fraction f = GetFractionFromIndex(0);
			ni.SetNumber(f,false,false);
		}
//		// commented Debug.Log("setting FRAC prop:"+frac);
	}




	#endregion


	public override Fraction GetFractionFromIndex(int i){
		int n = 0;
		Fraction f = new Fraction(1,1);
		while (n==0){
			n = Random.Range(lowerFrac.numerator,upperFrac.numerator+1);
			if (n != 0) f = new Fraction(n,Random.Range(lowerFrac.denominator,upperFrac.denominator));
		}
//		// commented Debug.Log("L/U"+lowerFrac.ToString()+","+upperFrac.ToString()+" ... got frac from index:"+i+":"+f);
		return f;
	}

//

//	public override int GetStartingBlocksCount(){
//		int ret = box ? wallX * 4 + wallY * 4 + wallZ * 4 - 8 : wallX * wallY * wallZ;
//		// commented Debug.Log("start blocksc count on:"+name+" with box:"+box+" was:"+ret);
//		return ret;
//	}
}
