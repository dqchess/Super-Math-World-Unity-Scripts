using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNowMessageTrigger : MonoBehaviour {

	public static string textTriggerKey = "textTriggerKey";
	public string thingToSay;
	public Sprite icon;
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
//			float duration = thingToSay.Length / 6f;
			PlayerNowMessage.inst.Display(thingToSay,Color.white,other.transform.position,icon);
		}
	}
}
