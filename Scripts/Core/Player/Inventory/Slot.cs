using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;



public class Slot : MonoBehaviour, IDropHandler {
	public bool allowNumbers = true;
	public bool allowGadgets = true;
	public bool allowHat = false; // slottype head and allowhat bool is redundant / inefficient
	public bool allowArmor = false;
	public bool allowBoots = false;
	public int index;
	public Image slotKeyImage;
	public Text slotKeyText;
	public SlotType type = SlotType.Backpack;

	public GameObject item {
		get {
			if(transform.childCount>0){
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	#region IDropHandler implementation
	public virtual void OnDrop (PointerEventData eventData)
	{
//		// commented Debug.Log("drop.");
		if (DragHandler.itemBeingDragged == item) {
			// commented Debug.Log("Dropped item onto the same location in inventory");
			return;
		}
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.timesItemDroppedOnInventorySlot,1);
		// Does it pass the type check?
		GameObject draggedItem3d = DragHandler.itemBeingDragged.GetComponent<InventoryItem>().item3d;
		NumberInfo ni = draggedItem3d.GetComponent<NumberInfo>();
		Gadget gp = null;
		if (item && item.GetComponent<InventoryItem>() && item.GetComponent<InventoryItem>().item3d.GetComponentInChildren<Gadget>()){
			gp = item.GetComponent<InventoryItem>().item3d.GetComponentInChildren<Gadget>();
		}
		Gadget gad = draggedItem3d.GetComponent<Gadget>();
		ClothingArmor ca = draggedItem3d.GetComponent<ClothingArmor>();
		ClothingBoots cb = draggedItem3d.GetComponent<ClothingBoots>();
		ClothingHat ch = draggedItem3d.GetComponent<ClothingHat>();

		if (draggedItem3d.GetComponent<PickUppableObject>().itemType == ItemType.Hat && !allowHat){
			return;
		}

		if ((ni && !allowNumbers)
			|| (gad && !allowGadgets)
			|| (ca && !allowArmor)
			|| (cb && !allowBoots)
			|| (ch && !allowHat)){
			return;
		} 
//		else Debug.Log(" allowed! gadg?:"+gp+", allowgad:"+allowGadgets);
//		// commented Debug.Log("gp:"+gp);
		// If we successfully dropped a number onto the equipped empty hand
		int thisBeltIndex = -1;
		for(int i=0;i<Inventory.inst.beltSlots.Length;i++){
			if (this == Inventory.inst.beltSlots[i]){
				thisBeltIndex = i;
//				if (Inventory.inst.CompareSlot(Inventory.inst.beltSlots[thisBeltIndex],Inventory.inst.beltSlots[Inventory.inst.selectedIndex])){
//					
//				}
			}
		}

		if (item){
			// dropped one item onto another
			if (ni && !NumberManager.IsCombineable(ni)) return; //SwapItems(item,draggedItem3d);
//			NumberManager.IsCombineable(ni))
			NumberInfo ni2 = item.GetComponent<InventoryItem>().item3d.GetComponent<NumberInfo>();
			if (ni2 && !NumberManager.IsCombineable(ni2)) return; //SwapItems()

			if (ni && ni2){ // Dragged one number onto another
				Fraction result = Fraction.Add(ni.fraction,ni2.fraction);
//				Debug.Log("drag successful:"+ni.fraction+" dragged to "+ni2.fraction+"="+result);
				if (result.numerator == 0) {
					ni.ZeroFX(ni.transform.position);
					PlayerGadgetController.inst.GetCurrentGadget().CheckAmmoNull(ni2.gameObject,result);
					Destroy(draggedItem3d);
					Destroy(DragHandler.itemBeingDragged);
					Destroy(item);
					AudioManager.inst.PlayNumberShatter(Player.inst.transform.position);
					EffectsManager.inst.CreateShards(Player.inst.transform.position + Player.inst.transform.forward*3 + Player.inst.transform.up);
					return;
				}
				ni2.SetNumber(result);
				item.GetComponent<InventoryItem>().SetUpDigits(ni2);
				if (PlayerGadgetController.inst.ThrowGadgetEquipped()){

					PlayerGadgetController.inst.GetCurrentGadget().UpdateAmmoGraphics();
				}
				SinGrowFX(item);

				AudioManager.inst.PlayNumberEat(Player.inst.transform.position);
				ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject,null,(x,y) => x.HasChanged ());
				Destroy(draggedItem3d);
				Destroy(DragHandler.itemBeingDragged);
			}
			if (gp){
				// Dropped a number onto a gadget.
				if (ni && !NumberManager.IsCombineable(ni)) return; // don't allow gadget to collect invalid numbers.
//				Debug.Log("dropped:"+ni.fraction+"o onto:"+gp);
//				gp.DropAmmo();
				if (gp.IsValidAmmo(ni.gameObject)){
					gp.DropAmmo();
					SinGrowFX(item);

					gp.OnCollectItem(ni.gameObject);
//					Debug.Log("gadget "+gp.myName+"collected:"+ni.gameObject);
					AudioManager.inst.PlayNumberEat(Player.inst.transform.position);
					ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject,null,(x,y) => x.HasChanged ());
//					Destroy(draggedItem3d); // this is handled in the gadget's oncollect method, some gadgets do not destroy the number but set it directly as the numberheld e.g. wavegadget
//					Debug.Log("finishing drag of item3d:"+draggedItem3d+" and itembeingdragged;"+DragHandler.itemBeingDragged);
					Destroy(DragHandler.itemBeingDragged);
				} else {
					
				}
			}
		} else { // if(!item){
			// dropped onto an empty slot.
			DragHandler.itemBeingDragged.transform.SetParent (transform);
//			Debug.Log("set "+DragHandler.itemBeingDragged+" to "+transform+" parent");
//			ExecuteEvents.ExecuteHierarchy<IHasChanged>(gameObject,null,(x,y) => x.HasChanged ()); // prevent the item from returning to original slot maybe?
		}
		// If we dragged something to the belt space we had selected, then reselect it to update that space.
//		if (Inventory.inst.ThisSpaceSelected(this)){
//			Inventory.inst.SelectSpace(this);
//
//			// commented Debug.Log("Hi");
//			//			Inventory.inst.SelectBeltSpace(Inventory.inst.selectedIndex);
//		} 

		Inventory.inst.UpdateBeltSelection();
	}

	public void Collect(GameObject o, bool muted=false){
//		Debug.Log("Adding "+o+" to slot "+this.name);
		// was AddItemToSlot
		// Note that ITEM is the 2D item we see in inventory
		// and ITEM3D is the item that still exists in the game world but is now deactivated.
		PickUppableObject pip = o.GetComponent<PickUppableObject>();
		pip.OnInventoryCollect(muted);
		NumberInfo ni = o.GetComponent<NumberInfo>();
		GameObject inventoryItem = (GameObject)Instantiate(Inventory.inst.itemPrefab);
		// Removing hand
		if (ni){
			// Make the inventory icon for NumberInfos only.
			inventoryItem.GetComponent<InventoryItem>().SetUpDigits(ni);
		} else if (pip.inventoryIcon != null) {
			inventoryItem.GetComponent<Image>().sprite = pip.inventoryIcon;
			if (pip.useColor == true){
				inventoryItem.GetComponent<Image>().color = pip.inventoryIconColor;
			}
		} 
		inventoryItem.transform.SetParent(transform,false); //parent = emptySlot.transform;
		inventoryItem.GetComponent<InventoryItem>().item3d = o;
		foreach(IMyPickupable im in o.GetComponents(typeof(IMyPickupable))){
			im.OnPlayerPickup();
		} 
		o.SetActive(false);



	}

	public GameObject GetItem2D(){
		if (transform.childCount > 0){
			return transform.GetChild(0).gameObject;
		}
		return null;
	}

	public GameObject GetItem3D(){
		if (transform.childCount > 0){
			InventoryItem ii = transform.GetChild(0).gameObject.GetComponent<InventoryItem>();
			if (ii) return ii.item3d;
		}
		return null;
	}



	public void ClearSlot(){
		if (GetItem3D()){ 
			// null the ref to the item3d, don't destroy it
			InventoryItem ii = transform.GetChild(0).gameObject.GetComponent<InventoryItem>();
			if (ii) ii.item3d = null;
		}
		if (GetItem2D()){ Destroy(GetItem2D()); }
	}

	void SinGrowFX(GameObject item){
		SinGrow sg = item.AddComponent<SinGrow>();
		sg.SetAttr(1.6f,3.5f); // slowed down a bit

	}
	#endregion
}