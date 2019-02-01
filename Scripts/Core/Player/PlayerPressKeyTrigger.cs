using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerPressKeyTrigger : MonoBehaviour {


	public string keyToPress = "w"; // fragile: only use w, space, shift, e
	public string function = "walk forwards";

	bool used = false;

	void OnTriggerEnter(Collider other){
		if (!used){
			used = true;
			if (other.tag == "Player"){
				PressKeyDialogue.inst.Display(keyToPress,function);
				//			float duration = thingToSay.Length / 6f;
				//			PlayerNowMessage.inst.Display(thingToSay,Color.white,other.transform.position,icon);
			}
		}
	}
}
