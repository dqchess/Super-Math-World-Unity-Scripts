using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GadgetLocationOnPlayer {
	RightArm,
	Body
}

public class Gadget : MonoBehaviour {

	public UserEditableObject userEditableObjectRef;

	#region FAKE user editable (because we want multiple selective inheritance and this is a hacky way to do this for gadgets)
	virtual public void SetProperties(SimpleJSON.JSONClass N){
//		userEditableObjectRef.SetProperties(N);
	}

	virtual public SimpleJSON.JSONClass GetProperties(SimpleJSON.JSONClass N){
		return N;
	}

	virtual public GameObject[] GetUIElementsToShow(){
		return new GameObject[] { 
			LevelBuilder.inst.POCMheightButton 
		};
	}

	virtual public void OnGameStarted() {
		
	}

	#endregion

	public Vector3 GetAmmoDropPosition(){
		return Player.inst.transform.position + Vector3.up * 1 + Player.inst.transform.forward * 1.5f;
	}

	public GameObject ammoGraphics;
	public GameObject gadgetGraphicsPrefab;
	public GameObject gadgetGraphics;
	public Vector3 playerHoldingEulerAngles;
	public Vector3 testVector3;
	public Vector3 playerHoldingPos;
	public float playerHoldingScale = 1;
	public bool initiated = false;
//	public bool canThrow = true;
	public float coolDown = 0f;

	public GadgetLocationOnPlayer gadgetLocationOnPlayer = GadgetLocationOnPlayer.RightArm;

	public virtual void Start(){


	} // virtual starts now?

	public virtual void NullifyAmmo(){
		
	}

	public virtual void Init(){}
	public virtual List<Ammo> GetAmmoInfo(){
		return new List<Ammo>();
	}

	public virtual void OnPlayerAction(){
		// commented Debug.Log ("on player action! root");
		// hmm.. what?
	}

	virtual public GameObject DropOneAmmo(Ammo a){
		return null;
	}

	public delegate void OnCollectItemDelegate(GameObject o);
	public OnCollectItemDelegate onCollectItemDelegate;
	public virtual void OnCollectItem(GameObject o) {
		if (onCollectItemDelegate != null){
			onCollectItemDelegate(o);
		}
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni){
			OnCollectNumber (ni);
		}
	}

	public GameObject AmmoCloseby(float maxPickupRange){
		// was there ammo right in front of me and I was pointing at it?
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
		//		Debug.Log("ray:"+ray);
		if (Physics.Raycast(ray,out hit,maxPickupRange)){
			//			Debug.Log("hit:"+hit.collider.name);
			GameObject o = hit.collider.gameObject;
			PickUppableObject pip = o.GetComponent<PickUppableObject>();
			if (pip && pip.CanPickUp){
				// don't allow us to pick up items too far over our own head.
				if (o.transform.position.y > Player.inst.transform.position.y + 4) {
					return null;
				}
				if (CanCollectItem(o)) {
					return o;
				}
			}
		}
		return null;
	}

	public GenericButton ButtonCloseby(float maxPickupRange){
		// was there ammo right in front of me and I was pointing at it?
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
		//		Debug.Log("ray:"+ray);
		if (Physics.SphereCast(ray,1f,out hit,maxPickupRange)){
			//			Debug.Log("hit:"+hit.collider.name);
			GenericButton bta = hit.collider.transform.root.gameObject.GetComponentInChildren<GenericButton>();
			if (bta){
				// don't allow us to pick up items too far over our own head.
				return bta;
			}
		}
		return null;
	}



	public GameObject GadgetCloseby(float maxPickupRange){
		// was there ammo right in front of me and I was pointing at it?
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
		//		Debug.Log("ray:"+ray);
		if (Physics.Raycast(ray,out hit,maxPickupRange)){
			
			GameObject o = hit.collider.gameObject;
			PickUppableObject pip = o.GetComponent<PickUppableObject>();
			Gadget g = o.GetComponent<Gadget>();
			if (pip && pip.CanPickUp && g){
				
				return o;
			}
		}
		return null;
	}


	public virtual void OnMouseHoldLeft() {}

	public virtual void OnCollectNumber(NumberInfo ni){
		CheckAdviceNeeded();
	}
	public virtual void OnCollectNumber(NumberInfo ni, int clipSize) {}	

	public virtual bool CanCollectNumber (NumberInfo ni) {
		return false;
	}

	public virtual bool IsValidAmmo(GameObject o){
		if (!o) return false;
		return o.GetComponent<NumberInfo>() && !o.GetComponent<ResourceNumber>();
	}


	public virtual bool CanCollectItem( GameObject o){
//		if (o.GetComponent<ResourceNumber>()
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni) return CanCollectNumber(ni);
		else return false; // in the future we may have gadgets that can collect objects..but mostly numbers.
	}

	public virtual bool CanCollectNumber(){
		return CanCollectNumber(null);
	}

	public virtual void RemoveAmmo(Ammo a){	}


	float adviceTimer = 0;
	float chooseTimer = 0;

	List<GameObject> highlightCopies = new List<GameObject>();

	public enum GadgetLoadingState {
		Ready, // 
		Hinting, // "Player should press L"
		Loading, // Player already pressed L
		Full // not used, because we escape this case with a conditional before checking any gadget load state in update loop
	}
	public GadgetLoadingState state = GadgetLoadingState.Ready;

	public virtual void GadgetUpdate() {
		
		if (MouseLockCursor.mouseShows > 0) {
//			// commented Debug.Log("nmouse:"+GetGadgetName);
			return;
		}


		if (GadgetNeedsAdvice() ){ // if we've equippedd a gadget capable of loading a number ..
			if (adviceTimer < 0){
				CheckAdviceNeeded();
			}
//			Debug.Log("advict:"+adviceTimer+",cht:"+chooseTimer);
			// count down until next text will be displayed.
			adviceTimer -= Time.deltaTime;
			chooseTimer -= Time.deltaTime;
			if ((state == GadgetLoadingState.Hinting || state == GadgetLoadingState.Ready) && Input.GetKeyDown(KeyCode.L) && GadgetNeedsAdvice()){
				chooseTimer = 5f;
				adviceTimer = 0;
				state = GadgetLoadingState.Loading;
				for (int i=0;i<9;i++){
					GameObject highlightCopy = (GameObject)Instantiate(Inventory.inst.highlight.gameObject,Inventory.inst.beltSlots[i].transform.position,Inventory.inst.highlight.rotation);
					highlightCopy.transform.SetParent(Inventory.inst.beltSlots[i].transform.parent);
					highlightCopies.Add(highlightCopy);
					highlightCopy.GetComponent<RectTransform>().sizeDelta = new Vector2(120,120);

					highlightCopy.GetComponent<Image>().color = new Color(1,1,1,0.2f);
					if (highlightCopy.GetComponent<Outline>()){
						Destroy(highlightCopy.GetComponent<Outline>());
					}
					SinPulsate sp = highlightCopy.AddComponent<SinPulsate>();
					sp.amplitude = 0.1f;
					sp.pulsateSpeed = 3.5f;
				}
				state = GadgetLoadingState.Loading;
			}

			// If we're in loading state, 
			if (state == GadgetLoadingState.Loading){
				if (Input.GetKeyDown(KeyCode.Alpha1)){
					Slot beltSlot = Inventory.inst.beltSlots[0];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha2)){
					Slot beltSlot = Inventory.inst.beltSlots[1];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha3)){
					Slot beltSlot = Inventory.inst.beltSlots[2];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha4)){
					Slot beltSlot = Inventory.inst.beltSlots[3];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha5)){
					Slot beltSlot = Inventory.inst.beltSlots[4];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha6)){
					Slot beltSlot = Inventory.inst.beltSlots[5];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha7)){
					Slot beltSlot = Inventory.inst.beltSlots[6];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha8)){
					Slot beltSlot = Inventory.inst.beltSlots[7];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				} else if (Input.GetKeyDown(KeyCode.Alpha9)){
					Slot beltSlot = Inventory.inst.beltSlots[8];
					AttemptToLoadItemIntoGadgetFromBeltSlot(beltSlot);
				}  else if (Input.anyKeyDown && chooseTimer < 0){
//					Debug.Log("anykey");
					StopLoadingGadget();
				}
			}
		} else {
//			if (Input.GetKeyDown(KeyCode.L)){
//				if (
//					this.GetType() == typeof(GadgetMultiblaster) ||
//					this.GetType() == typeof(GadgetZooka) ||
//					this.GetType() == typeof(GadgetWave) ||
//					this.GetType() == typeof(GadgetBow)
//				)	{

//				}
//			}
		}


		if (Input.GetMouseButtonDown(0) && Time.timeScale != 0){
//			// commented Debug.Log ("click gadget"+this.name+" Time:"+Time.realtimeSinceStartup);
			MouseButtonDown();
		}
		if (Input.GetMouseButtonUp(0)){
			MouseButtonUp();
		}
	}

	virtual public void DropFirstAmmo(){}

//	virtual public bool CanLoadAmmoFromPlayerTouched(GameObject o){
//		return IsValidAmmo(o) && CanCollectItem(o);
//	}

	void AttemptToLoadItemIntoGadgetFromBeltSlot(Slot beltSlot){
		GameObject itemToCollect = beltSlot.GetItem3D();

		if (itemToCollect){
			if (this.IsValidAmmo(itemToCollect)){
				if (!this.CanCollectItem(itemToCollect)){
					// for multiblaster, replace it. Or for zooka, 
					if (this.GetType() == typeof(GadgetZooka)){
						this.DropFirstAmmo();
					} else {
						this.ClearAmmo();
					}
				}
				this.OnCollectItem(itemToCollect);
				//						beltSlot.GetComponent<InventoryItem>().
				NumberInfo ni = itemToCollect.GetComponent<NumberInfo>();
				if (ni){
					Inventory.inst.DisplayBottomText( "Loaded a "+ni.fraction);
				} else {
					Inventory.inst.DisplayBottomText( "Loaded a "+itemToCollect.GetComponent<UserEditableObject>().myName);
				}
				beltSlot.ClearSlot();
			} else{
				Inventory.inst.DisplayBottomText( "Couldn't load a: "+itemToCollect.GetComponent<UserEditableObject>().myName);
			}
		} else {
			Inventory.inst.DisplayBottomText( "Nothing was available in "+beltSlot.name);
		}
		StopLoadingGadget();
		adviceTimer = 5;
	}

	void StopLoadingGadget(){
		state = GadgetLoadingState.Ready;
		foreach(GameObject o in highlightCopies){
			Destroy(o);
		}
		highlightCopies.Clear();
	}

	virtual public bool IsEmpty(){
		return false;
	}

	virtual public void MouseButtonDown() {
//		Fire();

	}
	virtual public void MouseButtonUp() {}

	public virtual void GadgetLateUpdate() {} // for things like spinning around that will render after all other physics/updates, so you don't get glitches/snaps


	public virtual void Fire(){

	}

	public virtual string GetGadgetName(){
		
		return "Undefined gadget name.";

	}

	public virtual void UpdateAmmoGraphics(bool redraw = false) {
		CleanComponentsForAmmo(ammoGraphics);
	}

	public virtual void ClearAmmo(){

	}

	public virtual List<Ammo> GetAmmo(){
		return new List<Ammo>();
	}

	public virtual SimpleJSON.JSONArray GetAmmoJson(){
		// (most) gadgets do not serialize their ammo in inventory so this function is needed for them to preserve ammo during level instance loads
		SimpleJSON.JSONArray N = new SimpleJSON.JSONArray();
		foreach(Ammo a in GetAmmo()){
			SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
			item[Fraction.numeratorKey].AsInt = a.ammoValue.numerator;
			item[Fraction.denominatorKey].AsInt = a.ammoValue.denominator;
			N.Add(item);
		}
		return N;
	}

	public virtual void SetAmmoJson(SimpleJSON.JSONArray N){
//		Debug.Log("setamo:"+N.ToString());
		foreach(SimpleJSON.JSONClass item in N.AsArray.Childs){
			GameObject ammoObj = NumberManager.inst.CreateNumber(new Fraction(item[Fraction.numeratorKey].AsFloat,item[Fraction.denominatorKey].AsFloat),Vector3.zero);
			OnCollectNumber(ammoObj.GetComponent<NumberInfo>());
		}
	}

	public virtual string GetInventoryDescription(){
		return "No inventory description available.";
	}

//	public virtual string GetInventoryDescription(){
//		return "No inventory description available.";
//	}

	bool GadgetNeedsAdvice(){
		return this.GetType() == typeof(GadgetMultiblaster) ||
			this.GetType() == typeof(GadgetZooka) ||
			this.GetType() == typeof(GadgetWave) ||
			this.GetType() == typeof(GadgetBow);
	}

	public void CheckAdviceNeeded(){
		if (!GadgetNeedsAdvice()) {
			Inventory.inst.HideBottomText();
			 
		} else {
			if (state == GadgetLoadingState.Loading) {
				Inventory.inst.DisplayBottomText( "Select a belt position 1-9 to load that item.");
				adviceTimer = 20f;
				
			} else if (state == GadgetLoadingState.Ready && this.GetAmmo().Count == 0 && Inventory.inst.BeltContainsANumber()) {
				state = GadgetLoadingState.Hinting;
				Inventory.inst.DisplayBottomText( "Press L to load "+userEditableObjectRef.myName);
				adviceTimer = 20f;
			} else {
				Inventory.inst.HideBottomText();
				state = GadgetLoadingState.Ready;
			}
		}

	}


	public virtual void OnEquip() {
		if (PlayerGadgetController.inst.GetCurrentGadget() == this) return;
		adviceTimer = 0;
//		Debug.Log("Advtimer = 0");
		CheckAdviceNeeded();
//		return;
//		PlayerGadgetController.inst.EquipGadget(this);
//		// commented Debug.Log ("Global var inst pnc cur gad this;"+this);
//		PlayerGadgetController.inst.CurrentGadget().OnUnequip();
		PlayerGadgetController.inst.GetCurrentGadget().OnUnequip();
		PlayerGadgetController.inst.SetCurrentGadget(this);
		if (gadgetGraphics){
			gadgetGraphics.SetActive(true);
		} else {
			if (gadgetGraphicsPrefab){ // throw won't have this
				gadgetGraphics = (GameObject) Instantiate (gadgetGraphicsPrefab);
				gadgetGraphics.transform.parent = PlayerGadgetController.inst.GetPlayerLocationForGadget(gadgetLocationOnPlayer);
				Quaternion rot = new Quaternion();
				rot.eulerAngles = playerHoldingEulerAngles;
				gadgetGraphics.transform.localRotation = rot;
				gadgetGraphics.transform.localScale = Vector3.one * playerHoldingScale;
				gadgetGraphics.transform.localPosition = playerHoldingPos;
			}
		}

		UpdateAmmoGraphics(true);

//		PlayerGadgetController.GetPlayerLocationForGadget(gadgetLocationOnPlayer).position;
//
//		gadgetGraphics.activeInHierarchy playerHoldingEulerAngles;
//		public Vector3 playerHoldingPos;
//		public float playerHoldingScale;
		if (this.GetType() != typeof(GadgetThrow)) gameObject.SetActive(false);
	}

	public virtual void OnUnequip(){
//		PlayerGadgetController.inst.EquipGadget(null);
//		Debug.Log ("unequip:"+GetGadgetName());
//		Debug.Log("base unequp");
		if (gadgetGraphics) {
//			Debug.Log ("destroyed graphics:"+gadgetGraphics.name);
			gadgetGraphics.SetActive(false);
		}

		MascotAnimatorController.inst.SetHoldingBoolsFalse();
		if (ammoGraphics) Destroy (ammoGraphics);
	}

	virtual public Vector3 GetAimVector() {
		return Camera.main.transform.forward;
	}

	virtual public void CheckBulletSpawnForHoops(GameObject bullet) {
		if (!bullet) return;
		if (!bullet.GetComponent<RecordPosition>()) { return; }
		bullet.GetComponent<RecordPosition>().nowPosition = Player.inst.transform.position;
		bullet.GetComponent<RecordPosition>().lastPosition = Player.inst.transform.position;
		
		foreach(Collider c in Physics.OverlapSphere(Player.inst.transform.position, 0.1f)) {
			if(c.gameObject.GetComponent<NumberHoop>()) {
				c.gameObject.GetComponent<NumberHoop>().CheckCrossing(bullet.GetComponent<Collider>());
			}
		}
		
	}



	virtual public Vector3 GetWeaponStartPos() {
		return Camera.main.transform.position + Camera.main.transform.forward * 6.5f;
	}

	virtual public GameObject DropAmmo(bool destroyDroppedNumber=true){ return null; }


	virtual public GameObject CleanComponentsForAmmo(GameObject ammoGraphics){
		if (!ammoGraphics) {
//			// commented Debug.Log ("no ammographics here!");
			return null;
		}
		//		// commented Debug.Log ("killing stuff on ammoghx");
		List<Component> comps = new List<Component>();
		comps.AddRange(ammoGraphics.GetComponentsInChildren(typeof(Component)));
		comps.AddRange(ammoGraphics.GetComponents(typeof(Component)));
		foreach(Component comp in comps){
			//			// commented Debug.Log ("comp: " +comp.GetType() + "typoef " + typeof(MeshFilter));
			if (comp == null) continue;
			if (comp.GetType()==typeof(AlgebraInfo)){
				AlgebraInfo ai = comp as AlgebraInfo;
				ai.ClearBalls();
			}
			if (
				
				comp.GetType()==typeof(Collider)
				|| comp.GetType()==typeof(BoxCollider)
				|| comp.GetType()==typeof(SphereCollider)
				|| comp.GetType()==typeof(Rigidbody)
				|| comp.GetType()==typeof(Rotate)
				|| comp.GetType()==typeof(FollowTransform)
				|| comp.GetType()==typeof(ParticlesOnBounce)
				|| comp.GetType ()==typeof(Animal)
				|| comp.GetType ()==typeof(Animal_Bird)
				|| comp.GetType ()==typeof(Animal_Fish)
				|| comp.GetType ()==typeof(LineRenderer)
				|| comp.GetType ()==typeof(HideMeshAtDistance)
				){
				UnityEngine.Object.Destroy(comp);
			}
			//			// this was blacklist (exclude these) but now let's whitelist and only get rid of collider.
			//			continue;
			//
			//			if (comp.GetType()==typeof(MeshFilter)) { continue; }
			//			if (comp.GetType()==typeof(Transform)) { continue; }
			//			if (comp.GetType()==typeof(MeshRenderer)) { continue; }
			//			if (comp.GetType()==typeof(Renderer)) { continue; }
			//			if (comp.GetType()==typeof(ParticleEmitter)) { continue; }
			//			if (comp.GetType()==typeof(ParticleRenderer)) { continue; }
			//			if (comp.GetType()==typeof(ParticleAnimator)) { continue; }
			//			if (comp.GetType()==typeof(ParticleSystem)) { continue; }
			//			if (comp.GetType()==typeof(CubeWarmer)) { continue; }
			//			if (comp.GetType()==typeof(NumberInfo)) { continue; }
			//			if (comp.GetType()==typeof(SometimesFacePlayer)) {
			//				comp.SendMessage("FacePlayerOnce"); // awk!
			//				continue; 
			//			}
			//			// commented Debug.Log("destroying " + comp+ " on " + ammoGraphics.name);
			//			UnityEngine.Object.Destroy(comp);
			//			if (ni.GetComponentInChildren<SometimesFacePlayer>()){
			//				ni.GetComponentInChildren<SometimesFacePlayer>().FacePlayerOnce();
			//			}
		}
		return ammoGraphics;
	}

	virtual public void SetPlayerAnimation(){}

	virtual public void CleanObjectOnCollect(GameObject obj){
		TimedObjectDestructor tod = obj.GetComponent<TimedObjectDestructor>(); // Very awkward way to stop this from happening..
		if(tod) {
			tod.StopAllCoroutines();
			UnityEngine.Object.Destroy(tod);
		}
	}

	virtual public void GadgetOnTriggerEnter(Collider other){}

	virtual public void PlayOnEquipAudio (){}

	virtual public void ModifyAmmo(NumberModifier.ModifyOperation nmf){}

	virtual public void CheckAmmoNull(GameObject num,Fraction result){}

	virtual public void OnDestroy(){
		if (gadgetGraphics) Destroy(gadgetGraphics);
	}
	virtual public int GetMaxAmmo(){
		return 0;
	}
}
