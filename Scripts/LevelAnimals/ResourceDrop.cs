using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ResourceDrop : MonoBehaviour {

	public static string key = "resouceDrop"; // acgh this was mis-spelled and now dpeloyed levels don't have the correct spelled one, it was "resouceDrop"
	public static string droppedKey = "resourceDropCompleted"; // some deployed levels don't have this key in JSON yet.

	public Transform drop;
	public Fraction frac;
	public bool dropped = false;

	void Update () {
		
	}

	public void DropResource(){
		if (frac.numerator != 0){
			AudioManager.inst.PlayItemGetSound();
			GameObject resource = NumberManager.inst.CreateNumber(frac,drop.position,NumberShape.Tetrahedron);
			resource.GetComponent<Rigidbody>().useGravity = true;
			resource.GetComponent<Rigidbody>().isKinematic = false;
			dropped = true;
		}
	}

	public void SetProperties(SimpleJSON.JSONClass N){
//		Debug.Log("N:"+N.ToString());
		if (N.GetKeys().Contains(key)){
			frac = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,(SimpleJSON.JSONClass)N[key]);
			dropped = N[key][droppedKey].AsBool;
//			WebGLComm.inst.Debug("Set prop on resource;"+N.ToString());
		}
	}

	public SimpleJSON.JSONClass GetProperties(SimpleJSON.JSONClass N){
		N[key] = new SimpleJSON.JSONClass();
		N[key] = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,(SimpleJSON.JSONClass)N[key]);
		N[key][droppedKey].AsBool = dropped;
//		WebGLComm.inst.Debug("get prop on resource;"+N.ToString());
		return N;
	}
}
