using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Screenshotter : MonoBehaviour {
//	public KeyCode screenshotKey = KeyCode.F12;
	public Camera currentScreenshotCamera;
	public RenderTexture rawSource;
	public LayerMask layersToSnap;
	List<Camera> cams = new List<Camera>();
	Texture2D screenShot;
	int index = 0;
	Dictionary<Tree,Material> treeReplacementMaterials = new Dictionary<Tree, Material>();
	public Material treeScreenshotMaterialGreen;
	public Material treeScreenshotMaterialYellow;

	public static Screenshotter inst;

	public void SetInstance(){
		inst = this;
	}

	void Start(){
//		rawSource = new RenderTexture(200,123,16);
//		rawSource.width = 200;
//		rawSource.height = 123;
	}

	public void SwapTreeMaterials(){
		treeReplacementMaterials.Clear();
		foreach(Tree t in FindObjectsOfType<Tree>()){
			Material[] mats = t.GetComponent<Renderer>().materials;
			treeReplacementMaterials.Add(t,mats[1]);
			if (t.name.Contains("chubby")) mats[1] = treeScreenshotMaterialGreen;
			else if (t.name.Contains("birch")) mats[1] = treeScreenshotMaterialYellow;
			t.GetComponent<Renderer>().materials = mats;
		}
	}

	public void RestoreTreeMaterials(){
		foreach(KeyValuePair<Tree,Material> kvp in treeReplacementMaterials){
			Material[] mats = kvp.Key.GetComponent<Renderer>().materials;
			mats[1] = kvp.Value;
			kvp.Key.GetComponent<Renderer>().materials = mats;
		}
		treeReplacementMaterials.Clear();
	}

	public void UpdateCameraList(){
		cams.Clear();
		foreach(ScreenshotCamera sc in FindObjectsOfType<ScreenshotCamera>()){
			cams.Add(sc.gameObject.GetComponent<Camera>());
		}
	}

	public void CycleCamForward(bool flag){
		if (flag){
			index++;
			index %= cams.Count;
		} else {
			index--;
			if (index < 0) index = cams.Count - 1;
		}
		currentScreenshotCamera = cams[index];
		currentScreenshotCamera.cullingMask = layersToSnap;
		UpdateScreenshot();
	}

	bool rotatingRight = false;
	bool rotateRightDir = false;
	public void BeginRotateCameraRight(bool flag){
		t=-.05f;
		rotatingRight = true;
		rotateRightDir = flag;
	}

	bool rotatingUp = false;
	bool rotateUpDir = false;
	public void BeginRotateCameraUp(bool flag){
		t=-.05f;
		rotatingUp = true;
		rotateUpDir = flag;
	}

	public void RotateCameraRight(bool flag){
		int dir = flag ? 1 : -1;
		int deg = 5;
		currentScreenshotCamera.transform.Rotate(Vector3.up,dir*deg,Space.World);
		UpdateScreenshot();
	}

	public void RotateCameraUp(bool flag){
		int dir = flag ? 1 : -1;
		int deg = 5;
		currentScreenshotCamera.transform.Rotate(-Vector3.right,dir*deg,Space.Self);
		UpdateScreenshot();
	}

	public void EndRotate(){
		rotatingUp = false;
		rotatingRight = false;
	}

	float t =0;
	public void Update(){
		t -= Time.unscaledDeltaTime;
		if (t < 0){
			t = .015f;
			if (rotatingRight){
				RotateCameraRight(rotateRightDir);
			} else if (rotatingUp){
				RotateCameraUp(rotateUpDir);
			}
		}
	}

	public static int resWidth=200;
	public static int resHeight=123;

	public void UpdateScreenshot() {
		currentScreenshotCamera.cullingMask = layersToSnap;
		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		currentScreenshotCamera.targetTexture = rt;
		rt.useMipMap = true;
//		// commented Debug.Log("render texture stats. mipmap?"+rt.useMipMap+", renderbuffer:"+rt.colorBuffer);
        screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGBAHalf, false);
		currentScreenshotCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		currentScreenshotCamera.targetTexture = rawSource;
		currentScreenshotCamera.Render();
		RenderTexture.active = null;

        Destroy(rt);
	}

	public string GetCurrentScreenshotByteArrayAsString(){
//		if (screenShot != null){
//			byte[] bArray = screenShot.EncodeToJPG();
//			string data = System.Convert.ToBase64String(bArray);
//			#if UNITY_EDITOR
//			System.IO.File.WriteAllBytes("screenshot_test.jpg", bArray );
//			#endif
////			string data2 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(bArray));
//			Debug.Log("64 data:"+data);
////			Debug.Log("data2:"+data2);
//			return data;
//		} else {
//			Debug.Log("no data");
//			return "no_data";
//		}
		if (currentScreenshotCamera == null) return "no_cam";
		currentScreenshotCamera.cullingMask = layersToSnap;

		RenderTexture rt = new RenderTexture(resWidth,resHeight,16);

		Texture2D shot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, true);
		currentScreenshotCamera.targetTexture = rt;
		//		rt.useMipMap = true;
		//		// commented Debug.Log("render texture stats. mipmap?"+rt.useMipMap+", renderbuffer:"+rt.colorBuffer);
		currentScreenshotCamera.Render();
		RenderTexture.active = rt;
		shot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		byte[] bArray = shot.EncodeToJPG(85);
		string data = System.Convert.ToBase64String(bArray);
		#if UNITY_EDITOR
		System.IO.File.WriteAllBytes("screen_shot.jpg",bArray);
		#else


		#endif
		RenderTexture.active = null;
		currentScreenshotCamera.targetTexture = rawSource;
		return data;
	}

	public SimpleJSON.JSONClass GetScreenshotCameraInfo(){
		// When we save a level, let's report the current screenshot camera's rotation and position
		// So that later when the level is loaded and re-save we can use the same screenshot camera 
		// So that screenshots are not changed on resaves later.
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		if (currentScreenshotCamera){
			N["position"] = JsonUtil.GetTruncatedPosition(currentScreenshotCamera.transform);
			N["rotation"] = JsonUtil.GetRotation(currentScreenshotCamera.transform);
		}
		return N;
	}

	public void SetScreenshotCameraInfo(SimpleJSON.JSONClass N){
		UpdateCameraList();
//		WebGLComm.inst.Debug("setting screenshot cam info:"+N.ToString());
		Vector3 pos = JsonUtil.GetRealPositionFromTruncatedPosition(N);
		for (int i=0;i<cams.Count;i++){
			Camera c = cams[i];
			if (Vector3.SqrMagnitude(pos - c.transform.position) < 5){
				currentScreenshotCamera = c;
				index = i;
				currentScreenshotCamera.transform.rotation = JsonUtil.GetRealRotationFromJsonRotation(N); // We found the same screenshot camera that was used before but perhaps its rotation is new.
//				WebGLComm.inst.Debug("Set screenshot cam success:"+pos);
				return;
			}
		}
//		WebGLComm.inst.Debug("current screenshot camera failed! No camera with position near:"+pos);
	}

	public void SetUpCamera(){
		// This is called when the Save Dialogue is opened
		// If this level is loading from an existing level class, that class should (after Sep 15 2016) have set screenshot camera info
		// If it didn't, the current cam will be null, so set it now
		if (currentScreenshotCamera == null){
			index = 0;
			currentScreenshotCamera = cams[index];
		}
	}

	void OnDestroy(){
//		screenShot.
	}

//	public void TakeScreenshot(){
//		WebGLComm.inst.SendScreenshot(screenShot);
//	}

//	IEnumerator SendScreenshot(Texture2D shot) {	
//		byte[] data = shot.EncodeToPNG();
//		WWWForm form = new WWWForm();
//		form.AddBinaryData("image", data, "screenshot.png", "image/png");
//		Hashtable headers = form.headers;
////			headers["Cookie"] = "sessionid=" + GlobalVars.sessionId;
//		WWW www = ServerComm.GetWWW(Config.serverBaseUrl + "client/screenshot/", form.data, headers);
//		yield return www;		
//		// commented Debug.Log(www.text);
//	}
}
