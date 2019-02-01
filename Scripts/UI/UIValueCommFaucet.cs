using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIValueCommFaucet : UIValueComm {

	public InputField offset;
	public InputField interval;

	public override void OnMenuOpened(){
		NumberFaucet nf = LevelBuilder.inst.currentPiece.GetComponent<NumberFaucet>();
		Debug.Log("nf offset:"+nf.offset);
		Debug.Log("nf interval;"+nf.interval);
		float result;
		if (nf.interval == 0){
			interval.text = "0";
		} else {
			interval.text = nf.interval.ToString("0.00");
		}
		if (nf.offset == 0){
			Debug.Log("off 0");
			offset.text = "0";
		} else {
			Debug.Log("0, it was not");
			offset.text = nf.offset.ToString("0.00");
		}


	}


	public override void SetObjectProperties(){
		base.SetObjectProperties();
		NumberFaucet nf = LevelBuilder.inst.currentPiece.GetComponent<NumberFaucet>();
		Debug.Log("interval:"+interval.text+",offset;"+offset.text);
		float result;
		if (float.TryParse(offset.text,out result)) nf.SetOffset(result);
		if (float.TryParse(interval.text, out result)) nf.interval = result;
//		if (interval.text != "") nf.interval = float.Parse(interval.text);
//		if (offset.text != "") nf.SetOffset(float.Parse(offset.text));
	}
}
