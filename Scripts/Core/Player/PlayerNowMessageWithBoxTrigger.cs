using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNowMessageWithBoxTrigger : MonoBehaviour {

	public static string textTriggerKey = "textWithBoxTriggerKey";
	public string thingToSay;
	public Sprite icon;
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
//			float duration = thingToSay.Length / 6f;
			PlayerNowMessageWithBox.inst.Display(thingToSay,icon,other.transform.position);
		}
	}
}
