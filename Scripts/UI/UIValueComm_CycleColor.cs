using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIValueComm_CycleColor : MonoBehaviour {

	public Transform colorGrid;

	public void OnEnable(){

	}

//	public void CycleColor(int i){
//		UEO_ColorCycler cc = LevelBuilder.inst.currentPiece.GetComponentInChildren<UEO_ColorCycler>();
//		if (cc){
//			cc.CycleColor(i);
//		}
//	}

	public void SetColorNatural(int i){
		UEO_ColorCycler[] cc = LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ColorCycler>();
		foreach(UEO_ColorCycler c in cc){// note this grabs ALL color cyclers inside, for example if we are DraggingParent with many colored objs inside.
			SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
			N[UEO_ColorCycler.colorGroupKey] = UEO_ColorCycler.naturalKey;
			N[UEO_ColorCycler.colorManipulatorKey].AsInt = i;
			c.SetProperties(N);
		}
	}

	public void SetColorColorful(int i){
		UEO_ColorCycler[] cc = LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ColorCycler>();
		foreach(UEO_ColorCycler c in cc){ // note this grabs ALL color cyclers inside, for example if we are DraggingParent with many colored objs inside.
			SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
			N[UEO_ColorCycler.colorGroupKey] = UEO_ColorCycler.colorfulKey;
			N[UEO_ColorCycler.colorManipulatorKey].AsInt = i;
			c.SetProperties(N);
		} 
	}

	public void SetColorSponged(int i){
		UEO_ColorCycler[] cc = LevelBuilder.inst.currentPiece.GetComponentsInChildren<UEO_ColorCycler>();
		foreach(UEO_ColorCycler c in cc){ // note this grabs ALL color cyclers inside, for example if we are DraggingParent with many colored objs inside.
			SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
			N[UEO_ColorCycler.colorGroupKey] = UEO_ColorCycler.spongedKey;
			N[UEO_ColorCycler.colorManipulatorKey].AsInt = i;
			c.SetProperties(N);
		} 
	}

//	public void SetColor(int i){
//		UEO_ColorCycler cc = LevelBuilder.inst.currentPiece.GetComponentInChildren<UEO_ColorCycler>();
//		cc.SetProperties(i);
//	}
}
