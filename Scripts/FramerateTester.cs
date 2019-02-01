using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FramerateTester : MonoBehaviour {

	[System.Serializable]
	public class TestObject {
		public string name;
		public GameObject prefab;
	}



	public static FramerateTester inst;
	public void SetInstance(){
		inst = this;
	}


	[SerializeField] public TestObject[] items;
	List<GameObject> testObjects = new List<GameObject>(); // created during test.

	void Start () {
		
	}

	float t = 0;
	float automaticTestInterval = 5f;
	int itemIndex = 0;
	int quantityIndex = 0;
	int[] quantities = new int[]{ 128, 256, 512, 1024, 2048 };
	float spacing = 1f;
	float height = 60f;

	string title = "";
	string info = "";
	// Update is called once per frame
	public bool testing = false;


	int frameCount = 0;
	float dt = 0.0f;
	float fps = 0.0f;
	float updateRate = 4.0f;  // 4 updates per sec.

	void Update () {
		frameCount++;
		dt += Time.deltaTime;
		if (dt > 1.0f/updateRate)
		{
			fps = frameCount / dt;
			frameCount = 0;
			dt -= 1.0f/updateRate;
		}





		if (SMW_CHEATS.inst.cheatsEnabled){
			if (Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.U)){
				InitTestEnvironment();
				title = "Test set up and ready.";
				Debug.Log("got");
			}

		}

		if (testing) {
			t += Time.deltaTime;
			if (fps > 2){ // ignore 0 framerate during instantiation period.
				totalFpsDuringTest.Add(fps);
			}

			foreach(int f in totalFpsDuringTest){
				if (f > maxFps) maxFps = f;
				if (f < minFps) minFps = f;
			}
			totalFramesDuringTest += 1;

			string seconds = t.ToString();
			if (seconds.Length > 4) seconds = seconds.Substring(0,4);

			string message = "test! RETURN resets, ARROWS change qty/item, S stops\n\n";
			message += "Current test:"+quantities[quantityIndex].ToString()+" of item "+items[itemIndex].name+"\n";
			message += "Current FPS:"+fps+", test running for "+seconds+" seconds\n\n";
			message += "Prev test results:\n"+info;
			DebugText.inst.Display(message);
			if (Input.GetKeyDown(KeyCode.Return)){
				InitTestEnvironment();
			}

			if (Input.GetKeyDown(KeyCode.RightArrow)){
				RecordPreviousTestResults();
				quantityIndex ++;
				RunTest();

			}
			if (Input.GetKeyDown(KeyCode.LeftArrow)){
				RecordPreviousTestResults();
				quantityIndex --;
				RunTest();
			}
			if (Input.GetKeyDown(KeyCode.UpArrow)){
				RecordPreviousTestResults();
				itemIndex ++;
//				quantityIndex = 0;
				RunTest();
			}
			if (Input.GetKeyDown(KeyCode.DownArrow)){
				RecordPreviousTestResults();
//				quantityIndex = 0;
				itemIndex --;
				RunTest();
			}
			if (Input.GetKeyDown(KeyCode.X)){
				testing = false;
				foreach(GameObject o in testObjects){
					Destroy(o);
				}
				testObjects.Clear();

				//				ClearTestObjects();
				DebugText.inst.Hide();
				Player.inst.UnfreezePlayer();
			}

			if (Input.GetKeyDown(KeyCode.S)){
				testing = false;

//				ClearTestObjects();
				DebugText.inst.Hide();
				Player.inst.UnfreezePlayer();
			}
			if (Input.GetKeyDown(KeyCode.S)){
				foreach(GameObject o in testObjects){
					o.isStatic = true;
				}
			}

		}

	}

	List<float> totalFpsDuringTest = new List<float>();
	float testStageDuration = 0f;
	int totalFramesDuringTest = 0;
	int maxFps = 0;
	int minFps = 1000;
	void InitTestEnvironment(){
		testing = true;
		Debug.Log("test init");

		
		GameManager.inst.DestroyAllEphemeralObjects();
		if (!Player.frozen) Player.inst.FreezePlayer();
		MapManager.inst.SelectTerrainByName("Grassland");
		totalFpsDuringTest.Clear();
		ClearTestObjects();
		t = 0;

		quantityIndex = 0; // because first test advances this *before* running itself so that text stats line up with the indices
		itemIndex = 0; 
		Camera.main.farClipPlane = 5000; // player can see everything.
		info = "";
		RunTest();
	}

	void BuildObjects(int qty, GameObject item){
		for (int i=0;i<qty;i++){
			float buffer = 20f; // stops spikeys from atacking me when created.
			float rightFactor = 0.1f;
			float startRight = -50;
			Vector3 pos = Player.inst.transform.position + Player.inst.transform.forward * (i * spacing + buffer) + Player.inst.transform.right * (rightFactor * i * spacing + startRight) + Vector3.up * 10f;
			GameObject testItem = (GameObject) Instantiate(item,pos, Quaternion.identity);
			testObjects.Add(testItem);
		}
	}

	void RecordPreviousTestResults(){
		float totalFps = 0;
		foreach(int f in totalFpsDuringTest){ totalFps += f; }
		totalFpsDuringTest.Clear();
		float avgFpsLastTest = totalFps / (float)totalFramesDuringTest;
		info = quantities[quantityIndex].ToString()+" x "+items[itemIndex].name+" gave "+ avgFpsLastTest+" FPS (avg) \n" + info;
//		WebGLComm.inst.Debug("Current: "+fps+" FPS for "+quantities[quantityIndex].ToString()+" x "+items[itemIndex].name+"");

	}

	void RunTest() {
		t = 0;
		if (quantityIndex < 0) quantityIndex = quantities.Length - 1;
		if (itemIndex < 0) itemIndex = items.Length - 1;
		quantityIndex %= quantities.Length;
		itemIndex %= items.Length;
		ClearTestObjects();
//		t = automaticTestInterval * Mathf.Pow(2.5f,quantities[quantityIndex]);
		BuildObjects(quantities[quantityIndex],items[itemIndex].prefab);
		ResetFpsRecords();

	}
	void ResetFpsRecords() {
		totalFpsDuringTest.Clear();
		totalFramesDuringTest = 0;
		maxFps = 0;
		minFps = 1000;
		testStageDuration = t;
	}


	void ClearTestObjects(){
		foreach(GameObject o in testObjects){
			Destroy(o);
		}
		testObjects.Clear();
	}
}
