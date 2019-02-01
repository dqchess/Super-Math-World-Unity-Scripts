using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketHat : Market {

	public Sprite icon;

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Player>()){
			if (Inventory.inst.InventoryFull()){
				PlayerNowMessageWithBox.inst.Display("I'd love to sell you some hats, but your inventory is full! Press E to oepn inventory, and drop some items!",icon,transform.position);
			} else {
				PlayerDialogue.inst.ShowPlayerDialogue("Would you like to shop for hats?","Hat Hovel",icon);
				PlayerDialogue.inst.playerPressedOKDelegate += OpenStore;
				PlayerDialogue.inst.playerPressedCancelDelegate += PlayerCanceled;
				PlayerDialogue.inst.playerPressedBackboardDelegate += PlayerCanceled;
			}
		}
	}

	void OpenStore(){
		PlayerDialogue.inst.playerPressedOKDelegate -= OpenStore;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerCanceled;
		PlayerDialogue.inst.playerPressedBackboardDelegate -= PlayerCanceled;
		MarketUI.inst.ShowMarket(itemsForSale);


	}

	void PlayerCanceled(){
		PlayerDialogue.inst.playerPressedOKDelegate -= OpenStore;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerCanceled;
		PlayerDialogue.inst.playerPressedBackboardDelegate -= PlayerCanceled;
	}
}
