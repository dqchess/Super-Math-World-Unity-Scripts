using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapStager : MonoBehaviour {

	// This is an item placed down during Heatmap mode on the eidtor.
	// it blocks the path of heatmap avatars, causing them to freeze until this object is destroyed.
	// This allows a group of students to be "staged" in between challenges, so we can see everyone start off on challenge number 3 from an equal time.
	// Without this object to stage the group, the group gets very spread out over >3 challenges so that laggards are not even to challenge 3 yet but fast students are already at challenge 5

	List<HeatmapAvatar> frozen = new List<HeatmapAvatar>();
	List<float> times = new List<float>();
	List<HeatmapAvatar> all = new List<HeatmapAvatar>();
	void Start(){
		foreach(HeatmapData hd in FindObjectOfType<HeatmapManager>().heatmaps){
			all.Add(hd.avatar);
		}
		Debug.Log("all:"+all.Count);
	}

//	void OnTriggerEnter(Collider other){
//		HeatmapAvatar ha = other.GetComponent<HeatmapAvatar>();
//		if (ha){
//			ha.frozen = true;
//			frozen.Add(ha);
//		}
//	}

	int blackHoleCount = 0;
	public void BlackHoleDetected(){
		blackHoleCount ++;
		// A player dropped off. The count gets reset for all Stagers whenever a stager is destroyed, ensuring that black holes are only counted per stager and not double counted.
		// Note this means you cannt destroy any stager unless you want all subsequent black holes to be counted towards the next stager.
	}

	public void AdvanceTimer(float speedFactor){
		timer += Time.deltaTime * speedFactor;
	}


	int minSeconds = 0;
	int maxSeconds = 0;
	int averageSeconds = 0;
	void OnDestroy(){
		FindObjectOfType<HeatmapManager>().SetSpeedFactor(1); // set time to normal.
		float a = 0;
		foreach(float f in times){
			a += f;
		}
		averageSeconds = Mathf.RoundToInt(a/times.Count);
		FindObjectOfType<HeatmapStageTracker>().TrackStage(frozen.Count,blackHoleCount,averageSeconds,minSeconds,maxSeconds);
		foreach(HeatmapAvatar ha in frozen){
			ha.frozen = false;
		}
		foreach(HeatmapStager hs in FindObjectsOfType<HeatmapStager>()){
			hs.blackHoleCount = 0;
			hs.timer = 0;
		}

	}

	public float timer = 0;
	void Update(){
		
//		Debug.Log("bounds;"+GetComponent<Renderer>().bounds);
		foreach(HeatmapAvatar hd in all){
			if (frozen.Contains(hd)) continue;
			bool c = GetComponent<Renderer>().bounds.Contains(hd.transform.position);
//			Debug.Log("hd trnsf;"+hd.transform.position+". inbounds?:"+c);
			if (c){
				if (frozen.Count == 0){
					// first one
					minSeconds = Mathf.RoundToInt(timer);
				}
//				Debug.Log("FREEZE:"+hd.name);
				frozen.Add(hd);
				hd.frozen = true;
				times.Add(timer);
				maxSeconds = Mathf.RoundToInt(timer); // just overwrite for each successive hit
			}
		}
	}
}
