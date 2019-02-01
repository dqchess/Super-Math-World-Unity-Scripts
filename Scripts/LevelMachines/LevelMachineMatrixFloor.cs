using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum PlayerGridDirection {
	X, // pos x -- player forward is parallel with x axis of grid 
	Z, // pos z -- player forward is parallel with z axis of grid ("normal")
	x, // neg x -- player forward is parallel with x axs, facing opposite dir
	z // neg z -- player forward is parallel with z axs, facing opposite dir
}

public class LevelMachineMatrixFloor : UserEditableObject {

	#region UserEditable
	public override void SetProperties(SimpleJSON.JSONClass N){
//		Debug.Log("matrifloor setpro:"+N.ToString());
		base.SetProperties(N);
		if (N.GetKeys().Contains(JsonUtil.dimensionsKey)){
			ClearFloor();
			SetupFloor(N[JsonUtil.dimensionsKey][JsonUtil.sizeXkey].AsInt,N[JsonUtil.dimensionsKey][JsonUtil.sizeZkey].AsInt);
			SetFloorPlantedStates(N[plantedArrayKey].AsArray);
//			Debug.Log("Matrix floor Set prop:"+N.ToString());
		}
	}

	public override void OnLevelBuilderObjectPlaced(){
		Debug.Log("Placed: "+name);
		ClearFloor();
		SetupFloor(sizeX,sizeZ);
	}


	public override SimpleJSON.JSONClass GetProperties(){
		// return the properties
		SimpleJSON.JSONClass N = base.GetProperties();// new SimpleJSON.JSONClass();
//		Debug.Log("Matrix floor Get prop:"+N.ToString());
//		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[JsonUtil.dimensionsKey][JsonUtil.sizeXkey].AsInt = sizeX;
		N[JsonUtil.dimensionsKey][JsonUtil.sizeZkey].AsInt = sizeZ;
		N[plantedArrayKey] = new SimpleJSON.JSONArray();
		for (int i=0;i<sizeX;i++){
			for (int j=0;j<sizeZ; j++){
				if (squares[i,j].planted){
					SimpleJSON.JSONClass planted = new SimpleJSON.JSONClass();
					planted[posX].AsInt = i;
					planted[posZ].AsInt = j;
					planted[color] = JsonUtil.ConvertColorToJson(squares[i,j].targetColor);
					planted[plantPrefabKey].AsInt = squares[i,j].flowerIndex;
					N[plantedArrayKey].Add(planted);
				}
			}
		}
		return N;
	}

	public override GameObject[] GetUIElementsToShow() {
		List<GameObject> ui = new List<GameObject>();
		ui.AddRange(base.GetUIElementsToShow());
		ui.Add(LevelBuilder.inst.POCMheightButton);
		ui.Add(LevelBuilder.inst.POCMmatrixFloorSizeButton);
		return ui.ToArray();
//		return new GameObject[] { LevelBuilder.inst.POCMheightButton, LevelBuilder.inst.POCMmatrixFloorSizeButton };
	}

	#endregion



	void ClearFloor(){
//		Debug.Log("Clearfloor");
		if (squares != null){
			foreach(MatrixFloorSquare sq in squares){
				if (sq && sq.gameObject) {
					sq.Die();
//					Destroy(sq.gameObject);
				} else {
//					Debug.Log("couldn't kill:"+sq);
				}
			}
		}
	}



	public static string plantedArrayKey = "plantedArray"; // for the list of positions
	public static string plantPrefabKey = "ppi";
	public static string fullyPlanted = "fullyPlanted"; // set to true when resource number has been dropped. TODO: Move this to the ResourceDrop class so it is serialized that this resource already got dropped.
	public static string posX = "pX"; // for each floor space posiiton, short because there will be a lot of them
	public static string posZ = "pZ"; 
	public static string color = "col";

	public float gridScale = 3.0f;
	public int sizeX = 10;
	public int sizeZ = 20;

	MatrixFloorSquare[,] squares;
	public Color floorColor;
	// Use this for initialization

	void Start () {
//		Debug.Log("start");
//		squares = new MatrixFloorSquare[sizeX,sizeZ];
//		SetupFloor(sizeX,sizeZ);
		if (LevelBuilder.inst.levelBuilderIsShowing){
			ClearFloor();
			SetupFloor(sizeX,sizeZ);
		}

	}




	void SetFloorPlantedStates(SimpleJSON.JSONArray PA){
		foreach(SimpleJSON.JSONClass s in PA.AsArray.Childs){
			// The planted square were saved in the properties
			// Iterate through these planted squares and plant them as well as set their color.
//			squares[s[posX],s[posZ]].targetColor = s[color];
			squares[s[posX].AsInt,s[posZ].AsInt].Plant(s[plantPrefabKey].AsInt,JsonUtil.ConvertJsonToColor((SimpleJSON.JSONClass)s[color]));
		}
	}

	void SetupFloor(int x,int z){


		squares = new MatrixFloorSquare[x,z];
		sizeX = x;
		sizeZ = z;
		for (int i=0;i<x;i++){
			for (int j=0;j<z; j++){
				GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
				Quaternion rot = new Quaternion();
				rot.eulerAngles = new Vector3(90,transform.rotation.eulerAngles.y,0);
				q.transform.rotation = rot;
				q.transform.localScale = gridScale * Vector3.one * 0.95f ; //gaps
				q.transform.position = transform.position + transform.right * i * gridScale + Vector3.up * .03f + transform.forward * j * gridScale; // new Vector3(i*gridScale,0.3f,j*gridScale);
				MatrixFloorSquare mfs = q.AddComponent<MatrixFloorSquare>();
				mfs.GetComponent<Renderer>().material = Utils.FadeMaterial();
				squares[i,j] = mfs;
				mfs.positionX = i;
				mfs.positionZ = j;
				q.transform.parent = transform;
				mfs.Init(this);
			}
		}
	}

	bool CheckAllFlowersPicked(){
//		Debug.Log("Checking all flowers picked..");
		bool allFlowersPicked = true;
		for(int i=0;i<sizeX;i++){
			for(int j=0;j<sizeZ;j++){
				if (squares[i,j].HasNumberFlower()){
//					Debug.Log("this one flower had a flower!"+i+","+j);
					allFlowersPicked = false;
				}
		
			}
		}
		return allFlowersPicked;
	}


	void ResetFarmFloor(){
		checkFlowersPicked = false;
		finished = false;
		ClearFloor();
		SetupFloor(sizeX,sizeZ);
	}

	float finishedCheckTimer = 0;
	bool finished =  false;
	float checkFlowersPickedTimer = 1f;
	void Update(){
		if (checkFlowersPicked){
			checkFlowersPickedTimer -= Time.deltaTime;
			if (checkFlowersPickedTimer < 0){
				checkFlowersPickedTimer = Random.Range(2,3f);
				if (CheckAllFlowersPicked()){
					ResetFarmFloor();

				}
			}
		}
//		if (!PlayerOverPanel()) seedSizeMatchesFloorSquares = false;
		hiTimer -= Time.deltaTime;
		if (hiTimer < 0){
			hiSquares.Clear();
			foreach(MatrixFloorSquare sq in squares){
				if (!sq.planted) {
					sq.targetColor = floorColor;
				}
			}
		}
		finishedCheckTimer -= Time.deltaTime;
		if (finishedCheckTimer < 0 && !finished){
			finishedCheckTimer = Random.Range(0.5f,1);
			finished = true;
			foreach(MatrixFloorSquare sq in squares){
				if (!sq.planted){
					finished = false;
				}
			}
			if (finished) {
				GetComponentInChildren<ResourceDrop>().DropResource();
				GrowFlowers();
			}
		}
	}


	void GrowFlowers(){
		// Grow flowers after it's all planted.
		int n = -1;
		for(int i=0;i<sizeX;i++){
			for(int j=0;j<sizeZ;j++){
				squares[i,j].GrowFlower(n);
				n --; // each flower is negative starting with -1.
			}
		}

		// Now that flowers are grown, check every N seconds if all flowers have been "picked". If they have, RESET the entire farm floor so you can plant it again.
		// But make sure we wait 10 seconds before doing thi check (give the plants time to grow and finish, etc, before checking their status. otherwise we might check a floor square that SHOULD have a number or WILL have a number in 2-3 seconds, but doesn't yet bcause its first (non-number primitive) flower is still growing.
		StartCoroutine(CheckFlowersPickedAfterSeconds(20f));
	}
	bool checkFlowersPicked = false;
	IEnumerator CheckFlowersPickedAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		checkFlowersPicked = true;
	}
	Color hiColor;
	public bool seedSizeMatchesFloorSquares = false; // Is current seed plantable? set to false below if the seed size goes outside the bounds of the floor, e.g. a 5x5 seed is placed on a 5x4 floor
	List<MatrixFloorSquare> hiSquares = new List<MatrixFloorSquare>();
	float hiTimer = 0; // always counts down and when it reaches zero, hilightedFloorSquare are cleared.
	float colorTransitionSpeed = 5.5f; // how fast do panels change
//	float getPanelsTimer = 0;
	public List<MatrixFloorSquare> GetPanels(MatrixFloorSeed seed){
//		getPanelsTimer -= Time.deltaTime;
//		if (getPanelsTimer > 0) return new List<MatrixFloorSquare>();
//		getPanelsTimer = 0;
		// Which of the 4 corners is the player closest to?
		// How far (x,z) is the player away from that corner?
		// 


		hiSquares.Clear();

		hiTimer = 0.1f;
		// first, player should be on top of a panel.

		MatrixFloorSquare playerPanel = PlayerOverPanel();
		// Note that getpanels is only called if PlayerActivePanel
		// and PlayerActivePanel only checks if MatrixFloorSeed is equipped.
		if (playerPanel){
			if ((seed.sizeX > sizeX && seed.sizeX > sizeZ) || (seed.sizeZ > sizeX && seed.sizeZ > sizeZ)) {
				// seed was too big for any orientation
				PlayerNowMessage.inst.Display("That seed ("+seed.sizeX+","+seed.sizeZ+") is too big for this farm ("+sizeX+","+sizeZ+").",Player.inst.transform.position);
				for(int i=0; i<sizeX; i++){
					for(int j=0; j<sizeZ; j++){
						squares[i,j].blinking = true;
					}
				}
//				getPanelsTimer = 5f;
				return new List<MatrixFloorSquare>();
			}

			seedSizeMatchesFloorSquares = true;
			// which way was player facing?
			Vector3 xDir = squares[1,0].transform.position - squares[0,0].transform.position;
			Vector3 zDir = squares[0,1].transform.position - squares[0,0].transform.position;
			float playerRotationY = Player.inst.transform.rotation.eulerAngles.y;
//			Debug.Log("roty:"+playerRotationY+", A xdir:"+Vector3.Angle(Player.inst.transform.forward,xDir)+",AzDir:"+Vector3.Angle(Player.inst.transform.forward, zDir));
			PlayerGridDirection playerDir = PlayerGridDirection.X;
			if (Vector3.Angle(Player.inst.transform.forward,xDir) < 45) playerDir = PlayerGridDirection.X;
			else if (Vector3.Angle(Player.inst.transform.forward, zDir) < 45)  playerDir = PlayerGridDirection.Z;
			else if (Vector3.Angle(Player.inst.transform.forward, -xDir) < 45) playerDir = PlayerGridDirection.x;
			else if (Vector3.Angle(Player.inst.transform.forward, -zDir) < 45) playerDir = PlayerGridDirection.z;
			

			int startX = 0; // should be beteween 0 and sizeX - seedSizeX or seedSizeZ, depending on where player is.
			int startZ = 0;
			int curSizeX = 0;
			int curSizeZ = 0;

			switch(playerDir){
			case PlayerGridDirection.X:
//				Debug.Log("pos X");
				// Player forward (Z) was parallel with X axis, so flip x and z for the seed.
				// starting X should be playerPanel.x (closest panel to player)
				// starting Z should be playerpanel.z MINUS half of the seed X.
				startX = Mathf.Min(sizeX- seed.sizeZ,playerPanel.positionX);
				startZ = Mathf.Min(sizeZ - seed.sizeX, Mathf.Max(0,playerPanel.positionZ - Mathf.RoundToInt(seed.sizeX/2)));
				curSizeX = seed.sizeZ;
				curSizeZ = seed.sizeX;
				break;

			case PlayerGridDirection.Z:
//				Debug.Log("pos Z");
				// Player forward (Z) was parallel with Z axis, the "normal" case.
				// starting X should be playerpanel.x MINUS half of the seed X.
				// starting Z should be playerPanel.z (closest panel to player)
				startX = Mathf.Min(sizeX-seed.sizeX,Mathf.Max(0,playerPanel.positionX - Mathf.RoundToInt(seed.sizeX/2)));
				startZ = Mathf.Min(sizeZ - seed.sizeZ ,playerPanel.positionZ);
				curSizeX = seed.sizeX;
				curSizeZ = seed.sizeZ;
				break;

			case PlayerGridDirection.x:
//				Debug.Log("neg x");
				// Player forward (Z) was parallel with negative X axis.
				// starting X should be sizeX - seedsizeX. 
				// starting Z should be playerpanel.z MINUS half of the seed X.
				startX = Mathf.Min(sizeX - seed.sizeZ,Mathf.Max(0,sizeX - seed.sizeZ + 1 - (sizeX - playerPanel.positionX))); 
				startZ = Mathf.Min(sizeZ - seed.sizeX,Mathf.Max(0,playerPanel.positionZ - Mathf.RoundToInt(seed.sizeX/2)));
				curSizeX = seed.sizeZ;
				curSizeZ = seed.sizeX;
				break;
			case PlayerGridDirection.z:
//				Debug.Log("neg z");
				// Player forward (Z) was parallel with negative Z axis.
				// starting X should be sizeX - seedsizeX. 
				// starting Z should be playerpanel.z MINUS half of the seed X.
				startX = Mathf.Min(sizeX - seed.sizeX, Mathf.Max(0, playerPanel.positionX - Mathf.RoundToInt(seed.sizeX/2)));
				startZ = Mathf.Max(0, sizeZ - seed.sizeZ + 1 - (sizeZ - playerPanel.positionZ)); 
				curSizeX = seed.sizeX;
				curSizeZ = seed.sizeZ;
				break;
			default:
				break;
			}
//			Debug.Log("player dir;"+playerDir.ToString());

//			int playerY = Mathf.RoundToInt((Player.inst.transform.rotation.eulerAngles.y%360)/90f)*90; // 0, 90


			// Given that player is closest to X axis of grid
			// playersquare will be z = 0, x = [0,sizex]
			// so, we want the seed to start with X = playersquare.x - (int)seedsizex/2, and Z = 0

			// Given that player is closest to Z axis of grid
			// playersquare will be x = 0, z = [0,sizeZ]
			// so, we want the seed to start with X = 0, Z = playersquare.z - int(seedsizex/2), and flip x and z

			// Given that player is closest to TOP x axis of grid
			// playersquare will be x = sizeX, z = [0,sizeZ]
			// so, we want the seed to start with X = 0, Z = playersquare.z - int(seedsizex/2), and flip x and z

//			Debug.Log("startx:"+ startX +",startz:"+startZ+", xz;"+curSizeX+","+curSizeZ);
			for(int i=0; i<curSizeX; i++){
				for(int j=0; j<curSizeZ; j++){
					int x = startX + i;
					int z = startZ + j;
					if (x < sizeX && z < sizeZ){ // inside my bounds?
						squares[x,z].blinking = false;

						hiSquares.Add(squares[x,z]);
						if (squares[x,z].planted == true){ // fail because one plot is already planted
							seedSizeMatchesFloorSquares = false;
						}
					} else { // fail out of range
//						Debug.Log("out of range;"+x+","+z);
						seedSizeMatchesFloorSquares = false;
					}
				}
			}
		} 
		foreach(MatrixFloorSquare sq in squares){
			if (!sq.planted && !hiSquares.Contains(sq)) {
				sq.targetColor = floorColor;
				sq.blinking = false;
			} 
		}
		foreach(MatrixFloorSquare mf in hiSquares){
			if (!seedSizeMatchesFloorSquares){
				if (!mf.planted){
					mf.blinking = true;
				}
			} else {
				mf.targetColor = seed.seedColor;
			}
		}
		if (hiSquares.Count == 0){
			seedSizeMatchesFloorSquares = false;
		}
		if (seedSizeMatchesFloorSquares) {
			PlayerNowMessage.inst.Display("Click to plant the seed here.",Player.inst.transform.position);
		}
		return hiSquares;
	}

	float checkTimer = 0;
	float interval = 0.2f;
	MatrixFloorSquare cachedOver = null;
	MatrixFloorSquare PlayerOverPanel(){
		// PlayerOverPanel only checks if MatrixFloorSeed is equipped.
		checkTimer -= Time.deltaTime;
		if (checkTimer < 0){
			cachedOver = null;
			checkTimer = interval;
//			Vector3 offset = Player.inst.transform.forward * gridScale/.707f;
			float dist = Mathf.Infinity;
//			float diff = 0;
			MatrixFloorSquare closest = null;
			foreach(MatrixFloorSquare mfs in squares){
				float diff = Vector3.SqrMagnitude(mfs.transform.position-Player.inst.transform.position);
				if (diff < dist) {
					dist = diff;
					closest = mfs;
//					Debug.Log("diff less dist:"+diff);
				}
			}
			float maxPlayerDistSqrd = 200f;
			if (Vector3.SqrMagnitude(closest.transform.position-Player.inst.transform.position) > maxPlayerDistSqrd){
//				Debug.Log("closest too far");
				closest = null;
			}
			cachedOver = closest;


//			Vector3 offset = Vector3.zero;
//			foreach(RaycastHit hit in Physics.SphereCastAll(Player.inst.transform.position+offset,0.5f,Vector3.down)){
//				MatrixFloorSquare mfs = hit.collider.GetComponent<MatrixFloorSquare>();
//				if (mfs){
//					cachedOver = mfs;
//					return mfs;
//				}
//			}
		}
		return cachedOver;

	}



	public void SeedUnequipped(){
		foreach(MatrixFloorSquare sq in squares){
			if (!sq.planted) {
				sq.ResetColor(floorColor);
			}
		}
	}


}
