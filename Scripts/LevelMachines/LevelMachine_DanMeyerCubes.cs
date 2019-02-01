using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LevelMachine_DanMeyerCubes : UserEditableObject {

	public static string sizeKey = "DanMeyerPuzzleSize";
	public static string deactivatedCubesArrayKey = "DeactivatedCubesArray"; // an integer list of which cubes in the array are activated.
	public enum CubeState {
		Ready,
		Sovling
	}

	public CubeState cubeState = CubeState.Ready;

	public List<DanMeyerCube> cubes = new List<DanMeyerCube>();

	public GameObject danMeyerCubePrefab;
	public Transform editorCameraParent;
	public ResourceDrop resourceDrop;

	public Text sizeX;
	public Text sizeY;
	public Text sizeZ;
	public Text count;
	public GameObject solvingP;


	float cubeScale = 2f; // 
	Vector3 puzzleSize = new Vector3(3,3,3);




	public bool editing = false; // while editing this object the camera controls are "hot" and local camera is "on"


	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.AddRange(new GameObject[] {
			LevelBuilder.inst.POCMEditDanMeyerCube,
//			LevelBuilder.inst.POCMResourceDropButton,
//			LevelBuilder.inst.placedObjectContextMenuNumberWallSizeButton,
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMheightButton,
//			//			LevelBuilder.inst.POCMsequentialFractionButton
		});
		return els.ToArray();
	}


	void Start(){
		
//		SetPuzzleSize(puzzleSize);
	}

	void SetPuzzleSize(Vector3 s){
		SetPuzzleSize((int)s.x,(int)s.y,(int)s.z);
	}
	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(sizeKey) && N.GetKeys().Contains(deactivatedCubesArrayKey)){
			puzzleSize = JsonUtil.StringToIntVector3(N[sizeKey].Value);
			SetPuzzleSize((int)puzzleSize.x,(int)puzzleSize.y,(int)puzzleSize.z); // creates a solid block of cubes

			// deactivate
			// deactivates cubes to show the pattern (a user had previously deactivated select cubes and "saved" the puzzle in that state)
			List<Vector3> deactivatedCubesList = new List<Vector3>();
			foreach(SimpleJSON.JSONNode v3 in N[deactivatedCubesArrayKey].AsArray.Childs){
				if (v3.Value == "") continue;
//				else Debug.Log("v3 val:"+v3.Value+", len;"+v3.Value.Length);
				Vector3 v3actual = JsonUtil.StringToIntVector3(v3.Value);
				deactivatedCubesList.Add(v3actual);
	//			Debug.Log("add;"+v3actual);
			}
	//		Debug.Log("deact cube list ct:"+deactivatedCubesList.Count);
			foreach(DanMeyerCube dmc in cubes){
	//			Debug.Log("compare:"+dmc.indexedPosition + " ==== "+deactivatedCubesList[0]);
				if (deactivatedCubesList.Contains(dmc.indexedPosition)){ // note that int vector3 may be problem e.g. vector3(0,1,2) != vector3(0.0001,2,3)
	//				Debug.Log("MATCH");
					dmc.TurnCubeOff(true);
				}
			}
		}
		UpdateCountText();
	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N[sizeKey] = JsonUtil.IntVector3ToString(puzzleSize);
		N[deactivatedCubesArrayKey] = (SimpleJSON.JSONArray)GetListOfDeactivatedCubesAsArray();

		return N;
	}

	SimpleJSON.JSONArray GetListOfDeactivatedCubesAsArray(){
		int i=0;
		SimpleJSON.JSONArray ar = new SimpleJSON.JSONArray();
		foreach(DanMeyerCube dmc in cubes){
			if (dmc.cubeActive == false ) ar.Add(JsonUtil.IntVector3ToString(dmc.indexedPosition));
		}
		return ar;
	}



	public void SetPuzzleSize(int x, int y, int z){
		// Creates and destroys cubes to fit the puzzle size.
		// clear existing cubes
		foreach(DanMeyerCube dmc in cubes){
			Destroy(dmc.gameObject);
		}
		cubes.Clear();


		for (int i=0;i<x;i++){
			for (int j=0;j<y;j++){
				for (int k=0;k<z;k++){
					GameObject newCube = (GameObject)Instantiate(danMeyerCubePrefab,transform.position + (transform.right * i + transform.up * j + transform.forward * k) * cubeScale,transform.rotation);
					DanMeyerCube dmc =newCube.GetComponent<DanMeyerCube>();
					dmc.indexedPosition = new Vector3(i,j,k);
					cubes.Add(dmc);
					newCube.transform.parent = transform;
					newCube.transform.localScale = Vector3.one * 0.98f * cubeScale;
				}

			}
		}

//		// And purge all cubes that were outside, if we shrank
//		List<Vector3> toRemove = new List<Vector3>();
//		if (x < puzzleSize.x || y < puzzleSize.y || z < puzzleSize.z){
//			foreach(KeyValuePair<Vector3,DanMeyerCube> kvp in cubes){
//				if (kvp.Key.x > x || kvp.Key.y < y || kvp.Key.z < z){
//					Destroy(kvp.Value.gameObject);
//					toRemove.Add(kvp.Key);
//				}
//			}
//		}
//		foreach(Vector3 rem in toRemove){
//			cubes.Remove(rem);
//		}
//		puzzleSize = new Vector3(x,y,z);
		RepositionEditorCameraParent();
		sizeX.text = x.ToString();
		sizeY.text = y.ToString();
		sizeZ.text = z.ToString();
		UpdateCountText();
		solvingP.transform.localPosition = CenterOfCube()/2f; // particle system should also be centered
//		Debug.Log("Sizex:"+sizeX.text);
	}

	public void UpdateWallSizeBasedOnText(){
//		Debug.Log("update based on txt");
		int x = int.Parse(sizeX.text);
		int y = int.Parse(sizeY.text);
		int z = int.Parse(sizeZ.text);
//		Debug.Log("x:"+x);
		puzzleSize = new Vector3(x,y,z); // this is so camera will get repositioned correctly in the next function... 
		SetPuzzleSize(x,y,z); // lol ...bad code whatever it works

	}

	void RepositionEditorCameraParent(){
		// after resizing make sure the camera is pivoted to the *center* of the puzzle.
		editorCameraParent.localPosition = CenterOfCube();
		Quaternion rot = editorCameraParent.transform.rotation;
		rot.eulerAngles = new Vector3(45,45,0); // look from the corner
		editorCameraParent.localRotation = rot;
		editorCameraParent.GetComponentInChildren<Camera>().transform.localPosition = new Vector3(0,0,-puzzleSize.magnitude*1.1f*cubeScale); // move camera back from the puzzle
	}


	Vector3 CenterOfCube(){
		return puzzleSize * (cubeScale / 2f) - Vector3.one * cubeScale/2f; // keep in mind puzzle starts at 0,0,0 and grows outwards with the first cube centered at 0,0,0 
	}

	public void BeginEditing (){
		MouseLockCursor.ShowCursor(true,"begin edit dan meyer cube");
//		LevelBuilder.inst.overrideLevelBuilderMouseEvents = true;
//		LevelBuilder.inst.camSky.enabled = false;
//		LevelBuilder.inst.camUI.enabled = false;
//		LevelBuilder.inst.mode = LBMode.EditingSeparately;
		LevelBuilder.inst.RestrictInput(true);
		editing = true;
		editorCameraParent.gameObject.SetActive(true);
		UpdateCountText();
	}

	public void EndEditing (){
		MouseLockCursor.ShowCursor(false,"end edit dan meyer cube");
		LevelBuilder.inst.RestrictInput(false);
//		LevelBuilder.inst.overrideLevelBuilderMouseEvents = false;
		// todo potential bug, what if the object closes or is destroyed while right mouse button held down (stacked mouselocks?)
		editing = false;
		editorCameraParent.gameObject.SetActive(false);
	}

	void Update(){
		
		if (editing){
			// hm? this happens inside DanMeyerCubeClicker which is on the camera, duh
		}
		if (SMW_CHEATS.inst.cheatsEnabled) UpdateCountText();
	}

	public void UpdateCountText(){
		count.text = NumberCubesActive().ToString();
	}

	// player threw a number at the puzzle, did they solve it??

	public int NumberCubesActive(){
		int c = 0;
		foreach(DanMeyerCube dmc in cubes){
			if (dmc.cubeActive) c++;
		}
		return c;
	}


	public void TrySolve(NumberInfo ni){
		if (cubeState == CubeState.Ready){
			
			StartCoroutine(Solving(ni.fraction));
			float kickForce =400f; // get thatwrong answer away from me!
			ni.GetComponent<Rigidbody>().AddForce((ni.transform.position - editorCameraParent.position)*kickForce);
		}
	}

	IEnumerator Solving(Fraction fr){
		solvingP.gameObject.SetActive(true);
		cubeState = CubeState.Sovling;
		// cache the correct answer
		bool answerCorrect = Mathf.RoundToInt(fr.GetAsFloat()) == NumberCubesActive();
//		Debug.Log("answer :" +answerCorrect+", int getasflt:"+(Mathf.RoundToInt(fr.GetAsFloat()))+", numcubesactive:"+NumberCubesActive());

		// Index starting cubes positions
		Dictionary<DanMeyerCube,Vector3> cubeStartingPositions = new Dictionary<DanMeyerCube,Vector3>();	
		foreach(DanMeyerCube dmc in cubes){
			cubeStartingPositions.Add(dmc,dmc.transform.localPosition);
		}

		// Float cubes apart and play ahh sound
		AudioManager.inst.PlayHeal1(transform.position);

		// cubes float softly apart and rotate
		float awaySpeed = 2f;
		StartCoroutine(HighlightCubesSequentially());
		for(float i=0;i<2.2f;i+=Time.deltaTime){
			foreach(DanMeyerCube dmc in cubes){
				float upFactor = .5f; // float upwards as well
				Vector3 awayVector = Vector3.Normalize(dmc.transform.localPosition -CenterOfCube()) + Vector3.up*upFactor;
				dmc.transform.position += Random.Range(0,1f) * awayVector * Time.deltaTime * awaySpeed;
				dmc.transform.Rotate(new Vector3(5,5,5) * Time.deltaTime);
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}

		// and light up
//		yield return new WaitForSeconds(2.2f); // wait for highilghts to finish

		if (answerCorrect){
//			Debug.Log("Answer correct");
			yield return StartCoroutine(Solve(fr));
		} else {
//			Debug.Log("Answer wrong!");
			Error(fr.ToString());

			// return cubes to starts
			for(float i=0;i<2.2f;i+=Time.deltaTime){
//				Debug.Log("returning. i:"+i);
				foreach(DanMeyerCube dmc in cubes){
					Vector3 toVector =  dmc.transform.localPosition - cubeStartingPositions[dmc];
					dmc.transform.localPosition = Vector3.Lerp(dmc.transform.localPosition,cubeStartingPositions[dmc],Time.deltaTime*awaySpeed);
					Quaternion targetRot = Quaternion.identity;
					dmc.transform.localRotation = Quaternion.Slerp(dmc.transform.localRotation,Quaternion.identity,1f*Time.deltaTime);
				}
				yield return new WaitForSeconds(Time.deltaTime);
			}
//			Debug.Log("<color=#f44>FINISHED!</color>");
			foreach(DanMeyerCube dmc in cubes){
				dmc.transform.localPosition = cubeStartingPositions[dmc];
				dmc.transform.localRotation = Quaternion.identity;
			}
			AudioManager.inst.PlayHeavyClick(transform.position + CenterOfCube());

		}


		cubeState = CubeState.Ready;
		solvingP.gameObject.SetActive(false);

		yield break;
	}

	IEnumerator HighlightCubesSequentially(){
//		Debug.Log("<color=#110>CUBES HIGHLIGHTING</color>");
		yield return new WaitForSeconds(1.5f);
		int cubesToSkip = NumberCubesActive() / 5;
		int ct = 0;
		foreach(DanMeyerCube dmc in cubes){
			if (dmc.cubeActive) {
				LevelBuilder.inst.MakeHighlightFX2(dmc.transform,"");
				yield return 0;
//				yield return new WaitForSeconds(0.17f);
			}
			ct += 1;
			//			Destroy(dmc.gameObject);
		}
		yield break;
	}

	IEnumerator DestroyCubesRandomly(){
		yield return new WaitForSeconds(0.5f);
		float pitch = .6f;
		foreach(DanMeyerCube dmc in cubes){
			if (dmc){
				Destroy(dmc.gameObject);
//				AudioManager.inst.PlayBubblePop(dmc.transform.position,pitch);
				EffectsManager.inst.MakeLittleBlueExplosion(dmc.transform.position);
				pitch += .01f;
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
		
	}


	IEnumerator Solve(Fraction fr){
//		Dictionary<Transform,float> fuses = new Dictionary<Transform,float>();	
		// burst cubes apart
		float dt = .4f;
		foreach(DanMeyerCube dmc in cubes){
			if (dmc){
	//			fuses.Add(dmc.transform,Random.Range(.5f,2.2f));
				Rigidbody rb = dmc.gameObject.AddComponent<Rigidbody>();
				Vector3 awayVector = dmc.transform.localPosition - CenterOfCube();
				float awayForce =155;
				rb.AddForce(awayVector * awayForce);
			}
		}
		yield return (StartCoroutine(DestroyCubesRandomly()));
		resourceDrop.DropResource();


		// pop them one by one
//		int cubesToSkip = NumberCubesActive() / 5;
//		float percentChanceToSkipTime = 1/(float)cubesToSkip; // do 1 / n cubes every frame

		yield return 0;



		Destroy(this.gameObject);
		Debug.Log("<color=#f55>DESTROYED</color>");
		PlayerNowMessage.inst.Display("Correct! This structure had "+fr.ToString()+" cubes.");
	}

	void Error(string e){
		
		AudioManager.inst.PlayWrongAnswerError(editorCameraParent.position,1,1);

		PlayerNowMessage.inst.Display("The number of cubes did not equal "+e+". Try again!");
//		Debug.Log("<colo=#00f>Error</color>");
	}


	public override void OnDestroy(){
		base.OnDestroy();
//		Debug.Log("<color=#f5f>DIED</color>");
	}

}
