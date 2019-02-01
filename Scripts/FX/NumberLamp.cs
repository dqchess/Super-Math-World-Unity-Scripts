using UnityEngine;
using System.Collections;

public class NumberLamp : UEO_SimpleObject {


	public NumberLampTrigger lamp;

	#region UserEditable 
	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		return JsonUtil.ConvertFractionToJson(Fraction.fractionKey, lamp.lampNumber.fraction,N);
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		lamp.lampNumber.SetNumber(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N));
//		// commented Debug.Log("set:"+lamp.lampNumber+" to;"+lamp.lampNumber.fraction.ToString());
	}
	public override void OnGameStarted(){
		base.OnGameStarted();
		GetComponent<AudioSource>().Play();
	}

	void Update(){
		if (Utils.IntervalElapsed(5)){ // lol. Should be a delegate or something. But instead we just check every five seconds! Simple!
			if (LevelBuilder.inst.levelBuilderIsShowing){
				GetComponent<AudioSource>().Stop();
			} else {
				if (!GetComponent<AudioSource>().isPlaying){
					GetComponent<AudioSource>().Play();
				}
			}
		}
	}



	#endregion
}
