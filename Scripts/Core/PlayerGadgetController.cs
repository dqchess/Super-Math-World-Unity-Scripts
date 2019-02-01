using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// Player gadget controller. Should have all static methods so we can PlayerGadgetController.SomeMethod(someVar);
public class PlayerGadgetController : MonoBehaviour {

	public static PlayerGadgetController inst;

	public GameObject thrownNumber = null; // added by Morgan to track what number was thrown (so it can be given a lower collision/eating priority)
	float thrownTime = 0;
	public float thrownNumberTimeout = 0.4f;
	float handsDownTimeout=0;
	public GadgetThrow gadgetThrow {
		get {
			if (PlayerCostumeController.inst.initialized){ 
				return PlayerCostumeController.inst.curCharInfo.playerRightArm.GetComponent<GadgetThrow>();
			} else return null;
		}
	}

	public Transform ammoPosition {
		get {
			return gadgetThrow.transform;
		}
	}

	public void SetCurrentGadget(Gadget g){
		currentGadget = g;
	}

	public delegate void EventHandler(GameObject hitobj);
	public event EventHandler PlayerTouched;

	public List<Gadget> GetAllGadgetInInventory(){
		List<Gadget> gadgets = new List<Gadget>();
		foreach(GameObject o in Inventory.inst.GetInventoryItems()){
			//			Debug.Log("got inv item:"+o.name);
			Gadget gip = o.GetComponent<Gadget>();
			if (gip){
				gadgets.Add(gip);
			}
		}
		return gadgets;
	}

//	public List<Ammo> GetAllGadgetsAmmoInfo(){
//		// goes through all gadgets in inventory and returns any ammo, so that animals may target this ammo.
//		List<Ammo> ret = new List<Ammo>();
//		foreach(GameObject o in Inventory.inst.GetInventoryItems()){
////			Debug.Log("got inv item:"+o.name);
//			Gadget gip = o.GetComponent<Gadget>();
//			if (gip){
////				Debug.Log("it gadget;"+gip.myName);
//				ret.AddRange(gip.GetAmmoInfo());
//			}
//		}
//		return ret;
//	}


	private Gadget currentGadget = null;



	public void SetInstance(){
		inst = this;
		EquipGadget(gadgetThrow); // uhghghghgh... hurt. Why is this in setinstance. because something else calls me too early?
		
	}

	
	void OnControllerColliderHit(ControllerColliderHit hit) {
//		Debug.Log("PGC hit;"+hit.collider.name);
		GameObject hitObj = hit.collider.gameObject;
		PlayerHitObject(hitObj);
	}
	
	public virtual void PlayerHitObject(GameObject hitObj){
		if (PlayerTouched != null) {
			PlayerTouched(hitObj);
//			// commented Debug.Log ("delegate fired");
		} 
		hitObj.SendMessage("PlayerTouched",SendMessageOptions.DontRequireReceiver);
		if (hitObj == thrownNumber && thrownNumberTimeout > 0) {
//			// commented Debug.Log("hit thrown no.");
			return;
		}
			
		PickUppableObject pip = hitObj.GetComponent<PickUppableObject>();
		if (pip && pip.CanPickUp) {
			MonsterAIRevertNumber mairn = hitObj.GetComponent<MonsterAIRevertNumber>();
			if (!mairn || !mairn.bNeedsRevert){
				
				Inventory.inst.OnPlayerTouchedObject(hitObj);
			}
		}

		if (hitObj.GetComponent<SoapedNumber>()){
			PlayerNowMessage.inst.Display("You can't pick up soapy numbers!");
		}
	}	

	int thrownNumberOriginalLayer;
	public void DontCollideWithPlayerForSeconds(GameObject o, float s){
		thrownNumberOriginalLayer = o.layer;
		o.layer = LayerMask.NameToLayer("DontCollideWithPlayer");
		thrownNumberTimeout=s;
		thrownNumber = o;
	}


	void Update(){
//		if (Input.GetKeyDown(KeyCode.N)){
//			PlayerNowMessage.inst.Display("gadget throw:"+gadgetThrow.myName,transform.position);
//		}

		if (thrownNumberTimeout > 0){
			thrownNumberTimeout-=  Time.deltaTime;
			if (thrownNumberTimeout <.1f) {
				if (thrownNumber) thrownNumber.layer = thrownNumberOriginalLayer;
				thrownNumber = null; // after some time, forget what number we threw.
			}
		}
		
		if (currentGadget){

			currentGadget.GadgetUpdate();
		}
		
	}
	
	void LateUpdate(){
		if (currentGadget){
			currentGadget.GadgetLateUpdate();
		}
	}

	public Transform GetPlayerLocationForGadget(GadgetLocationOnPlayer glp){
		CharacterInfo ci = null;
		if (PlayerCostumeController.inst.currentCharacter == CharacterType.Simple_Boy) ci = PlayerCostumeController.inst.characters[0];
		if (PlayerCostumeController.inst.currentCharacter == CharacterType.Simple_Girl) ci = PlayerCostumeController.inst.characters[1];
		switch (glp){
		case GadgetLocationOnPlayer.Body: return ci.playerBody; break;
		case GadgetLocationOnPlayer.RightArm: return ci.playerRightArm; break;
		default: return null; break;
		}
	}
	
	public bool ThrowGadgetEquipped (){
		if (currentGadget){
			if (currentGadget.GetType() == typeof(GadgetThrow)){
				return true;
			}
		}
		return false;
	}
	public void EquipGadget(Gadget g){
		if (!g) return;


		if (currentGadget == g) {
			currentGadget.OnEquip(); // re equip this gadget because it might have a new item?
			return;
		}
		if (currentGadget) {
			currentGadget.OnUnequip();
		}
		currentGadget = g;
		currentGadget.OnEquip();

//		// commented Debug.Log("equipped;"+currentGadget.GetGadgetName());
	}

	public bool PreventPickup(GameObject o){
		if (o == thrownNumber && thrownNumberTimeout > 0){ return true; }
		return false;
	}

	public Gadget GetCurrentGadget(){
		if (currentGadget == null){
			currentGadget = gadgetThrow;
		}
//		Debug.Log("got cur gad:"+currentGadget.GetGadgetName());
		return currentGadget;	
	}

	public void ReEquipCurrentGadget(){
		currentGadget.OnEquip();
		Inventory.inst.UpdateBeltSelection();
//		Inventory.inst.EquipItemInSlot(Inventory.inst.beltSlots[Inventory.inst.selectedIndex]);
	}

}

