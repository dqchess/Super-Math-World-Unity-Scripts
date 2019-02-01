
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType {
	Belt,
	Backpack,
	OneInBelt, // for example resource numbers should go into belt but never more than 1.
	Gem, // for example gems which evaporate when picked up
	Head,
}

[System.Serializable]
public class InventorySprites{
	public Sprite posNumSphere;
	public Sprite negNumSphere;
	public Sprite posNumCube;
	public Sprite negNumCube;

}

public class Inventory : MonoBehaviour, IHasChanged {

	public static string backpackKey = "backpackKey";
	public static string beltKey = "beltKey";
	public static string headKey = "headKey";
	public static string selectedBeltIndexKey = "selectedBeltIndexKey";
	public static string gemCountKey = "gemCountKey";

	public GameObject bottomText;
	public GameObject bottomTextBackboard;
	public GameObject invText;
	public GameObject showBackboardOnOpen;
	public Image backboardOnOpen1;
	public Image backboardOnOpen2;
	public Transform highlight;
	public Transform inventoryParent;
	public Transform beltParent;
	public Transform beltDefaultPos;
	public Transform beltOpenPos;
	public Transform beltTargetPos;
	public Transform beltDisabledPos;
	bool beltMoving = false;
	bool backboardImageFading = false; // fades in and out the white backboard(s) of the inventory on open/close
	public Camera uiCam;
	public Slot[] backpackSlots;
	public Slot[] beltSlots;
	public Slot slotHead;
//	public Transform slotHand;
//	public Transform slotHead;
//	public Transform slotBody;
//	public Transform slotFeet;
//	public Text inventoryText;
//	public GameObject inventoryParent;
	public static Inventory inst;
	public GameObject itemPrefab;
	public GameObject digitsPrefab;
	public int selectedIndex = 0;
	public Text gemCountText;
	public Image gemIcon;
	public Text gemCountTextLevelBuilder;
	public Transform draggingObjectParent;
	public int gemCount = 0;
	[SerializeField] public InventorySprites sprites;

	public delegate void BeltSpaceSelectedDelegate(Slot slot);
	public BeltSpaceSelectedDelegate beltSpaceSelectedDelegate;

	public void SetInstance(){
		inst = this;
	}
	
	// Use this for initialization
	void Start () {
		HasChanged ();
		inventoryParent.gameObject.SetActive(false);
		StartCoroutine(SelectBeltSpaceE(0,.01f)); 
		for (int i=0;i<beltSlots.Length;i++){
			beltSlots[i].index = i;
		}
//		GadgetThrow.inst.onPlayerThrow += UpdateBeltPosition;
	}




	#region IHasChanged implementation
	public void HasChanged ()
	{
		// apparently this wall says nothing
		foreach (Slot s in inventoryParent.GetComponentsInChildren<Slot>()){
			GameObject item = s.item;
			if (item){

			}
		}
	}
	#endregion

	float pickupTimeout = 0;
//	float inventoryTextTimer = 0;
	bool bottomTextFading = true;

	void Update(){
		if (SMW_CHEATS.inst.cheatsEnabled && Input.GetKeyDown(KeyCode.G)){
			AddToGems(500000);
//			gemCount += 500000;
		}
//		if (Input.GetKeyDown(KeyCode.L)){

//			LoadInventory(j);
//		}
		bottomTextTimer -= Time.deltaTime;
		if (bottomTextTimer < 0 && bottomText.activeSelf){
			bottomTextFading  = true;
			bottomTextTimer = 8f;
		}
		if (bottomTextFading){
			float fadeSpeed = 3f;
			bottomText.GetComponent<Text>().color = Color.Lerp(bottomText.GetComponent<Text>().color,Color.clear,Time.deltaTime * fadeSpeed);
			bottomTextBackboard.GetComponent<Image>().color = Color.Lerp(bottomTextBackboard.GetComponent<Image>().color,Color.clear,Time.deltaTime * fadeSpeed);
			if (Vector4.Distance(bottomText.GetComponent<Text>().color,Color.clear) < .1f){
				bottomText.GetComponent<Text>().color = Color.clear;
				bottomTextBackboard.GetComponent<Image>().color=Color.clear;
				bottomTextFading = false;		
				bottomText.SetActive(false);
				bottomTextBackboard.SetActive(false);
			}
		}
		
		if (LevelBuilder.inst && LevelBuilder.inst.levelBuilderIsShowing) return;

//
//		bool hasItemInBackpack = false;
//		bool hasItemInBelt = false;
//
//
//
//		foreach(Slot s in backpackSlots){
//			if (s.item && s.item.GetComponent<InventoryItem>().item3d){
//				hasItemInBackpack = true;
//			}	
//		}
//


//		bool hasItem = false;
		foreach(Slot s in beltSlots){ 
			GameObject item3d = s.GetItem3D();
			if (item3d) {
				foreach(IMyUpdateable c in item3d.GetComponents(typeof(IMyUpdateable))){
					c.Update(); // update wont run on these disabled objects otherwise.
				}
			}
		}


		if (beltMoving){
			float beltLerpSpeed = 5.5f;
			beltParent.position = Vector3.Lerp(beltParent.position,beltTargetPos.position,Time.deltaTime * beltLerpSpeed);
			if (Vector3.SqrMagnitude(beltParent.position-beltTargetPos.position) < 7){
				beltParent.position = beltTargetPos.position;
				beltMoving = false;
			}
		}

		pickupTimeout -= Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I)){
			string fadeLetter = "e";
			if (Input.GetKeyDown(KeyCode.E)) fadeLetter = "E";
			if (Input.GetKeyDown(KeyCode.I)) fadeLetter = "I";

			if (!inventoryOpen && GameManager.inst.CanDisplayDialogue() && inventoryAvailable && !CanvasMouseController.inst.controlsDialogue.activeSelf){
				ShowInventory();
			} else {
				HideInventory();
			}
			EffectsManager.inst.FadeLetter(fadeLetter);
		}
		if (inventoryOpen && Input.GetKeyDown(KeyCode.W)){
			HideInventory();
		}
		if (PlayerGadgetController.inst && PlayerGadgetController.inst.GetCurrentGadget() && PlayerGadgetController.inst.GetCurrentGadget().state == Gadget.GadgetLoadingState.Loading) {
			choosingAmmoTimeout = .3f;
		}  else {
			choosingAmmoTimeout -= Time.deltaTime; // after gadget is no longer in loading state, give us a second before switching to the gagdet whose number we pressed
			// this is to prevent player from *equipping* belt space 4 when intent was to *load* item in belt space 4 into currently equipped gadget (in a diff belt space)
		}
		if (PlayerVehicleController.inst.currentVehicle == null && PlayerGadgetController.inst && PlayerGadgetController.inst.GetCurrentGadget() && PlayerGadgetController.inst.GetCurrentGadget().state != Gadget.GadgetLoadingState.Loading && choosingAmmoTimeout < 0 && !Player.frozen){
			
			if (Input.GetKeyDown(KeyCode.Alpha1)){
				SelectBeltSlot(beltSlots[0]);
			} else if (Input.GetKeyDown(KeyCode.Alpha2)){
				SelectBeltSlot(beltSlots[1]);
			} else if (Input.GetKeyDown(KeyCode.Alpha3)){
				SelectBeltSlot(beltSlots[2]);
			} else if (Input.GetKeyDown(KeyCode.Alpha4)){
				SelectBeltSlot(beltSlots[3]);
			} else if (Input.GetKeyDown(KeyCode.Alpha5)){
				SelectBeltSlot(beltSlots[4]);
			} else if (Input.GetKeyDown(KeyCode.Alpha6)){
				SelectBeltSlot(beltSlots[5]);
			} else if (Input.GetKeyDown(KeyCode.Alpha7)){
				SelectBeltSlot(beltSlots[6]);
			} else if (Input.GetKeyDown(KeyCode.Alpha8)){
				SelectBeltSlot(beltSlots[7]);
			} else if (Input.GetKeyDown(KeyCode.Alpha9)){
				SelectBeltSlot(beltSlots[8]);
			}
		}

		if (backboardImageFading){
			float fadeSpeed = 6f;
			Color targetColor = Color.white;
			if (!inventoryOpen){
				targetColor = new Color(1,1,1,0);
			}
			Color newColor =  Color.Lerp(backboardOnOpen1.color,targetColor,Time.deltaTime * fadeSpeed);
			backboardOnOpen1.color = newColor;
			backboardOnOpen2.color = newColor;
			if (Vector4.Magnitude(backboardOnOpen1.color - targetColor) < .03f){
				backboardOnOpen1.color = targetColor;
				backboardOnOpen2.color = targetColor;
				backboardImageFading = false;
			}
		}
	}

	float choosingAmmoTimeout = 0;

	float justSelectedScale = 1.5f;
	public void HighlightSlot(Transform t){

//		if (t.gameObject.GetComponent<Outline>()) {
//			// commented Debug.Log("outline exists on;"+t.name);
//			return; 
//		}
//		Outline o = t.gameObject.AddComponent<Outline>();
//		o.effectDistance = new Vector2(1,-1);
//		o.effectColor = Color.green;
//		highlight.SetParent(t);

		if (!highlight.GetComponent<ShrinkToSize>()) {
			highlight.gameObject.AddComponent<ShrinkToSize>();
		}
		highlight.position = t.position; // put on top!
		highlight.localScale = justSelectedScale * Vector3.one;

	}



	public bool isShowing {
		get {
			return inventoryOpen;
		}
	} 

	void CleanInventory(){
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){ // there is an image
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
				if (!item || !item.item3d){
					s.ClearSlot();
				}
			}
		}
	}

	bool inventoryOpen = false;
	void ShowInventory(){
		CleanInventory();

		bottomText.SetActive(false);
		invText.gameObject.SetActive(true);
		bottomTextBackboard.SetActive(false);
		PlayerNowMessage.inst.Hide();
//		showBackboardOnOpen.SetActive(true);
		List<GameObject> items = GetInventoryItems();
		if (items.Count > 0) {
			List<string> helps = new List<string>();
			helps.Add("Hey there "+WebGLComm.loggedInAsUser+"! Drag and drop numbers to combine them. Press E to close Inventory.");
			helps.Add("Hello "+WebGLComm.loggedInAsUser+"! Drag items off to the side to remove them. Press E to close Inventory.");
			helps.Add("Hi "+WebGLComm.loggedInAsUser+"! Drag items to move them around. Press E to close Inventory.");
			helps.Add("Ahoy "+WebGLComm.loggedInAsUser+"! You can load gadgets by dragging numbers on top. Press E to close Inventory.");
			invText.GetComponent<Text>().text= helps[Random.Range(0,helps.Count)];
		} else {
			invText.GetComponent<Text>().text = "Your inventory is empty. Press E to close it.";
		}
		invText.GetComponent<SimpleTypewriter>().InitText();
		AudioManager.inst.PlayInventoryOpen();
		inventoryOpen = true;
		MouseLockCursor.ShowCursor(true,"inv");
		Player.inst.FreezePlayer();
		Player.inst.DisableMouseLook();
		inventoryParent.GetComponent<SinPop>().Begin();
		InGameHUD.inst.Hide();
		beltMoving = true;
		beltTargetPos = beltOpenPos;
		foreach(Slot s in beltSlots){
			s.GetComponent<Image>().color = new Color(1,1,1,.8f);
		}
		backboardImageFading = true;
//		gameObject.SetActive(true);
	}

	public void HideInventory(bool instantlyMoveBeltParentT = false){
		if (inventoryOpen){
			foreach(DragHandler dh in FindObjectsOfType<DragHandler>()){
				dh.DragEnded();
			}
			invText.gameObject.SetActive(false);
//			showBackboardOnOpen.SetActive(false);
			bottomText.SetActive(false);
			inventoryOpen = false;
//			Debug.Log("close1");
			AudioManager.inst.PlayInventoryClose();
			Player.inst.UnfreezePlayer();
			Player.inst.EnableMouseLook();
			MouseLockCursor.ShowCursor(false,"inv");
			beltTargetPos = beltDefaultPos;
			if (instantlyMoveBeltParentT){
				beltParent.position = beltTargetPos.position;
			} else {
				beltMoving = true;
			}
			foreach(Slot s in beltSlots){
				s.GetComponent<Image>().color = Color.clear;
			}
			inventoryParent.gameObject.GetComponent<ShrinkAndDisable>().Begin();
			InGameHUD.inst.Show();
//			inventoryParent.gameObject.SetActive(false);
		}
		backboardImageFading = true;
	}

	public Slot GetEmptySlot(bool excludeFirst = false){
		return GetEmptyBeltSlot(excludeFirst);
//		return GetEmptyBackpackSlot();
	}

	public Slot GetEmptyBeltSlot(bool excludeFirst = false){
		foreach(Slot s in beltSlots){
			if (s.item == null) {
				if (excludeFirst && GetBeltIndexFromSlot(s) == 0) continue;
				return s;
			} else {
//				// commented Debug.Log("s.item:"+s.item.name);
			}
		}
		return GetEmptyBackpackSlot();
	}

	public int GetBeltIndexFromSlot(Slot s){
		return s.index;
//		for (int i=0;i<beltSlots.Length;i++){
//			if (s == beltSlots[i]) return i;
//		}
//		return -1;
	}

	public Slot GetEmptyBackpackSlot(){
		foreach(Slot s in backpackSlots) {
//			// commented Debug.Log("comp:"+s.name);
			if (s.item == null) {
				return s;
			} else {
//				// commented Debug.Log("s.item:"+s.item.name);
			}
		}
		return null;
	}


	public void PlayerThrewHeldItem(GameObject thrownItem){
		foreach(Slot s in GetComponentsInChildren<Slot>()){
			GameObject o = s.item;
			if (o){
				if (o.GetComponent<InventoryItem>()){
					if (o.GetComponent<InventoryItem>().item3d == thrownItem){
						Destroy(o);
						return;	
					}
				}
			}
		}

	}

	public void UpdateBeltSelection(bool suppressAudio = true){


	
		EquipItemInSlot(beltSlots[selectedIndex],"update");
//		UpdateBeltPosition();
//		SelectBeltSpace(selectedIndex,suppressAudio);
	}

	public bool CompareSlot(Slot a, Slot b){
		return a == b;
	}


	public void ClearGadgetsFromInventory(){
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
				if (item && item.item3d){
					Gadget g = item.item3d.GetComponent<Gadget>();
					if (g){ // don't destroy resource numbers!
						Destroy(item.gameObject);
						Destroy(item.item3d);
					}
				}
				//				PlayerGadgetController.inst.GetCurrentGadget().OnUnequip();
				//				PlayerGadgetController.inst.EquipGadget(PlayerGadgetController.inst.gadgetThrow);
				PlayerGadgetController.inst.GetCurrentGadget().NullifyAmmo();
				SelectBeltSlot(beltSlots[selectedIndex],"cleardgadgewts"); // reselect now that gadget is gone.
			}
		}
	}

	public void ClearNumbersFromInventory(){
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
				if (item && item.item3d){
					NumberInfo ni = item.item3d.GetComponent<NumberInfo>();
					if (ni && !item.item3d.GetComponent<ResourceNumber>()){ // don't destroy resource numbers!
						Destroy(item.gameObject);
						Destroy(item.item3d);
					} else {
						Gadget g = item.item3d.GetComponent<Gadget>();
						if (g) {
							g.DropAmmo();
						}
					}
				}
				//				PlayerGadgetController.inst.GetCurrentGadget().OnUnequip();

				PlayerGadgetController.inst.GetCurrentGadget().NullifyAmmo();
			}
		}
	}

	public void ClearInventory(bool preserveResourceNumbers = false){
//		WebGLComm.inst.Debug("UTY clear inv");
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		slots.Add(slotHead);
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
				if (item){
					if (item.item3d){
						NumberInfo ni = item.item3d.GetComponent<NumberInfo>();
						if (ni && (!item.item3d.GetComponent<ResourceNumber>() || preserveResourceNumbers == false)){ // don't destroy resource numbers!
							Destroy(item.gameObject);
						}
					} else {
						Destroy(item.gameObject); // lol, destroy it anyway
					}
				}
//				PlayerGadgetController.inst.GetCurrentGadget().OnUnequip();
				PlayerGadgetController.inst.EquipGadget(PlayerGadgetController.inst.gadgetThrow);
				PlayerGadgetController.inst.GetCurrentGadget().NullifyAmmo();
			}
		}
	}

	public void ModifyInventoryItems (NumberModifier.ModifyOperation fn)
	{
//		// commented Debug.Log("Not implemeneted.");
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
//		slots.Add(slotHand.GetComponent<Slot>());
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();

//				// commented Debug.Log("got item:"+item+ " on slot:" +s.name);
				if (item.item3d) {
					NumberInfo ni = item.item3d.GetComponent<NumberInfo>();
					Gadget gip = item.item3d.GetComponent<Gadget>();
					if (ni && NumberManager.IsCombineable(ni)){ // !item.item3d.GetComponent<ResourceNumber>()){
						Fraction result = fn(ni.fraction);
						ni.SetNumber(result);
						if (result.numerator == 0){
							
							// number will destroy itself, no further action needed except to destroy item.
//							DestroyInventoryItemFromSlot(s);
							PlayerGadgetController.inst.gadgetThrow.CheckAmmoNull(ni.gameObject, result);
							NumberManager.inst.DestroyOrPool(ni);
							Destroy(item.gameObject);
//							// commented Debug.Log("updating ammo gfx.");
							// What if it was the number held of gadget throw? 
							// Maybe deal with that in the gadget throw itself?
						} else {
							

							ni.gameObject.AddComponent<SinGrow>();
							item.SetUpDigits(ni);
//							MonsterAIRevertNumber mairn = ni.GetComponent<MonsterAIRevertNumber>();
//							if(mairn) {
//								mairn.origFrac = ni.fraction;
//							}
						}
	//					sp.CreateInventoryDigits();
					} else if (gip){
//						// commented Debug.Log("mod?");
						gip.ModifyAmmo(fn);
					}
					if (PlayerGadgetController.inst.ThrowGadgetEquipped()) PlayerGadgetController.inst.gadgetThrow.UpdateAmmoGraphics(true);
				}

			}
		}

		//		RecalcInfoText();
	}


	void ModifyGroup(GameObject[] thisGroup, NumberModifier.ModifyOperation fn){

	}

	public List<NumberInfo> GetNumbersInInventory(){
		
		List<NumberInfo> ret = new List<NumberInfo>();
		//		// commented Debug.Log("Not implemeneted.");
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		//		slots.Add(slotHand.GetComponent<Slot>());
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();

				//				// commented Debug.Log("got item:"+item+ " on slot:" +s.name);
				if (item.item3d) {
					NumberInfo ni = item.item3d.GetComponent<NumberInfo>();
					if (ni){
						ret.Add(ni);
					}
				}
			}
		}
		return ret;
	}

	public GameObject DropInventoryItem(GameObject o){ // This is the first time I've had an argument and a return value be the same object. Problem?
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		//		slots.Add(slotHand.GetComponent<Slot>());
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();

				//				// commented Debug.Log("got item:"+item+ " on slot:" +s.name);
				if (item.item3d) {
					if (item.item3d == o) {
						Destroy(item.gameObject); // Destroy the inventory stats
						o.transform.position = Player.inst.transform.position + Player.inst.transform.right * 2; // drop the item next to the player
						PlayerGadgetController.inst.DontCollideWithPlayerForSeconds(o,2); // don't let player pick it up immediately
						o.SetActive(true); // activate the item
						if (PlayerGadgetController.inst.ThrowGadgetEquipped()){
							if (PlayerGadgetController.inst.gadgetThrow.numberHeld == o){
								PlayerGadgetController.inst.GetCurrentGadget().DropAmmo(false);
//								PlayerGadgetController.inst.GetCurrentGadget().UpdateAmmoGraphics();
							}
						}
						return o;
					}
				}
			}
		}
		return null;
	}

	bool inventoryAvailable = true;
	public void InventoryAvailable(bool f){
		inventoryAvailable = f;	


	}

	public List<GameObject> GetInventoryItems() {
//		Debug.Log("counting items!");
		List<GameObject> items = new List<GameObject>();
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		//		slots.Add(slotHand.GetComponent<Slot>());
		foreach(Slot s in slots){
			GameObject item = s.GetItem3D();
			if (item) {
//				Debug.Log("Item:"+item.name);
				items.Add(item);
			}
//			if (s.GetItem3D()) items.Add(s.GetItem3D())
//			if (s.transform.childCount > 0){
//				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
//				if (item.item3d) {
//					Debug.Log("item:"+item.item3d);
//					items.Add(item.item3d);
//				}
//			} else {
////				Debug.Log("Slot "+s.name+" empty");
//			}
		}
//		Debug.Log("items count;"+items.Count);
		return items;
	}

	public bool Contains(GameObject o){
		UserEditableObject ueo = o.GetComponent<UserEditableObject>();
		if (ueo){
			if (ueo.myName == "prefab - ammo sphere") return true;
			// All "Ammo" items are considered a part of the player inventory and don't need to be destroyed
		}
		return GetInventoryItems().Contains(o);
	}



	public void LoadInventory(string json){
//		WebGLComm.inst.Debug("UTY load inv");

		if (json == "") return;
//		Debug.Log("loading inv");
		Inventory.inst.ClearInventory();
		bool muted = true; // no sound or fx on laoding inv.
		SimpleJSON.JSONClass N = (SimpleJSON.JSONClass)SimpleJSON.JSONClass.Parse(json);
		if (N.GetKeys().Contains(headKey)){
			SimpleJSON.JSONClass item = (SimpleJSON.JSONClass)N[headKey];
			GameObject o = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName(item["name"].Value);
			UserEditableObject ueo = o.GetComponent<UserEditableObject>();

			ueo.SetProperties(item["properties"] as SimpleJSON.JSONClass);
			CollectItemIntoSlot(slotHead,o,muted);
			UEO_MarketObject clth = o.GetComponent<UEO_MarketObject>();
			clth.EquipClothing();
		}
		foreach(SimpleJSON.JSONClass item in N[backpackKey].AsArray.Childs){
			GameObject o = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName(item["name"].Value);

			UserEditableObject ueo = o.GetComponent<UserEditableObject>();

//			if (o.GetComponent<NumberInfo>()) o.transform.localScale = Vector3.one * NumberManager.inst.numberScale; // todo: Vadim How should I save scale information wrt to props of inventory items? I haven't serialized the scale or other info here..
//			SimpleJSON.JSONClass props = (SimpleJSON.JSONClass)item["properties"];
			ueo.SetProperties(item["properties"] as SimpleJSON.JSONClass);
//			Debug.Log("setting prop:"+item["properties"].ToString());
			CollectItemIntoSlot(backpackSlots[item["position"].AsInt],o,muted);
			Gadget g = o.GetComponent<Gadget>();
			if (g){
//				Debug.Log("gadget;"+g);
				g.SetAmmoJson(item["gadget_ammo"].AsArray);
			}
		}
		foreach(SimpleJSON.JSONClass item in N[beltKey].AsArray.Childs){
			GameObject o = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName(item["name"].Value);
			UserEditableObject ueo = o.GetComponent<UserEditableObject>();


			ueo.SetProperties(item["properties"] as SimpleJSON.JSONClass);
//			Debug.Log("setting prop:"+item["properties"].ToString());

			CollectItemIntoSlot(beltSlots[item["position"].AsInt],o,muted);
			Gadget g = o.GetComponent<Gadget>();
			if (g){
//				Debug.Log("N gadget ammor;"+item["gadget_ammo"].ToString());
				g.SetAmmoJson(item["gadget_ammo"].AsArray);
			}
		}
		StartCoroutine(SelectBeltSpaceE(N[selectedBeltIndexKey].AsInt,.1f));
		gemCount = N[gemCountKey].AsInt;
		UpdateGemCount();
	}
	IEnumerator SelectBeltSpaceE(int ind, float s){
		// It's annoying that I can't do this in the same frame that I load the inventory..
		// I don't remember why! But it had problems
		yield return new WaitForSeconds(s);
		SelectBeltSlot(beltSlots[ind],"E");
	}
	public void SaveInventory(){
//		WebGLComm.inst.Debug("UTY save inv");
		SimpleJSON.JSONClass N = Inventory.inst.ConvertInventoryToJson();
		#if UNITY_EDITOR
		//		LoadLevel(LevelJsonSamples.json1);
		PlayerPrefs.SetString("Inventory",N.ToString());
//		Debug.Log("saved inv;"+N.ToString());
		#else
		WebGLComm.inst.SavePlayerInventory(N.ToString());
		#endif
	}

	public SimpleJSON.JSONClass ConvertInventoryToJson(){
//		List<Slot> slots = new List<Slot>();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[backpackKey] =  new SimpleJSON.JSONArray();
		UserEditableObject u = GetUeoFromSlot(slotHead);
		if (u){
			SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
			item["name"] = u.GetName;
			item["properties"] = u.GetProperties();
			N[headKey] = item;
		}
		for (int i=0;i<backpackSlots.Length;i++){
			Slot s = backpackSlots[i];
			UserEditableObject ueo = GetUeoFromSlot(s);
			if (ueo){
				SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
				item["name"] = ueo.GetName;
				item["properties"] = ueo.GetProperties();
				item["position"].AsInt = i;
//				item[JsonUtil.scaleKey].AsFloat = ueo.transform.localScale;
				Gadget g = ueo.gameObject.GetComponent<Gadget>();
				if (g){
					item["gadget_ammo"] = g.GetAmmoJson();
				}
				N[backpackKey].Add(item);
			} 
		}
		N[beltKey] =  new SimpleJSON.JSONArray();
		for (int i=0;i<beltSlots.Length;i++){
			Slot s = beltSlots[i];
			UserEditableObject ueo = GetUeoFromSlot(s);
			if (ueo){
				
				SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
				item["name"] = ueo.GetName;
				item["properties"] = ueo.GetProperties();
				item["position"].AsInt = i;
//				item[JsonUtil.scaleKey].AsFloat = ueo.transform.localScale;
				Gadget g = ueo.gameObject.GetComponent<Gadget>();
				if (g){
//					Debug.Log("gadget in belt, ammo;"+g.GetAmmoJson().ToString());
					item["gadget_ammo"] = g.GetAmmoJson();
				}
				N[beltKey].Add(item);
			}
		}
		N[selectedBeltIndexKey].AsInt = selectedIndex;
		N[gemCountKey].AsInt = gemCount;	
//		Debug.Log("convert inv to json;"+N.ToString());
		return N;

	}

	UserEditableObject GetUeoFromSlot(Slot s){
		if (s.transform.childCount > 0){
			InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
			if (item && item.item3d){
				UserEditableObject ueo = item.item3d.GetComponent<UserEditableObject>();
				if (ueo){
					return ueo;
				}
			}
		}
		return null;
	}

	public void HideBelt(){
//		Debug.Log("hide belt");
		beltParent.gameObject.SetActive(false);
		beltDefaultPos.gameObject.SetActive(false);
	}
	public void ShowBelt(){
//		Debug.Log("show belt");
		beltParent.gameObject.SetActive(true);	
		beltDefaultPos.gameObject.SetActive(true);
	}
	/*
 * Atomic, specific methods for user actions
 * e.g. "drag and drop" goes through all possible things that may have happened between the object being dragged and the slot
 * selecting belt space only does the "what gadget equipped" update
 * picking up an item should check A) did i run into something that can be collected into the gadget i have equipped B) can the thing be addded
 * 
 * */


	Slot BeltContainsGadgetInSlot(Gadget g){
		foreach(Slot s in beltSlots){
			GameObject item = s.GetItem3D();
			if (item){
				if (item.GetComponent<UserEditableObject>().myName == g.gameObject.GetComponent<UserEditableObject>().myName){
					return s;
				}
			}
		}
		return null;
	}
	public void OnPlayerTouchedObject(GameObject o){
		if (pickupTimeout > 0) {
			return;
		}
		pickupTimeout = .1f; // prevent player from firing too many pickup events 
		Gadget g = o.GetComponent<Gadget>();
		PickUppableObject pip = o.GetComponent<PickUppableObject>();
		if (!pip.CanPickUp) return;
		if (!pip.allowDuplicates){
			foreach(GameObject item in GetInventoryItems()){
				UserEditableObject ueo = item.GetComponent<UserEditableObject>();
				if (ueo.myName == o.GetComponent<UserEditableObject>().myName){
								
					Slot sl = BeltContainsGadgetInSlot(g);
					if (sl){
						SelectBeltSlot(sl,"playertouched");
					}
					float rad = 10f;
					for(int i=0;i<20;i++){
						if (Random.Range(0,1f) > 0.5f) EffectsManager.inst.GemDropX1(o.transform.position+MathUtils.RandomInsideHalfSphere*rad);
						if (Random.Range(0,1f) > 0.9f) EffectsManager.inst.GemDropX10(o.transform.position+MathUtils.RandomInsideHalfSphere*rad);
						if (Random.Range(0,1f) > 0.99f) EffectsManager.inst.GemDropX50(o.transform.position+MathUtils.RandomInsideHalfSphere*rad);
					}
					AudioManager.inst.PlayItemGetSound();
					Destroy(o);
//					PlayerNowMessage.inst.Display("You already have a "+ueo.myName+"!");
					return;
				}

			}
		}

		if (o.GetComponent<PickUppableObjectGem>()) {
			o.GetComponent<PickUppableObjectGem>().PickupGem();
			// Special case for gems is handled here since they are destroyed on pickup .. should handle this more generally
			return; 

		}

//		Debug.Log("collect? :"+o.name+", curgad throw? :"+PlayerGadgetController.inst.ThrowGadgetEquipped()+", curgad:"+PlayerGadgetController.inst.GetCurrentGadget().GetGadgetName()+" ..");

		if (!PlayerGadgetController.inst.ThrowGadgetEquipped()){ 
			// A gadget besides throw gadget was equipped.
			if (PlayerGadgetController.inst.GetCurrentGadget().CanCollectItem(o)){
				// equipped gadget could receive this object
//				Debug.Log("Can collect");
				PlayerGadgetController.inst.GetCurrentGadget().OnCollectItem(o);
			} else {
//				Debug.Log("Cant collect");
				// equipped gadget not compatible with this object. 
//				Debug.Log("collects?");
				Slot collectingSlot = CollectItemIntoInventory(o);
//				Debug.Log("no slots?");
				if (collectingSlot.type == SlotType.Belt && beltSlots[selectedIndex].GetItem3D().GetComponent<PickUppableObject>().priority <= o.GetComponent<PickUppableObject>().priority){
					// The collecting slot was a belt slot.
					// The newly collected gadget had an equal or higher priority to the equipped gadget, so select it.
					SelectBeltSlot(collectingSlot,"second part");
				} else {
					// the object had a lower priority than the gadget equipped, so do nothing after it has been collected.
				}
			}
		} else { 
			// Throw gadget (hand) was equipped.
			Slot collectingSlot = CollectItemIntoInventory(o);
			if (collectingSlot){
				if (collectingSlot.type == SlotType.Belt){
					// object was added to belt.
					
					NumberInfo ni = o.GetComponent<NumberInfo>();
					if (g){
						// The object was a gadget, so equip it immediately.
						g.OnEquip();
//						SelectBeltSlot(collectingSlot);
					} else {
						// Something was collected, and it wasn't a gadget.
						PlayerGadgetController.inst.gadgetThrow.NullifyAmmo();
						PlayerGadgetController.inst.gadgetThrow.OnCollectItem(o);
//						SelectBeltSlot(collectingSlot);
					} 
				} else {
					// The item was put into inventory. No further action needed.
					// full hand
				}
			}
		}
	}

	Slot CollectItemIntoInventory(GameObject o){
		
//		PickUppableObject pip =o.GetComponent<PickUppableObject>();
		Slot availableSlot = GetAvailableSlot(o);

		if (availableSlot){
			CollectItemIntoSlot(availableSlot, o);
		} else {
			PlayerNowMessage.inst.Display("Your inventory is full. Press E to open it.",Player.inst.transform.position);
			// Fail to pick up item
		}
		return availableSlot;
	}

	bool DuplicateItemInBelt(GameObject o){
		// hmm, what's the best way to compare arbitrary item types?
		// For now we only care about resource numbers
		foreach(Slot s in beltSlots){
			GameObject i3 = s.GetItem3D();
			if (i3 && i3.GetComponent<ResourceNumber>() && o.GetComponent<ResourceNumber>()){
				return true;
			}
		}
		return false;
	}

	public bool InventoryFull(){
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		foreach(Slot s in slots){
			if (s.GetItem3D() == null){
				return false;
			}
		}
		return true;
	}

	Slot GetAvailableSlot(GameObject o){
		PickUppableObject pip = o.GetComponent<PickUppableObject>();
		SlotType preference = pip.slotPreferenceType;
		Slot returnSlot = null;
		if (preference == SlotType.Gem){
				
		} else if (preference == SlotType.OneInBelt){
			if (DuplicateItemInBelt(o)){
				returnSlot = FirstAvailableBackpackSlot();
				if (returnSlot == null) returnSlot = FirstAvailableBeltSlot(o);	
			} else {
				returnSlot = FirstAvailableBeltSlot(o);
				if (returnSlot == null) returnSlot = FirstAvailableBackpackSlot();	
			}
		} else if (preference == SlotType.Belt){
			returnSlot = FirstAvailableBeltSlot(o);
			if (returnSlot == null) returnSlot = FirstAvailableBackpackSlot();
		} else {
			returnSlot = FirstAvailableBackpackSlot();
			if (returnSlot == null) returnSlot = FirstAvailableBeltSlot(o);
		}
		return returnSlot;
	}

	Slot FirstAvailableBeltSlot(GameObject o){
		Gadget g = o.GetComponent<Gadget>();
		ResourceNumber n = o.GetComponent<ResourceNumber>();
		for (int i=0;i<beltSlots.Length;i++){
			if (beltSlots[i].item) continue;
			if (i == 0 && (g != null || n != null)) continue; // resource numbers and gadgets dont go in first slot.
			return beltSlots[i];
		}
		return null;
	}

	public Slot FirstAvailableBackpackSlot(){
		foreach(Slot s in backpackSlots){
			if (s.item) continue;
			return s;
		}
		return null;
	}




	public void CollectItemIntoSlot(Slot availableSlot, GameObject o, bool muted = false){
		


//			beltTargetPos = beltDefaultPos;

		availableSlot.Collect(o,muted);

		if (availableSlot.type == SlotType.Belt){
			GameObject equippedItem3d = beltSlots[selectedIndex].GetItem3D();

			if (equippedItem3d == null){
				// no held item, equip new one.
				SelectBeltSlot(availableSlot,"collect item normal");
//				EquipItemInSlot(availableSlot,"collect item normal");
//				Debug.Log("priority;"+p1+","+p2);
			} else {
				// object we picked up is higher priority and could change the user's currently selected belt space
				int p1 = o.GetComponent<PickUppableObject>().priority;
				int p2 = equippedItem3d.GetComponent<PickUppableObject>().priority;
				if (p1 >= p2){
					SelectBeltSlot(availableSlot,"collect item bag");
//					EquipItemInSlot(availableSlot,"collect item bag");	
				}
//				Debug.Log("failed priority;"+p1+","+p2);
			}

		} else {
			
			DisplayBottomText("Press E to open inventory.");
//			Debug.Log("not belt");
		}
		UpdateBeltPosition();

	}



	void ColorizeSlot(Slot selectedSlot){
		HighlightSlot(selectedSlot.transform);
		foreach(Slot s in beltSlots){
			s.slotKeyImage.color = new Color(1,1,1,0.2f);
//			s.slotKeyText.color = Color.black;
		};
		selectedSlot.slotKeyImage.color = Color.yellow;
//		selectedSlot.slotKeyText.color = Color.white;
	}

	int NumberOfItemsInBelt(){
		int items = 0;
		foreach(Slot s in beltSlots){
			if (s.GetItem3D() != null){
				items++;
			}
		}
		return items;
	}

	int timesBeltSelected =0;
	int timesToDisplayBeltHelp = 3;
	float beltHelpInterval = 15f; // don't display belt helps in a row without more than 10 seconds between them.
	float timeLastBeltHelped = -15f; // timestamp the last time we showed belt help
	void SelectBeltSlot(Slot selectedSlot, string source = "default"){
//		Debug.Log("selecting;"+selectedSlot+" from:"+source);

		if (timesBeltSelected < timesToDisplayBeltHelp && NumberOfItemsInBelt() > 1 && Time.time > timeLastBeltHelped + beltHelpInterval ){
			timeLastBeltHelped = Time.time;
			Inventory.inst.DisplayBottomText( "Press 1, 2 or 3 on your keyboard to select");
			timesBeltSelected ++;
		} else {
//			Debug.Log("didn't dipsplay. timessel:"+timesBeltSelected+", time:"+Time.time+", detl:"+timeLastBeltHelped+beltHelpInterval+", numitem belt:"+NumberOfItemsInBelt());
		}
		if (beltSpaceSelectedDelegate != null){
			// This delegate needs to let MatrixFloorSeed know that this slot was just selected so it can check if it is now equipped,
			// even if this belt space was already selected,
			// Because it might have been empty before and not set equipped to true for this object (for example when we first collect this seed object)
			beltSpaceSelectedDelegate(selectedSlot);
		}
		ColorizeSlot(selectedSlot);

		if (selectedSlot == beltSlots[selectedIndex]) {
			// Check gadget throw 
//			Debug.Log("index ;"+selectedIndex);
			return;
		} else {
			
			selectedIndex = GetBeltIndexFromSlot(selectedSlot);
			EquipItemInSlot(selectedSlot,"else ..?");
		}
		if (selectedSlot.GetItem3D()) {
//			Debug.Log("delegate fired, sel space:"+selectedSlot.GetItem3D().name);
		} else {
//			Debug.Log("selected empty slot.");
		}
		GameObject item2D = selectedSlot.GetItem2D();
		if (item2D){
			ShrinkToSize ss = item2D.GetComponent<ShrinkToSize>();
			if (!ss) ss = item2D.gameObject.AddComponent<ShrinkToSize>();
			item2D.transform.localScale = Vector3.one * justSelectedScale;
				
//			SinGrow sg = item2D.AddComponent<SinGrow>();
//			sg.SetAttr(1.3f,3.5f);
		}
	}

	void EquipItemInSlot(Slot selectedslot, string source ="dfault"){
//		Debug.Log("equip;"+selectedslot+" from:"+source);
		GameObject item3d = selectedslot.GetItem3D();
		if (item3d){
			Gadget gadget =  item3d.GetComponent<Gadget>();
			if (gadget &&  gadget.GetType() != typeof(GadgetThrow)){
				gadget.OnEquip();
			} else {
				// an object that was not a gadget was in the slot selected, so equip it into throw gadget
				PlayerGadgetController.inst.gadgetThrow.OnEquip();
				PlayerGadgetController.inst.gadgetThrow.NullifyAmmo();
//				Debug.Log("gadgt throw equipping:"+item3d.name);
				PlayerGadgetController.inst.gadgetThrow.OnCollectItem(item3d);
			}
			lastEquippedItem = item3d;
		} else {
			// Nothing in slot, equip empty hand
			PlayerGadgetController.inst.gadgetThrow.NullifyAmmo();
			PlayerGadgetController.inst.gadgetThrow.OnEquip();
		}
	}

	public GameObject lastEquippedItem;
	public GameObject equippedItem {
		get {
			if (beltSlots[selectedIndex].GetItem3D() && beltSlots[selectedIndex].GetItem3D() == lastEquippedItem){
				return lastEquippedItem;
			} else return null;
		}

	}
	public void UpdateBeltPosition(GameObject o = null){
//		Debug.Log("update:");
		if (inventoryOpen) beltTargetPos = beltOpenPos;
		else if (
			GetInventoryItems().Count > 0 && 
			!CanvasMouseController.inst.gameStartedDialogueShowing && 
			!PlayerDialogue.inst.showing
		) {
			beltTargetPos = beltDefaultPos;

//			Debug.Log("belt target;"+beltTargetPos.name);
		}
		else { 
//			Debug.Log("belt disabled. Count;"+GetInventoryItems().Count+", gamestartedd:"+CanvasMouseController.inst.gameStartedDialogueShowing+", playdishow:"+PlayerDialogue.inst.showing);
			beltTargetPos = beltDisabledPos;
			bottomText.SetActive(false);
		}
		beltMoving = true;

	}

	public void DropSomeNumbers(float chance = 0.7f){
		List<Slot> slots = new List<Slot>();
		slots.AddRange(backpackSlots);
		slots.AddRange(beltSlots);
		AudioManager.inst.PlayWrongAnswerError(Player.inst.transform.position,1,1);
		foreach(Slot s in slots){
			if (s.transform.childCount > 0){
				InventoryItem item = s.transform.GetChild(0).GetComponent<InventoryItem>();
				if (item && item.item3d && !item.item3d.GetComponent<ResourceNumber>() && !item.item3d.GetComponent<Gadget>()){ // TODO: add IsDroppableByEnemyAction bool to PickUppableObject?
					NumberInfo ni = item.item3d.GetComponent<NumberInfo>();
					if (ni){
						if (Random.Range(0,1f)<chance) {
							Vector2 randCirc = Random.insideUnitCircle;
							float dist = 2f;
							float forwardoffset = 3f;
							float upOffset = 2f;
							item.item3d.transform.position = Player.inst.transform.position + new Vector3(randCirc.x,upOffset,randCirc.y) * dist + Player.inst.transform.forward * forwardoffset;
							item.item3d.SetActive(true);
							EffectsManager.inst.CreateSmallPurpleExplosion(item.item3d.transform.position,1,1);
							Rigidbody rb = item.item3d.GetComponent<Rigidbody>();
							if (!rb) rb = item.item3d.AddComponent<Rigidbody>();
							rb.isKinematic = false;
							rb.useGravity = true;
							rb.transform.localScale *= 0.6f;
							rb.drag = 0;
							GameObject plusTrail = SMW_GF.inst.CreatePlusTrail(rb.transform.position);
							plusTrail.transform.parent = rb.transform;
							TimedObjectDestructor tod = rb.gameObject.AddComponent<TimedObjectDestructor>();
							tod.DestroyNow(5f);

							Collider col = rb.GetComponent<Collider>();
//							if (!col) col = rb.gameObject.AddComponent<Collider>();
							if (col) col.enabled = false;
//							col.isTrigger =  false;

							float force = Random.Range(2250f,4200f);
							float upForce = 0.6f;
							rb.AddForce((Utils.FlattenVector(rb.transform.position - Player.inst.transform.position) + Vector3.up*upForce) * force);
							s.ClearSlot();

						}
					}
				}
			}
		}
		UpdateBeltSelection();
		PlayerGadgetController.inst.GetCurrentGadget().UpdateAmmoGraphics();
	}
	public void DisableBelt(){
		beltTargetPos = beltDisabledPos;
		beltParent.transform.position = beltDisabledPos.position;
	}

	public bool BeltContainsANumber(){
		foreach(Slot s in beltSlots){
			GameObject itm = s.GetItem3D();
			if (itm && itm.GetComponent<NumberInfo>() && !itm.GetComponent<ResourceNumber>()){
				return true;
			}
		}
		return false;
	}

	float bottomTextTimer = 0;
	public void DisplayBottomText(string s, Color? c = null){
		bottomTextFading = false;
		bottomText.SetActive(true);
		bottomTextBackboard.SetActive(true);
		bottomTextBackboard.GetComponent<Image>().color = Color.black;

		bottomText.GetComponent<Text>().color = c ?? Color.yellow;
		bottomText.GetComponent<Text>().text = s;
		bottomTextTimer = 7f;
		bottomText.GetComponent<SimpleTypewriter>().InitText();

	}

	public void HideBottomText(){
		bottomTextTimer = 0;
	}

	public void UpdateGemCount(){
		gemCountText.text = gemCount.ToString();
		gemCountTextLevelBuilder.text = gemCount.ToString(); // it's smaller version when editor is open, so two text objects are kept with same text always
		gemIcon.transform.localScale = Vector3.one * 1.2f;
	}

	public bool AddToGems(int amount){
		if (gemCount + amount < 0) {
			return false; // can't subtract more gems than we have, e.g. can't afford this purchase
		}
		gemCount += amount;
		UpdateGemCount();
		return true;
	}
}


namespace UnityEngine.EventSystems {
	public interface IHasChanged : IEventSystemHandler {
		void HasChanged();
	}
}