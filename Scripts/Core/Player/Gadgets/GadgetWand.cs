using UnityEngine;
using System.Collections;

public class GadgetWand : Gadget {

	public override void Start(){
		Init();


	}


	public override void Init(){
		if (!initiated){
//			// commented Debug.Log ("wand init");
			initiated = true;
//			canThrow = false;
//			// commented Debug.Log ("gadg can throw:"+GetComponent<Gadget>().canThrow);
		}
	}



	public override void OnPlayerAction(){
//		// commented Debug.Log ("on player action!");
//		FindObjectOfType<SpellButtonUI>().OpenSpellUI();
//		SMW_GF.inst.CreateCircleOfNumbers(transform.position);
	}

}
