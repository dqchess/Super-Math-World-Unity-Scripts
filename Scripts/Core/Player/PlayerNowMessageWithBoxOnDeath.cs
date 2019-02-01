using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNowMessageWithBoxOnDeath : MonoBehaviour, IDestroyedByPlayer {

	public static string textTriggerKey = "textBoxOnDeathKey";
	public string thingToSay;
	public Sprite icon;
	public void DestroyedByPlayer(){
//			float duration = thingToSay.Length / 6f;
		PlayerNowMessageWithBox.inst.Display(thingToSay,icon,transform.position);

	}
}
