using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetBow : Gadget {

	public GameObject arrowPrefab;


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
	public override void GadgetUpdate () {
		base.GadgetUpdate();
		canFireTimer-=Time.deltaTime;

//		if (numberHeld == null) {
//			DropAmmo();
//		}
	}

	public override bool CanCollectNumber(){
		return numberHeld == null;
	}

	public override bool CanCollectItem( GameObject o){ 
		return (!numberHeld && IsValidAmmo(o) && base.IsValidAmmo(o));
//		NumberInfo ni = o.GetComponent<NumberInfo>();
//		if (numberHeld){
//			DropAmmo();
//			return true;
//		} else return false;

	}


	public override bool IsValidAmmo(GameObject o) {
		// an alternate different system compared to CanCollectNumber, probably more apt
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (!ni) return false;
		if (o.GetComponent<ResourceNumber>()) return false;

		bool flag = (ni.fraction.numerator < 10 && ni.fraction.numerator > -10);
		if (!flag) {
			PlayerNowMessage.inst.Display("That number is too big for the Bow gadget.",transform.position);
			return flag;
		}
		flag = ni.fraction.denominator == 1;
		if (!flag) PlayerNowMessage.inst.Display("The Bow gadget only collects integers.");
		return flag;

	}

	//
	public override bool CanCollectNumber (NumberInfo ni) {
		if (numberHeld) return false;
		if (!ni) return false;
		if (!IsValidAmmo(ni.gameObject)){
//			PlayerNowMessage.inst.Display("That number is too big for this gadget.",transform.position);
			return false;
		}
		return true;
	}



	public override string GetGadgetName(){
		return "Wave";
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


	public override List<Ammo> GetAmmo(){
		List<Ammo> nh = new List<Ammo>();
		if (numberHeld) {
			NumberInfo nhi = numberHeld.GetComponent<NumberInfo>();
			if (nhi) {
				nh.Add(new Ammo(nhi.fraction));
//				return nh;
			}
		} 
		return nh;
	}

	public override void RemoveAmmo(Ammo a){
		Destroy(numberHeld);
	}


	public override void OnCollectNumber (NumberInfo ni) {

//		Debug.Log("wave colect item:"+ni.name);
//		numberHeld = (GameObject)Instantiate(arrowPrefab,numberHeldT.position,numberHeldT.rotation);
		numberHeld = (GameObject)Instantiate(arrowPrefab,gadgetGraphics.transform.position,gadgetGraphics.transform.rotation);

		numberHeld.GetComponent<Rigidbody>().isKinematic = false;
		numberHeld.GetComponent<NumberInfo>().SetNumber(ni.fraction);
		numberHeld.SetActive(false);
		NumberManager.inst.DestroyOrPool(ni);

		AudioManager.inst.PlayEquipNumber();
		UpdateAmmoGraphics();
		base.OnCollectNumber(ni);
	}



	override public void MouseButtonDown(){
		//		// commented Debug.Log("throw mouse down");
		Fire();
	}

	void ApplyMotionToThrownNumber(){
		// The "physical" throwing happens here.
		float force = 44; // arrow force
		if (Input.GetKey (KeyCode.W)) force *=1.5f; // seriously?
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) force *=1.7f; // seriously?
		float forwardOffset=2.2f;


		//			Vector3 newPos = PlayerGadgetController.inst.GetPlayerLocationForGadget(gadgetLocationOnPlayer).position + Camera.main.transform.forward * 1.5f;
		numberHeld.transform.position=Player.inst.transform.position + Player.inst.transform.forward * 1.5f + Vector3.up * 3f;
		Vector3 dir = GetAimVector();
		float upangle = Vector3.Angle(Player.inst.transform.forward, dir); // 52 when looking ahead, 25 when // 67° when looking up // 52° when looking down // 17° when looking forward // old val?
		force = force * Mathf.Clamp((110 - upangle) / 45, .2f, 1f);
		//		float upvect = transform.TransformVector(Camera.main.transform.forward).y; // 0.82 when facing down, -.7 when facing up, 0 when facing forwards
		//		if (upvect > .2f) force /= Mathf.Pow(upvect,2);
		numberHeld.GetComponent<Rigidbody>().isKinematic = false;
		numberHeld.GetComponent<Rigidbody>().velocity = Vector3.zero;
		numberHeld.GetComponent<Rigidbody>().AddForce(dir*force, ForceMode.VelocityChange);

	}
	//
	override public Vector3 GetAimVector() {
		if (PlayerUnderwaterController.inst.playerUnderwater) return Camera.main.transform.forward;
		else return Camera.main.transform.forward + Camera.main.transform.up*.2f;
	}


	public override void Fire ()
	{
		//		// commented Debug.Log ("fire!");
		if (Time.timeScale == 0) return;
		if (Player.frozen) return;
		if (canFireTimer>0) return;
		if (!numberHeld) { 
			return; 
		}
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.numbersThrown,1); // userData.numbersThrown++;
		canFireTimer = .3f;
		numberHeld.SetActive(true);
		AudioManager.inst.PlayArrowFire(Player.inst.transform.position);
		ApplyMotionToThrownNumber();
		MascotAnimatorController.inst.FireBow();
		CheckBulletSpawnForHoops(numberHeld); // huh.. ok..


		//		PlayerInventory.inst.ClearSelectedBeltItem(); // This is awkward..shouldn't fail, right? Since the selected belt item had to be this one since it's active/available? Sure. 
		//		Debug.Log("player throw:");
		numberHeld=null;
		UpdateAmmoGraphics();
	}



	public override void OnEquip() {
//		Debug.Log("bow eq");
		base.OnEquip();
		MascotAnimatorController.inst.HoldRightArm(true);


	}

	public override void OnUnequip(){
		base.OnUnequip();
//		Debug.Log ("wave unequipped..");
//		numberHeld = null;

		MascotAnimatorController.inst.HoldRightArm(false);
	}


	public override void ModifyAmmo (NumberModifier.ModifyOperation fn)
	{
		
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
//		Debug.Log("wave ammoclear");
		numberHeld = null;
		//		RecalcInfoText();
	}





	public override GameObject DropAmmo (bool destroyDroppedNumber = true)
	{
		if (destroyDroppedNumber) Destroy(numberHeld);
//		Debug.Log("wave drop");
		NullifyAmmo();
		return numberHeld;
	}



	override public void UpdateAmmoGraphics(bool redraw=false) {

		//		// commented Debug.Log("update:"+numberHeld);
		//		if (PlayerGadgetController.inst.currentWeapon != this) return; // don't mess with graphics if this weapon isn't selected.
		base.UpdateAmmoGraphics();
		if (ammoGraphics) UnityEngine.GameObject.Destroy(ammoGraphics);
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
			} else {
//				Debug.Log("no ni");
			}

//			Debug.Log("ammograph:"+ammoGraphics);
			CleanComponentsForAmmo(ammoGraphics);

			PickUppableObject pip = numberHeld.GetComponent<PickUppableObject>();
			ammoGraphics.transform.parent =  gadgetGraphics.transform;

			ammoGraphics.transform.localScale = Vector3.one * 1.6f;
			
			Transform m1 = ammoGraphics.transform.Find("mesh");
			if (m1) {
				m1.localScale = Vector3.one * 0.5f;
				//				// commented Debug.Log ("set to 0.5");
			}
			Transform m2 = ammoGraphics.transform.Find("mesh2");
			if (m2) m2.localScale = playerHoldingScale * Vector3.one;

			Quaternion rot = new Quaternion();
			rot.eulerAngles = new Vector3(30,126,12);
			ammoGraphics.transform.localRotation = rot;
			ammoGraphics.transform.localPosition = new Vector3(0.37f,0.09f,-0.42f); // = gadgetGraphics.transform.position + Vector3.up * 5;// new Vector3(.3f,.078f,-.1f);

			ammoGraphics.SetActive(true);
			ni = ammoGraphics.GetComponent<NumberInfo>();
			if (ni){
				ni.InitSinGrowAttributes(.100f);
				ni.SinGrowOnce();
			}

		} else {
//			Debug.Log("update ammo num null");
			numberHeld = null;
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
//				Debug.Log("ammonull");
				NullifyAmmo();
			}
			//			// commented Debug.Log("YeS:"+num);
			//			// commented Debug.Log(",frac;"+num.GetComponent<NumberInfo>().fraction);
		}

	}

	public override void NullifyAmmo(){
//		Debug.Log("ammo nulled.");
		numberHeld = null;
		//		// commented Debug.Log("numberheld:"+numberHeld);

		UpdateAmmoGraphics();
	}

	public bool ObjectIsEquipped(GameObject o){
//		if (numberHeld) Debug.Log("comparing "+o.name+" with "+numberHeld.name);
//		else Debug.Log("no num held.");
		// Is this object currently held by GadgetThrow?
		// Useful for objects (like seeds) that we want to affect the world while we are holding them in our hand
		return (PlayerGadgetController.inst.ThrowGadgetEquipped() && numberHeld != null && numberHeld == o);

	}
	public override bool IsEmpty(){
		return numberHeld == null;
	}
}
