using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum AudioPickupSound {
	None,
	Gadget,
	Number,
	Gem,
	Item
}

public enum ItemType {
	Regular,
	Hat
}

public class PickUppableObject : MonoBehaviour { // Inventory item
	
//	public bool handOnly = false; // don't allow to put in main inventory.
//	public bool canBeGadgetAmmo = true;
	public ItemType itemType = ItemType.Regular;
	public bool allowDuplicates = true;
	public AudioPickupSound audioPickupSound = AudioPickupSound.None;
	public bool usePlayerRotation;
	Vector3 origScale = Vector3.one;
	Quaternion origRot;
	public float heldScale = 1f;
	public Sprite inventoryIcon;
	public bool useColor = false;
	public Color inventoryIconColor;
	public bool kinematicOnThrow = false;
	public float upYoffset = 3f;
	public SlotType slotPreferenceType;
//	public int numSlots=4; // ? needed here?
	public int priority = 1;


	public string inventoryText;
	public bool putInHandOnPickup = true;
	public string messageOnPickup = "";
	public bool alwaysFacePlayerAsAmmo = false;
	virtual public void Start(){

		origRot = transform.rotation;
		origScale = transform.localScale;
	}

	public void OnPlayerTouch(){
		if (GetComponentInChildren<SinHover>()) GetComponentInChildren<SinHover>().enabled = false;
		OnPlayerAction ();
	}

	public void SetProperties(SimpleJSON.JSONClass N){
		if (N.GetKeys().Contains(JsonUtil.scaleKey)){
			origScale = JsonUtil.GetScaleFromInt(N[JsonUtil.scaleKey].AsInt);
		}
	}

	public void OnPlayerAction(string s=""){

	}

	void OnPlayerDrop () {
		if (this.GetComponent<Collider>()){
			this.GetComponent<Collider>().isTrigger = false;
		}
	}

	void OnPlayerThrow(){
		if (this.GetComponent<Collider>()){
			this.GetComponent<Collider>().isTrigger = false;
		}
		if (GetComponentInChildren<SinHover>()) GetComponentInChildren<SinHover>().enabled = true;
		if (usePlayerRotation) {
			transform.rotation = Player.inst.transform.rotation;
		} else{
			transform.rotation = origRot;
		}
		transform.localScale = origScale;
		if (kinematicOnThrow) {
			GetComponent<Rigidbody>().isKinematic = true;
			transform.position += Player.inst.transform.forward * 5f;

			transform.position = new Vector3(transform.position.x,Player.inst.transform.position.y + upYoffset,transform.position.z);
		}
	}



	public void Update(){
		
	}

	public virtual void OnInventoryCollect(bool muted = false){
		if (this.GetComponent<Collider>()){
			this.GetComponent<Collider>().isTrigger = true;
		}
		if (!LevelBuilder.inst.levelBuilderIsShowing && !Inventory.inst.isShowing){
			PlayPickupSound();

			if (messageOnPickup != "" && !muted){
				PlayerNowMessage.inst.Display(messageOnPickup,transform.position);
			}
		}
		foreach(IMyPlayerPickedUp pickup in this.GetComponents<IMyPlayerPickedUp>()){
			pickup.PlayerPickedUp();
		}

	}

	public void PlayPickupSound () {
		switch(audioPickupSound){
		case AudioPickupSound.None:
			break;
		case AudioPickupSound.Gadget:
			AudioManager.inst.PlayGadgetPickup(Player.inst.transform.position);
			break;
		case AudioPickupSound.Number: 
			AudioManager.inst.PlayEquipNumber();
			break;
		case AudioPickupSound.Gem:
			AudioManager.inst.PlayShortBeep();
			break;
		case AudioPickupSound.Item:
			AudioManager.inst.PlayItemCollect();
			break;
		default:break;

		}
	}

	public bool CanPickUp {
		get {
			return this.enabled && !this.gameObject.GetComponent<SoapedNumber>();
		}
	}

}

