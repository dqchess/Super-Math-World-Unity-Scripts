using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;


public class NumberHoopSoaper : NumberHoop {


	#region UserEditable 



	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> elements = new List<GameObject>();
		elements.AddRange(base.GetUIElementsToShow());
		elements.Add(LevelBuilder.inst.POCMcopyButton);
		elements.Add(LevelBuilder.inst.POCMheightButton);
		return elements.ToArray();

	}

	#endregion



	public override void UseHoop(GameObject obj, RecordPosition record, bool direction){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		base.UseHoop(obj,record,direction);
		NumberInfo ni = obj.GetComponent<NumberInfo>();

		Player p = obj.GetComponent<Player>();

		// Set number only if player is holding it.
		if (p){
			if (PlayerGadgetController.inst.ThrowGadgetEquipped()){
				if (GadgetThrow.inst.numberHeld){
					ni = GadgetThrow.inst.numberHeld.GetComponent<NumberInfo>();
				}
			}
		}

		if (ni){
			if (ni.IsSoapable()){
				if (direction){
					ni.SoapNumber();
				} else {
					ni.UnSoapNumber();
				}
			}
		} 

		if (p) {
			GadgetThrow.inst.UpdateAmmoGraphics();	
		}

	}


}






