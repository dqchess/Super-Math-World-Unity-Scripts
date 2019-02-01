using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatmapData {
	public string name;
	public string cls;
	public HeatmapAvatar avatar; // this is the avatar we will instantiate, need to delete it at some point. Note costume JSON is "stored" because we set cosutme on instantiating this obj
	public string avatarJson = "";
//	public List<Vector3> positions = new List<Vector3>(); // new list item every 5 seconds of player time
	public GameObject legendObject; // which 2d legend object corresponds to this 3d avatar object?
//	public int startTime; //from heatmap.session.start_time, converted to unix time.

}

public class HeatmapManager : MonoBehaviour {

	// This script (accessible from the Heatmap button in LevelBuilder)
	// will pull heatmaps from the server for this level and present a UI for options for showing this heatmap data.
	// UI will allow you to show all heatmaps for all time on this level, or iterate through heatmaps linerally and progress through real time
	// heatmap data shows avatar json and position every 5 seconds, so we use a non-skinned mesh renderer prefab to indicate the player (skinned is expensive in masse)
	// then we lerp the position to the next point on the heatmap
	// starting times are grabbed from session information

	string sampleData = "";

	public static int heatmapTimeInterval = 5;
//	public Transform controlsTransformParent; // for enabling controls on startup
	public Text speed;
	public Text time;
	public Transform legendList;
	public GameObject legendObjectPrefab;
	public GameObject legendObjectPrefabClass;
	public ScrollRect legendScroll;

	void Start(){
	}

	void OnEnable(){
		
//		DisableControls();
		#if UNITY_EDITOR
		sampleData = Utils.LoadStringFile("heatmap.txt");
		SetHeatmapData(sampleData);
		#else
		GetHeatmapDataForLevel();
		#endif

	}

	public GameObject avatarPrefab; // should be unskinned but it isn't right now
	public static HeatmapManager inst;

	public void SetInstance(){
		inst = this;
	}

	public List<HeatmapData> heatmaps = new List<HeatmapData>();

	public void GetHeatmapDataForLevel(){
		WebGLComm.inst.GetHeatmapDataForLevel();
	}

	public void SetHeatmapData(string json){
//		Debug.Log("json:"+json);
		SimpleJSON.JSONArray hms = (SimpleJSON.JSONArray)SimpleJSON.JSONArray.Parse(json).AsArray; // get list of objects from string memory and convert to json array
		Debug.Log("heatmap obj:"+hms.ToString());
		foreach(SimpleJSON.JSONClass n in hms){

			// Create a new heatmap avatar data class and populate it
			HeatmapData data = new HeatmapData();
			// Instantaite and 
			data.avatar = (HeatmapAvatar)Instantiate(avatarPrefab).GetComponent<HeatmapAvatar>() as HeatmapAvatar;
			data.avatarJson = n["avatarJson"];
			foreach(SimpleJSON.JSONClass nn in n["positions"]["points"].AsArray.Childs){
				// Add all the heatmap posiitons to the avatar class list so we can iterate through them and move the 3d avatar around
				if (nn["pos"].ToString().Contains(",")){
					data.avatar.positions.Add(JsonUtil.GetRealPositionFromTruncatedPosition(nn["pos"]));
				}
			}
			data.name = n["name"];
			data.cls = n["class"];
			heatmaps.Add(data);


			// set costume and colors of 3d Avatar object
			data.avatar.transform.position = data.avatar.positions[1];
			SimpleJSON.JSONClass avatarJson = (SimpleJSON.JSONClass)SimpleJSON.JSONClass.Parse(data.avatarJson);
			Color bodyColor = PlayerCostumeController.inst.allMaterials[avatarJson["BodyColorIndex"].AsInt].color;

			data.avatar.SetHeatmapAvatarProperties(bodyColor,data.name);
			data.avatar.gameObject.name = "instnaced avatar ata time;"+Time.time;
			data.avatar.GetComponentInChildren<PlayerCostumeController>().SetCharacterMaterials(data.avatarJson);

			// Instantaite a 2d legend item for managing this avatar
			GameObject legendObj =(GameObject) Instantiate(legendObjectPrefab);
//			Debug.Log("made legend.:"+data.avatar.name);
			// Was a class already existing for this legend object?
			bool parentWasSet = false;
			GameObject legendObjClass = null; 
			foreach(HeatmapLegendItemClass clss in FindObjectsOfType<HeatmapLegendItemClass>()){
				if (clss.className == data.cls){
					legendObjClass = clss.gameObject;
					legendObj.transform.SetParent(clss.transform); // all avatar legend objs are children of classes
					parentWasSet = true; // We are done, no need to create cls object
				}
			}
			if (!parentWasSet){
				// no cls obj found, create cls object
				legendObjClass = (GameObject)Instantiate(legendObjectPrefabClass);
				HeatmapLegendItemClass clss = legendObjClass.GetComponent<HeatmapLegendItemClass>();
				clss.className = data.cls; // set the name of this class object (there will only be one class object with this name)
				clss.classNameText.text = data.cls; // redundant but is the visiable 2d text obj not the hidden string ..lol
				legendObjClass.transform.SetParent(legendList); // classes are a child of the master legend list, all avatar legend objs are children of classes
				legendObj.transform.SetParent(legendObjClass.transform);
			}

			// Finally set the height of the class item based on number of items in that class, so that heights all line up
			if (legendObjClass){ // should have been set from either finidng the pre-existing class obj in the foreach, or creating on if !parentWasSet
				legendObjClass.GetComponent<HeatmapLegendItemClass>().UpdateRectHeight();
			}

			HeatmapLegendItem legendItem = legendObj.GetComponent<HeatmapLegendItem>();
			string totalTime = Utils.DisplayAsTimeFromSeconds(data.avatar.positions.Count*heatmapTimeInterval);
			Color hairColor = PlayerCostumeController.inst.allMaterials[avatarJson["HairColorIndex"].AsInt].color;
			Color headColor = PlayerCostumeController.inst.allMaterials[avatarJson["HeadColorIndex"].AsInt].color;
			legendItem.SetHeatmapLegendProperties(data.avatar,data.name,data.cls,totalTime,bodyColor,headColor,hairColor);
			data.legendObject = legendObj;

			if (!snappedThisPlay){ //only do once.
				snappedThisPlay = true;
				LevelBuilder.inst.SnapPanToPosition(data.avatar.transform.position);
			}
			if (debug) WebGLComm.inst.Debug("Created;"+data.avatar.name);

			// Create the objects from string memory.

			// Note that this creates the peice wherever it was in world space when the copy was made, so we'll reposition the items to current mouse pos after creation.
		}

		// Whew, we're done populating heatmap data and 2d and corresponding map 3d objects!
		// Now calculate how high the 2d content is so it all fits in the scrollRect!

		float avgChildHeight = 40f;
		float heatmapsHeight = 500 + Mathf.Max(0,(heatmaps.Count-8)*avgChildHeight); // each heatmap avatar 2d has a height about 40
		float classesHeight = FindObjectsOfType<HeatmapLegendItemClass>().Length * 100f; // classes have height too
		float totalHeight = heatmapsHeight + classesHeight;
		legendScroll.content.sizeDelta = new Vector2(0,heatmapsHeight + classesHeight);
//		EnableControls();
	}

//	void EnableControls(){
//		foreach(Transform t in controlsTransformParent){
//			t.gameObject.SetActive(true);
//		}
//	}
//
//	void DisableControls(){
//		foreach(Transform t in controlsTransformParent){
//			t.gameObject.SetActive(false);
//		}
//	}

	public bool debug=false ;
	bool snappedThisPlay = false; // tHis is for level buidler to snap to the first instanced avatar so we can watch them, but only do it once per play session.
	public void Play(){
		snappedThisPlay = false;
		playing = true;
		speedFactor = 1f;
		foreach(HeatmapData hd in heatmaps){
			hd.avatar.secondsElapsedInt = 0;
		}
		UpdateSpeedText();
//		Debug.Log("play w heatmap:"+heatmaps.Count);
	}

	public void SetSpeedFactor(int factor){
		speedFactor = factor;
		UpdateSpeedText();
	}

	public void FastForward(int factor){
		speedFactor += factor;
		speedFactor = Mathf.Max(speedFactor,1); // don't drop below 1
		UpdateSpeedText();
	}
	void UpdateSpeedText(){
		foreach(HeatmapData hd in heatmaps){
			if (hd.avatar){
				hd.avatar.GetComponent<HeatmapAvatar>().SetSpeed(speedFactor);
			}
		}
		speed.text = "Speed: "+Mathf.RoundToInt(speedFactor).ToString()+"x";
		// Show all avatars all at once!
		// Destroy existing ones then iterate through EACH POSITION and make an avatar!
	}

	public void Pause(){
		speedFactor = 0;
		UpdateSpeedText();
	}

	public void Reset(){
		foreach(HeatmapData hd in heatmaps){
			hd.avatar.gameObject.SetActive(false);
			hd.avatar.secondsElapsedInt = 0;
		}
	}

	void OnDisable(){
		foreach(Transform t in legendList){
			Destroy(t.gameObject);
		}
		foreach(HeatmapData hd in heatmaps){
			Destroy(hd.avatar.gameObject);
		}
		heatmaps.Clear();
		transform.parent.gameObject.SetActive(false); // if we closed level buidler, close heatmap as well.
		playing = false;

	}

	bool playing = false;

//	int timeElapsed = 0;
	int earliestUnixTimeForHeatmap = 2147483647; // max int 32
	float speedFactor = 1;
//	public void ModifySpeedFactor(float f){
//		speedFactor += f;
//	}
	void Update(){
		
		if (playing){
			
			//During play mode we iterate through all the heatmap objects
			// if the heatmap object's session start time and duration is within the bounds of the current elapsed time it will be displayed only at that location
			// else not displayed
//			time.text = Utils.DisplayAsTimeFromSeconds(timeElapsed);

			// Advance the timestep for each avatar which sets their position
			foreach(HeatmapData hd in heatmaps){
				hd.avatar.AdvanceTime(speedFactor);
			}

			// Advance the timestep for each stager
			foreach(HeatmapStager hs in FindObjectsOfType<HeatmapStager>()){
				hs.AdvanceTimer(speedFactor);
			}

		}
	}

	public Image showNamesCheck;
	public Image showTrailsCheck;
	public void ToggleNames(){
		showNamesCheck.gameObject.SetActive(!showNamesCheck.gameObject.activeSelf);
		bool flag = showNamesCheck.gameObject.activeSelf;
		foreach(HeatmapData hd in heatmaps){
			hd.avatar.name.gameObject.SetActive(flag);
		}
	}
	public void ToggleTrails(){
		showTrailsCheck.gameObject.SetActive(!showTrailsCheck.gameObject.activeSelf);
		bool flag = showTrailsCheck.gameObject.activeSelf;
		foreach(HeatmapData hd in heatmaps){
			hd.avatar.GetComponentInChildren<TrailRenderer>().enabled = flag;
		}
	}
}
