using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnergyDoor : UEO_SimpleObject {


	public Transform[] doorColliders;
	public ParticleSystem p;

	public LevelMachineBattery battery;

	public override void StartMachine(bool levelWasJustLoaded=false){
		OpenDoor();
	}

	public void OpenDoor(){
		p.emissionRate = 0;
		foreach(Transform t in doorColliders){
			t.GetComponent<Collider>().enabled = false;
		}
	}

	public override GameObject[] GetUIElementsToShow ()
	{
		return new GameObject[]{ 
			LevelBuilder.inst.POCMFractionButton,
			LevelBuilder.inst.POCMheightButton
		};
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		// we use the generic fraction key.
//		Debug.Log("setprop endoor:"+N.ToString());
		if (N.GetKeys().Contains(Fraction.fractionKey)) SetMaxCharge(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N)); 
		// For setting current charge we use the specific key (this can't be set during level builder editing mode)
		if (N.GetKeys().Contains(LevelMachineBattery.currentChargeKey)) SetCurrentCharge(JsonUtil.ConvertJsonToFraction(LevelMachineBattery.currentChargeKey,N));
	}

	void SetMaxCharge(Fraction f){
		battery.SetMaxCharge(f);
	}

	void SetCurrentCharge(Fraction f){
		battery.SetChargeLevel(f);
	}

	public override SimpleJSON.JSONClass GetProperties(){
		// For getting the data about how much charge was collected (as a result of user playing and inserting charge into the battery), use our specific currentchargeKey

		SimpleJSON.JSONClass N = base.GetProperties();
		JsonUtil.ConvertFractionToJson(LevelMachineBattery.currentChargeKey,battery.totalChargeCollected,N);
		// For reporting max charge to Fraction setter in UI which doesn't know about maxChargeKey, we use the generic fraction key
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,battery.maxCharge,N); 
//		Debug.Log("got prop:"+N.ToString());
		return N;
	}


}
