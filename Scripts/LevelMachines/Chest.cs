using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Chest : UserEditableObject {

	#region UserEdtiable
	public static string chestKey;
	public static string chestContentsKey;
	// How to do "chest contents"?
	// Can we just put something dumb like points or cat gifs?
	// Maybe accumulate points to use more level builder items!

	public override SimpleJSON.JSONClass GetProperties() {
		SimpleJSON.JSONClass N = base.GetProperties();
		N[chestKey] = new SimpleJSON.JSONClass();
		N[chestKey][chestContentsKey] = "Chest contents (unfinished)";
		return N;
	}
	public override void SetProperties(SimpleJSON.JSONClass N){
		// commented Debug.Log("chest props unfinished");	
		base.SetProperties(N);
	}
	public override GameObject[] GetUIElementsToShow() {
		return null; // should show a "change chest contents" button?
	}



	#endregion
	public Transform lid;
	public Transform lidOpen;
	public Texture treasureIcon;
	public string treasureText = "You found a treasure!";
	public Texture[] extraTextures;
	public Texture gemTexture;
	public GameObject heart;

	int gemAmount = 5;

//	public Texture cowboyHatTexture;

	bool opening = false;

	void Start(){

		if (!PlayerPrefs.HasKey("ChestOpenedCount_Arena")){
			PlayerPrefs.SetInt ("ChestOPenedCount_Arena",0);
		}
		if (!PlayerPrefs.HasKey("ChestOpenedCount")){
			PlayerPrefs.SetInt ("ChestOpenedCount",0);
		}
	}

	bool touched = false;
	void OnPlayerTouch(){
		if (!touched){
			Player.inst.FreezePlayer();
//			Player.inst.FreezePlayerFreeLook();
			touched = true;
			AudioManager.inst.PlayChestOpen();
			opening = true;
		}
	}


	void GiveGift(GameObject o, string m){
		Vector3 giftPos = transform.position+Vector3.up*7;
		GameObject gift = (GameObject)Instantiate(o,giftPos,Quaternion.identity);
		gift.GetComponent<Collider>().isTrigger = true;
		treasureText = m;
	}
	
	bool HasGadgetOfType(string g){
//		List<GameObject> gadgets = GlobalVars.inst.inv.GetAllItemsOfType<Gadget>();
//		foreach (GameObject gadget in gadgets){
//			if (gadget.GetComponent<Gadget>().GetGadgetName == g){
//				return true;
//			}
//		}
//		return false;
		return false;
	}

	float openTimer=0;
	void Update(){
		if (opening){
			openTimer+=Time.deltaTime;
			if (openTimer < 1.6f){
				float openSpeed = 2;
				lid.transform.rotation = Quaternion.Slerp (lid.transform.rotation,lidOpen.transform.rotation,Time.deltaTime*openSpeed);
			} else {
				Vector3 prizePos = transform.position + Vector3.up * 5;
				AudioManager.inst.PlaySparkle1();
				int rand = Random.Range (0,100);
				if (rand < 60) {
					treasureIcon = gemTexture;
					gemAmount = Random.Range (0,150);
					treasureText = "You found "+gemAmount+" gems!";
//					SMW_GF.inst.GemDrop(prizePos, gemAmount);
				} else if (rand < 80){
					GameObject h = (GameObject)Instantiate(heart,prizePos,Quaternion.identity);
					treasureText = "You found a heart!";
				} else if (rand < 85){
//					GameObject w = (GameObject)Instantiate(FindObjectOfType<Inventory>().wand,prizePos,Quaternion.identity);
//					treasureText = "The magic wand!";
				} else if (rand < 96){
					GameObject h = (GameObject)Instantiate(heart,prizePos,Quaternion.identity);
					treasureText = "You found a heart!";
				} else if (rand < 97){
					treasureText = "This chest is empty.";
				} else if (rand < 98){
//					GameObject h = (GameObject)Instantiate(heart,prizePos,Quaternion.identity);
//					treasureText = "You found a magic wand!";
				} else {
//					GameObject b = (GameObject)Instantiate(FindObjectOfType<Inventory>().boomerang,prizePos,Quaternion.identity);
//					treasureText = "You found a square root boomerang!";
				}
				FindObjectOfType<HelpTextBox>().ShowBig(treasureText,treasureIcon);
				opening = false;
				Destroy (this);
			}
		}
	}
}
