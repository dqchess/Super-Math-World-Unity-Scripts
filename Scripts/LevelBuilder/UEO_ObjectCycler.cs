using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UEO_ObjectCycler : MonoBehaviour {

	public static string objectIndexKey = "objectIndex";
	public int objectIndex;
	public GameObject[] objectsToCycle; //hmm problematic?


	public SimpleJSON.JSONClass GetProperties(SimpleJSON.JSONClass N){
		N[objectIndexKey].AsInt = objectIndex;
		
		return N;
	}
	
	public void SetProperties(SimpleJSON.JSONClass N){
		objectIndex = N[objectIndexKey].AsInt;;
		UpdateObject();
	}



	public void CycleObject(int i){
		objectIndex += i;
		if (objectIndex < 0) objectIndex = objectsToCycle.Length - 1;
		else objectIndex %= objectsToCycle.Length;
		UpdateObject();
	}

	void UpdateObject(){
		foreach(GameObject o in objectsToCycle){
			o.SetActive(false);
		}
		objectsToCycle[objectIndex].SetActive(true);
	}


}
