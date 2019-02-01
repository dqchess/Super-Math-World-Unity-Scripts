using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelMachine_TownBattery : UEO_SimpleObject {

//	 public static string activatedKey = "TownBatteryActivated"; // We don't need to store if it was activated, because it will know if it was activated because battery level and target battery level are the same
	// Because the object becomes activated when actualBatteryLevel == targetBatteryLevel (in LevelMachineBattery)
	// And the actualBatteryLevel is stored in Level Instances.

	public Renderer[] lightningRodTips;
	public Material activatedMaterial;
	public ParticleSystem lightningParticles;
	public LevelMachineBattery battery;
	bool activated = false;

	public Transform prison;
	public ParticleSystem prisonParticles;

	public override void StartMachine(bool levelWasJustLoaded = false){ // legacy method name from LevelMachineBattery.
		if (!levelWasJustLoaded){
			Debug.Log("started without level having been loaded.");
			// levelWasJustLoaded -- if it is set in SetProperties, onload is set to True so we don't save a new level instance at that time
			// we only want to save the level instance if the player completed a StartMachine by filling up the battery for the *first* time
			JsonLevelSaver.inst.SaveLevel(SceneSerializationType.Instance);
			AudioManager.inst.PlayElectricArc(transform.position,.5f,1);
			PlayerDialogue.inst.ShowPlayerDialogue("You restored power to the town","Great job!",icon);
//			Debug.Log("8 light");
			for(int i=0;i<2;i++){
				RandomLightningFX(8f);
			}
//			WebGLComm.inst.SendTownBatteryEvent();
		}

		// These post level FX happen regardless of whether the user completed the machinesolved() action or it was completed upon placement.
		activated=true;
		foreach(Transform t in prison.GetComponentsInChildren<Transform>()){
			Collider c = t.GetComponent<Collider>();
			if (c) c.enabled = false;
		}
		prisonParticles.emissionRate = 0;
		lightningParticles.gameObject.SetActive(true);
		foreach(Renderer r in lightningRodTips){
			r.material = activatedMaterial;
			r.GetComponent<Rotate>().enabled = true;
		}
	}

	public override GameObject[] GetUIElementsToShow ()
	{
		return new GameObject[]{ 
			LevelBuilder.inst.POCMFractionButton,
			LevelBuilder.inst.POCMheightButton
		};
	}


	void SetMaxCharge(Fraction f){
		battery.SetMaxCharge(f);
	}
	
	void SetCurrentCharge(Fraction f){
		// Note that if Currentcharge will == maxcharge the object will "StartMachine" as soon as it is loaded.
		bool levelWasJustLoaded = true;
		battery.SetChargeLevel(f,levelWasJustLoaded);
	}





	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		// For setting properties via FractinValueUIComm in level builder
		// we use the generic fraction key.
//		Debug.Log("setprop endoor:"+N.ToString());
//		if (N.GetKeys().Contains(activatedKey)){
//			
//			activated = N[activatedKey].AsBool;
//		}
		if (N.GetKeys().Contains(Fraction.fractionKey)) SetMaxCharge(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N)); 
		// For setting current charge we use the specific key (this can't be set during level builder editing mode)
		if (N.GetKeys().Contains(LevelMachineBattery.currentChargeKey)) {
			// Note this setcurrentcharge will pass a levelwasjustloaded=true to LevelMachineBattery to distinguish whether the user completed this action 
			// or it was completed beacuse the level was just loaded
			SetCurrentCharge(JsonUtil.ConvertJsonToFraction(LevelMachineBattery.currentChargeKey,N));
		}
	}


	public override SimpleJSON.JSONClass GetProperties(){
		// For getting the data about how much charge was collected (as a result of user playing and inserting charge into the battery), use our specific currentchargeKey
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(LevelMachineBattery.currentChargeKey,battery.totalChargeCollected,N);
		// For reporting max charge to Fraction setter in UI which doesn't know about maxChargeKey, we use the generic fraction key
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,battery.maxCharge,N); 
//		N[activatedKey].AsBool = activated;
//		Debug.Log("got prop:"+N.ToString());
		return N;
	}

	public override void OnLevelBuilderObjectPlaced(){
		DestroyDuplicates<LevelMachine_TownBattery>();
	}

	public virtual void DestroyDuplicates<T> () where T : LevelMachine_TownBattery
	{
		//		// commented Debug.Log("T:");
		foreach(T t in FindObjectsOfType<T>()){
			if (t == this) continue;
			Destroy(t.gameObject);
		}

	}

	float lightningTimer = 0;
	void Update(){



		if (activated){
			lightningTimer -= Time.deltaTime;
			if (lightningTimer < 0){
				lightningTimer = Random.Range(1,10f);
//				Debug.Log("single light");
				RandomLightningFX();

			}
		}
	}

	void RandomLightningFX(float duration=2){
		duration = Random.Range(duration/1.5f,duration*1.5f);
		SMW_GF.inst.CreateLightning(lightningRodTips[Random.Range(0,lightningRodTips.Length - 1)].transform,lightningRodTips[Random.Range(0,lightningRodTips.Length - 1)].transform,duration);
	}
}
