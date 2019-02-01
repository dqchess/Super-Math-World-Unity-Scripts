using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetZooka : Gadget {




	public List<Ammo> rocketAmmo = new List<Ammo>();
	public List<GameObject> rocketAmmoGraphics = new List<GameObject>();
	int maxAmmo = 3;
	//	LineRenderer lineRenderer;
	float fireTimeout=0;


	public override List<Ammo> GetAmmo(){
		return rocketAmmo;
	}
	
	public override void RemoveAmmo(Ammo a){
		rocketAmmo.Remove(a);
	}	

	public override List<Ammo> GetAmmoInfo(){
		
		return rocketAmmo;
//		List<Ammo> ret = new List<Ammo>();
//		return rocketAmmo
//		}
	}

	public override GameObject DropOneAmmo(Ammo a){
		if (rocketAmmo.Contains(a)){
			rocketAmmo.Remove(a);
			UpdateAmmoGraphics();
			return NumberManager.inst.CreateNumber(a.ammoValue,Player.inst.transform.position); // create a brand new number that something was asking for
		} else {
			return null;
		}
	}

	override public void DropFirstAmmo(){
		if (rocketAmmo.Count > 0){
			rocketAmmo.Remove(rocketAmmo[0]);
			UpdateAmmoGraphics();
		}
	}
//	public override void GiveDefaultAmmo(Fraction f){
//		for (int i=0;i<3; i++){
//			rocketAmmo.Add(new Ammo(f));
//		}
//		UpdateAmmoGraphics();
//	}

	public override bool IsValidAmmo(GameObject o){
		return o != null && o.GetComponent<NumberInfo>() != null;
	}

	public override bool CanCollectItem( GameObject o){ 
		NumberInfo ni = o.GetComponent<NumberInfo>();
		bool flag = false;
		if (!ni){
			PlayerNowMessage.inst.Display("The Zooka can only hold number orbs!");
			flag = false;
		} else if (rocketAmmo.Count >= maxAmmo){
			PlayerNowMessage.inst.Display("The Zooka can only hold 3 numbers!");
			flag = false;
		} else flag = true;
		if (flag) flag = CanCollectNumber(ni);
		if (!base.IsValidAmmo(o)) flag = false;
		return flag;

	}

	public override bool CanCollectNumber(){
		return (rocketAmmo.Count < maxAmmo);
	}

	public override bool CanCollectNumber (NumberInfo ni)
	{
		
		if (ni){
			if (ni.GetComponent<ResourceNumber>()) return false;
		}
		//		// commented Debug.Log ("ammo: "+rocketAmmo.Count);
		return rocketAmmo.Count < maxAmmo; // maxAmmo
	}
	
	
	
	override public void GadgetUpdate(){
		base.GadgetUpdate();
		fireTimeout -= Time.deltaTime;
	}

	public override void MouseButtonDown(){
		Fire();
	}
	
	
	public override void OnCollectNumber (NumberInfo ni)
	{
//		if(ErrorTestPickupNumber(ni)) { return; }
		if (!CanCollectNumber(ni)) return;
		base.OnCollectNumber(ni);
		// TODO: enqueue an ammo 'proxy' object
		rocketAmmo.Add(new Ammo(ni.fraction));
		NumberManager.inst.DestroyOrPool(ni);
		AudioManager.inst.PlayEquipNumber();
		UpdateAmmoGraphics();
	}
	
	
	override public void Fire() {
		if (fireTimeout > 0) return;
		if (Player.frozen) return;
		base.Fire ();
		fireTimeout = .1f;
//		if (rocketAmmo.Count <= 0){
//			AudioManager.inst.PlayClick2();
//			base.AttemptPickup();
//			return;
//		}

		if (rocketAmmo.Count <= 0) return;
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.timesZookaFired,1); // userData.timesZookaFired++;
		Ammo ammo = rocketAmmo[0];
		rocketAmmo.Remove(ammo);
		
		UpdateAmmoGraphics();
		
		// TODO: Make the actual rocket (once the ammo is a proxy)
		GameObject rocket = NumberManager.inst.CreateNumber(
			ammo.ammoValue, new Vector3(0,0,0),NumberShape.Sphere);
		rocket.layer = LayerMask.NameToLayer("DontCollideWithPlayer"); // don't collide with player
//		// commented Debug.Log("zooka dont collide");
		
		// PLAYER fired this rocket, find player specific code here
		Vector3 rocketStartPos = Camera.main.transform.position + Camera.main.transform.forward*10;
		
		rocket = NumberManager.inst.MakeIntoRocket(rocket);
		
//		// commented Debug.Log ("Rocket fired");
		rocket.transform.position = GetWeaponStartPos();
		CheckBulletSpawnForHoops(rocket);
		AudioManager.inst.PlayRocketLaunch(rocket.transform.position);
		
		float rocketPower = 600;
		
		rocket.GetComponent<Rigidbody>().mass = .1f; // light rockets
		rocket.GetComponent<Rigidbody>().AddForce(GetAimVector()*(rocketPower));
		rocket.GetComponent<NumberInfo>().PlayerTouched();
		
	}
	
	public override void ModifyAmmo (NumberModifier.ModifyOperation fn)
	{
		List<Ammo> toremove = new List<Ammo>();
		foreach(Ammo a in rocketAmmo) {
			a.ammoValue = fn(a.ammoValue);
			if (a.ammoValue.numerator == 0){
				
				toremove.Add(a);
			}
			//			// commented Debug.Log(a.ammoValue);
		}

		foreach(Ammo a in toremove){
			rocketAmmo.Remove(a);
		}
		UpdateAmmoGraphics();
	}
	
//	public override void ClientFiredObject (GameObject obj)
//	{
//		NumberManager.inst.MakeIntoRocket(obj);
//		AudioManager.inst.PlayRocketLaunch(obj.transform.position);
//	}
	
	public override void ClearAmmo ()
	{
		rocketAmmo.Clear();
		UpdateAmmoGraphics();
	}
	
	public override GameObject DropAmmo (bool destroyDroppedNumber = true)
	{
		rocketAmmo.Clear();
		UpdateAmmoGraphics(true);
		return null;
	}
	
	
	public override void OnEquip(){
		base.OnEquip();
//		GadgetLocationOnPlayer.RightArm;
		UpdateAmmoGraphics();// weaponObject.SendMessage("UpdateAmmoGraphics",SendMessageOptions.DontRequireReceiver); // Reset the ammo stack graphics whenever we select this weapon.
		
	}

	public override void OnUnequip(){
		base.OnUnequip();

	}
	
//	public override void ClientFire ()
//	{
//		if(rocketAmmo.Count > 0)
//		{
//			Ammo ammo = rocketAmmo[0];
//			rocketAmmo.Remove(ammo);
//		}
//		
//		UpdateAmmoGraphics();
//	}
//	
	
	
	override public void UpdateAmmoGraphics(bool redraw=true) {
		if (PlayerGadgetController.inst.GetCurrentGadget() !=this) return;
//		return;
//		if (Player.GetComponent<PlayerGadgetController>().currentWeapon != this) return; // if weapon was not selected, don't bother updating graphics
		
		int i = 0;
		foreach(GameObject g in rocketAmmoGraphics) {
			Destroy(g);
		}
		
		rocketAmmoGraphics.Clear();
		
		foreach(Ammo a in rocketAmmo) {
			GameObject obj = NumberManager.inst.CreateNumberAmmo(a.ammoValue);
			obj.AddComponent<SinGrow>();
			obj.transform.parent = gadgetGraphics.transform;
			if (i==0) {
				obj.name = "first ammo";
				obj.transform.localScale = Vector3.one*.47f; // new Vector3(0.7f, 0.7f, 0.7f);
				obj.transform.localPosition = new Vector3(0.333f, 1.3270614f, -0.46481f);
			} else if (i==1) {
				obj.transform.localScale = Vector3.one*.35f; // new Vector3(0.7f, 0.7f, 0.7f);
				obj.transform.localPosition = new Vector3(0.733f, 1.924316f, -0.109242f);
			} else if (i==2) {
				obj.transform.localScale = Vector3.one*.38f; // new Vector3(0.7f, 0.7f, 0.7f);			
				obj.transform.localPosition = new Vector3(0.902552f, 1.5976f, -0.08938886f);
			}
//			else // commented Debug.Log("oops, more than 3 rockets. Self.explode());");
			rocketAmmoGraphics.Add(obj);
			i++;
		}
	}

	public override bool IsEmpty(){
		
//		Debug.Log("Rocket ct:"+rocketAmmo.Count);
		return rocketAmmo.Count == 0;
	}
}
