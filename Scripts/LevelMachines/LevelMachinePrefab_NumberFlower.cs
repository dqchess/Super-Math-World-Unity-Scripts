using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelMachinePrefab_NumberFlower : UserEditableObject {

	public Renderer flower;

	// Use this for initialization
	void Start () {
		// Set random rotation and size on start
		Quaternion rot = transform.rotation;
		rot.eulerAngles = new Vector3(Random.Range(-2f,2f),Random.Range(0,360f),Random.Range(-2f,2f));
		transform.rotation = rot;
		transform.localScale *= Random.Range(1f,1.3f);
		Material[] mats = flower.materials;
		mats[4].color = Utils.RandomColor();
		flower.materials = mats;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		NumberInfo ni = GetComponentInChildren<NumberInfo>();
		if (ni){
			return JsonUtil.ConvertFractionToJson(Fraction.fractionKey,ni.fraction,N as SimpleJSON.JSONClass);
		} else return N;
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		NumberInfo ni = GetComponentInChildren<NumberInfo>();
		if (N.GetKeys().Contains(Fraction.fractionKey)){
			if (ni){
				ni.SetNumber(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N));	
			} else {
				Debug.Log("ERROR: no number in this flower!:"+name);
			}
		} else {
			if (ni) {
				NumberManager.inst.DestroyOrPool(ni);
//				Destroy(ni.gameObject); // this flower shouldn't exist, but was included with the prefab .. so we destroy it after placement because json says it shouldn't have a numberinfo
			} else {
				Debug.Log("ERROR: no number in this flower!:"+name);
			}
		}

	}
}
