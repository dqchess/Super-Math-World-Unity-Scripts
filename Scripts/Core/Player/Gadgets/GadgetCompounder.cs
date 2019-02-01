using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetCompounder : Gadget {


	// Todo:
	// This gadget uses only one number as its ammo 
	// it can be fired as many times as it has "charge"
	// and has a default "charge" of 1
	// charge can be increasd by dropping blue energy resource on this gadget in your inventory. It will then show up as part of the gadget adn will tick down by integers with every "fire".
	// collectEnergyNumber()
	// Fire() checks energynumber.val and -=1 if <0 destroy it and empty ammo


	public Transform energyHoldPos;
	float energyNumScale = 0.3665927f;
	public Ammo compounderAmmo;
	public GameObject compounderAmmoGraphics;

	float fireTimeout=0;


	public override List<Ammo> GetAmmo(){
		List<Ammo>  ret = new List<Ammo>();
		ret.Add(compounderAmmo);
		return ret;
	}
	
	public override void RemoveAmmo(Ammo a){
		compounderAmmo = null;
	}	
	

	public override bool CanCollectNumber (NumberInfo ni)
	{
		
		if (ni){
			if (ni.GetComponent<ResourceNumber>()) return false;
		}
		//		// commented Debug.Log ("ammo: "+rocketAmmo.Count);
		return compounderAmmo == null;
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
		compounderAmmo = new Ammo(ni.fraction);
		NumberManager.inst.DestroyOrPool(ni);
		UpdateAmmoGraphics();
	}
	
	
	override public void Fire() {
		if (fireTimeout > 0) return;
		if (Player.frozen) return;
		base.Fire ();
		fireTimeout = .1f;

	}
	
	public override void ModifyAmmo (NumberModifier.ModifyOperation fn)
	{
		
		compounderAmmo.ammoValue = fn(compounderAmmo.ammoValue);
		if (compounderAmmo.ammoValue.numerator == 0) {
			ClearAmmo();

		} else {
			UpdateAmmoGraphics();
		}
	}

	public override void ClearAmmo ()
	{
		
		compounderAmmo = null;
		UpdateAmmoGraphics();
	}
	
	public override GameObject DropAmmo (bool destroyDroppedNumber = true)
	{
		ClearAmmo();
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
		if (compounderAmmoGraphics) {
			Destroy(compounderAmmoGraphics);
			compounderAmmoGraphics = null;
		}

		GameObject obj = NumberManager.inst.CreateNumberAmmo(compounderAmmo.ammoValue);
		obj.AddComponent<SinGrow>();
		obj.transform.parent = gadgetGraphics.transform;
			
		obj.name = "first ammo";
		obj.transform.localScale = Vector3.one*.47f; // new Vector3(0.7f, 0.7f, 0.7f);
		obj.transform.localPosition = new Vector3(0.333f, 1.3270614f, -0.46481f);
		compounderAmmoGraphics = obj;
	}
}
