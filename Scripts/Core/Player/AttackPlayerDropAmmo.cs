using UnityEngine;
using System.Collections;

public class AttackPlayerDropAmmo : MonoBehaviour {

	void OnExplode(){
		float radius=20;
		Collider[] cols= Physics.OverlapSphere(transform.position,radius);
		foreach(Collider c in cols){
			if (c.tag=="Player"){
//				GlobalVars.inst.pnc.DropAmmo();
				Inventory.inst.ClearNumbersFromInventory();
			}
		}
	}
}
