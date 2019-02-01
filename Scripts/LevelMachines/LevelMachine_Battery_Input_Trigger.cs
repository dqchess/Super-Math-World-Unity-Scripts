using UnityEngine;
using System.Collections;

public class LevelMachine_Battery_Input_Trigger : MonoBehaviour {

	public LevelMachineBattery lmb;

	public void OnTriggerEnter(Collider other){
//		// commented Debug.Log ("Trig: "+other.name);
		lmb.OnReceivedNumber(other);
		
	}
}
