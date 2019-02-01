using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetThrow : Gadget
{


//	public List<Component> linkedComponents = new List<Component>(); // { new Animal } ();
//	public IEnumerable<T> = new 

	public delegate void OnPlayerCollect(GameObject o);
	public OnPlayerCollect onPlayerCollect;

	public delegate void OnPlayerThrow(GameObject o);
	public OnPlayerCollect onPlayerThrow;

	public delegate void OnPlayerGadgetThrowEquip(GameObject o);
	public OnPlayerGadgetThrowEquip onPlayerGadgetThrowEquip;

	public delegate void OnPlayerGadgetThrowUnEquip();
	public OnPlayerGadgetThrowUnEquip onPlayerGadgetThrowUnEquip;


	public static GadgetThrow inst;
	public void SetInstance(){
		inst = this;
		PlayerGadgetController.inst.EquipGadget(this);
	}

	public override void Start(){
		base.Start();
//		// commented Debug.Log ("testVector3:"+testVector3);
//		// commented Debug.Log ("player hollding pos was;"+playerHoldingPos);
//		// commented Debug.Log ("rot was;"+playerHoldingEulerAngles);
	}

	void OnEnable(){
		StopAllCoroutines();
	}

	public SimpleJSON.JSONArray GetAmmoJson(){
		// We let other gadgets return their actual ammo so the ammo can be populated into the gadget after loading a level instance
		// Throw gadget is exceptional because its ammo is not destroyed, and is actually an inventory object which was already seraialized
		// Other gadgets do not serialize their ammo in inventory so this function is needed for them
		return new SimpleJSON.JSONArray();
	}

	float canFireTimer=0;
	public GameObject numberHeld;
	float numberHeldOriginalScale;
	float ammoTimer = 0f;
	public Ammo ammo;
	float timeSinceLastNumberThrown = 10;
	float canPickUpNumberOnClickAfterSeconds = 0.5f;
	float maxPickupRange = 20f;
	public override void GadgetUpdate () {
		base.GadgetUpdate();
		timeSinceLastNumberThrown += Time.deltaTime;
		canFireTimer-=Time.deltaTime;
		ammoTimer -= Time.deltaTime;
		if (ammoTimer < 0){
			ammoTimer = .1f;
			if (ammoGraphics){
				if (Vector3.SqrMagnitude(ammoGraphics.transform.position-playerHoldingPos) > .1f){
//					// commented Debug.Log("ammo gphx pos:"+playerHoldingPos);
					ammoGraphics.transform.localPosition = playerHoldingPos;
				}
			}
		}
		if (numberHeld == null) {
			DropAmmo();
		}
	}

	public override bool CanCollectNumber(){
		return numberHeld == null;
	}

	public override bool CanCollectItem(GameObject o){
		return numberHeld == null && o.GetComponent<Gadget>() == null; // only collect if we have no numberheld AND the object to collect was not a gadget
	}
//
	public override bool CanCollectNumber (NumberInfo ni) {
		return numberHeld == null;
	}

//	void Update(){
//		GadgetUpdate();
//	}

	public override string GetGadgetName(){
		return "Throw";
	}

	public override List<Ammo> GetAmmoInfo(){
		List<Ammo> ret = new List<Ammo>();
		if (numberHeld){
			NumberInfo ni = numberHeld.GetComponent<NumberInfo>();
			if (ni){
				ret.Add(new Ammo(ni.fraction));
			}
		}
//		Debug.Log("ret:"+ret);
		return ret;
	}

	public override GameObject DropOneAmmo(Ammo a){
		if (numberHeld){
			NumberInfo ni = numberHeld.GetComponent<NumberInfo>();
			if (ni){
				numberHeld.SetActive(true);
				numberHeld.transform.position = GadgetThrow.inst.transform.position;
				GameObject ret = numberHeld;
				numberHeld = null;
				ammo = null;
				UpdateAmmoGraphics(true);
				Debug.Log("dropped one ammo so why do I still have ammographsc");

				return ret;

			}
		}
		return null;
	}

	
	public override List<Ammo> GetAmmo(){
		List<Ammo> nh = new List<Ammo>();
		if (numberHeld) {
			NumberInfo nhi = numberHeld.GetComponent<NumberInfo>();
			if (nhi) {
				nh.Add(new Ammo(nhi.fraction));
				return nh;
			}
		} 
		return null;
	}
	
	public override void RemoveAmmo(Ammo a){
		Destroy(numberHeld);
	}
	
	
	public override void OnCollectNumber (NumberInfo ni) {
//		// commented Debug.Log("Throw colect;"+ni);
		base.OnCollectNumber(ni);
		OnCollectItem (ni.gameObject);
	}
	
	public override void OnCollectItem(GameObject obj){
//		base.OnCollectItem(obj);
//		// commented Debug.Log("here..");
		if (!CanCollectNumber()) {
//			// commented Debug.Log("could not collecT:"+obj);
			return;
		}
		obj.transform.parent = null;
//		Debug.Log("collected;"+obj.name+",par:"+obj.transform.parent);

//		PlayerGadgetController.inst.
		if (obj) obj.SendMessage("OnPlayerCollect",SendMessageOptions.DontRequireReceiver);
//		if (obj == PlayerGadgetController.inst.PreventPickup(thrownNumber)) {
////			// commented Debug.Log ("oops was thrown number");
//			return;
//		s}

		SetPlayerAnimation();
		if (!obj) return; 
		numberHeldOriginalScale = obj.transform.lossyScale.x;
//		Debug.Log("num or scl:"+numberHeldOriginalScale);
		CleanObjectOnCollect(obj);

		numberHeld = obj;
//		// commented Debug.Log("nh pick:"+numberHeld);
//		// commented Debug.Log("YES.  obj:"+obj);
		if (onPlayerCollect != null){
			onPlayerCollect(obj);
		}
//
//		foreach(Component c in linkedComponents){
//			if (obj.GetComponent<c>()){
//				obj.GetComponent<c>().OnPlayerThrow();
//			}
//		}
//

//		return;

		UpdateAmmoGraphics();


		numberHeld.SetActive(false);
	}
	
	public override void SetPlayerAnimation(){
		MascotAnimatorController.inst.HoldRightArmPalmUp(true);
	}
	
	void SetThrownNumberProperties(){
		if (!numberHeld.GetComponent<Rigidbody>()) {
			Rigidbody rb = numberHeld.AddComponent<Rigidbody>();
			rb.mass = 10;
			rb.drag = .2f;
		}
		numberHeld.GetComponent<Rigidbody>().isKinematic = false; // Release its frozen state.
		numberHeld.transform.parent=null;
		numberHeld.GetComponent<Collider>().enabled=true;
		if (numberHeldOriginalScale == 0){
			numberHeldOriginalScale = 1; // side effect of loading inv.
		}
		numberHeld.transform.localScale = Vector3.one*numberHeldOriginalScale; // Not sure about this "scale" thing. We'll see.
	}

	override public void MouseButtonDown(){
//		// commented Debug.Log("throw mouse down");
		if (Time.timeScale == 0) return;
		if (Player.frozen) return;
		if (canFireTimer>0) return;
		if (numberHeld) { 
			Fire();
		} else {
//			Debug.Log("throw try pickup");
			if (TryPickupItemIntoEmptyHand()) return;
			else TryPushNearbyButton();

		}

	}

	void TryPushNearbyButton(){
		float maxButtonPressRange = 25f;
		GenericButton bta = ButtonCloseby(maxButtonPressRange);
		if (bta) bta.PressMe();
	}

	bool TryPickupItemIntoEmptyHand(){
		if (timeSinceLastNumberThrown > canPickUpNumberOnClickAfterSeconds){
			// we clicked the mouse but weren't hodling anything. Was there a number in front of us we could pick up?
			GameObject o = AmmoCloseby(maxPickupRange);
			GameObject g = GadgetCloseby(maxPickupRange);
			GameObject toCollect = o ? o : g ? g : null;
			if (toCollect){
//				Inventory.inst.item

				Inventory.inst.OnPlayerTouchedObject(toCollect); // Inventory.inst.beltSlots[Inventory.inst.selectedIndex],toCollect); // by definition this is available right? Because numberheld null?
				return true;
			}
//			else if (g){
//				Inventory.inst.CollectItemIntoSlot(Inventory.inst.beltSlots[Inventory.inst.selectedIndex],g); // by definition this is available right? Because numberheld null?
//			}
//
		}
		return false;
	}
	
	void ApplyMotionToThrownNumber(){
		// The "physical" throwing happens here.
		float force = 18; // hardcoded throw force. TODO: Let player hold down the mouse button to build up a force meter?
		if (Input.GetAxis("Vertical") > 0) force *=1.3f; // seriously?
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) force *=1.3f; // seriously?
//		Debug.Log("force;"+force);
		float forwardOffset=2.2f;

			
//			Vector3 newPos = PlayerGadgetController.inst.GetPlayerLocationForGadget(gadgetLocationOnPlayer).position + Camera.main.transform.forward * 1.5f;
		numberHeld.transform.position=Player.inst.transform.position + Player.inst.transform.forward * 1.5f + Vector3.up * 3f;
		Vector3 dir = GetAimVector();
		float upangle = Vector3.Angle(Player.inst.transform.forward, dir); // 52 when looking ahead, 25 when // 67° when looking up // 52° when looking down // 17° when looking forward // old val?
//		if (Camera.main.transform.forward.y > 0) force = force * Mathf.Clamp((110 - upangle) / 45, .2f, 1f);
		if (Camera.main.transform.forward.y < 0) {
//			force = force * Mathf.Clamp((110 - upangle) / 45, .2f, 1f);
			float forcemod = 1f - upangle/60f; /// (upangle / 52f);
			force *= forcemod;
//			Debug.Log("upangle:"+upangle+", forcemod;"+forcemod);
		}
//		float upvect = transform.TransformVector(Camera.main.transform.forward).y; // 0.82 when facing down, -.7 when facing up, 0 when facing forwards
//		if (upvect > .2f) force /= Mathf.Pow(upvect,2);
		Vector3 realForce = force * dir;
		float maxYforce = 7f;
		realForce = Utils.ClampY(realForce,-maxYforce,maxYforce);
		numberHeld.GetComponent<Rigidbody>().velocity = Vector3.zero;
		numberHeld.GetComponent<Rigidbody>().AddForce(realForce, ForceMode.VelocityChange);


	}
//
	override public Vector3 GetAimVector() {
		if (PlayerUnderwaterController.inst.playerUnderwater) return Camera.main.transform.forward;
		else return Camera.main.transform.forward + Camera.main.transform.up * .3f;
	}


	public override void Fire ()
	{
//		// commented Debug.Log ("fire!");

		timeSinceLastNumberThrown = 0;
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.numbersThrown,1); // userData.numbersThrown++;
		if (numberHeld.GetComponent<PlayerCantDrop>()) return; // can't drop that arena key
		PlayerGadgetController.inst.DontCollideWithPlayerForSeconds(numberHeld,.3f);
		canFireTimer = .3f;
		AudioManager.inst.PlayNumberThrow();
		numberHeld.SetActive(true);
		PlayerGadgetController.inst.thrownNumber = numberHeld;
		MascotAnimatorController.inst.HoldRightArm(false);
		SetThrownNumberProperties();
//		foreach(IOnPlayerThrow pt in numberHeld.GetComponents(typeof(IOnPlayerThrow))){
//			pt.OnPlayerThrow();
//		}
		ApplyMotionToThrownNumber();
		CheckBulletSpawnForHoops(numberHeld); // huh.. ok..
//		numberHeld.SendMessage("OnPlayerThrow",SendMessageOptions.DontRequireReceiver);
		if (onPlayerThrow != null){
			onPlayerThrow(numberHeld);
		}

		Inventory.inst.PlayerThrewHeldItem(numberHeld);
//		PlayerInventory.inst.ClearSelectedBeltItem(); // This is awkward..shouldn't fail, right? Since the selected belt item had to be this one since it's active/available? Sure. 
//		Debug.Log("player throw:");
		numberHeld.SendMessage("OnPlayerThrow",SendMessageOptions.DontRequireReceiver);
		numberHeld=null;
		ammo = null;
		UpdateAmmoGraphics();
		StartCoroutine(UpdateAfterSeconds(.01f));
	}

	IEnumerator UpdateAfterSeconds(float s){
		// The only function of this wait loop is to "hide" the belt if the inventory is empty after we threw
		// (we threw the last remaining item in our inv)
		// we can't check for this during the same frame as we threw, because the item3d of the belt slot won't have been destroyed yet, 
		// if we UpdateBeltPosition before the end of the frame then although item3d was destroyed its still counted so we wait one frame before checking.
		yield return new WaitForSeconds(s);
		Inventory.inst.UpdateBeltPosition();
	}


	public override void OnEquip() {
		base.OnEquip();
//		Debug.Log("equiped..");
		if (onPlayerGadgetThrowEquip != null){
//			Debug.Log("delegate..");
			onPlayerGadgetThrowEquip(numberHeld);
		}
//		// commented Debug.Log ("?");
		if (numberHeld) {
			MascotAnimatorController.inst.HoldRightArm(true);
		} else {
//			PlayerInventory.inst.MoveNumberFromInventoryToBelt();

//			StartCoroutine(MoveNextNumberToHandAfterSeconds(1.5f));
		}

	}
	
	public override void OnUnequip(){
		base.OnUnequip();
		if (onPlayerGadgetThrowUnEquip != null){
			onPlayerGadgetThrowUnEquip();
		}
//		// commented Debug.Log ("throw unequipped..");
		numberHeld = null;
		ammo=null;
		MascotAnimatorController.inst.HoldRightArm(false);
		MascotAnimatorController.inst.HoldRightArmPalmUp(false);
	}


	public override void ModifyAmmo (NumberModifier.ModifyOperation fn)
	{
		// commented Debug.Log("mod ammo");
//		return;
		//		UpdateAmmoGraphics();
		//
		//		return; // handled by inventory now?
		if(numberHeld) {
			NumberInfo ni = numberHeld.GetComponent<NumberInfo>();
			if (!ni) return;
			Fraction fracc = fn(ni.fraction);
			// commented Debug.Log("set ni:"+fracc);
			ni.SetNumber(fracc);
			MonsterAIRevertNumber mairn = ni.GetComponent<MonsterAIRevertNumber>();
			if(mairn) {
				mairn.SetNumber(ni.fraction); 
			}
			if (fracc.numerator == 0){
				DropAmmo(true);
				NullifyAmmo();
			}
			UpdateAmmoGraphics(true);
			// commented Debug.Log("modified! ");

		}
		//		RecalcInfoText();
	}
	
	public override void ClearAmmo ()
	{
		// commented Debug.Log ("CleareD");
		Destroy(numberHeld);
		numberHeld = null;
		ammo=null;
		//		RecalcInfoText();
	}
	
	//	public override void ClientFire ()
	//	{
	//		if(numberHeld)
	//		{
	//			numberHeld.transform.parent=null;
	//			numberHeld = null;
	//			AudioManager.inst.PlayNumberThrow();
	//		}
	//	}
	
	
	
	public override GameObject DropAmmo (bool destroyDroppedNumber = true)
	{
//		// commented Debug.Log("Trying to drop:"+numberHeld);
		if(numberHeld == null) { return null; }
		if (numberHeld.GetComponent<PlayerCantDrop>()) { return null; }
		GameObject ret = numberHeld;
		NumberInfo ni = numberHeld.GetComponent<NumberInfo>();
		
		if (numberHeld.GetComponent<Rigidbody>()){
			numberHeld.GetComponent<Rigidbody>().isKinematic = false; // Release its frozen state.
		}
		if (ni) {
			if (destroyDroppedNumber) {
				Destroy (ni.gameObject);
//				ni.pickupFlag = false;
//				ni.SetInstabilityTimer(0.2f);
			}
		}
		float forwardOffset = 1.2f;
		if (Input.GetAxis("Vertical") > 0) forwardOffset += 1;
		if (Input.GetKey(KeyCode.LeftShift)) forwardOffset += 1;
		Vector3 newPos = numberHeld.transform.position + Player.inst.transform.forward*forwardOffset;
		numberHeld.transform.position = newPos;
		numberHeld.transform.parent=null;
		numberHeld.transform.localScale = Vector3.one*numberHeldOriginalScale;
//		// commented Debug.Log ("scale now:"+numberHeld.transform.localScale);
		numberHeld.GetComponent<Collider>().enabled=true;
		numberHeld.SetActive(true);
		numberHeld.name += "dropped at "+Time.time;
		Inventory.inst.PlayerThrewHeldItem(numberHeld);
		NullifyAmmo();
		return ret;
	}



	
	override public void UpdateAmmoGraphics(bool redraw=false) {
		
//		// commented Debug.Log("update:"+numberHeld);
//		if (PlayerGadgetController.inst.currentWeapon != this) return; // don't mess with graphics if this weapon isn't selected.
		base.UpdateAmmoGraphics();
		if (ammoGraphics) UnityEngine.Object.Destroy(ammoGraphics);
//		if (ammoGraphics) UnityEngine.Object.Destroy(ammoGraphics);
		//		// commented Debug.Log("modifying throw gfx");
		if (numberHeld) {
//			// commented Debug.Log ("number held:"+numberHeld);
			//			// commented Debug.Log ("number held:"+numberHeld.GetComponent<NumberInfo>().fraction);

			//			// commented Debug.Log ("throw static");
			NumberInfo ni = numberHeld.GetComponent<NumberInfo>();
			if (ni){
				// for numbers we do want to keep certain scripts on them as ammo, such as alwaysfaceplayer
				ammoGraphics = NumberManager.inst.CreateNumberAmmo(ni.fraction,ni.myShape);
				// Did the oriignal number info have soap bubbles? (bad place for this to be? lol)
				if (ni.IsSoapable() && ni.soapFx.activeSelf){
					
					GameObject soapCopy = (GameObject)GameObject.Instantiate(ni.soapFx,ammoGraphics.transform.position,ammoGraphics.transform.rotation);
					soapCopy.transform.parent = ammoGraphics.transform;
					ParticleSystem ps = soapCopy.GetComponentInChildren<ParticleSystem>();
					ParticleSystem.ShapeModule pss = ps.shape;
					pss.radius *= 0.5f;
//					ps.sh
				}
			} else {
				// otherwise, do not duplicate/strip the ammo, intead reconstruct it from scratch.
				ammoGraphics = Utils.ConstructFakeCopyOfAmmo(numberHeld);

				ammoGraphics.transform.rotation = Quaternion.identity;
			}
//			(GameObject)UnityEngine.Object.Instantiate(numberHeld);

//			if (ueo) ueo.isSerializeableForSceneInstance = false;

//			ammoGraphics.name = "ammo graphics";
			CleanComponentsForAmmo(ammoGraphics);
			
			PickUppableObject pip = numberHeld.GetComponent<PickUppableObject>();
			ammoGraphics.transform.parent =  PlayerGadgetController.inst.GetPlayerLocationForGadget(gadgetLocationOnPlayer);
			if (pip){
				ammoGraphics.transform.localScale *= pip.heldScale;
			} else {
				ammoGraphics.transform.localScale = Vector3.one * 0.5f;// * 0.5f; // * Player.inst.transform.localScale.x
			}
			Transform m1 = ammoGraphics.transform.Find("mesh");
			if (m1) {
				m1.localScale = Vector3.one * 0.5f;
				//				// commented Debug.Log ("set to 0.5");
			}
			Transform m2 = ammoGraphics.transform.Find("mesh2");
			if (m2) m2.localScale = playerHoldingScale * Vector3.one;
			
			ammoGraphics.transform.localPosition = playerHoldingPos;// Vector3.zero;// new Vector3(.3f,.078f,-.1f);
			if (pip.alwaysFacePlayerAsAmmo){
				ammoGraphics.AddComponent<AlwaysFacePlayer>();
			}
//			ammoGraphics.transform.LookAt(Player.inst.transform);
//			ammoGraphics.transform.rotation = Utils.FlattenRotation(ammoGraphics.transform.rotation);
			ammoGraphics.SetActive(true);
			ni = ammoGraphics.GetComponent<NumberInfo>();
			if (ni){
				ni.InitSinGrowAttributes(.100f);
				ni.SinGrowOnce();
			}
//			// commented Debug.Log("ammogrpahics:"+ammoGraphics.transform.localPosition+", playerhod:"+playerHoldingPos);
//			// commented Debug.Log ("setting ammograph localpos to:"+ammoGraphics.transform.localPosition+", but player hollding pos was;"+playerHoldingPos);
//			// commented Debug.Log ("moved to:"+ammoGraphics.transform.localPosition);
//			ammoGraphics.transform.localRotation = Quaternion.identity;
//			ammoGraphics.SendMessage("OnPlayerEquip",SendMessageOptions.DontRequireReceiver);
		} else {
			numberHeld = null;

			MascotAnimatorController.inst.HoldRightArm(false);
		} 
		
	}

	public override void CheckAmmoNull (GameObject num,Fraction result){
		// Sigh.. due to modifying numbers in inventory and elsewhere we sometimes get a null number because ti was zeroed during a modification. But throwgadget doesnt' KNOW that the 
		// number was modified -- or rather, that it was destroyed -- because destroyed won't happen until the END of the frame.
		// This, destroy(object); // commented Debug.Log(object); will still return the object, so we have to do hacky things like this to make sure the number should still exist this frame
//		// commented Debug.Log("check nul. num:"+num.GetInstanceID()+",numhe:"+numberHeld.GetInstanceID()+", result;"+result);
		if (num == null) return;
		if ( numberHeld == null) return;
		if (num.GetInstanceID() == numberHeld.GetInstanceID()){
	
			if (result.numerator ==0){
	
				NullifyAmmo();
			}
//			// commented Debug.Log("YeS:"+num);
//			// commented Debug.Log(",frac;"+num.GetComponent<NumberInfo>().fraction);
		}

	}

	public override void NullifyAmmo(){
//		// commented Debug.Log("ammo null.");
		numberHeld = null;
//		// commented Debug.Log("numberheld:"+numberHeld);

		UpdateAmmoGraphics();
		MascotAnimatorController.inst.HoldRightArm(false);
	}

	public bool ObjectIsEquipped(GameObject o){
		if (numberHeld) Debug.Log("comparing "+o.name+" with "+numberHeld.name);
		else Debug.Log("no num held.");
		// Is this object currently held by GadgetThrow?
		// Useful for objects (like seeds) that we want to affect the world while we are holding them in our hand
		return (PlayerGadgetController.inst.ThrowGadgetEquipped() && numberHeld != null && numberHeld == o);

	}

}

