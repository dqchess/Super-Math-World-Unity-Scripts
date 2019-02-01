using UnityEngine;
using System.Collections;

public class AspectUtility : MonoBehaviour {

	public float wantedAspectRatio = 1.77778f;
	public Camera cam;
//	public bool fixedXWidth = false;
//	 Camera backgroundCam;
//	float startingOrthoSize;
	public bool bubugg = false;
	Rect startingRect;
	bool intialized = false;
	void Init () {
		if (intialized) return;
		intialized =true;
		startingRect = cam.rect;
//		if (debugg) Debug.Log("start x, y, wid, he:"+startingRect.x+","+startingRect.y+","+startingRect.width+","+startingRect.height);
//		cam = GetComponent<Camera>();

	}

	public void SetCamera (		) {
		return;
		Init();
//		Debug.Log("attempt set cam on:"+this.name);
		float currentAspectRatio = (float)Screen.width / Screen.height;
		// If the current aspect ratio is already approximately equal to the desired aspect ratio,
		// use a full-screen Rect (in case it was set to something else previously)
		Rect desiredRect;
		if ((int)(currentAspectRatio * 100) / 100.0f == (int)(wantedAspectRatio * 100) / 100.0f) {
			cam.rect = startingRect; //new Rect(0.0f, 0.0f, 1.0f, 1.0f);
//			Debug.Log("startrect.");
//			if (backgroundCam) {
//				Destroy(backgroundCam.gameObject);
//			}
			return;
		}

		// Pillarbox
		if (currentAspectRatio > wantedAspectRatio) {
			float inset = 1.0f - wantedAspectRatio/currentAspectRatio;
			// for sky cam,
			// rect x actually need to be 0.3 of the "middle part", discouting the stripes at the right and left. The returend valued will be a decimal 0 -1.
			// So.. we get the value of 0.3 * desired width
			// wait the total stripe widht on the outside is called "inset", so..
			// 

			desiredRect = new Rect(inset/2f + startingRect.x*(1-inset), 0.0f, startingRect.width*(1.0f-inset), 1.0f);


//			if (startingRect.x == 0) desiredRect = new Rect(inset/2f, 0.0f, startingRect.width*(1.0f-inset), 1.0f);
//			else desiredRect = new Rect(startingRect.x + inset*(1-startingRect.x)/2f, 0.0f, startingRect.width*(1.0f-inset), 1.0f);

		}
		// Letterbox
		else {
			float inset = 1.0f - currentAspectRatio/wantedAspectRatio;
			desiredRect = new Rect(startingRect.x, inset/2f, startingRect.width, 1.0f-inset);
			if (cam.orthographic){
//				cam.orthographicSize = startingOrthoSize / (1.0f-inset);
			}
		}
//		if (debugg) Debug.Log("des x, y, wid, he:"+desiredRect.x+","+desiredRect.y+","+desiredRect.width+","+desiredRect.height);
		if (cam.rect != desiredRect){
			cam.rect = desiredRect;
			WebGLComm.inst.Debug("new cam rect;"+desiredRect);
//			WebGLComm.inst.Debug("resized cam "+cam.name+" to "+cam.rect);
		} 
//		else {
//			WebGLComm.inst.Debug("did not resize cam "+cam.name+" to "+desiredRect+", cam is already;"+cam.rect);
//		}
//		foreach(Camera c in pairedCameras){
//			if (c.rect != desiredRect) {
//				
//				c.rect = cam.rect;
//			}
//		}
//
	}

	public  int screenHeight {
		get {
			return (int)(Screen.height * cam.rect.height);
		}
	}

	public  int screenWidth {
		get {
			return (int)(Screen.width * cam.rect.width);
		}
	}

	public  int xOffset {
		get {
			return (int)(Screen.width * cam.rect.x);
		}
	}

	public  int yOffset {
		get {
			return (int)(Screen.height * cam.rect.y);
		}
	}

	public Rect screenRect {
		get {
			return new Rect(cam.rect.x * Screen.width, cam.rect.y * Screen.height, cam.rect.width * Screen.width, cam.rect.height * Screen.height);
		}
	}

//	public  Vector3 mousePosition {
//		get {
//			Vector3 mousePos = Input.mousePosition;
//			mousePos.y -= (int)(desiredRect.y * Screen.height);
//			mousePos.x -= (int)(desiredRect.x * Screen.width);
//			return mousePos;
//		}
//	}
//
//	public  Vector2 guiMousePosition {
//		get {
//			Vector2 mousePos = Event.current.mousePosition;
//			mousePos.y = Mathf.Clamp(mousePos.y, cam.rect.y * Screen.height, cam.rect.y * Screen.height + cam.rect.height * Screen.height);
//			mousePos.x = Mathf.Clamp(mousePos.x, cam.rect.x * Screen.width, cam.rect.x * Screen.width + cam.rect.width * Screen.width);
//			return mousePos;
//		}
//	}
}