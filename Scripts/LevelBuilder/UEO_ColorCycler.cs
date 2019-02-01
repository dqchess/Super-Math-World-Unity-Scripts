using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UEO_ColorCycler : MonoBehaviour {

	public enum MaterialGroup {
		Colorful,
		Natural,
		Sponged
	}
	public static string spongedKey = "Sponged"; // MUST be identical to enum MaterialGroup options
	public static string naturalKey = "Natural";
	public static string colorfulKey = "Colorful";

	public MaterialGroup materialGroup = MaterialGroup.Colorful;
	public static string colorManipulatorKey = "colorManipulatorString";
	public static string colorGroupKey = "colorGroupKey";
	public int colorIndex = 0;

//	public Material[] localMaterials;
	// Use this for initialization
	public bool seekChildren = true;


	public SimpleJSON.JSONClass GetProperties(SimpleJSON.JSONClass N){
		N[colorManipulatorKey].AsInt = colorIndex;
		N[colorGroupKey] = materialGroup.ToString(); // string value format from enum.tostring is "Natural", as opposed to "MaterialGroup.Natural"
//		Debug.Log("Getprop:"+N.ToString());
		return N;
	}

	public void SetProperties(SimpleJSON.JSONClass N){
		if (N[colorGroupKey].Value == colorfulKey) materialGroup = MaterialGroup.Colorful;
		else if (N[colorGroupKey].Value == naturalKey) materialGroup = MaterialGroup.Natural;
		else if (N[colorGroupKey].Value == spongedKey) materialGroup = MaterialGroup.Sponged;
		colorIndex = N[colorManipulatorKey].AsInt;
//		Debug.Log("Setprop:"+N.ToString());
		UpdateColor();
	}




	void UpdateColor(){
		Material newMat;
		if (materialGroup == MaterialGroup.Natural){
			if (colorIndex >= EffectsManager.inst.naturalMaterials.Length) colorIndex = 0;
			newMat = EffectsManager.inst.naturalMaterials[colorIndex];
		} else if (materialGroup == MaterialGroup.Colorful){
			if (colorIndex >= EffectsManager.inst.coloredMaterials.Length) colorIndex = 0;
			newMat = EffectsManager.inst.coloredMaterials[colorIndex];
		} else if (materialGroup == MaterialGroup.Sponged){
			if (colorIndex >= EffectsManager.inst.spongeMaterials.Length) colorIndex = 0;
			newMat = EffectsManager.inst.spongeMaterials[colorIndex];
		} else {
			Debug.Log("<color=#f00>No material</color> for index;"+colorIndex);

			newMat = null;
		}
		if (seekChildren){
			foreach(Renderer r in GetComponentsInChildren<Renderer>(true)){
				r.material = newMat; 
			}
		} else {
			GetComponent<Renderer>().material = newMat;
		}
	}
}
