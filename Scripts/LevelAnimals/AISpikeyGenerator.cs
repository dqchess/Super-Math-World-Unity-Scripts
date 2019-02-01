using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISpikeyGenerator : UEO_SimpleObject {

	#region UserEditable
	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,spikeyFraction,N);
		return N;
	}
	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		spikeyFraction = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
	}
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> elements = new List<GameObject>();
		elements.AddRange(base.GetUIElementsToShow());
		elements.Add(LevelBuilder.inst.POCMFractionButton);
		return elements.ToArray();
	}
	#endregion
	
	// make a static prefab class for levelbuilderobjectmanager.prefabs
	// e.g. public prefab.monster.spikey();
	public GameObject spikeyPrefab;
	public Fraction spikeyFraction;
	public Transform generatorT;
	public NumberInfo spikeyCounter;
//	float spikeyCheckTimer
//	float childCheckInterval = 0.2f;
	float timer = 0f;
	float generateInterval = 3.2f;
	float childCheckInterval = 0.2f;
	void Update(){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		// todos: while static.countdown()<0: // to eliminate local timer vars and update clutter
		if (Utils.IntervalElapsed(generateInterval)){
			GenerateSpikey();
		}
		if (Utils.IntervalElapsed(childCheckInterval)){
			if (checkChildren && ChildrenSumEqualsZero()){
				DestroyMe();
			}
		}
	}

	bool ChildrenSumEqualsZero(){
		Fraction total = new Fraction(0,1);
		foreach(NumberInfo ni in GetComponentsInChildren<NumberInfo>()){
			if (ni != spikeyCounter) total = Fraction.Add(ni.fraction,total);
		}
		spikeyCounter.SetNumber(total);
		return total.numerator == 0;
	}

	void DestroyMe(){
		Destroy(gameObject);
		EffectsManager.inst.CreateShards(transform.position);
		AudioManager.inst.PlayNumberShatter(transform.position);
		PlayerNowMessage.inst.Display("Spikey generator reached zero and was destroyed!");
	
	}
	bool checkChildren = false; // don't need to check until we've generated a few.
	GameObject GenerateSpikey(){
		checkChildren = true;
		GameObject spikey = (GameObject)Instantiate(spikeyPrefab,generatorT.position,Quaternion.identity);
		spikey.GetComponent<NumberInfo>().SetNumber(spikeyFraction);
		float throwForce = 5200f;
		spikey.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce);
		spikey.transform.parent = transform;
		TimedObjectDestructor tod =  spikey.AddComponent<TimedObjectDestructor>();
		tod.DestroyNow(20f);
		return null;
	}
}
