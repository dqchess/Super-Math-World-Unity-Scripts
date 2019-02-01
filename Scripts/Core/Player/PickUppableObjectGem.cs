using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class PickUppableObjectGem : PickUppableObject { // Inventory item



	public int value = 1;
	public void PickupGem(){
		PlayPickupSound();
		Inventory.inst.AddToGems(value);
		Destroy(this.gameObject);
	}
}

