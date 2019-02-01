using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeController : MonoBehaviour {


	public static string headColorKey = "headColor";
	public static string hairStyleKey = "hairStyle";
	public static string bodyStyleKey = "bodyStyle";
	public static string beardStyleKey = "beardStyle";
	public static string hairColorKey = "hairColor";
	public static string bodyColorKey = "bodyColor";
	public static string beardColorKey = "beardColor";


	public Transform playerHeadObjects;
	public static PlayerCostumeController inst;
	[SerializeField] public CharacterInfo[] characters;


	public CharacterInfo curCharInfo;
	public Renderer playerHeadGraphics;

	public Transform[] hair;
	public Transform[] beards;
	public Material[] allMaterials;


	public void SetProperties(SimpleJSON.JSONClass N){
//		Debug.Log("setporp:"+N.ToString());
		// Deactivate previous character
		DeactivateAllCharacterObjects();

		characters[N[bodyStyleKey].AsInt].root.gameObject.SetActive(true);

		// Set current character
		curCharInfo = characters[N[bodyStyleKey].AsInt];


		// set hair style
		hair[N[hairStyleKey].AsInt].gameObject.SetActive(true);

		// set beard style
		beards[N[beardStyleKey].AsInt].gameObject.SetActive(true);

		// set beard color
//		Debug.Log("beard color indes;"+N[beardColorKey].AsInt);
//		Debug.Log("beard index:"+N[beardStyleKey].AsInt);
		if(beards[N[beardStyleKey].AsInt].gameObject.GetComponent<Renderer>()) beards[N[beardStyleKey].AsInt].gameObject.GetComponent<Renderer>().sharedMaterial = allMaterials[N[beardColorKey].AsInt];


		// Move head object to current active player
		playerHeadObjects.transform.parent = curCharInfo.playerHead;
		playerHeadObjects.transform.localPosition = Vector3.zero;
		playerHeadObjects.transform.localRotation = Quaternion.identity;

		// set head color
		Material[] mats = playerHeadGraphics.sharedMaterials;
		for (int i=0;i<mats.Length;i++){
//			Debug.Log("head color key:"+ N[headColorKey].AsInt);
			mats[i] = allMaterials[N[headColorKey].AsInt];
		}
		playerHeadGraphics.sharedMaterials = mats;

		// set body color
		curCharInfo.playerBodyGraphics.sharedMaterial = allMaterials[N[bodyColorKey].AsInt];

		// Set hair color
		if (hair[GetHairStyleIndex()].GetComponent<Renderer>()){
			mats = hair[GetHairStyleIndex()].GetComponent<Renderer>().sharedMaterials;
			for (int i=0;i<mats.Length;i++){
				mats[i] = allMaterials[N[hairColorKey].AsInt];
			}
			hair[GetHairStyleIndex()].GetComponent<Renderer>().sharedMaterials = mats;
		}

	}


	void DeactivateAllCharacterObjects(){
		for (int i=0;i<characters.Length;i++){
			characters[i].root.gameObject.SetActive(false);
		}
		foreach(Transform t in hair){
			t.gameObject.SetActive(false);
		}
		foreach(Transform t in beards){
			t.gameObject.SetActive(false);
		}
	}



	public SimpleJSON.JSONClass GetProperties(SimpleJSON.JSONClass N = null){
//		Debug.Log("gotprop");
		if (N == null) N = new SimpleJSON.JSONClass();
		N[bodyStyleKey].AsInt = GetBodyStyleIndex();
		N[hairStyleKey].AsInt = GetHairStyleIndex();
		N[beardStyleKey].AsInt = GetBeardStyleIndex();
		N[hairColorKey].AsInt = GetHairColorIndex();
		N[bodyColorKey].AsInt = GetBodyColorIndex();
		N[beardColorKey].AsInt = GetBeardColorIndex();
		N[headColorKey].AsInt = GetHeadColorIndex();
		return N;
	}

	public int GetHeadColorIndex(){
		
		for(int i=0;i<allMaterials.Length;i++){
			if (playerHeadGraphics.sharedMaterial == allMaterials[i]){
				return i;
			}
		}
		return 0;
	}

	public int GetBeardColorIndex(){
		for(int i=0;i<allMaterials.Length;i++){
			if (beards[GetBeardStyleIndex()].GetComponent<Renderer>() && beards[GetBeardStyleIndex()].GetComponent<Renderer>().sharedMaterial == allMaterials[i]){
				return i;
			}
		}
		return 0;
	}

	public int GetBodyColorIndex(){
		for(int i=0;i<allMaterials.Length;i++){
			if (characters[GetBodyStyleIndex()].playerBodyGraphics.sharedMaterial == allMaterials[i]){
				return i;
			}
		}
		return 0;
	}

	public int GetHairColorIndex(){
		for(int i=0;i<allMaterials.Length;i++){
			if (hair[GetHairStyleIndex()].GetComponent<Renderer>() && hair[GetHairStyleIndex()].GetComponent<Renderer>().sharedMaterial == allMaterials[i]){
				return i;
			}
		}
		return 0;
	}

	public int GetBeardStyleIndex(){
		return Utils.GetActiveIndexFromTransform(beards);
	}


	public int GetHairStyleIndex(){
		return Utils.GetActiveIndexFromTransform(hair);
	}

	public int GetBodyStyleIndex(){
		if (characters[0].root.gameObject.activeSelf) return 0;
		if (characters[1].root.gameObject.activeSelf) return 1;
		return 0;
	}
}

