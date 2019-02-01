using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class EditorTesting : MonoBehaviour {

	public Sprite border4;
	public Sprite border3;
	public Sprite border2;
	public bool useHotkeys = false;
	public bool debug = false;
	public GameObject debugArrow;
	public GameObject camerafly;
	public GameObject ghostPrefab;
	public static EditorTesting inst;
	public GameObject crudeGhostBlocker;
	public WaterConfig waterConfig;
	public GameObject gadgetsPrefab;
	public GameObject colliderBullshit;
	public void SetInstance(){
		inst = this;
	}


	public void DebugArrow(Vector3 origin, Vector3 fwd, Color c){
		GameObject da = (GameObject)GameObject.Instantiate(debugArrow,origin,Quaternion.identity);
		da.transform.forward = fwd;
		da.GetComponentInChildren<Renderer>().material.color = c;
		da.name = "debug arrow at :"+Time.time;
	}

	// Use this for initialization
	void Start () {
//		// commented Debug.Log("editor testing aliiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiie");
		#if UNITY_EDITOR
		#else
		useHotkeys = false;
		#endif
	}

	// Update is called once per frame
	int skyboxInt = 0;
	float skyboxTimer = 0;
	Transform parent1;
	Transform parent2;
	List<GameObject> cubes = new List<GameObject>();

//	public GameObject rollableWallPrefab;

	public bool ghosting = false;
	public GameObject extraCamera;
	void Update () {
		if (SMW_CHEATS.inst.cheatsEnabled){
//			if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.B)) {
//				GameObject bs = (GameObject)Instantiate(colliderBullshit,Player.inst.transform.position + Player.inst.transform.forward * 10f, Player.inst.transform.rotation);
//			}
//
//			if (Input.GetKeyDown(KeyCode.C)){
//				extraCamera.SetActive(!extraCamera.activeSelf);
////				Camera.main.GetComponent<UnityStandardAssets.ImageEffects.OutlineFogPostFX>().enabled = !Camera.main.GetComponent<UnityStandardAssets.ImageEffects.OutlineFogPostFX>().enabled;
//			}
			Camera editingCam = null;
			if (Input.GetKey(KeyCode.V)) editingCam = Camera.main;
			if (Input.GetKey(KeyCode.B)) editingCam = LevelBuilder.inst.camSky;
			if (editingCam){
				if (Input.GetKeyDown(KeyCode.Alpha1)) editingCam.renderingPath = RenderingPath.DeferredLighting;
				if (Input.GetKeyDown(KeyCode.Alpha2)) editingCam.renderingPath = RenderingPath.DeferredShading;
				if (Input.GetKeyDown(KeyCode.Alpha3)) editingCam.renderingPath = RenderingPath.Forward;
				if (Input.GetKeyDown(KeyCode.Alpha4)) editingCam.renderingPath = RenderingPath.UsePlayerSettings;
				if (Input.GetKeyDown(KeyCode.Alpha5)) editingCam.renderingPath = RenderingPath.VertexLit;
				if (Input.GetKeyDown(KeyCode.Alpha6)) editingCam.renderingPath = RenderingPath.DeferredLighting;
				if (Input.GetKeyDown(KeyCode.Alpha0)) editingCam.depth = 5;
				if (Input.GetKeyDown(KeyCode.Equals)) editingCam.depth +=1;
				if (Input.GetKeyDown(KeyCode.Minus)) editingCam.depth -=1;
				PlayerNowMessage.inst.DisplayInstant("cam:"+editingCam.name+ " rend path:"+editingCam.renderingPath+", depth:"+editingCam.depth);


			}



		}
	}

	public void BeginGhosting(){
		crudeGhostBlocker.SetActive(true);
		Vector3 ghostStartPos  = Camera.main.transform.position;
		Vector3 highest = Vector3.zero;
		if (LevelBuilder.inst.levelBuilderIsShowing){
			foreach(RaycastHit h in Physics.RaycastAll(LevelBuilderCamSkyManager.inst.GetRayThroughCenterOfMap())){
				if (h.collider.transform.root.GetComponentInChildren<UserEditableObject>() || h.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")){
					if (h.point.y > highest.y) highest = h.point;
				}
			}
			ghostStartPos = highest + Vector3.up*125f;
			LevelBuilder.inst.camUI.enabled = false;
		}
		Player.inst.FreezePlayer();
		CreateGhost(ghostStartPos);

		return;
	}
	void CreateGhost(Vector3 ghostStartPos){
		
		GameObject ghost = (GameObject)Instantiate(ghostPrefab);
		ghost.transform.position = ghostStartPos;
		ghost.transform.rotation = Camera.main.transform.rotation;
		Time.timeScale = 0;
//		foreach(HideMeshAtDistance hd in FindObjectsOfType<HideMeshAtDistance>()){
//			hd.Show();
//		}
		ghost.GetComponent<MouseLook>().rotationY = -45;

		ghosting = true;
		ghost.transform.rotation = LevelBuilder.inst.camSky.transform.rotation;
	}



	public void EndGhosting(){
		LevelBuilder.inst.camUI.enabled = true;
		Player.inst.UnfreezePlayer();
		Time.timeScale = 1;
		ghosting = false;
	}

	public void DebugSphere(Vector3 p){
		GameObject debugSphere =GameObject.CreatePrimitive(PrimitiveType.Sphere);
		debugSphere.transform.position = p;
		debugSphere.transform.localScale = Vector3.one*4;
		debugSphere.name = "Debug sphere";
	}

	public void TempDebugSphere(Vector3 p,Color c){
		float tempDebugInterval = 0.2f;
		if (Utils.IntervalElapsed(tempDebugInterval)){
			GameObject debugSphere =GameObject.CreatePrimitive(PrimitiveType.Sphere);
			debugSphere.transform.position = p;
			debugSphere.transform.localScale = Vector3.one*.5f;
//			debugSphere.name = "Debug sphere";
			debugSphere.GetComponent<Renderer>().material.color = c;
			Destroy(debugSphere.GetComponent<Collider>());
			TimedObjectDestructor tod = debugSphere.AddComponent<TimedObjectDestructor>();
			tod.DestroyNow(tempDebugInterval);
		}
	}



	void BuildCubeSwarm(){
		if (parent1 == null){
			parent1 = new GameObject().transform;
		}
		if (parent2 == null){
			parent2 = new GameObject().transform;
		}
		for (int i=0;i<100;i++){
			//				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 120 + Random.insideUnitSphere * 30 + Vector3.up * 40;
			GameObject cube = NumberManager.inst.CreateNumber(new Fraction(1,1),pos,NumberShape.Cube);

			//				cube.transform.position = pos;
			//				cube.AddComponent<Rigidbody>();
			//				cube.GetComponent<Rigidbody>().isKinematic = true;
			//				cube.AddComponent<NumberInfo>();
			cube.transform.parent = parent1;
			cubes.Add(cube);
			cube.AddComponent<Rigidbody>();
			if (cubes.Count % 3 != 0){
//				cube.GetComponent<NumberInfo>().DisableTexts();
			}
		}
		// commented Debug.Log("cube count;"+cubes.Count);
	}
//	List<RaycastResult> GetUIObjectsUnderCursor(){
//		List<RaycastResult> objectsHit = new List<RaycastResult> ();
//		PointerEventData cursor = new PointerEventData(EventSystem.current);                            // This section prepares a list for all objects hit with the raycast
//		cursor.position = Input.mousePosition;
//
//		EventSystem.current.RaycastAll(cursor, objectsHit);
//		return objectsHit;
//	}
}
