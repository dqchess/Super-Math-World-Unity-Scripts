using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class HeatmapStageTracker : MonoBehaviour {

	public Text stages;


	public void TrackStage(int passed, int failed, int avgSeconds, int minSeconds, int maxSeconds){
		stages.text += "\n:" + passed +", "+failed+ ", "+Utils.DisplayAsTimeFromSeconds(avgSeconds)+", "+Utils.DisplayAsTimeFromSeconds(minSeconds)+", "+Utils.DisplayAsTimeFromSeconds(maxSeconds);
	}
}
