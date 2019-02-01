using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatPickup : UEO_SimpleObject {

	public static string hatIndexKey = "hat_index_key";
	public static string colorIndexKey = "hat_color_key";
	public GameObject[] hats;
	public Material[] colors;
	int hatIndex = 0;
	int colorIndex = 0;


	public void CycleHat(int i){
		hatIndex += i;
		if (hatIndex < 0) hatIndex = hats.Length - 1;
		else hatIndex %= hats.Length;
		SetHatActive();
	}


	public void CycleColor(int i){
		colorIndex += i;
		if (colorIndex < 0) colorIndex = colors.Length - 1;
		else colorIndex %= colors.Length;
		SetHatActive();
	}

	public GameObject GetCurrentHat(){
		return hats[hatIndex];
	}

	public override GameObject[] GetUIElementsToShow ()
	{
		List<GameObject> elsToShow = new List<GameObject>();
		elsToShow.AddRange(base.GetUIElementsToShow());
		elsToShow.Add(LevelBuilder.inst.POCMhatButton);
		return elsToShow.ToArray();
	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N[hatIndexKey].AsInt = hatIndex;
		N[colorIndexKey].AsInt = colorIndex;
		return N;
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		hatIndex = N[hatIndexKey].AsInt;
		colorIndex = N[colorIndexKey].AsInt;
		SetHatActive();
	}

	void SetHatActive(){
		foreach(GameObject o in hats){
			o.SetActive(false);
		}
		hats[hatIndex].SetActive(true);
//		hats[hatIndex].GetComponent<Renderer>().material = colors[colorIndex];
	}


}
