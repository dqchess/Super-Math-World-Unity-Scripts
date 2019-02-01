using UnityEngine;
using System.Collections;

public class AttackPlayerModifyAmmo : MonoBehaviour {



	void OnExplode(){
		float radius=20;
		Collider[] cols= Physics.OverlapSphere(transform.position,radius);
		foreach(Collider c in cols){
			if (c.tag=="Player"){
//				GlobalVars.inst.pnc.DropAmmo();
				NumberInfo ni = GetComponent<NumberInfo>();
				NumberModifier.ModifyOperation nmf = (x => x);
				nmf = (x => Fraction.Add(x, ni.fraction));
//				GlobalVars.inst.inv.ClearInventory();


				Inventory.inst.ModifyInventoryItems(nmf);
//				GlobalVars.inst.pnc.ModifyAmmoNumbers(nmf);
			}
		}
	}
}
