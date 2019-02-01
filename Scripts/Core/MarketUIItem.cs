using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarketUIItem : MonoBehaviour {

	public Text name;
	public Text price;
	public Image icon;
	public UEO_MarketObject item;

	public void ClickItem(){
		if (Inventory.inst.gemCount > item.price){
			PlayerDialogue.inst.playerPressedOKDelegate += PlayerPressedOK;
			PlayerDialogue.inst.playerPressedCancelDelegate += PlayerPressedCancel;
			PlayerDialogue.inst.ShowPlayerDialogue("Would you like to buy this for "+price.text+" gems?",name.text,icon.sprite);
		} else {
			PlayerDialogue.inst.playerPressedOKDelegate += PlayerPressedCancel;
			PlayerDialogue.inst.playerPressedCancelDelegate += PlayerPressedCancel;
			PlayerDialogue.inst.ShowPlayerDialogue("You can't afford this item! ("+price.text+" gems)",name.text,icon.sprite);
		}
	}

	void PlayerPressedOK(){
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
		Slot availableSlot = Inventory.inst.FirstAvailableBackpackSlot();
		if (availableSlot == null){
			MarketUI.inst.Message("Your inventory is full so you can't buy this item.");
		}
		if (Inventory.inst.AddToGems(-item.price)){
			GameObject newItem = (GameObject)Instantiate(item.gameObject);
			Inventory.inst.CollectItemIntoSlot(availableSlot,newItem);
			MarketUI.inst.Message("Thanks for your purchase! Your new item is now in your inventory.");
		} else {
			MarketUI.inst.Message("You don't have enough gems for that!");
		}

	}

	void PlayerPressedCancel(){
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
	}
}
