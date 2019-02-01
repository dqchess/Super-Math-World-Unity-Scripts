using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UEO_MarketObject : UEO_SimpleObject, IMyDragEnded {

	public string itemName = "hat name";
	public int price = 1000;
	public GameObject hatGraphics; // created when equipped into slotHEad only.

	public void DragEnded(Slot s){
//		Debug.Log("dragend here");
		// drag ended. Update the status of the graphics prefab.	
		// note this only works for hats right now, need to differentiate bewteen types of clothing for the future.
		if (hatGraphics) {
//			Debug.Log("Dest hat graph");
			Destroy(hatGraphics);
		}
		if (s && s.type == SlotType.Head){
//			Debug.Log("equip hat");
			EquipClothing(s);
		}
	}
	public void EquipClothing(Slot s = null){
//		Debug.Log("equipping..");
//		if (hatGraphics) Destroy(hatGraphics); // clear hat graphics in any case
//		if (s && s.type == SlotType.Head){
		if (hatGraphics) Destroy(hatGraphics);
		hatGraphics = (GameObject)Instantiate(gameObject); // clone of yourself
		Destroy(hatGraphics.GetComponent<Collider>());
		Destroy(hatGraphics.GetComponent<UEO_MarketObject>()); // remove relevant scripts

		hatGraphics.transform.parent = PlayerCostumeController.inst.curCharInfo.playerHead;
//			hatGraphics.transform.localPosition = new Vector3(0,0.165f,0);
		hatGraphics.transform.localPosition = Vector3.zero;
		hatGraphics.transform.localRotation = Quaternion.identity;
		hatGraphics.transform.localScale = Vector3.one * 0.5f;
		hatGraphics.SetActive(true);
//		}
	}

	public override void OnDestroy(){
		base.OnDestroy();
		if (hatGraphics) Destroy(hatGraphics);
	}


}
