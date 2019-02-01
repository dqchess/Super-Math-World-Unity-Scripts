using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCompletedLevelTrigger : MonoBehaviour {



	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
//			float duration = thingToSay.Length / 6f;
			WebGLComm.inst.PlayerCompletedLevel();
			AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.levelCompleted,1);
			AnalyticsManager.inst.SendAnalytics();
//			PlayerNowMessage.inst.Display(thingToSay,other.transform.position);
		}
	}
}
