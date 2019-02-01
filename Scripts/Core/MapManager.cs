using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class Map {
	public string name;
	public Transform map;
	public Material skybox;
}

[System.Serializable]
public class MapSkyboxFogColorRel {
	public Material mapSkybox;
	public Color fogColor;
}

public class MapManager : MonoBehaviour {

//	public Transform[] terrains;
	[SerializeField] public Map[] maps; 
	[SerializeField] public MapSkyboxFogColorRel[] skyboxRels;
	public GameObject waterFX;
	public GameObject waterFXskyCam;
	public GameObject blockPlayerCube;
	public UltimateToonWaterC utwc;
//	[SerializeField]
	public Map currentMap;
	public Transform mapIconsParent; // for enable all maps cheat

	public float camBoundBuffer = 1500f;
	public static MapManager inst;
	SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		
		foreach(Map m in maps){
			m.map.gameObject.SetActive(false);
		}
		SelectTerrain(maps[0]);

//		UpdateTerrain(terSize);
//		utwc  = FindObjectOfType<UltimateToonWaterC>()
	}

	int test = 0;
	void Update () {
//		if (Input.GetKeyDown(KeyCode.M)){
//			int ind = test%maps.Length;
////			// commented Debug.Log("ind:"+ind);
//			SelectTerrain(maps[ind]);
//			PlayerNowMessage.inst.Display("Map:"+maps[ind].name);
//			test++;
//		}
	}


	public void SetCameraZoomByTerrainSize(){
		LevelBuilder.inst.camSky.orthographicSize = currentMap.map.GetComponent<Terrain>().terrainData.size.x / 11f + 75;

	}

	void SelectTerrain(Map m, bool centerOnPlayer=true){
		// commented Debug.Log("selecting:"+m.name);
		if (currentMap != null && currentMap.map != null){
			currentMap.map.gameObject.SetActive(false);
//			// commented Debug.Log("current map off");
		} else {
//			// commented Debug.Log("There was no current map...DUN DUN DUNNNN!!");
		}
		currentMap = m;
		m.map.gameObject.SetActive(true);
		RenderSettings.skybox = m.skybox;
		Vector3 terSize = m.map.gameObject.GetComponent<Terrain>().terrainData.size;
		UpdateTerrain(terSize);
		if (LevelBuilder.inst.levelBuilderIsShowing && centerOnPlayer){
			StartCoroutine("CenterOnPlayerAfterSeconds",0.2f);
		}
		SetTerrainTreeBillboardDistance();
		SetFogColor();
	}

	void SetFogColor(){
		foreach(MapSkyboxFogColorRel rel in skyboxRels){
			if (rel.mapSkybox == currentMap.skybox){
				RenderSettings.fogColor = rel.fogColor;
				return;
			}
		}
	}

//	public void SetAudioForCurrentMap(){
//		BackgroundAudioManager.inst.SetAudioAmbiance(currentMap.ambiantSound);
//	}

	IEnumerator CenterOnPlayerAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		LevelBuilder.inst.ActionCenterOnPlayer();
	}
	public void SetTerrainTreeBillboardDistance(){
		if (LevelBuilder.inst.levelBuilderIsShowing) {
			currentMap.map.GetComponent<Terrain>().treeBillboardDistance = 2000;
		} else {
			currentMap.map.GetComponent<Terrain>().treeBillboardDistance = GameManager.treeBillboardDistance;
		}
	}

	public void SelectTerrainByName(string name, bool centerOnPlayer=true){
//		WebGLComm.inst.Debug("set terrain:"+name);
		foreach(Map m in maps){
			if (m.name.Replace("'","") == name.Replace("'","")){
//				WebGLComm.inst.Debug("matched terrain:"+m.map.name);		
				SelectTerrain(m,centerOnPlayer);
				return;
			}
//			// commented Debug.Log("compare failed:"+name+","+m.name);
		}
		// commented Debug.LogError("NO VALID TERRAIN FOR NAME:"+name);
	}

	public void SelectTerrainByTransform(Transform t){
		foreach(Map m in maps){
			if (m.map == t){
				SelectTerrain(m);
				return;
			}
		}
		// commented Debug.LogError("NO VALID TERRAIN FOR TRANSFORM:"+t);
	}

	public void UpdateTerrain(Vector3 terSize){ 
		
		// Does not actually modify the actual current terrain size but sets other related properites based on the current selected terrain size
		//  A hobclob of different actions we take when terrain modified, including positions of player, water, bounds, horizon
		currentMap.map.GetComponent<Terrain>().basemapDistance = 2000;



		blockPlayerCube.transform.position = new Vector3(terSize.x/2f,0,terSize.z/2f);
		blockPlayerCube.transform.localScale = Vector3.one * terSize.x / 1000f * 1.1f;

		LevelBuilder.inst.camSkyNorthEastBoundary.transform.position = new Vector3(terSize.x,0,terSize.z);
		//		// commented Debug.Log("tersize x:"+terSize.x);
		float hardcodedYposForTerrainFX = -210f;
		float hardcodedYscaleForTerrainFX = 500f;
		float hardcodedToonWaterHeight = 41.68f;
		//		float hardcodedHorizonObjectTransformPosY = 44f;
		waterFX.transform.position = new Vector3(terSize.x/2f,hardcodedYposForTerrainFX,terSize.z/2f);
		waterFX.transform.localScale = new Vector3(terSize.x*3f,hardcodedYscaleForTerrainFX,terSize.z*3f);

		waterFXskyCam.transform.position = new Vector3(terSize.x/2f,23f,terSize.z/2f);
		float waterFxMod = 1.4f;
		waterFXskyCam.transform.localScale = new Vector3(terSize.x + camBoundBuffer * waterFxMod,22f,terSize.z + camBoundBuffer * waterFxMod); // hardcoded values here
		waterFXskyCam.SetActive(LevelBuilder.inst.levelBuilderIsShowing);


		//		// commented Debug.Log("utwc size:"+utwc.config.size);
		float waterSizeDelta = 2f; // waterSmallerOffsetForHorizonFXtoBlendSmoothly
		utwc.config.meshPointDistance = 48;
//		utwc.config.size = new Vector2(terSize.x*waterSizeDelta,terSize.z*waterSizeDelta);
		utwc.config.size = new Vector2(2000,2000);
		//		// commented Debug.Log("utwc size:"+utwc.config.size);
		utwc.transform.position = new Vector3(terSize.x/2f,hardcodedToonWaterHeight,terSize.z/2f);
		utwc.Init();


		//		Horizon.HorizonMaster hm = FindObjectOfType<Horizon.HorizonMaster>();
		//		hm.transform.position = new Vector3(terSize.x/2f,0,terSize.z/2f);
		////		hm.transform.Find("Horizon[ON]_Transition_Terrain").transform.localPosition = new Vector3(0,45,0);
		//		hm.transform.Find("Horizon[ON]_Transition_Terrain").transform.localScale = new Vector3(terSize.x,1,terSize.z);
		//		hm.transform.Find("BakedAndCombinedAndReduced").transform.localScale = Vector3.one * terSize.x / 1000f + Vector3.up;




		if (LevelBuilder.inst.firstTimeOpened) {
			SetCameraZoomByTerrainSize();
		}
	}

//	public void MovePlayerToCenterOfTerrainOrStart(){
//		// unless, there was aalready a player start in the scene! Not the ideal place do this check but oh well
//		if (FindObjectOfType<PlayerStart>() && !FindObjectOfType<PlayerStart>().destroyedThisFrame) {
//			FindObjectOfType<PlayerStart>().SetPlayerStart();
////			// commented Debug.Log("found playerstart! Moving player there instead of to terrain center.");
//			return;
//		}
//	}

	public GameObject dest;
	public Transform GetPlayerStartPositionForCurrentTerrain(){
		RaycastHit hit = new RaycastHit();
		Terrain at = currentMap.map.GetComponent<Terrain>();
		Ray ray = new Ray(new Vector3(at.terrainData.size.x/2f,500f,at.terrainData.size.z/2f),Vector3.down);
		Vector3 destPos = new Vector3(at.terrainData.size.x/2f,150f,at.terrainData.size.z/2f);
//		if (Physics.Raycast(ray,out hit,Mathf.Infinity,FindObjectOfType<SceneLayerMasks>().terrainAndWaterForObjectPlacement)){ // Note this may cause hit point with infinite negative y value.
		if (Physics.Raycast(ray,out hit,5000f,FindObjectOfType<SceneLayerMasks>().terrainAndWaterForObjectPlacement)){
			destPos = hit.point + Vector3.up * 2f;
		}
		//		// commented Debug.Log("ray hit dist:"+hit.distance);

		dest.transform.position = destPos;
//		Debug.Log("got player start dest! Vec3:"+dest.transform.position+", rot:"+dest.transform.rotation+", euler:"+dest.transform.eulerAngles);
		return dest.transform;
	}

	public void EnableAllMaps(){
		foreach(Transform t in mapIconsParent){
			t.gameObject.SetActive(true);
		}
	}
}
