using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class GadgetMultiblaster : Gadget, IMyPlayerPickedUp, IMyPlayerDropped {

//	public static string key = "multiblasterKey";
	// upoffset 		return 5f;
	Fraction defaultAmmo = new Fraction(1,1);

	#region UserEdtiable
	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		defaultAmmo = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
	}
	public override SimpleJSON.JSONClass GetProperties(SimpleJSON.JSONClass N){
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,defaultAmmo,N);
		return N;
	}
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject>  els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMFractionButton);
		return els.ToArray();
	}


	#endregion

	public List<Ammo> bulletAmmo = new List<Ammo>();
	public List<GameObject> bulletAmmoGraphics = new List<GameObject>();
//	public Vector3 playerHoldingEulerAngles;
//	public Vector3 playerHoldingPos;
//	public float playerHoldingScale;
	bool firing=false;
	float bulletScale = 1f;
	int maxAmmo = 10;
	float bulletInterval = .1318f;

	public void PlayerDropped(){
		defaultAmmo = new Fraction(0,1);
//		Debug.Log("player dropped");
	}
	public void PlayerPickedUp(){
		if (defaultAmmo.numerator != 0){
			OnCollectItem(NumberManager.inst.CreateNumber(defaultAmmo,Vector3.zero));
		}
//		Debug.Log("player up!");
	}

	override public void GadgetUpdate(){
		base.GadgetUpdate();
		coolDown -= Time.deltaTime;
		if (coolDown < 0) coolDown = 0;
		base.GadgetUpdate();
		if (firing){
			if (coolDown > 0){
				firing=false;
			}
			coolDown = bulletInterval;
		}
		if (holdingMouse){
			Fire();
		}
	}
	bool holdingMouse = false;
	override public void MouseButtonDown(){
		holdingMouse = true;
	}

	override public void MouseButtonUp(){
		holdingMouse = false;
	}



	override public void OnMouseHoldLeft(){
//		//		// commented Debug.Log("player froezen: "+PlayerFrozen+" at "+Time.time);
//		if (PlayerFrozen) return;
//		
//		FireM();
//		
	}

	int clipSize = 10;
	public override void OnCollectNumber(NumberInfo ni){
		base.OnCollectNumber(ni);
		OnCollectNumber(ni,clipSize);
	}

//	public override bool CanDropNumberOntoGadgetFromInventory(){
//		return true;
//		// always allow this gadget to be populated from dropping a number onto it in inventory.
//	}

	public override bool CanCollectItem( GameObject o){ 
		bool flag = false;
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni){
			if (bulletAmmo.Count == 0){
				flag = true;
			} else if (bulletAmmo[0].ammoValue == ni.fraction && bulletAmmo.Count != GetMaxAmmo()){ // ammo was the same value as what we already have and wasn't full.
				flag = true; // full, but same number, so re-fill it.
			} else {
				flag = false; // full of a different number, don't collect it.
			}
			
			flag = true;
		} else flag = false;
		if (flag) {
			flag = CanCollectNumber(ni);
		}
		if (!base.IsValidAmmo(o)) flag = false;
		return flag;

	}

	public override void OnCollectNumber (NumberInfo ni, int clipSize)
	{
		if (!CanCollectNumber(ni)) return;
		ClearAmmo();
		AudioManager.inst.PlayEquipNumber();
		for(int i = 0; i < clipSize; i++) {
			bulletAmmo.Add(new Ammo(ni.fraction));
		}
		NumberManager.inst.DestroyOrPool(ni);
		UpdateAmmoGraphics(true);
		base.OnCollectNumber(ni);

	}



	public override bool CanCollectNumber(){
		return bulletAmmo.Count == 0;
	}

	public override bool CanCollectNumber (NumberInfo ni)
	{
		//  Collecting same number already loaded? Then reload.
		if (!ni) return false;
		if (ni.GetComponent<ResourceNumber>()) return false;
		if (bulletAmmo.Count > 0){
			if (Fraction.Equals(bulletAmmo[0].ammoValue,ni.fraction) && bulletAmmo.Count != maxAmmo) {
				ClearAmmo();
				return true;
			}
		}

		// else only load new number if empty.
		return bulletAmmo.Count <= 0;

	}

	public override List<Ammo> GetAmmo(){
		return bulletAmmo;
	}

	public override List<Ammo> GetAmmoInfo(){
//		Debug.Log("bullet ammo count get ammo:"+bulletAmmo.Count);
		return bulletAmmo;
	}
	

	public override GameObject DropOneAmmo(Ammo a){
		if (bulletAmmo.Contains(a)){
			DropAmmo();
			UpdateAmmoGraphics();
			return NumberManager.inst.CreateNumber(a.ammoValue,Player.inst.transform.position); // create a brand new number that something was asking for
		} else {
			return null;
		}
	}




	float lastFiredTime=0;
	override public void Fire() {
		base.Fire ();
		if (Player.frozen) return;
//		// commented Debug.Log ("click");
		if (coolDown > 0) return;
		coolDown = bulletInterval;
		// "You're empty" ("so are you")
		if (bulletAmmo.Count <= 0){
			if (Time.time > lastFiredTime + 0.5f){
				
//				base.AttemptPickup();
				// This is where if you go up to a number with an empty multiblaster and click, you "collect" that tnumber. It's stupid and even if it isn't it shouldn't be here!LOL
				GameObject o = AmmoCloseby(20);
				if (o){
					OnCollectItem(o);
				}
			}
			AudioManager.inst.PlayClick2();
			return;
		}
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.timesMultiblasterFired,1);// userData.timesMultiblasterFired++;
		lastFiredTime = Time.time;
		
		//		AudioManager.inst.PlaySimpleShortShotLaser(Player.inst.transform.position);
		AudioManager.inst.PlayShortLaser(0.15f);
		// smoke effect
//		Vector3 muzzlePos = Player.GetComponent<PlayerGadgetController>().machineGunGraphics.transform.position + Player.GetComponent<PlayerGadgetController>().machineGunGraphics.transform.forward*.3f;

		EffectsManager.inst.CreateSmokePuff(PlayerGadgetController.inst.GetPlayerLocationForGadget(GadgetLocationOnPlayer.RightArm).position,GetAimVector());
		Ammo ammo = bulletAmmo[bulletAmmo.Count-1];
		bulletAmmo.Remove(ammo);

		UpdateAmmoGraphics(true);

		Vector3 offsetVector = -Camera.main.transform.forward;
		GameObject bullet = NumberManager.inst.CreateNumber(
			ammo.ammoValue, new Vector3(0,0,0), NumberShape.Sphere);
		bullet.AddComponent<MultiblasterBullet>();
		GameObject plusTrail = SMW_GF.inst.CreatePlusTrail(bullet.transform.position);
		plusTrail.transform.parent = bullet.transform;
		bullet.transform.localScale *= bulletScale;
		bullet.layer = LayerMask.NameToLayer("DontCollideWithPlayer"); // yeah sounds right
//		// commented Debug.Log("bullet dont collide.");
		SphereCollider sc = bullet.GetComponent<SphereCollider>() as SphereCollider;
		sc.radius =.4f; // increased? or decreased? for some reason.
		NumberInfo ni = bullet.GetComponent<NumberInfo>();
		Destroy(bullet.GetComponent<PickUppableObject>()); // ugh
		bullet.AddComponent<DoesDieOnImpact>();
//		ni.combineLayer = Mathf.NegativeInfinity
//		ni.combineLayer = -65535;
		ni.neverEats = true;
//		ni.pickupFlag=false;
//		ni.neverEats=true; // don't allow machine gun bullets to eat monsters!!!
//		ni.comb = Combineability.allOverride; // .. what..
//		ni.SetStability(false);
//		ni.SetInstabilityTimer(2.6f);
//		ni.pickupFlag=false; // you can't pick these up
		ni.PlayerTouched();

		bullet.GetComponent<Rigidbody>().isKinematic=false; // Why is this true by default?! because "player can't pickup" makes it true?
		bullet.GetComponent<Rigidbody>().useGravity=false;
		
		bullet.transform.position = GetWeaponStartPos();
		CheckBulletSpawnForHoops(bullet);
		
		float bulletPower = 700;
		bullet.GetComponent<Rigidbody>().mass = .1f;
		bullet.GetComponent<Rigidbody>().AddForce(GetAimVector()*bulletPower);
	}

	public override void SetAmmoJson(SimpleJSON.JSONArray N){
//		Debug.Log("setting ammo json:"+N.AsArray.Count);
		foreach(SimpleJSON.JSONClass item in N.AsArray.Childs){
			GameObject ammoObj = NumberManager.inst.CreateNumber(new Fraction(item[Fraction.numeratorKey].AsFloat,item[Fraction.denominatorKey].AsFloat),Vector3.zero);
			OnCollectNumber(ammoObj.GetComponent<NumberInfo>(),N.AsArray.Count); // just collect N times the first one, since multiblaster multiplies the ammo by 10 when it loads.
			return;
		}
	}

	override public void UpdateAmmoGraphics(bool redraw=false) {
//		if (Player.GetComponent<PlayerGadgetController>().currentWeapon != this) return; // don't mess with graphics if this weapon isn't selected.
		if (!gadgetGraphics) return;
		
		if (bulletAmmoGraphics.Count == 0 || redraw){
			foreach(GameObject o in bulletAmmoGraphics){
				Destroy(o); //Destroy(o);
			}
			bulletAmmoGraphics.Clear ();
			
			int i = 0;
			//			// commented Debug.Log ("redrawing "+bulletAmmo.Count+" pieces");
//			if (bulletAmmo.Count < 1) return;


			foreach(Ammo a in bulletAmmo) {
				GameObject obj = NumberManager.inst.CreateNumberAmmo(a.ammoValue);
				obj.AddComponent<SinGrow>();
				obj.transform.parent = gadgetGraphics.transform;
				
				if (i==0) { // don't populate one bullet, it goes on the shaft.
//					obj.transform.parent = gadgetGraphics.transform;
					obj.transform.localPosition=new Vector3(0,0,-.45f);
					float bs = .35f; 
					obj.transform.localScale = new Vector3(bs,bs,bs);
					bulletAmmoGraphics.Add(obj);
					i++;
				} else {
					float ammoScale = .15f;
					float radius = .28f;					
					obj.transform.localScale = new Vector3(ammoScale,ammoScale,ammoScale);
					int degreesToComplete = 360;
					int count = 9;
					float arcLength = degreesToComplete / count;
					float xPos = Mathf.Sin(Mathf.Deg2Rad*(i+1)*arcLength)*(radius); // x and y calculated with "trigonometry"
					float yPos = Mathf.Cos(Mathf.Deg2Rad*(i+1)*arcLength)*(radius);
					Vector3 pos = new Vector3(xPos,yPos,-.5f);
					obj.transform.localPosition = pos;
					bulletAmmoGraphics.Add(obj);
					i++;
				}
			}

		} else {
//			// commented Debug.Log ("destroyed 0 because "+bulletAmmoGraphics.Count+","+redraw	);
			UnityEngine.Object.Destroy(bulletAmmoGraphics[0].GetComponent<SinGrow>());
//			Destroy (bulletAmmoGraphics[0]);
			Destroy(bulletAmmoGraphics[0]);
			bulletAmmoGraphics.RemoveAt(0);
		}
		
	}

	public override string GetGadgetName ()
	{
		return "Multiblaster";
	}

	public override void ClearAmmo ()
	{
		bulletAmmo.Clear();
		UpdateAmmoGraphics(true);
	}

	public override void OnEquip() {
		holdingMouse = false;
//		Debug.Log("multi equip;");
		base.OnEquip();
//		// commented Debug.Log ("?");
		MascotAnimatorController.inst.HoldRightArm(true);
	}

	public override void OnUnequip(){
		base.OnUnequip();
//		Debug.Log("mb unequp");
		MascotAnimatorController.inst.HoldRightArm(false);
	}

	public override void ModifyAmmo (NumberModifier.ModifyOperation fn)
	{
		foreach(Ammo a in bulletAmmo) {
			a.ammoValue = fn(a.ammoValue);
			if (a.ammoValue.numerator ==0){
				DropAmmo(true);
				return;
			}
			//			// commented Debug.Log(a.ammoValue);
		}
		UpdateAmmoGraphics(true);
	}

	public override GameObject DropAmmo(bool f=false){
		List<Ammo> toremove = new List<Ammo>();
		bulletAmmo.Clear();
//		foreach(Ammo a in bulletAmmo){
//			toremove.Add(a);
//		}
//
//		foreach(Ammo a in toremove){
//			bulletAmmo.Remove(a);
//		}
		UpdateAmmoGraphics(true);
		return null;
	}

	public override bool IsEmpty(){
		return bulletAmmo.Count == 0;
	}
	public override int GetMaxAmmo(){
		return maxAmmo;
	}

}
