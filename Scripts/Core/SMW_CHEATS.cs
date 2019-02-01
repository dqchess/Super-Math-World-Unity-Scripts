using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SMW_CHEATS : MonoBehaviour {

	public bool cheatsEnabled = false;
	public static SMW_CHEATS inst;
	public GameObject debugText;
	public Text debugText1;
	public bool memoryMonitor = false;

	public void SetInstance(){
		inst = this;
	}
	// Use this for initialization
	void Start () {
		debugText.SetActive(false);
		#if UNITY_EDITOR
		cheatsEnabled = true;
		#endif

	}
	
	// Update is called once per frame
	int rigidbodyType = 0;
	float timeLastCheated = 0f;
	void Update () {
		
		if (Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.C) && Time.time - timeLastCheated > 1f){
			timeLastCheated = Time.time;	
			cheatsEnabled = !cheatsEnabled;
			debugText.SetActive(true);
			// commented Debug.Log("cheater");
//			SMW_CHEATS.inst.cheatsEnabled = true;
			PlayerNowMessage.inst.DisplayInstant("C:"+cheatsEnabled);
			if (cheatsEnabled){
				MapManager.inst.EnableAllMaps();
				PauseMenu.inst.EnableStatsButton();
				//			foreach(TeacherLevelRestrictItem t in FindObjectsOfType<TeacherLevelRestrictItem>()){
				//				t.UpdateLockedStatus(6);
				//			}
				FindObjectOfType<TeacherLevelRestrictionManager>().SetTeacherLevel(6);
			}


		}




		if (!cheatsEnabled) return;
		if (Input.GetKeyDown(KeyCode.M) && Input.GetKeyDown(KeyCode.E)){
			memoryMonitor = !memoryMonitor;
			WebGLComm.inst.Debug("Memory monitor:"+memoryMonitor);
		}

//		if (Input.GetKeyDown(KeyCode.C)){
//			int c =0;
//			rigidbodyType ++;
//			rigidbodyType %= 3;
//			foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene()){
//				Rigidbody rb = ni.GetComponent<Rigidbody>();
//				if (rb) {
//					switch(rigidbodyType){
//					case 0: 
//						rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
//						break;
//						case 1:
//						rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
//						break;
//						case 2:
//						rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
//						break;
//					default:break;
//					}
//					c ++;
//				}
//			}
//			string s ="";
//			switch(rigidbodyType){
//			case 0: 
//				s = "CollisionDetectionMode.Discrete";
//				break;
//			case 1:
//				s = "CollisionDetectionMode.Continuous";
//				break;
//			case 2:
//				s = "CollisionDetectionMode.ContinuousDynamic";
//				break;
//			default:break;
//			}
//			PlayerNowMessage.inst.Display(c+"Numbers given "+s);
//		}
			
		if (Input.GetKeyDown(KeyCode.J)){
//			SMW_FPSWE.inst.JumpNow();
//			SMW_FPSWE.inst.controller.SimpleMove(Vector3.up * 50f);
//			FPSInputController.inst.motor.inputMoveDirection = new Vector3(FPSInputController.inst.motor.inputMoveDirection.x,0,FPSInputController.inst.motor.inputMoveDirection.z);
//			if (Input.GetKey(KeyCode.LeftShift)) SMW_FPSWE.inst.SetMomentum(Vector3.up * -50f);
//			else 
			FPSInputController.inst.motor.SetMomentum(Vector3.up * 5000f);
//			FPSInputController.inst.motor.inputJump = true;
		}

//		if (Input.GetKey(KeyCode.G) && Input.GetKeyDown(KeyCode.H) && !FindObjectOfType<EditorTesting>().ghosting){
//			FindObjectOfType<EditorTesting>().BeginGhosting();
//		}

		if (!LevelBuilder.inst.levelBuilderIsShowing){
			if (Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.D)){
				Vector3[] points = Utils.GetCircleOfPoints(360,20,8);
				GameObject[] lbos = new GameObject[6];
				lbos[0] = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("Gadget - Multiblaster");
				lbos[1] = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("Gadget - Wave");
				lbos[2] = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("Gadget - Boomerang");
				lbos[3] = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("Gadget - Bow");
				lbos[4] = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("Gadget - Zooka");
				lbos[5] = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("Gadget - Sword");
				Quaternion rot = Player.inst.transform.rotation;
				rot.eulerAngles += new Vector3(0,-45,0);
				for(int i=0;i<6; i++){
					lbos[i].transform.position = Player.inst.transform.position + (rot * points[i]);
				}
			
			} else if (Input.GetKeyDown(KeyCode.K)){ 
				SMW_GF.inst.CreateCircleOfNumbers(Player.inst.transform.position);
			} else if (Input.GetKeyDown(KeyCode.P)) {
				//					GameObject wheelO = new GameObject("Rollable wheel");
				//					wheelO.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 40 + Vector3.up * 50;
				//					NumberWallCreatorRound.CreateRollableWall(
				//						wheelO.transform.position,
				//						20,
				//						360,
				//						3,
				//						1,
				//						2, 
				//						new Fraction(8,1),
				//						wheelO.transform);
				//					wheelO.transform.Rotate(new Vector3(0,0,90));
			} 
		}


		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.K)){
			//			NumberManager.inst.CreateNumberGroup(Player.inst.transform.position+Player.inst.transform.forward*5+new Vector3(0,5,0));
			//			if (SMW_GF.inst.PurchaseForGems(900)){
//			SMW_GF.inst.CreateCircleOfNumbers(Player.inst.transform.position);
			//			}
//			EffectsManager.inst.CreateSmokePuffBig(Player.inst.transform.position,Vector3.up,100);

		}
		#endif
//

		

	}
	public bool PlayerDebug(Transform t, Color col){
		if (cheatsEnabled && Utils.IntervalElapsed(.2f) && Input.GetKey(KeyCode.P)){
			float careRange = 20f; 
			float careAngle = 20f; // cone within which we care about
			Vector3 dirToTarget = t.position - Camera.main.transform.position;
			if (Vector3.Magnitude(Player.inst.transform.position-t.position) < careRange && Vector3.Angle(Camera.main.transform.forward,dirToTarget)<careAngle ) {
				EffectsManager.inst.DrawDottedLine(Player.inst.transform.position+Vector3.up*-3f,t.position,col);
				return true;
			}
		}
		return false;
	}
}
