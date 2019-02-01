using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LevelBuilderCamSkyManager : MonoBehaviour {


	int maxZoom = 1200;
	int minZoom = 5;

	public GameObject leftArrow;
	public GameObject rightArrow;
	public GameObject downArrow;
	public GameObject upArrow;
	public GameObject zoomIn;
	public GameObject zoomOut;
	public GameObject rotateLeft;
	public GameObject rotateRight;
	public Transform centerOfMapObject;
	Vector3 startPos;
	public static LevelBuilderCamSkyManager inst;
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		startPos = transform.position;
//		UpdateZoomMod();
//		// assumes camera starts at the center
//		int range = 1000;
//		bounds[0] = new Vector2 ( transform.position.x - 1000, transform.position.
	}


	public Ray GetRayThroughCenterOfMap(){
		return SkyCamera.ScreenPointToRay(SkyCamera.WorldToScreenPoint(SkyCamera.transform.position +SkyCamera.transform.forward));
	}
	bool scrolling = false;
	void Update(){

		if (contPanRight) PanRightAction ();
		if (contPanForward) PanForwardAction();
		if (contRot) RotateAction();
		if (zooming) ZoomInAction();


//		float vert = Input.GetAxis("Vertical");
//		float horiz = Input.GetAxis("Horizontal");
//		if (vert > 1){
//			PanForward(true);
//		} else if (vert < 1){
//			PanBackward(true);
//		}
//		if (horiz > 1){
//			PanRight(true);
//		} else  {
//			
//		}
//
//		if (horiz < 1){
//			PanLeft(true);
//		} else {
//			PanLeft(false);
//		}
		if (LevelBuilder.inst.panning) return;
		if (EditorTesting.inst.ghosting) return;
		if (LevelBuilder.inst.saveDialogueGO.activeSelf) return;
		if (LevelBuilder.inst.editingSpeech) return; // don't allow hotkeys while editing speech
		if (Input.GetKeyDown(KeyCode.Q)){ // note that rotations are allowed even if we're duplicating, moving, or submenus are open.
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(rotateLeft, pointer, ExecuteEvents.pointerEnterHandler);
			LevelBuilder.inst.RotateCameraLeft(true);
		} else if (Input.GetKeyDown(KeyCode.E)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(rotateRight, pointer, ExecuteEvents.pointerEnterHandler);
			LevelBuilder.inst.RotateCameraLeft(false);
		}
//		if (Input.GetKeyDown(KeyCode.P)){
//			LevelBuilder.inst.ToggleGridSnap();
//		} 
//		if () return;
		if (LevelBuilder.inst.placedObjectContextMenu.activeSelf) {
			// Don't allow hotekeys to work if moving, duplicating, or submenus are open.
			return;
		}

//		if (Input.GetKeyDown(KeyCode.P)){
//			LevelBuilder.inst.TogglePlacementType();
//		}


		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(leftArrow, pointer, ExecuteEvents.pointerEnterHandler);
			panAmountRight = -1;
			PanRightAction();
		} else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(leftArrow, pointer, ExecuteEvents.pointerExitHandler);
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(rightArrow, pointer, ExecuteEvents.pointerEnterHandler);
			panAmountRight = 1;
			PanRightAction();
		} else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(rightArrow, pointer, ExecuteEvents.pointerExitHandler);
		} else if (Input.GetKeyUp(KeyCode.E)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(rotateRight, pointer, ExecuteEvents.pointerExitHandler);
		}

		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(upArrow, pointer, ExecuteEvents.pointerEnterHandler);

			panAmountForward = 1;
			PanForwardAction();
		} else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(upArrow, pointer, ExecuteEvents.pointerExitHandler);
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(downArrow, pointer, ExecuteEvents.pointerEnterHandler);
			panAmountForward = -1;
			PanForwardAction();
		} else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(downArrow, pointer, ExecuteEvents.pointerExitHandler);
		}
		if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus) || (Utils.GetMouseDeltaY() < 0 && Input.mousePosition.x > 342 / GameConfig.screenResolution.x * Screen.width)){
			scrolling = true;
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(zoomIn, pointer, ExecuteEvents.pointerEnterHandler);
			zoomAmount = Utils.GetMouseDeltaY() < 0 ? -2 : -1;
			ZoomInAction();
		} else if (Input.GetKeyUp(KeyCode.Equals) || Input.GetKeyUp(KeyCode.Plus)  || (Utils.GetMouseDeltaY() == 0 && scrolling)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(zoomIn, pointer, ExecuteEvents.pointerExitHandler);
		}
		if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Underscore) || (Utils.GetMouseDeltaY() > 0 && Input.mousePosition.x > 342 / GameConfig.screenResolution.x * Screen.width)){
//			// commented Debug.Log("Input.mousePosition.x;"+Input.mousePosition.x);
			scrolling = true;
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(zoomOut, pointer, ExecuteEvents.pointerEnterHandler);
			zoomAmount = Utils.GetMouseDeltaY() > 0 ? 2 : 1;
			ZoomInAction();
		} else if (Input.GetKeyUp(KeyCode.Minus) || Input.GetKeyUp(KeyCode.Underscore) || (Utils.GetMouseDeltaY() == 0 && scrolling)){
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			ExecuteEvents.Execute(zoomOut, pointer, ExecuteEvents.pointerExitHandler);
		}


		if (Utils.GetMouseDeltaY() != 0){ scrolling = true; } else scrolling = false;

	}

	int counter = 0;
	int interval = 0;
	bool contPanRight = false;
	bool contPanForward = false;
	bool contRot = false;
	bool zooming = false;
	int panAmountRight = 0;
	int panAmountForward = 0;
	int rotAmount = 0;
	int zoomAmount = 0;

	public void PanRight(bool b){
		contPanRight = b;
		panAmountRight = 1;
	}
	public void PanLeft(bool b){
		contPanRight = b;
		panAmountRight = -1;
	}
	public void PanForward(bool b){
		contPanForward = b;
		panAmountForward = 1;
	}
	public void PanBackward(bool b){
		contPanForward = b;
		panAmountForward = -1;
	}
	public void RotateLeft(bool b){
		contRot = b;
		rotAmount = 1;
	}
	public void RotateRight(bool b){
		contRot = b;
		rotAmount = -1;
	}
	public void ZoomIn(bool b){
		zooming = b;
		zoomAmount = -1;
	}
	public void ZoomOut(bool b){
		zooming = b;
		zoomAmount = 1;
	}


	float zoomMod = 1;
	void ZoomInAction(){
		// ortho size between 40 and 400.
		float zoomFactor = Input.GetKey(KeyCode.LeftShift) ? 65f : 5.5f;
		float newOrthoSize = SkyCamera.orthographicSize + zoomAmount * zoomFactor;
		SkyCamera.orthographicSize = Mathf.Clamp(newOrthoSize,minZoom,maxZoom);
		// if fov is min, move slow
		// if fov is max, move fast
	}
	void UpdateZoomMod(){
		// ortho size between 50 and 500
		// zoom mod should be between .1 and .5, right now it goes from .1 to 1
		float spread = maxZoom - minZoom;
		float n = (SkyCamera.orthographicSize - minZoom) / spread; // goes from 0.1 - 1
		zoomMod = (n + 0.1f) / 4f;
//		// commented Debug.Log("zoom mod:"+zoomMod);
	}
	void PanRightAction(){
		UpdateZoomMod();
		counter++;
		if (counter < interval) return;
		counter = 0;
		float moveSpeed = 20;

		Vector3 dir = SkyCamera.transform.right * moveSpeed * panAmountRight * (PanSpeedBasedOnOrthoSize);
		Vector3 newPos = transform.position + dir;
		MoveCameraToPosition(newPos);


	}

	float PanSpeedBasedOnOrthoSize {
		get {
			float ret = Mathf.Log(SkyCamera.orthographicSize)/30f;
//			Debug.Log("ret:"+ret);
			return ret;
//			return 1;
		}
	}

	void MoveCameraToPosition(Vector3 p){
		if (LevelBuilder.inst.CamSkyWithinBounds(p)){
			transform.position = p;
		}
	}

	void PanForwardAction(){
		UpdateZoomMod();
		counter++;
		if (counter < interval) return;
		counter = 0;
		float moveSpeed = 30;
		Vector3 dir = Utils.FlattenVector(SkyCamera.transform.forward) * moveSpeed * panAmountForward * (PanSpeedBasedOnOrthoSize);

		Vector3 newPos = transform.position + dir;
		MoveCameraToPosition(newPos);

	}

	Camera SkyCamera {
		get {
			return LevelBuilder.inst.camSky;
		}
	}


	void RotateAction(){
		counter++;
		if (counter < interval) return;
		counter = 0;
		float rotSpeed = 2;
		Quaternion rot = transform.rotation;
		rot.eulerAngles += new Vector3(0,rotAmount * rotSpeed,0);
		transform.rotation = rot;
	}

	public float ZoomLevel(){
		return SkyCamera.orthographicSize;
	}

	public void StopAllEvents(){ // on end drag, stop all events to stop ghost key repeats (for example after dragging you might get stuck in a "map panning left" situation.)

		// insert key unsticking function here
		PointerEventData pointer = new PointerEventData(EventSystem.current);
		ExecuteEvents.Execute(leftArrow, pointer, ExecuteEvents.pointerExitHandler);
		ExecuteEvents.Execute(rightArrow, pointer, ExecuteEvents.pointerExitHandler);
		ExecuteEvents.Execute(upArrow, pointer, ExecuteEvents.pointerExitHandler);
		ExecuteEvents.Execute(downArrow, pointer, ExecuteEvents.pointerExitHandler);
		ExecuteEvents.Execute(zoomOut, pointer, ExecuteEvents.pointerExitHandler);
		ExecuteEvents.Execute(zoomIn, pointer, ExecuteEvents.pointerExitHandler);

	}
}
