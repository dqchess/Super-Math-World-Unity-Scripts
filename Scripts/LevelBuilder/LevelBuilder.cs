using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using UnityEngine.EventSystems; // raycast result?
using System.Linq;


public enum CameraPositionMode {
	North,
	NorthEast,
	East,
	SouthEast,
	South,
	SouthWest,
	West,
	NorthWest
}

public enum LBMode {
	None,
	Placing,
	Editing,
	Duplicating,
	Moving,
	Snapping,
	EditingSeparately,
}

public class BuildableObject {
	public GameObject prefab;
	public Texture2D icon;

}

public enum BuildMode {
	None,
	PlacingObject,
	ObjectContextMenu,
	SequenceCreation
}

//public class DuplicatingObject {
//	// used for tracking a list of duplicating objects 
//	// so that when we duplicate a group we can save them 
//	// and can dupe them one by one in Update instead of all at once which causes chug
//	public string myName;
//	public Vector3 pos;
//	public Quaternion rot;
//	public Vector3 localScale;
//	public SimpleJSON.JSONClass props;
//	public float dupeTimer;
//
//
//}


public class LevelBuilder : MonoBehaviour {


	public ParticleSystem highlightParticles;
	public Material objectHighlightMat;
	public Material posiitonFxMat;

	public PlayerCostumeController levelBuilderAvatarCostumeController;
	public Text playerName;


	public GameObject speechEditDialogue;
	public GameObject textEditDialogue;
	public bool editingSpeech {
		get {
			return speechEditDialogue.activeSelf || textEditDialogue.activeSelf; 	
		}
	}
//	List<DuplicatingObject> dupeObjects = new List<DuplicatingObject>();
	public GameObject fakePreviewButton;
	public GameObject previewButton;
	public GameObject mapButtons;
	public GameObject blockPlayerCube;

	public Transform camSkyNorthEastBoundary;
	//endmoving

	public static LevelBuilder inst;
	public GameObject levelBuilderUiParent;
	public GameObject eventSystem;
	public Text markerObjectContextMenuItemTitle;

	public delegate void LevelBuilderOpenedDelegate();
	public LevelBuilderOpenedDelegate levelBuilderOpenedDelegate;


	public delegate void LevelBuilderPreviewClickedDelegate();
	public LevelBuilderPreviewClickedDelegate LevelBuilderPreviewClicked;

	public Camera camUI;
	public UIHoverHelp hoveringButton;
	public CameraPositionMode cameraMode = CameraPositionMode.South;
	public Sprite questionIcon;
	public Sprite warningIcon;
	public Sprite dragHand;
	public Texture2D blankCursor;
	public LBMode mode;

	public Camera camSky;
	public GameObject inventoryParent;
	public GameObject copyingDialogue;


	public GameObject placedObjectContextMenu;
	public GameObject placedObjectContextMenuCoverDark;
	public GameObject POCMFractionButton;
	public GameObject POCMFractionZeroButton;
	public GameObject POCMGroupButton;
	public GameObject POCMTextTriggerButton;
	public GameObject POCMTagSendMessageButton;

	public GameObject POCMModFaucetButton;
	public GameObject POCMModifyCharacterButton;

	public GameObject POCMResourceDropButton;
	public GameObject POCMModifyColorButton;
	public GameObject POCMObjectCyclerButton;
	public GameObject POCMRandomFractionButton;
	public GameObject POCMModifyAnimalRulesButton;
	public GameObject placedObjectContextMenuNumberWallSizeButton;
	public GameObject placedObjectContextMenuNumberWallRoundSizeButton;
	public GameObject placedObjectContextMenucharacterSpeechBubbleButton;

	public GameObject POCMheightButton;
	public GameObject POCMmatrixFloorSizeButton;

	public GameObject POCMintegerButton;
	public GameObject POCMhatButton;
	public GameObject POCMmodRiser;
	public GameObject POCMmodTowerHeight;

	public GameObject POCMcopyButton;
	public GameObject POCMcopyUpButton;
	public GameObject POCMModCannonButton;
	public GameObject POCMlinkLevelButton;
	public GameObject POCMcubeSizeManipulator;
	public GameObject POCMscaleManipulatorMultiple;
	public GameObject POCMEditDanMeyerCube;
	public GameObject placedObjectContextMenuLevelZoneButton;
	public Dropdown levelZoneDropdown;
	public InputField levelZoneInput;
	public GameObject playerMarker;

	public GameObject dialogueDuplicating;
	public GameObject sequenceCreationDialogue;

	public GameObject saveDialogueGO;
	public GameObject saveDialogueSaveButton;
	public GameObject saveDialoguePleaseWait;
	public GameObject saveDialogueSavecomplete;
	public GameObject saveDialogueSaveStarted;
//	public Text saveDialogueLevelURL;
//	public GameObject tutorialHelpWindow;
	public GameObject saveHelpWindow;
	[SerializeField] public List<Transform> submenus = new List<Transform>();
	[SerializeField] public List<Transform> markerMenus = new List<Transform>();

	

	public InputField fractionNumerator;
	public InputField fractionDenominator;
	public InputField LinkLevelDestinationInput;
	public Text LinkLevelDestinationName;
	public Text fractionButtonNumerator;
	public Text fractionButtonSlash;
	public Text fractionButtonDenominator;
	public Text fractionButtonInteger;
	public Text wallSizeX;
	public Text wallSizeY;
	public Text wallSizeZ;
	public Text numberRiserScale;
	public Text cameraDirectionText;
	public Text levelTitle;
	public Text roundWallHeight;
	public Text roundWallRadius;
	public Text roundWallDegrees;




	public bool levelBuilderIsShowing = false;
	Material m;

	bool dragging = false;
	public GameObject dragIconObj;
	public Image dragIconObjectImage;
	public Sprite dragIcon;
	public InputField levelNameInput;
	public InputField levelTagsInput;
	public InputField levelDescriptionInput;



	float minReselectRange = 0.05f; //0.085f;
	float maxReselectRange = 0.385f;
	






	bool placing = false;
	bool modifyingPlacedPiece = false;
	public GameObject currentPiece;
	GameObject hoveringPiece;

	

	List<GameObject> currentSequence = new List<GameObject>();
	List<List<GameObject>> sequencedObjects = new List<List<GameObject>>();

	Vector2 mousePositionContextMenu = Vector2.zero;
//	public Transform dragIconParent;

	public bool CamSkyWithinBounds(Vector3 p){ // TODO: Move this to Cam Sky manager
//		// commented Debug.Log("P.z: "+p.z+", camskynorthboundz:"+camSkyNorthBoundary.position.z);
//		// commented Debug.Log("P.z: "+p.z+", camskysouthboundz:"+camSkySouthBoundary.position.z);

		if (
			p.z > -MapManager.inst.camBoundBuffer //camSkySouthBoundary.position.z
			&& p.z < camSkyNorthEastBoundary.position.z + MapManager.inst.camBoundBuffer // camSkyNorthBoundary.position.z
			&& p.x > -MapManager.inst.camBoundBuffer  // camSkyWestBoundary.position.x
			&& p.x < camSkyNorthEastBoundary.position.x + MapManager.inst.camBoundBuffer// camSkyEastBoundary.position.x
		) return true;
		else {
			return false;
		}
	}
	public float hoverDelay = 8.9f;

	GameObject snapFromVertexSphere; // vertex snapping fx sphere
	GameObject snapToVertexSphere; // vertex snapping fx sphere
	void Start(){
		
		#if UNITY_EDITOR
		previewButton.SetActive(true);
		fakePreviewButton.SetActive(false);
		#endif
	
		// Init the vertex snapping spheres
		snapFromVertexSphere =GameObject.CreatePrimitive(PrimitiveType.Sphere);
		snapFromVertexSphere.transform.localScale = Vector3.one*1.5f;
		snapFromVertexSphere.GetComponent<Renderer>().material.color = Color.red;
		Destroy(snapFromVertexSphere.GetComponent<Collider>());
		snapFromVertexSphere.SetActive(false);
		snapFromVertexSphere.transform.parent = transform; // dont destroy me on scene load

		snapToVertexSphere =GameObject.CreatePrimitive(PrimitiveType.Sphere);
		snapToVertexSphere.transform.localScale = Vector3.one*1.5f;
		snapToVertexSphere.GetComponent<Renderer>().material.color = Color.blue;
		Destroy(snapToVertexSphere.GetComponent<Collider>());
		snapToVertexSphere.SetActive(false);
		snapToVertexSphere.transform.parent = transform; // dont destroy me on scene load

//		placedObjectContextMenu += 
//		HideLevelBuilder(true);


	}

	// Sequencing
//	float duplicateTimer = 0;
	float maintainMinimumPosY = 0;

	SimpleJSON.JSONClass currentPieceProperties = new SimpleJSON.JSONClass();
	public void UserPressedDuplicate(){
		// user pressed "copy" form the marker context object menu
		// this method creates a hovering piece that the user can drag around and sets it up so that each click duplicates the hovering piece
		// this object that's created is never itself placed -- eventually when theuser cancels copy mode this object is destroyed.
//		duplicateTimer = .1f;
		duplicateTransitionStarted = Time.time;
//		// commented Debug.Log("mode dupe");
		SetMode(LBMode.Duplicating);
		UserEditableObject ueo = currentPiece.GetComponent<UserEditableObject>();
		maintainMinimumPosY = currentPiece.transform.position.y;


		fixedYplane.SetActive(true);
		Vector3 p = fixedYplane.transform.position;
		fixedYplane.transform.position = new Vector3(p.x,currentPiece.transform.position.y,p.z);

//		CloseContextMenuSubMenus();
		GameObject o = null;
		if (ueo.GetType() == typeof(UEO_DraggingParent)){
			o = (GameObject)Instantiate(ueo.gameObject); // it's ok to use instances instead of prefabs here, because this object being created is never saved it's only
			ueo.OnObjectWasDuplicated();
			// used as a hovering piece visual for the user during the duplicate process
		} else {
			// regular duplciae a regular object
			currentPieceProperties = ueo.GetProperties();
//			Debug.Log("curprop:"+currentPieceProperties);
			o = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName(ueo.myName);
//			Debug.Log("o:"+o);
			o.GetComponent<UserEditableObject>().SetProperties(currentPieceProperties);
			o.GetComponent<UserEditableObject>().OnLevelBuilderObjectCreated();
		}

		o.transform.rotation = currentPiece.transform.rotation;
		SetHoveringPiece(o);
//		// commented Debug.Log("hover piece:"+hoveringPiece);
		dragging = true;
		ClosePlacedObjectContextMenu();
	}


	public GameObject fixedYplane; // for raycasting into a fixed height
	public void UserFinishedDuplicate(){
//		Debug.Log("finish dupe");
		fixedYplane.SetActive(false);
		Utils.RemoveInteractableLevelBuilderComponents(hoveringPiece);
//		FinishedDragParent();
		dialogueDuplicating.GetComponent<ShrinkAndDisable>().Begin();
		if (hoveringPiece) MapSmokePuff(hoveringPiece.transform.position);
		DestroyHoverPiece();

//		// commented Debug.Log("setnone userfin dup");
		SetMode(LBMode.None);
//		hoveringPiece = null;
//		dragging = false;

	}

	public void UserPressedAddSequence(){
		sequenceCreationDialogue.SetActive(true);
	}

	public void EnterSequenceSelectMode(){
		sequenceCreationDialogue.SetActive(false);
		currentSequence = new List<GameObject>();
	}
	
	public void AddObjectToCurrentSequence(){
	}


	List<Image> tempImages = new List<Image>();
	public void TempRedImage(GameObject go){
		Image o = go.GetComponent<Image>();
		if (!o){
			//			// commented Debug.Log("err, no outline comp on:"+go.name);
			return;
		}
		o.color = Color.red;
		if (tempImages.Contains(o)){
			//			// commented Debug.Log("err, alerady contaions");
			return;
		}
		tempImages.Add(o);
	}

	List<Outline> tempOutlines = new List<Outline>();
	void TempRedOutline(GameObject go){
		Outline o = go.GetComponent<Outline>();
		if (!o){
//			// commented Debug.Log("err, no outline comp on:"+go.name);
			return;
		}
		o.effectColor = Color.red;
		if (tempOutlines.Contains(o)){
//			// commented Debug.Log("err, alerady contaions");
			return;
		}
		tempOutlines.Add(o);
	}



	public void SnapPanToCurrentObject(){
		camSky.transform.parent.position = currentPiece.transform.position + markerUpOffset;
	}

	public void SnapPanToPosition(Vector3 p){
		camSky.transform.parent.position = p;
	}

	Vector3 markerUpOffset = new Vector3(0,3,0); // because the actual "center" of the object is usually not the pivot, which is at the "bottom" of the object. We want to center on the object for viewing it
	public void SmoothPanToCurrentObject(){
		if (currentPiece){
			SmoothPanTo(currentPiece.transform.position+ markerUpOffset);
		}
	}


	void SmoothPanTo(Vector3 p){
//		// commented Debug.Log("panto:"+p+",playerpos;"+Player.inst.transform.position);
//		if (TutorialManager.inst){
//			if (TutorialManager.inst.currentTutorial){
//				if (TutorialManager.inst.tutorialActive){
//					return; // No panning whilst tutorial is active.
//				}
//			}
//		}
//		// commented Debug.Log("smooth pan.. :"+p);
		// if P is pretty high up, we need to increase the x or z position of the pantarget, according to which direction we're facing (NSEW) by root 2 (cam views at 45 angle).
		// Maybe we can cast a ray parallel to camsky forward from the player to the terrain, and pan to that position? 

//		RaycastHit hit;
//		if (Physics.Raycast(p,camSky.transform.forward,out hit,Mathf.Infinity,FindObjectOfType<SceneLayerMasks>().terrainAndWaterForObjectPlacement)){
//			p = hit.point;
//
//		}


		panTarget = p; //new Vector3(p.x,camSky.transform.parent.position.y,p.z);
		panning = true;
	}

	void SetMode(LBMode m){
//		// commented Debug.Log("mode was set:"+m);
		if (m == LBMode.Duplicating || m == LBMode.Placing || m == LBMode.Moving){
			mapButtons.SetActive(false);
		} else {
			mapButtons.SetActive(true);
		}
		mode = m;
	}

	void SetHoveringPiece(GameObject o){
		
		if (o == null && hoveringPiece){
			DestroyHoverPiece();
		}
		hoveringPiece = o;
	}




	public bool firstTimeOpened = true;



	// 
	[System.NonSerialized] public bool panning = false;
	Vector3 panTarget;

	// Piece drag
	Vector3 draggingPieceOriginPosition = Vector3.zero;


	// Map drag

	Vector3 skyCamPivotPosWhenDragClicked = Vector3.zero;
//	bool mapDrag = false;
	bool waitForMouseUp = false;
	float closeMenuTimeout = 0;

	float dragThreshholdTimeMap = 0.05f;
	float dragThreshholdTimeItem = 0.15f;
	Vector2 mouseButtonDownLocation;
	Vector2 mouseButtonDownLocationScreen;
	float distanceMouseMovedWhileHeldDown =0;
	Vector2 mouseDownDelta;


	bool clickedMap = false;
	bool clickedTerrain = false;
	public GameObject canvasOverlay; //This is the wrong way to do sorting layers for camera, one is screen space overlay, one is cam space overlay, because i can't figure out how to make it look correct wihtout two types..need to switch this obj on and off for level builder to appear "in front" of it


	float hoverCheckTimer = -0;
	bool lastClickWasTerrain = false;
	Vector3 currentPos = Vector3.zero;
	Vector3 placementPos = Vector3.zero;
	bool clickedNearLevelBuilderObject = false;

	GameObject uiElementUnderCursor;
	RaycastHit hitSky;

	bool mouseDown = false;
	bool mouseUp = false;
	bool mouseRightUp = false;
	bool mouseHeld = false;
	bool mouseRightHeld = false;
	float mouseHeldForSeconds = 0;
	public List<RaycastResult> objectsHit;
	bool fadeBox = false;


	public GameObject draggingParent; // exposed for use by ClipboardManager.. ugh





	List<GameObject> GetItemsInsideBox(GameObject boxSelectObject){
		List<GameObject> ret = new List<GameObject>();
		Rect rect = boxSelectObj.GetComponent<RectTransform>().rect;
		Vector2 anc = boxSelectObj.GetComponent<RectTransform>().anchoredPosition;
		Vector2 sz = boxSelectObj.GetComponent<RectTransform>().sizeDelta;
		rect.xMin = anc.x;
		rect.xMax = anc.x + sz.x;	
		rect.yMin = anc.y;
		rect.yMax = anc.y + sz.y;
		//			rect.position = boxSelectObj.GetComponent<RectTransform>().anchoredPosition;
		//			Debug.Log("Rect:"+rect);
		//			rect.y += boxSelectObj.GetComponent<RectTransform>().anchoredPosition;

		foreach(LevelBuilderSelectableObject lbso in LevelBuilderObjectManager.inst.GetCachedObjects()){
			if (lbso){
				Vector2 rawScreenPos = camSky.WorldToScreenPoint(lbso.transform.position);
				Vector2 screenPos = new Vector2(rawScreenPos.x*ffx,rawScreenPos.y*ffy); // if window is actually larger than 1024x568 then we adjust for that
				if (rect.Contains(screenPos)){
					//					Debug.Log("grab:"+ueo.myName+", at :"+ueo.transform.position);

					ret.Add(lbso.gameObject);
				}
			} else {
				//					Debug.Log("ueo not grabbed:"+ueo.myName+" with pos;"+camSky.WorldToScreenPoint(ueo.transform.position));
			}
		}
		return ret;
	}

	float ffx = 0;
	float ffy = 0;
	void LateUpdate(){
		if (!LevelBuilder.inst.levelBuilderIsShowing) return;
		if (!camSky.enabled || !camUI.enabled) return;
		// Boxing is done here, because we want to draw it last to avoid jitter
		ffx = GameConfig.screenResolution.x / Screen.width; // adjust positions based on screenr esolution vs. expected xy positions and sizes of ui elements (based on a 1024x568 grid
		ffy = GameConfig.screenResolution.y / Screen.height;
		if (mouseRightHeld && boxing){
			
			float boxSizeX = mouseDownDelta.x * ffx; //camUI.ScreenToWorldPoint(mouseDownDelta).x;// * Screen.width;
			float boxSizeY = mouseDownDelta.y * ffy; // camUI.ScreenToWorldPoint(mouseDownDelta).y;// * Screen.height;
			Vector2 boxAnchorPosition = new Vector2(mouseButtonDownLocationScreen.x * ffx,mouseButtonDownLocationScreen.y * ffy);

			if (boxSizeX < 0) {
				boxAnchorPosition = new Vector2(Input.mousePosition.x*ffx,boxAnchorPosition.y);
				boxSizeX *= -1;
				// we dragged left
			}

			if (boxSizeY < 0) {
				boxAnchorPosition = new Vector2(boxAnchorPosition.x,Input.mousePosition.y*ffy);
				boxSizeY *= -1;
				// we dragged down
			}
			boxSelectObj.GetComponent<RectTransform>().anchoredPosition = boxAnchorPosition;
			boxSelectObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,boxSizeX);
			boxSelectObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,boxSizeY);


//				UnhighlightAll();
//				highlightObjectMultiple = new GameObject("highlight obj multiple");
			foreach(GameObject o in GetItemsInsideBox(boxSelectObj)){
				MakeHighlightFX(o.transform,"boxed");

			}


		}
		if (boxing && mouseRightUp){
//			Debug.Log("up");
			boxing = false;
//			UnhighlightAll();
			fadeBox = true;
			List<GameObject> boxedItems = GetItemsInsideBox(boxSelectObj);



			if (boxedItems.Count > 0){
				// create a temporary object
			
				draggingParent = MakeDraggingParentWithPieces(boxedItems);


				// center it with these pieces


				// place it with menu including move and height functions
				PlaceContextMenu(draggingParent);
			}

//			Debug.Log("anchor:"+boxSelectObj.GetComponent<RectTransform>().anchoredPosition+",rect bounds:"+rect.xMin+","+rect.xMax+","+rect.yMin+","+rect.yMax);
		}

		if (fadeBox){
			Color c= boxSelectObj.GetComponent<Image>().color;
			float fadeSpeed = 7f;
			boxSelectObj.GetComponent<Image>().color = Color.Lerp(c,new Color(c.r,c.g,c.b,0f),Time.deltaTime*fadeSpeed);
		


		} else {
			EffectsManager.inst.BlueRing(Vector3.zero + Vector3.up * -1000,Vector3.zero); // sadly if we don't emit this one here, the blue rings will stick around from the last time due to levelbuilder freezing time (or something)
		}
	}

	void FixedUpdate(){
		
		if (!levelBuilderIsShowing) return; 

//		// commented Debug.Log("Fixed");
		hitSky = GetMouseRaycastInformation();
		objectsHit = GetUIObjectsUnderCursor(); 
		HandleHoverHelp();
	}



	void FinishedMoving(){
		//					PlaceObject();
		fixedYplane.SetActive(false);
		dragging = false;
		currentPiece = hoveringPiece; 
		hoveringPiece = null;
//		Debug.Log("<color=#fff>Currentpiece</color>;"+currentPiece);
		PlaceContextMenu(currentPiece,true);
		AudioManager.inst.PlayLevelBuilderItemPlace();

		SetMode(LBMode.None);
//		currentPiece.GetComponent<UserEditableObject>().OnLevelBuilderObjectMoveFinished();

	}

	float deleteTimer = 0;
	bool mouseHeldDown = false;
//	public bool overrideLevelBuilderMouseEvents = false;
	void Update(){
		HandleHighlighterTimer(); // need this to "de highlight" things even after editor is closed.
		if (!levelBuilderIsShowing) return; 
//		if (mode == LBMode.EditingSeparately) return; // 
		if (!camSky.enabled || !camUI.enabled) return;
//		if (overrideLevelBuilderMouseEvents) return; // for some objects, we edit them using a separate camera and we don't want the levelbuilder stuff working

		List<GameObject> fxToDel = new List<GameObject>();
		float fxSpeed = 1200f;
		float cutoff = .1f;
		foreach(KeyValuePair<GameObject,Vector3> kvp in fxObjects){
			kvp.Key.transform.position = Vector3.MoveTowards(kvp.Key.transform.position, kvp.Value,Time.deltaTime * fxSpeed);
			if (Vector3.Distance(kvp.Key.transform.position,kvp.Value) < cutoff){
				fxToDel.Add(kvp.Key);
			}
		}
		foreach(GameObject o  in fxToDel){
			AudioManager.inst.PlayInventoryClose();
			fxObjects.Remove(o);
			Destroy(o);
		}


		if (Input.mouseScrollDelta.y != 0){
			foreach(RaycastResult rr in objectsHit){
				if (!rr.gameObject.GetComponent<TabContentBackground>()){
//					LevelBuilderTabManager.inst.ScrollCurrentTabToTop();
				}
			}
		}

		if (needLerpScrollVal){
			float lerpSpeed = 5;
			currentScrollRect.verticalScrollbar.value = Mathf.Lerp(currentScrollRect.verticalScrollbar.value,currentScrollRectTargetScrollVal,Time.deltaTime * lerpSpeed);
//			Debug.Log("val:"+currentScrollRect.verticalScrollbar.value);
			if (Mathf.Abs(currentScrollRect.verticalScrollbar.value-currentScrollRectTargetScrollVal) < .01f || Input.mouseScrollDelta.y != 0){
				currentScrollRect.verticalScrollbar.value = currentScrollRectTargetScrollVal;
				needLerpScrollVal = false;
			}
		}

		if (needLoadLevelObjects){
			loadLevelObjectsTimer -= Time.deltaTime;
			if (loadLevelObjectsTimer < 0){
				needLoadLevelObjects = false;
				AudioManager.inst.LevelBuilderOpened();
				JsonLevelLoader.inst.LoadTempLevelJson();

			}
		}


		if (rotatingCamera){
			camSky.transform.parent.rotation = Quaternion.RotateTowards(camSky.transform.parent.rotation,targetCameraRotation,Time.deltaTime * 350f);
			if (Mathf.Abs(camSky.transform.parent.rotation.eulerAngles.y - targetCameraRotation.eulerAngles.y) < 4) {
				camSky.transform.parent.rotation = targetCameraRotation;
				rotatingCamera = false;
			}   
		}

		if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) && !AnySubMenusAreOpen()){
			UserFinishedDuplicate();
			UserFinishedPlacingObject();
		}

		// hotkeys
		if (placedObjectContextMenu.activeSelf && !AnySubMenusAreOpen()){
			if (Input.GetKeyDown(KeyCode.M)){
				BeginMovingObject();
			} else if (Input.GetKeyDown(KeyCode.C)){
				CloseContextMenuSubMenus();

				dialogueDuplicating.GetComponent<SinPop>().Begin();
				UserPressedDuplicate();
			} else if (Input.GetKeyDown(KeyCode.Backspace)){
				DeleteCurrentObject();
			} 
		}

//		if (!AnySubMenusAreOpen() && Input.GetKeyDown(KeyCode.H)){
//			
//			TogglePositionHelper();
//		}


		if (deleteTimer >= 0) deleteTimer -= Time.deltaTime;

		mouseDown = Input.GetMouseButtonDown(0);
		mouseUp = Input.GetMouseButtonUp(0);
		mouseHeld = Input.GetMouseButton(0);
		mouseRightHeld = Input.GetMouseButton(1);
		mouseRightUp = Input.GetMouseButtonUp(1);
	
		// Attempting to refactor Update loop in the following way.
		// Right now I'm focusing on Events as the top level organization, such as mousedown, mouse hold, mouse up, and always. 
		// In each action we take into account what mode we were in, and what state the game is in, in order to decide what to do.
		// The refactor reverses this, and focuses on state first, handling clicks separately per state. I think this is more intuitive
		// And will help get rid of bugs. For example somtimes when dragging "Placing mode" a new object, it gets stuck, stops following cursor,
		// and then disappears when you click away..




		// BEGIN refactored code focusing on game state as top level:


		// Handle Mouse Clicks
		// ACTION: MOUSE DOWN
		/*
		 * 
		 * 
		 * 
		 * */


		if (Input.GetMouseButtonDown(1)){
			if (hoveringPiece) {
				hoveringPiece.transform.Rotate(new Vector3(0,hoveringPiece.GetComponent<UserEditableObject>().rotationSnap,0),Space.World);
				ShowIndicatorArrow(hoveringPiece);
			}
			//				UserFinishedDuplicate();
		} else if (Input.GetMouseButtonDown(2)){
			if (hoveringPiece) {
				hoveringPiece.transform.Rotate(new Vector3(0,-hoveringPiece.GetComponent<UserEditableObject>().rotationSnap,0),Space.World);
				ShowIndicatorArrow(hoveringPiece);
			}
			//				UserFinishedDuplicate();
		}

		if (hoveringPiece){
			// This is where we decide where the hovering piece will be displayed on the map
			// note relevant variabels palcementpos, currentpos, hoveringpiece, forcesnap
//			bool forceSnap = hoveringPiece.GetComponent<UserEditableObject>().GetType() == typeof(UEO_DraggingParent);
			placementPos = HandleGridSnap(currentPos, hoveringPiece.GetComponent<UserEditableObject>().gridSnapSpacing);
			placementPos = HandleUpOffset(placementPos); //Vector3.up * HandleUpOffset();
			hoveringPiece.transform.position = placementPos;

			if (MouseIsOverMapViewport()){
				hoveringPiece.SetActive(true);
			} else {
				hoveringPiece.SetActive(false);
			}
		}

		Transform objectToSelect = GetObjectToSelect();
		bool preserveHighlight = false;
		switch (mode){
		// NOTE: If there is no hoverinpiece, then the mode should unequivally be NONE or EDIITNG.
		case LBMode.Duplicating: 
//			duplicateTimer -= Time.deltaTime;
			if (mouseUp){
				if (CanPlaceHere() && DuplicateTransitionTimeReached()){
					DuplicateObject(placementPos,hoveringPiece);

				} else {
//					// commented Debug.Log("mouseup dupe, couldn't place here! "+placementPos);
				}
			}

			break;
		case LBMode.Editing: 
			MakePulsateFX(currentPiece);

			break;
		case LBMode.Moving: 
			
			if (mouseDown){
//				// commented Debug.Log("mousedown on moving");
				if (CanPlaceHere()){
					
					FinishedMoving();

				} else {
					Debug.Log("cannot place here");
					ReturnPieceToStartingPosition(hoveringPiece);
					SetHoveringPiece(null);
					fixedYplane.SetActive(false); // awkward place for this to be, i'd rather it be wrapped in finishmoving() which it is and call finishmoving() here, but can't because i don't want all those commmands in finishedmoving() to fire in this case so this is how it is for now :/
//					// commented Debug.Log("setnone moving");
					SetMode(LBMode.None);
				}
			}
			MakePulsateFX(hoveringPiece);



			break;
		case LBMode.None:
			if (objectToSelect && !boxing) {
				if (Input.GetKey(KeyCode.G)){
					List<GameObject> highObjs = new List<GameObject>();
					foreach(UserEditableObject u in LevelBuilderGroupManager.inst.GroupContainingObject(objectToSelect.GetComponent<UserEditableObject>())){
						highObjs.Add(u.gameObject);
					}
//						if (!highlightObjectMultiple){
//							highlightObjectMultiple = new GameObject("highlight obj multiple");
//						}
					foreach(GameObject o in highObjs){
						MakeHighlightFX(o.transform,"group");
					}
				} else {
//						MakeHighlightFX(objectToSelect.gameObject,highlightObject.transform,Color.yellow);
					MakeHighlightFX(objectToSelect.transform,"objtosel");
				}
			}
			if (Input.GetKey(KeyCode.LeftShift)){
				if (mouseUp && objectToSelect){

					// Establish the existing selected items, if any
					List<GameObject> piecesSelectedSoFar = new List<GameObject>();
					if (draggingParent){
						foreach(Transform t in draggingParent.transform){
							piecesSelectedSoFar.Add(t.gameObject);
						}
					}


					if (objectToSelect.GetComponent<UEO_DraggingParent>()){
//						Debug.Log("Dragp");
						// we have shift-selected something that was already selected. Attempt to unselect it.
						bool bbreak = false;
						foreach(RaycastHit hit3 in Physics.RaycastAll(camSky.ScreenPointToRay(Input.mousePosition))){
//							Debug.Log("hit3;"+hit3.collider.name);
							if (bbreak == true) break;
							foreach(Transform t in objectToSelect.transform){
//								Debug.Log("t in dp:"+t.name);
								if (t == Utils.GetTransformJustAboveRoot(hit3.collider.transform)){// awkwardly get the next-to-top level collider
									
									// verified we hit a direct child of the dragging parent. Remove it
									piecesSelectedSoFar.Remove(t.gameObject);
									t.parent = null;
//									Debug.Log("remove:"+objectToSelect.name+", ct:"+piecesSelectedSoFar.Count);
									bbreak = true; // lol nested break
									break;
								}
							}
						}
					} else {
						piecesSelectedSoFar.Add(objectToSelect.gameObject);
//						Debug.Log("add:"+objectToSelect.name+", ct:"+piecesSelectedSoFar.Count);
					}



					if (piecesSelectedSoFar.Count > 0){
						draggingParent = MakeDraggingParentWithPieces(piecesSelectedSoFar);

					
						foreach(Transform dt in draggingParent.transform){
							MakeHighlightFX(dt,"dragparent");
						}

//						draggingParent.transform.position = objectToSelect.transform.position;
					}
				}
			} else if (mouseUp && !placedObjectContextMenu.activeSelf && !mapIsBeingDragged && (uiElementUnderCursor == null || uiElementUnderCursor.name == "MapTarget") && deleteTimer < 0){
//				Debug.Log("mouseup");
//				Transform objectToSelect = GetObjectToSelect();
				if (objectToSelect) {
					if (Input.GetKey(KeyCode.G)){ // grouping
						List<UserEditableObject> gr = LevelBuilderGroupManager.inst.GroupContainingObject(objectToSelect.GetComponent<UserEditableObject>());
						if (gr.Count > 0){
							List<GameObject> gro = new List<GameObject>();
							foreach(UserEditableObject uu in gr){
								gro.Add(uu.gameObject);
							}
//							List<GameObject> gro = new List<GameObject>(gr.Cast<GameObject>());
								//gr.ConvertAll(x => (GameObject)x);
							draggingParent = MakeDraggingParentWithPieces(gro);
							draggingParent.GetComponent<UEO_DraggingParent>().groupedState = GroupedState.Grouped;
							PlaceContextMenu(draggingParent);
						} else {
							PlaceContextMenu(objectToSelect.gameObject);
							Debug.Log("tried to grab an object that was not in a group, while holding G for group mode");
						}
					} else {
//						Debug.Log("mode;"+mode);
//						Debug.Log("<color=#ff0'>Place context</color>;"+objectToSelect.gameObject.name);
						PlaceContextMenu(objectToSelect.gameObject);
					}
				}
			} else if (mouseRightUp){
//				Transform objectToSelect = GetObjectToSelect();
				if (objectToSelect) {
					float sqr = Vector2.SqrMagnitude(boxSelectObj.GetComponent<RectTransform>().sizeDelta);
					if (sqr < 10) {
						DeleteObject(objectToSelect.gameObject);
					}
				}
			} else if (Input.GetKeyUp(KeyCode.LeftShift)){
				if (draggingParent && !placedObjectContextMenu.activeSelf && !mapIsBeingDragged && (uiElementUnderCursor == null || uiElementUnderCursor.name == "MapTarget") && deleteTimer < 0){
					PlaceContextMenu(draggingParent);

				}

			}
				

			break;
		case LBMode.Placing: 
			if (mouseUp){
//				if (!MouseIsOverMapViewport()) {
//					// Dropped the item "off screen," destroy it
//					SetHoveringPiece(null);
//					SetMode(LBMode.None);
//				} else 
				if (CanPlaceHere()){ // Note a failed query  will drop a "cancel" flying ui object unless off-screen
					PlaceObject(); 
				} else {
					SetHoveringPiece(null);
					SetMode(LBMode.None);
				}
			}
			break;
		default:break;
		}
		if (Input.GetMouseButtonDown(1) && boxSelect && MouseIsOverMapViewport() && mode == LBMode.None){
//			Debug.Log("start");
			boxing = true;
			fadeBox = false;
			boxSelectObj.GetComponent<Image>().color = Utils.LightBlueColor();
		}


		Vector3 mousePosition = Input.mousePosition;
		if (mouseDown){
			mouseHeldForSeconds = 0;
			mouseHeldDown = true;
			lastClickWasTerrain = false;
			mouseButtonDownLocation = camUI.ScreenToViewportPoint(Input.mousePosition);
			distanceMouseMovedWhileHeldDown = 0;
		}
		if (Input.GetMouseButtonDown(1)){
			mouseButtonDownLocationScreen = Input.mousePosition;
			mouseDownDelta = Vector3.zero;
		}

		if (mouseUp){
			ClearDragIcon(true);
			mouseHeldDown = false;

		}

		if (mouseHeldDown){
			mouseHeldForSeconds += Time.deltaTime;
			distanceMouseMovedWhileHeldDown = Vector2.Distance(mouseButtonDownLocation,camUI.ScreenToViewportPoint(Input.mousePosition));
//			// commented Debug.Log("dist:"+distanceMouseMovedWhileHeldDown);
		}
		if (mouseRightHeld){
			mouseDownDelta =  (Vector2)Input.mousePosition - (Vector2)mouseButtonDownLocationScreen; //(Vector3)camUI.ScreenToViewportPoint(Input.mousePosition);
		}



	
//

		HandleMapPanning(panTarget);
		HandleTempRedOutlines();
		HandleTempRedImages();
		HandleMapDrag();
		HandleReturnMisplacedObjectsToStartingPosition();
		HandleDragIcon(mouseHeldDown);
		HandleMouseCloseToEdgeWhileMovingObject();
//		HandleDuplicatingObjectsList();
		HandleVertexSnapping(); // a user holds v
		HandleHighlightsForDraggingParent();

		//ENDUPDATE



	}

	void HandleHighlightsForDraggingParent(){
		if (draggingParent){
			foreach(Transform t in draggingParent.transform){
				MakeHighlightFX(t,"dragparent handled");
			}
		}
	}



	#region Vertex Snapping 



	void HandleVertexSnapping(){
		if (!Input.GetKey(KeyCode.LeftShift) && mode != LBMode.Duplicating && mode != LBMode.Moving){
			// Todo fragile/critical -- this may break if we are able to SWITCH modes in code below 
			// (cuasing the mode to switch will not allow the code block below to execute, which may result in a broken chain, e.g.
			// the highlight might not go away, the colliders might not re enable, or other
			// Fix: Snapping is a "mode" just like other modes and is handled in the Mode switch statement in Update, not here.
			if (Input.GetKey(KeyCode.V) && !AnySubMenusAreOpen()){
				if (placedObjectContextMenu.activeSelf) placedObjectContextMenu.GetComponent<ShrinkAndDisable>().Begin();
				// What do we want?
				// After pushing V the INITIAL piece hit is the one we want to STAY on -- no grabbing verts from other pieces!

	//			if (snappingPiece != null && Input.GetKey(KeyCode.L)){
	//				snapPieceLocked = true;
	//			} else {
	//				snapPieceLocked = false;
	//			}
				mode = LBMode.Snapping;
//				UnhighlightAll();
	//			if (Input.GetMouseButtonDown(0)){
	//				if (snappingPiece != null){
	//					Debug.Log("mouse down, snap piece:"+snappingPiece.snapPiece.name);
	//				}
	//			}
				if (Input.GetMouseButton(0) && snappingPiece != null && snappingPiece.snapPiece){
	//				Debug.Log("snappingPiece:"+snappingPiece.snapPiece.name);
					SetCollidersOnSnapPiece(snappingPiece,false);
					SnappingPiece pieceToSnapTo = TryFindSnapPiece(snapToVertexSphere);
					Vector3 snapDelta = Vector3.zero;
					if (pieceToSnapTo != null){
						//						Vector3 originVertWorldP = snappingPiece.meshFilter.transform.TransformPoint(snappingPiece.meshFilter.sharedMesh.vertices[snappingPiece.vertexIndex]);
						//						Vector3 destVertWorldP = pieceToSnapTo.meshFilter.transform.TransformPoint(pieceToSnapTo.meshFilter.sharedMesh.vertices[pieceToSnapTo.vertexIndex]);
						snapDelta = pieceToSnapTo.vertWorldPosition - snappingPiece.vertWorldPosition;
						snappingPiece.snapPiece.transform.position = snappingPiece.originalPosition + snapDelta; // move the object from its origin world position the distance from the origin vert to the new vert
					}
	//				EditorTesting.inst.TempDebugSphere(snappingPiece.vertWorldPosition + snapDelta,Color.red);// .transform.TransformPoint,c);
				}
				if (Input.GetMouseButtonUp(0)){
	//				Debug.Log("mouse up! snappingpiece:"+snappingPiece.snapPiece.name);
	//				Debug.Log("mouse up! colsen:"+snappingPiece.collidersEnabled);
					SetCollidersOnSnapPiece(snappingPiece,true);


				}
				if (!Input.GetMouseButton(0)){
					snappingPiece = TryFindSnapPiece(snapFromVertexSphere,true);
				}

			} else if (Input.GetKeyUp(KeyCode.V) && !AnySubMenusAreOpen()) {
				if (currentPiece) {
					placedObjectContextMenu.GetComponent<SinPop>().Begin();
					SmoothPanToCurrentObject();
				}
				if (snappingPiece != null) {
	//				Debug.Log("V up! Snappingpiece:"+snappingPiece.snapPiece.name);
	//				Debug.Log("V up! colsen:"+snappingPiece.collidersEnabled);
				} else {
	//				Debug.Log("v up no snappice");
				}
				SetCollidersOnSnapPiece(snappingPiece,true);
				mode = LBMode.None;
				lastSnappedPiece = null;
				snapToVertexSphere.SetActive(false);
				snapFromVertexSphere.SetActive(false);
				if (snapHighlightParent){
					Destroy(snapHighlightParent.gameObject);
				}
				RestoreAllSnappingColliders(); // A horrible function used to clean up the mess of de-activating and sometimes not re-activating colliders during vertex snapping.
			}
		}
	}

	List<Collider> snappedColliders = new List<Collider>();
	void RestoreAllSnappingColliders(){
		foreach(Collider c in snappedColliders){
			c.enabled = true;
		}
		snappedColliders.Clear();
	}

	SnappingPiece snappingPiece = null;


	class SnappingPiece {
		public GameObject snapPiece;
		public MeshFilter meshFilter;
		public int vertexIndex = -1;
		public Collider[] colliders;
		public bool collidersEnabled = true;
		public Vector3 originalPosition;
		public Vector3 vertWorldPosition;

	}

	Transform snapHighlightParent;
	SnappingPiece lastSnappedPiece;
	bool snapPieceLocked = false; // if true, snapping piece cannot change away from lastSnappedPiece

	void SetCollidersOnSnapPiece(SnappingPiece sn, bool flag){
//		Debug.Log("Try col!:"+flag);
		if (sn != null){
			foreach(Collider c in sn.colliders){
				if (c && c.enabled != flag) {
					c.enabled = flag;
					if (!flag && !snappedColliders.Contains(c)){
						snappedColliders.Add(c);
					}
//					Debug.Log(flag+"collider on:"+sn.snapPiece.name);
				}
			}
//			sn.collidersEnabled = flag;
		}
	}


	SnappingPiece TryFindSnapPiece(GameObject fxSphere, bool snapPieceLocked = false){
//		Debug.Log("try:");
		MeshFilter closestMf = null;
		int closestVertIndex = -1;
//		Debug.Log("hitsky:"+hitSky.collider);
//		Debug.Log("locked:"+snapPieceLocked);
		RaycastHit hit = GetRaycastHitUnderCursor(snapPieceLocked);
		if (snapHighlightParent){
			Destroy(snapHighlightParent.gameObject);
		}
		snapHighlightParent = new GameObject().transform;
//		Debug.Log("hitt:"+hitT);
		if (hit.collider){
			Vector3 closestP = Vector3.zero;
			MeshFilter mf = hit.collider.GetComponent<MeshFilter>();

			if (mf && (!snapPieceLocked || lastSnappedPiece == null || lastSnappedPiece.snapPiece == hit.collider.transform.root.gameObject) ){ // either snap piece was not lcoked, or if it was, it's ok because the root is the same
				Mesh mesh = mf.sharedMesh;
				Vector3[] vertices = mesh.vertices;
//				int[] triangles = mesh.triangles;
			
//				Debug.Log("hit triangle index:"+hitSky.triangleIndex+" while mf had :"+mesh.vertexCount);

				for (int vertexIndex = 0; vertexIndex < mesh.vertexCount;vertexIndex++){ // hitSky.triangleIndex * 3; vertexIndex < hitSky.triangleIndex * 3 + 3; vertexIndex++){
					
					Vector3 worldP = mf.transform.TransformPoint(vertices[vertexIndex]);
					if (Vector3.SqrMagnitude(worldP-hit.point) < Vector3.SqrMagnitude(closestP-hit.point)){
//						Debug.Log("worldp:"+worldP+", hitskyp:"+hitSky.point);
						closestP = worldP;
						closestMf = mf;
						closestVertIndex = vertexIndex;

					}
				}
			}
		}
		if (closestMf) {
//			Debug.Log("Close!:"+closestMf+", index:"+closestVertIndex);
			Vector3 vertWorldPosition = closestMf.transform.TransformPoint(closestMf.sharedMesh.vertices[closestVertIndex]);
//			EditorTesting.inst.TempDebugSphere(vertWorldPosition,c);
			fxSphere.transform.position = vertWorldPosition;
			fxSphere.SetActive(true);
			fxSphere.transform.localScale = Vector3.one * Mathf.Clamp(camSky.orthographicSize / 20f,1.5f,10f);
			SnappingPiece ret = new SnappingPiece();
			ret.snapPiece = hit.collider.transform.root.gameObject;
			ret.vertexIndex = closestVertIndex;
			ret.originalPosition = ret.snapPiece.transform.position;
			ret.meshFilter = closestMf;
			ret.colliders = ret.snapPiece.GetComponentsInChildren<Collider>();
//			ret.collidersEnabled = true;
			ret.vertWorldPosition = vertWorldPosition;

			MakeHighlightFX(ret.snapPiece.transform);
			lastSnappedPiece = ret;
//			Debug.Log("try "+Mathf.Round(Time.time)+", "+source+ ", got piece;"+hit.collider.gameObject.name);
			return ret;
		} else {
			fxSphere.SetActive(false);
//			Debug.Log("try "+Mathf.Round(Time.time)+", "+source+ ", nah");
			return null;
		}
		// now we have a closest mesh m, and the closest vertex p
	}

	#endregion // vertex snapping

	float duplicateTransitionStarted = 0;
	bool DuplicateTransitionTimeReached(){
		// prevent a "duplicate" action from executing until after a second after the user pressed "duplicate". Prevents accidental duplicating of object in the same frame when the user clicked the "duplicate" button, especially happens when lagging.
		float duplicateTransitionThreshhold = 0.4f;
		return Time.time > duplicateTransitionStarted + duplicateTransitionThreshhold;
	}






	bool needResetCamPan = false;
	void HandleMouseCloseToEdgeWhileMovingObject(){
		if (mode == LBMode.Duplicating || mode == LBMode.Moving || mode == LBMode.Placing){
			needResetCamPan = true;
//			// commented Debug.Log(camSky.ScreenToViewportPoint(Input.mousePosition).x+","+camSky.ScreenToViewportPoint(Input.mousePosition).y);

			Vector3 mp = camSky.ScreenToViewportPoint(Input.mousePosition);
			if (mp.x < -0.2f || mp.x > 1.2f || mp.y < -0.2f || mp.y > 1.2f){ 
				// mouse went out of bounds while copying or whatever. 
				if (mode == LBMode.Moving) {
					ReturnPieceToStartingPosition(hoveringPiece);
					fixedYplane.SetActive(false);
					SetHoveringPiece(null);
					SetMode(LBMode.None);
				} else if (mode == LBMode.Duplicating) {
					UserFinishedDuplicate();
					SetHoveringPiece(null);
					SetMode(LBMode.None);
				} else if (mode == LBMode.Placing) {
					// don't do anything!
				}
					
				
				return;
			} 
			Vector2 scrollDir = new Vector2(0,0);
			if (mp.x > 0.9f){
				scrollDir.x = 1;
			} else if (mp.x < 0.1f){
				scrollDir.x = -1;
			}
			if (mp.y > 0.85f){
				scrollDir.y = 1;
			} else if (mp.y < 0.15f){
				scrollDir.y = -1;
			}
			if (scrollDir.x > 0){
				LevelBuilderCamSkyManager.inst.PanRight(true);
			} else if (scrollDir.x < 0 && mode != LBMode.Placing){
				LevelBuilderCamSkyManager.inst.PanLeft(true);
			} else {
				LevelBuilderCamSkyManager.inst.PanLeft(false);
			}
			if (scrollDir.y > 0){
				LevelBuilderCamSkyManager.inst.PanForward(true);
			} else if (scrollDir.y < 0){
				LevelBuilderCamSkyManager.inst.PanBackward(true);
			} else {
				LevelBuilderCamSkyManager.inst.PanBackward(false);
			}
//			// commented Debug.Log("scrolldirx:"+scrollDir);
		} else if (needResetCamPan){ // After finishing duplicate, make sure we don't continue to pan the map!
			needResetCamPan = false;
			LevelBuilderCamSkyManager.inst.PanBackward(false);
			LevelBuilderCamSkyManager.inst.PanForward(false);
			LevelBuilderCamSkyManager.inst.PanLeft(false);
			LevelBuilderCamSkyManager.inst.PanRight(false);
		}
	}

	bool CanPlaceHere(){
		bool f = canPlaceHereC();
		if (!f && MouseIsOverMapViewport()){
			GameObject cancel = (GameObject) Instantiate(EffectsManager.inst.flyingCancel,placementPos,Quaternion.identity);
		}
		return f;
	}
	bool canPlaceHereC(){
//		bool canPlaceHere = false;
//		float footprint = 0;
		if (!hoveringPiece) return false;
//		UserEditableObject ueo = hoveringPiece.GetComponent<UserEditableObject>();
//		if (ueo){
//			footprint = ueo.Footprint;
//		}

		LevelBuilderObjectManager.inst.UpdateCachedObjects();
		foreach(LevelBuilderSelectableObject lbso in LevelBuilderObjectManager.inst.GetCachedObjects("LB.CanPlace?")){
			if (lbso){
				Transform t = lbso.transform;
				if (t.gameObject != hoveringPiece) {
					bool rotSame = false;
					// because rotations are funny and if you don't round, two "identical" objects could have slightly diff rotations.
					if (Mathf.Round(t.rotation.eulerAngles.y) == Mathf.Round(hoveringPiece.transform.rotation.eulerAngles.y)
						&& Mathf.Round(t.rotation.eulerAngles.x) == Mathf.Round(hoveringPiece.transform.rotation.eulerAngles.x)
						&& Mathf.Round(t.rotation.eulerAngles.z) == Mathf.Round(hoveringPiece.transform.rotation.eulerAngles.z)) rotSame = true; 
					if (t.position == hoveringPiece.transform.position && hoveringPiece.GetComponent<UserEditableObject>().myName == t.GetComponent<UserEditableObject>().myName && rotSame ){
						return false;
					}
				}
			}
		}




		// Hit nearby object?
		//		List<Transform> lobs = GetObjectsWithinRangeOfWorldPosition(placementPos, footprint);
		//		float hitObjsFootprint = 0;
//		foreach(Transform lobb in lobs){
//			UserEditableObject ueoo = lobb.GetComponent<UserEditableObject>();
//			if (ueoo){
//				if (lobb.gameObject != hoveringPiece) {
//					hitObjsFootprint += ueoo.Footprint;
//					if (ueoo.transform.position == ueo.transform.position){
//						// Hit an identical piece in the same location.
//
//						return false;
//					} else {
////							// commented Debug.Log("ueoo, ueo:"+ueoo.transform.position+","+ueo.transform.position);
//					}
//				}
//
//			}
//		}

		//			// commented Debug.Log("mouse over map:"+mouseIsOverMap+", ter:"+mouseButtonHeldDownWhileOverTerrain);

		return (MouseIsOverMapViewport() && MouseIsHoveringOverTerrain());


	}




	RaycastHit GetMouseRaycastInformation(){
		RaycastHit hitSky = new RaycastHit();
		Ray raySky = camSky.ScreenPointToRay(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0));
//		// commented Debug.Log("mousepos;"+Input.mousePosition);
		float maxYpos = -Mathf.Infinity;
		// If holding mouse

		//		RaycastHit hit;
		if (MouseIsOverMapViewport()){
			//			if (Physics.Raycast(raySky,out hitSky, Mathf.Infinity,SceneLayerMasks.inst.canPlaceLevelObjectsOnTheseLayers)){
			//				// commented Debug.Log("hit sky;"+hitSky.collider.name);	
			//			}
			//			currentPos = hitSky.point;
			//			hitSky = hit;
			RaycastHit[] hits = Physics.RaycastAll(raySky,Mathf.Infinity,SceneLayerMasks.inst.canPlaceLevelObjectsOnTheseLayers);
//			RaycastHit[] hits = new RaycastHit[0];//
			string hitstring = "";
			foreach(RaycastHit hh in hits){
				hitstring += hh.collider.name;
				GameObject hitObj = hh.collider.transform.root.gameObject;
				if (hitObj == hoveringPiece) continue;
				if (hh.point.y > maxYpos){
					maxYpos = hh.point.y;
					hitSky = hh;
					currentPos = hitSky.point;
//					if (hitObj.GetComponent<UserEditableObject>()){ 
//						Debug.Log("HIT:"+hitSky.collider.gameObject.name);
//					}
				}
			}
			//			// commented Debug.Log("hits:"+hitstring);
			//			// commented Debug.Log("cp:"+currentPos+", hit:"+hitSky.collider.name);


		}
//		// commented Debug.Log("hitsky;"+hitSky);
		return hitSky;
	}

	void HandleHoverHelp(){
		hoverCheckTimer -= Time.deltaTime;
		if (hoverCheckTimer < 0){
			hoverCheckTimer = 0.1f;
			
			if (false == AnyMenusAreOpen()){
				//				// commented Debug.Log("current tutorial null!");
				//		foreach(RaycastResult rr in objectsHit){
				if (objectsHit.Count > 0){
					uiElementUnderCursor = objectsHit[0].gameObject;
					UIHoverHelp btn = objectsHit[0].gameObject.GetComponent<UIHoverHelp>();
					if (btn){
						btn.ShowHoverHelp();
					} else {
						HoverHelpOff();
					}
				} else {
					uiElementUnderCursor = null;
				}
			} else {
				HoverHelpOff();
				//				// commented Debug.Log("cur tut:"+TutorialManager.inst.currentTutorial.name);
			}
		}

	}

	[System.NonSerialized] public float verticalSnapSpacing = 0.5f;
	Vector3 HandleUpOffset(Vector3 placementPos){
//		int verticalSnapSpacing = vec; //hoveringPiece.GetComponent<UserEditableObject>().VerticalSnapSpacing;
		if (mode == LBMode.Duplicating || mode == LBMode.Moving){
//			float newY = maintainMinimumPosY;
//			newY = Mathf.Round(newY / verticalSnapSpacing) * verticalSnapSpacing;
//			Debug.Log("hitkyy;"+hitSky.point.y+",miny;"+maintainMinimumPosY);
			return new Vector3(placementPos.x,maintainMinimumPosY,placementPos.z);
//		} else if (mode == LBMode.Moving && maintainCurrentPieceY){ // In case we want to maintain the Y position of what we're moving. Let's say for now we DONT'
//			return new Vector3(placementPos.x,movingY,placementPos.z);
		} else {
			float newY = (verticalSnapSpacing > 0 ? Mathf.Round(placementPos.y / verticalSnapSpacing) * verticalSnapSpacing : placementPos.y) + hoveringPiece.GetComponent<UserEditableObject>().UpOffset;
//			// commented Debug.Log("up:"+hoveringPiece.GetComponent<UserEditableObject>().UpOffset+", oly:"+placementPos.y+", newy;"+newY);
			return new Vector3(placementPos.x, newY, placementPos.z); 
		}

//		return hoveringPiece.GetComponent<UserEditableObject>().UpOffset;

	}


	Vector3 HandleGridSnap(Vector3 pp,float gridSnapSpacing, bool forceSnap = false){
		if (!forceSnap && !gridSnap) {
			return pp;
		} else {
			Vector3 ret = new Vector3(
				Mathf.Round( pp.x / gridSnapSpacing) * gridSnapSpacing, 
				pp.y, 
				Mathf.Round( pp.z / gridSnapSpacing)  * gridSnapSpacing
			);
			Debug.Log("grid snapping pp:"+pp+" to :"+ret);
			return ret;
		}
		


	}

	void ReturnPieceToStartingPosition(GameObject o){
		if (!draggedPiecesToReturn.ContainsKey(o)){
			if (!o.activeSelf) o.SetActive(true);
			draggedPiecesToReturn.Add(o,draggingPieceOriginPosition);
		}
		hoveringPiece = null;
		// commented Debug.Log("setnone returned");
		SetMode(LBMode.None);
	}


	RaycastHit GetRaycastHitUnderCursor(bool locked=false){
		float highestY = Mathf.NegativeInfinity;
//		Transform highest = null;
//		Debug.Log("hit>");
		RaycastHit hitt = new RaycastHit();
		Ray ray = camSky.ScreenPointToRay(Input.mousePosition);
		foreach(RaycastHit hit in Physics.SphereCastAll(ray,.5f)){
//			Debug.Log("hit:"+hit.collider.name);
			if (lastSnappedPiece != null && locked && hit.transform.root.gameObject != lastSnappedPiece.snapPiece){
//				Debug.Log("can't.");
				continue;
			}
			if (locked && lastSnappedPiece == null && currentPiece != null && hit.collider.transform.root.gameObject != currentPiece){
				// we were "locked" (tryinn to find a piece to "snap" or "move" to a new vertex position
				// current piece was not null, so marker menu is active
				// a snap piece was not already gotten
				// don't allow snap piece to not be the current piece!
				continue;
			}
			if (hit.point.y > highestY){
				hitt = hit;
				//				highest = hit.collider.transform;
				highestY = hit.point.y;
			}
		
		}
//		Debug.Log("hit:"+hitt.collider.name);
		return hitt;
	}

	Transform GetItemUnderCursor(){
//		RaycastHit hit;
//		Debug.Log("hit>"+Time.time);
		float highestY = Mathf.NegativeInfinity;
		Transform highest = null;
		Ray ray = camSky.ScreenPointToRay(Input.mousePosition);
		foreach(RaycastHit hit in Physics.RaycastAll(ray,Mathf.Infinity,SceneLayerMasks.inst.skyCamVisible)){
//			Debug.Log("hit;"+hit.collider.name);
			if (hit.collider.transform.root.GetComponent<UserEditableObject>()){
//				Debug.Log("hit success;"+hit.collider.name);
				if (hit.point.y > highestY){
					highest = hit.collider.transform.root;
					highestY = hit.point.y;
				}
//				Debug.Log("indeed clicked;"+hit.collider.name);
			}
		}
//		if (highest) Debug.Log("item under c:"+highest.name);
//		else Debug.Log("no high");
		return highest;
//		return null;
	}

	#region Highlighting

	float highTimer = 0f;
	class HighlightedObject {
		public Dictionary<Renderer,Material[]> highlightedMaterials = new Dictionary<Renderer, Material[]>();
		public float timeHighlighted = 0f;
		
	}
	Dictionary<Transform,HighlightedObject> highlightedObjects = new Dictionary<Transform,HighlightedObject>();
//	Dictionary<Transform,float> highlightLocations = new Dictionary<Transform,float>();
	Color transparentHighlightColor = new Color(1,1,0,0.45f); 
	void MakeHighlightFX(Transform t, string source = "default"){
		if (t.GetComponent<UEO_DraggingParent>()) return;
		if (placedObjectContextMenu.activeSelf) return;
		if (mode == LBMode.Moving || mode == LBMode.Placing) return;
		if (Input.GetKeyDown(KeyCode.V)) return; // not during vertex snap
		// Temporarily swaps all materials in the transform to a new yellow material
		// keeps track of WHEN this transform was requested to be highlighted, so it will time out after .15 seconds in Handle Highlight Timer and revert back to normal
//		Debug.Log("<color=#ff0>Highlight:</color>"+t.name);
		MakeHighlightFX2(t,source); // split into 2 because some editing features want to skip the above constraints -- highlighting in this case happens outside the context of editor cam
	}
	public void MakeHighlightFX2(Transform t, string source){
		if (highlightedObjects.ContainsKey(t)){ 
//			Debug.Log("Highlight<color=#700> failed</color>, ct: "+highlightedObjects.Count);
			// We were already highlighting this transform, so update the time highlighted
			highlightedObjects[t].timeHighlighted = Time.time;
		} else {
//			Debug.Log("Highlight<color=#070> success </color> "+t.name+ " from <color=#f70>"+source+ "</color> ct: "+highlightedObjects.Count);
			// Highlgiht this object for the first time
			HighlightedObject ho = new HighlightedObject();

			foreach(Renderer r in t.GetComponentsInChildren<Renderer>()){
				if (r.gameObject.GetComponent<IgnoreHighlight>()) continue;
				if (r.gameObject.GetComponent<ParticleRenderer>()) continue;
				if (r.gameObject.GetComponent<CCText>()) continue;
				ho.highlightedMaterials.Add(r,r.sharedMaterials);
				Material[] yellowmats = new Material[r.sharedMaterials.Length];
				for(int i=0;i<yellowmats.Length;i++){
					yellowmats[i] = new Material(Shader.Find("Transparent/Diffuse"));

					if (yellowmats[i].HasProperty("_Color")) yellowmats[i].color = transparentHighlightColor; 
				}
				r.sharedMaterials = yellowmats;

			}
			ho.timeHighlighted = Time.time;
			highlightedObjects.Add(t,ho);
		}
	}

	void HandleHighlighterTimer(){
		highTimer -= Time.deltaTime;
		if (highTimer < 0f){
			highTimer = 0.05f;
			float endHighlightThreshhold = .1f;
			Dictionary<Transform,HighlightedObject> toUnhighlight = new Dictionary<Transform,HighlightedObject>();
			foreach(KeyValuePair<Transform,HighlightedObject> kvp in highlightedObjects){
				float delta = Time.time - kvp.Value.timeHighlighted;
				if (delta > endHighlightThreshhold){
//					Debug.Log("adding to unhighi/light. delta:"+delta);
					toUnhighlight.Add(kvp.Key,kvp.Value);
					foreach(KeyValuePair<Renderer,Material[]> kvp2 in kvp.Value.highlightedMaterials){
						if (kvp2.Key){
							foreach(Material m in kvp2.Key.sharedMaterials){
								if (m.shader == Shader.Find("Transparent/Diffuse") && m.color == transparentHighlightColor){ // try and make sure we don't delete any non-highlight mats
//									Debug.Log("Highlight<color=#900> removed </color> "+m.name+ " ct: "+highlightedObjects.Count);
									Destroy(m);
								}
							}
							kvp2.Key.sharedMaterials = kvp2.Value;
//							Debug.Log("Highlight<color=#900> reset materials on  </color>"+kvp.Key.name);
						}
						//						Debug.Log("Set matierlas i THOUGHT");
					}
				} 
			}
			foreach(KeyValuePair<Transform,HighlightedObject> kvp in toUnhighlight){
//				foreach(KeyValuePair<Renderer,Material[]> kvp2 in kvp.Value.highlightedMaterials){
//					foreach(Material m in kvp2.Value){
//						DestroyImmediate(m,true);
//					}
//				}
//				DestroyImmediate(
				highlightedObjects.Remove(kvp.Key);
//				if (kvp.Key) Debug.Log("Highlight<color=#f00> removed final </color> "+kvp.Key.name+ " ct: "+highlightedObjects.Count);
			}

		}
	}

	#endregion
		
//		highTimer -= .1f;
//		if (!highlightLocations.ContainsKey(t)){
//			highlightLocations.Add(t,Utils.ApproxSizeXZ(t).magnitude * 3f);
//		}
//		Vector3 p = t.position + Vector3.up * 2f;
//			highlightLocations.Add(p,);
//		}
//	}

	
	Transform GetObjectToSelect(float selectRange = 12f){
		if (!MouseIsOverMapViewport() || AnyMenusAreOpen()) return null;
		// First, did we straight up click on an obj? If so just retunr that one.
		Transform ret = GetItemUnderCursor();
		if (ret) return ret;
		else { // NO? Ok, do a raycast from the cursor to the terrain and see if we clicked NEAR an object....I guess....
			
			ret = GetClosestItemNearCursor(currentPos);

			if (!ret) {
//				// commented Debug.Log("nothing to find");
				return null;
			}
			float dist = Vector3.Distance(ret.transform.position,currentPos);
			float max  = selectRange;

	
			return dist < max ? ret : null;
		}
	}

	void HandleReturnMisplacedObjectsToStartingPosition() {
		if (draggedPiecesToReturn.Count > 0){
			
			float returnSpeed = 5.7f;
			List<GameObject> toRemove = new List<GameObject>();
			foreach(KeyValuePair<GameObject,Vector3> kvp in draggedPiecesToReturn){
//				// commented Debug.Log("returning:"+kvp.Key.name);
				if (kvp.Key == null) toRemove.Add(kvp.Key);
				else {
					kvp.Key.transform.position = Vector3.Lerp(kvp.Key.transform.position,kvp.Value,Time.deltaTime * returnSpeed);
//					UpdateContextMenuPosition();
					if (Vector3.Distance(kvp.Key.transform.position,kvp.Value) < 2f){
						kvp.Key.transform.position = kvp.Value;
						kvp.Key.GetComponent<UserEditableObject>().OnLevelBuilderObjectMoveFinished();
						toRemove.Add(kvp.Key);

//						}
					}
				}
			}
			foreach(GameObject o in toRemove){
				
				draggedPiecesToReturn.Remove(o);
			}
		}

	}

	void HandleDragIcon(bool mouseHeldDown){
		if (mouseHeld){
			float xPosScreenThreshhold = 330f / 960f;
			Vector2 mousePos = camUI.ScreenToViewportPoint(Input.mousePosition);
			if ((mousePos.x < xPosScreenThreshhold) && mode != LBMode.Duplicating){
				if (!dragIconObj.activeSelf) {
					Cursor.visible = false;
					dragIconObj.SetActive(true);
				}
				dragIconObj.transform.position = camUI.ScreenToWorldPoint(Input.mousePosition);
				dragIconObj.transform.position = Utils.FlattenVectorZ(dragIconObj.transform.position);
			} else if (!mapIsBeingDragged){
				ClearDragIcon();
			}
		} else {
			ClearDragIcon();
		}


	}

//	bool MouseIsHoveringOverTerrain(){

//	float mouseBounds = 200f; // this is the distance OUTSIDE the terrain that people can place things.
	bool MouseIsHoveringOverTerrain(){
		Terrain at = MapManager.inst.currentMap.map.GetComponent<Terrain>();
		bool flag = hitSky.point.x > -MapManager.inst.camBoundBuffer 
			&& hitSky.point.z > -MapManager.inst.camBoundBuffer 
			&& hitSky.point.x < at.terrainData.size.x +MapManager.inst.camBoundBuffer 
			&& hitSky.point.z < at.terrainData.size.z + MapManager.inst.camBoundBuffer;
//		Debug.Log("hitsky:"+hitSky.point+", flag;"+flag);
		bool ret = hitSky.point.x > -MapManager.inst.camBoundBuffer 
			&& hitSky.point.z > -MapManager.inst.camBoundBuffer 
			&& hitSky.point.x < at.terrainData.size.x +MapManager.inst.camBoundBuffer 
			&& hitSky.point.z < at.terrainData.size.z + MapManager.inst.camBoundBuffer;
//		Debug.Log("mouse over ter?:"+ret+" with hitsky.point:"+hitSky.point);
		return ret;
//			mouseButtonHeldDownWhileOverTerrain = true;
//		}
	}


	bool mapIsBeingDragged = false;
	void HandleMapDrag(){

		bool canBeginDraggingMap = false;
		if (!hoveringPiece && 
			(!uiElementUnderCursor || uiElementUnderCursor.name == "MapTarget") 
			&& !panning
			&& MouseIsOverMapViewport() 
			&& !placedObjectContextMenu.activeSelf 
			&& !mapIsBeingDragged 
//			&& MouseIsHoveringOverTerrain() 
			&& AnyMenusAreOpen() == false 
			&& mode != LBMode.Duplicating
			&& mode != LBMode.Snapping
			&& distanceMouseMovedWhileHeldDown > .008f


		){
			canBeginDraggingMap = true;
//			// commented Debug.Log("dragtrue");
		} else {


//			// commented Debug.Log(
		}

		if (mouseHeld){
//			if (!MouseIsHoveringOverTerrain()) { // hmm
//				mapIsBeingDragged = false;
//			}
			{ // Dragging the map.
				if (canBeginDraggingMap && !mapIsBeingDragged){
					mapIsBeingDragged = true;

//					// commented Debug.Log("mousdown:"+mouseButtonDownLocation);
					mouseButtonDownLocation = camUI.ScreenToViewportPoint(Input.mousePosition); // reset this, maybe this is what's causing the snapping?


					skyCamPivotPosWhenDragClicked = camSky.transform.parent.position;
				}

				if (mapIsBeingDragged){
					
//					// commented Debug.Log("map being dragged..:");

					SetDragIcon(dragHand);
					panning = false;
					// Move the cam sky to the new mouse position, relative to the point we clicked and dragged from.
					// Some math to calculate how far to move the sky cam based on how far we clicked and dragged.
					float dX = mouseButtonDownLocation.x - camUI.ScreenToViewportPoint(Input.mousePosition).x;
					float dY = mouseButtonDownLocation.y - camUI.ScreenToViewportPoint(Input.mousePosition).y;


					// east
					// as dy increases, panning to the right. deltaViewportSpace = new Vector3((dX-dY)/sqrt2,0,(-dX-dY)/sqrt2);

//					// commented Debug.Log("mousdown3:"+mouseButtonDownLocation);
//					// commented Debug.Log("dy:"+dY);
					Vector3 deltaViewportSpace = Vector3.zero; 
					Vector3 rotDir = Vector3.zero; // Quaternion.AngleAxis(camSky.transform.parent.rotation.eulerAngles.y,Vector3.up) * camSky.transform.parent.right;
					//				// commented Debug.Log("rotdir:"+rotDir+", camskytransformpar Y:"+camSky.transform.parent.rotation.eulerAngles.y);
					float sqrt2 = 1.41421356f;

					switch(cameraMode){
	
					case CameraPositionMode.North:
						// drag up goes left, drag left goes down
						//						deltaViewportSpace = new Vector3(-dX,0,-dY);
						deltaViewportSpace = new Vector3((-dX-dY)/sqrt2,0,(dX-dY)/sqrt2);
						break;
					case CameraPositionMode.NorthEast:
						deltaViewportSpace = new Vector3(-dY,0,dX);
						break;
					case CameraPositionMode.East: 
//						deltaViewportSpace = new Vector3(-dY,0,dX);
						// up down works, left right reveresd
						deltaViewportSpace = new Vector3((dX-dY)/sqrt2,0,(dX+dY)/sqrt2);
						break;
					case CameraPositionMode.SouthEast:
						deltaViewportSpace = new Vector3(dX,0,dY);
						break;
					case CameraPositionMode.South:
						deltaViewportSpace = new Vector3((dX+dY)/sqrt2,0,(-dX+dY)/sqrt2);
						break;
					case CameraPositionMode.SouthWest:
						deltaViewportSpace = new Vector3(dY,0,-dX);
						break;
					case CameraPositionMode.West: 
						// right goes up, up goes left
//						deltaViewportSpace = new Vector3(dY,0,-dX);
						deltaViewportSpace = new Vector3((-dX+dY)/sqrt2,0,(-dX-dY)/sqrt2);
						break;
					case CameraPositionMode.NorthWest: 
						deltaViewportSpace = new Vector3(-dX,0,-dY);
						break;
					default:break;
					}
//					Debug.Log("camposmode;"+cameraMode+", deltaviewp;"+deltaViewportSpace);
					//				deltaViewportSpace = new Vector3(Mathf.Sqrt((Mathf.Pow(dX,2)+Mathf.Pow(dY,2))),0,-Mathf.Sqrt((Mathf.Pow(dX,2)+Mathf.Pow(dY,2))));
					//					current deltaViewportSpace = new Vector3((dX+dY)/sqrt2,0,(-dX+dY)/sqrt2);


					Vector2 viewportPoint1 = camSky.WorldToViewportPoint(currentPos);
					Vector2 viewportPoint2 = camSky.WorldToViewportPoint(currentPos + new Vector3(1,0,0));
					float oneWorldUnitInViewportDistance = Vector3.Distance(viewportPoint1,viewportPoint2);
//					// commented Debug.Log("oneworld:"+oneWorldUnitInViewportDistance +",delta;"+deltaViewportSpace);
					Vector3 camSkyTargetPos = skyCamPivotPosWhenDragClicked + deltaViewportSpace * (sqrt2/oneWorldUnitInViewportDistance);
					//				// commented Debug.Log("camera mode:"+cameraMode+", rotdir;"+rotDir+", oneworldunit:"+oneWorldUnitInViewportDistance+", view1,2:"+viewportPoint1+","+viewportPoint2);
					//				// commented Debug.Log("cam drag. when clicked:"+skyCamPivotPosWhenDragClicked+", oneworldunit:"+ oneWorldUnitInViewportDistance+", skycamtargetpos:"+camSkyTargetPos+", deltaviewportspace:"+deltaViewportSpace);
//					float distToPan = Vector3.Distance(camSkyTargetPos,camSky.transform.parent.position);
//					float maxDistToPan = 10;
//					if (CamSkyWithinBounds(camSkyTargetPos)){ // && distToPan < maxDistToPan){
						camSky.transform.parent.position = camSkyTargetPos; 
//					} 
				}
				//				// commented Debug.Log("Dragging. Clicked map?"+clickedMap);
				// 
			}
		} else {
			if (mapIsBeingDragged) {
				LevelBuilderCamSkyManager.inst.StopAllEvents();
			}
			mapIsBeingDragged = false;
		}
	}


	void HandleMapPanning(Vector3 panTarget){
		if (panning){

			float panSpeed = 8.5f;
			camSky.transform.parent.position = Vector3.Lerp(camSky.transform.parent.position,panTarget,panSpeed*Time.deltaTime);
			float minCutoffDist = 1;
			if (Vector3.Distance(camSky.transform.parent.position,panTarget)<minCutoffDist){
				panning = false;
				camSky.transform.parent.position = panTarget;
			}
//			UpdateContextMenuPosition();
		}

	}
	void HandleTempRedOutlines(){
		if (tempOutlines.Count > 0){
			List<Outline> toRemove = new List<Outline>();
			foreach(Outline o in tempOutlines){
				float revertSpeed = 1f;
				Color targetColor = new Color(0,0,0,0.5f);
				o.effectColor = Color.Lerp(o.effectColor,targetColor,Time.deltaTime * revertSpeed);
				if (Vector4.Distance(o.effectColor,targetColor) < 0.01f){
					
					o.effectColor = targetColor;
					toRemove.Add(o);
				}
			}
			foreach(Outline o in toRemove){
				tempOutlines.Remove(o);
			}
		}
	}

	void HandleTempRedImages(){
		if (tempImages.Count > 0){
			List<Image> toRemove = new List<Image>();
			foreach(Image o in tempImages){
				float revertSpeed = 1f;
				Color targetColor = Color.white; //new Color(0,0,0,0.5f);
				o.color = Color.Lerp(o.color,targetColor,Time.deltaTime * revertSpeed);
				if (Vector4.Distance(o.color,targetColor) < 0.01f){
					o.color = targetColor;
					toRemove.Add(o);
				}
			}
			foreach(Image o in toRemove){
				tempImages.Remove(o);
			}
		}
	}



	bool MouseIsOverMapViewport(){
		Vector2 currentMousePosition = camUI.ScreenToViewportPoint(Input.mousePosition);
		// 350 is hardcoded - it is the sum of UI element widths as set in inspector.
		bool touchingmap = currentMousePosition.x > 350/GameConfig.screenResolution.x;
//		// commented Debug.Log("curpos:"+currentMousePosition.y);
//		// commented Debug.Log("touchmap:"+touchingmap);
		return touchingmap;
	}

//	bool hoverFadingOut = false;


	float LerpColor(Text c,Color target, float speed){
		c.color = Color.Lerp(c.color,target,speed);
		return c.color.a;
	}
	float LerpColor(Image c,Color target, float speed){
		c.color = Color.Lerp(c.color,target,speed);
		return c.color.a;
	}

//	bool dragIconSet = false;
	void SetDragIcon(Sprite s, Color col = default(Color)){
//		// commented Debug.Log("setting drag icon:"+s);
		if (!dragIconObj.activeSelf) dragIconObj.SetActive(true);
		Cursor.visible = false;
//		// commented Debug.Log("setting drag icon:"+s);
		if (col == new Color(0,0,0,0)){
			col = Color.white;
//			// commented Debug.Log("col ?:"+col);
		}
//		// commented Debug.Log("drag icon set .."+col);
		dragIconObj.transform.position = Utils.FlattenVectorZ(camUI.ScreenToWorldPoint(Input.mousePosition));
		dragIconObjectImage.sprite = s;
		dragIconObjectImage.color = col;
		dragIcon = s;


	}


	void ClearDragIcon(bool nullify=false){
//		// commented Debug.Log("clearing drag");
		if (dragIconObj.activeSelf) dragIconObj.SetActive(false);


		Cursor.visible = true;
//		// commented Debug.Log("cleared. Null?:"+nullify);
//		Cursor.SetCursor(null,Vector2.zero,CursorMode.Auto);
		if (nullify || dragIconObjectImage.sprite == dragHand) {
			dragIconObjectImage.sprite = null;
			dragIconObjectImage.color = new Color(0,0,0,0);
		}

	
	}

	Dictionary<GameObject,Vector3> draggedPiecesToReturn = new Dictionary<GameObject,Vector3>();

		
	public void RotateLastPiecePlaced(int degrees){
		if (!currentPiece) {
			return;
		}
		currentPiece.transform.rotation = Quaternion.AngleAxis(currentPiece.transform.rotation.eulerAngles.y + degrees, Vector3.up);
	}

	bool SearchLayerCollision(int a, int b){
		if (a == 0 && b == 0) return true;
		if (a == 0 && b == 1) return false;
		if (a == 1 && b == 0) return false;
		if (a == 1 && b == 1) return true;
		return false;
	}
		


	Transform GetClosestItemNearCursor(Vector3 p){
		Transform closest = null;
		float lastDist = Mathf.Infinity;
		foreach(LevelBuilderSelectableObject lbso in LevelBuilderObjectManager.inst.GetCachedObjects("LB.GetClosest")){
			if (lbso){
				// todo: this is bad because there shouldn't be NULLS in lbso..
				Transform t = lbso.transform;
				float curDist = Vector3.Distance(t.position,p);
				if (curDist < lastDist){
					lastDist = curDist;
					closest = t;
				}
			}
		}
		return closest;
	}



	

	public void DeleteObject(GameObject pieceToDestroy){
//		Debug.Log("deleting:"+pieceToDestroy);
		LevelBuilderEventManager.inst.RegisterDeleteEvent(pieceToDestroy);
		UserEditableObject ueo = pieceToDestroy.GetComponent<UserEditableObject>();

		if (!ueo) {
			Debug.LogError("No ueo on obj to destroy:"+pieceToDestroy.name);
			return;
		}
		ueo.destroyedThisFrame = true;
		MapSmokePuff(pieceToDestroy.transform.position);
		if (pieceToDestroy.GetComponent<UEO_DraggingParent>()){
			foreach(UserEditableObject ueo2 in pieceToDestroy.GetComponentsInChildren<UserEditableObject>()){
				GameObject.Destroy(ueo2.gameObject);
			}
		}
		Destroy(pieceToDestroy);

		float vol = 0.5f;
		AudioManager.inst.PlayPoof(Player.inst.transform.position,vol);

	}

	public void DeleteCurrentObject(){
//		Debug.Log("del cur?");
		if (currentPiece == null){
			return;
		}
		deleteTimer = 0.2f;
		DeleteObject(currentPiece);
		UserFinishedPlacingObject();
		placedObjectContextMenu.gameObject.SetActive(false);
	}

	public GameObject MakeDraggingParentWithPieces(List<GameObject> pieces){
//		Debug.Log("making drag parent w pieces:"+pieces.Count);
		if (draggingParent) Destroy(draggingParent);
		GameObject newDragParent = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName("dragging parent");
		newDragParent.transform.rotation = Quaternion.identity;

		Vector3 avgWorldPosition = Vector3.zero;
		foreach(GameObject o in pieces){
			avgWorldPosition += o.transform.position;
		}
		avgWorldPosition /= pieces.Count;
//		Debug.Log("avg world y:"+avgWorldPosition.y);
//		bool forceSnap = true; // even if player has grid snap turne doff we still want to snap dragging parents because otherwise all children lose snap
		newDragParent.transform.position = HandleGridSnap(avgWorldPosition, 2.5f); // make sure we're snapped in, don't want to risk moving a bunch of snappe din objects off their grids
		draggingParent = newDragParent;

		// parent saved duplicates list to this
		foreach(GameObject o in pieces){
			o.transform.parent = newDragParent.transform;
		}

		return newDragParent;
	}


	public void SnapToTerrain(){
		if (draggingParent){
//			Debug.Log("Dragging parent exists!");
			List<GameObject> selectedObjects = new List<GameObject>();
			foreach(Transform t in draggingParent.transform){
				UEO_SnapToTerrain snapper = t.GetComponent<UEO_SnapToTerrain>();
				if (snapper){
					snapper.SnapToTerrain();
				} else {
					Utils.DropTransformToTerrain(t);
				}
				selectedObjects.Add(t.gameObject);
			}
			draggingParent = MakeDraggingParentWithPieces(selectedObjects);
			currentPiece = draggingParent;
		} else {
			UEO_SnapToTerrain snapper = currentPiece.GetComponent<UEO_SnapToTerrain>();
			if (snapper) {
				snapper.SnapToTerrain();
			} else {
				Utils.DropTransformToTerrain(currentPiece.transform);
			}
		}

		SnapPanToCurrentObject();
	}

	public void CopyUp(SinPop  heightMenuSinPop){
		UserEditableObject ueo = currentPiece.GetComponent<UserEditableObject>();
		float highestY = Utils.HighestY(ueo.transform);
		float lowestY = Utils.LowestY(ueo.transform);
		float copyInterval =  (highestY - lowestY); //(highestY - lowestY); // - ueo.transform.position.y;
		CopyAnObjectAndReselect(heightMenuSinPop,Vector3.up,copyInterval);
	}

	public void CopyRight( SinPop  heightMenuSinPop){
		
		float copyAmount = Utils.ApproxSizeXZ(currentPiece.transform).magnitude/1.41f;
		CopyAnObjectAndReselect(heightMenuSinPop,LevelBuilder.inst.camSky.transform.right,copyAmount);
	}

	public void CopyAnObjectAndReselect(SinPop heightMenuSinPop,Vector3 copyDir, float copyInterval){ // will copy an object up, select it, and re-open vertical move dialogue with this ref heightmenusinpop
		if (currentPiece){
			UserEditableObject ueo = currentPiece.GetComponent<UserEditableObject>();
			GameObject dupe = DuplicateObject(currentPiece.transform.position,currentPiece);

//			Debug.Log("copying obj at pos;"+currentPiece.transform.position.y+", up int:"+ueo.verticalCopyInterval);
//			PlaceContextMenu(dupe);
//			PlacedObjectContextMenuSubMenuOpen(heightMenuSinPop);
//			}
			currentPiece.transform.position += copyDir * copyInterval;
			SnapPanToCurrentObject();
		}
	}
	public GameObject DuplicateObject(Vector3 pos, GameObject sourceObj){
		GameObject check = (GameObject)Instantiate(EffectsManager.inst.flyingCheckmark);
		check.transform.position = pos + Vector3.up * 15;

		AudioManager.inst.PlayLevelBuilderItemPlace();
//		// commented Debug.Log("dupe obj");
		GameObject dupe= null;
		UserEditableObject ueo = sourceObj.GetComponent<UserEditableObject>();
		if (ueo.GetType() == typeof(UEO_DraggingParent)){
//			Debug.Log("duplicate obj w dragparent.");
			// for dragging parent, we need to iterate its children and place a dupe for each of those.
			List<UserEditableObject> copies = new List<UserEditableObject>();
			foreach(Transform t in ueo.transform){
				UserEditableObject ueo2 = t.GetComponent<UserEditableObject>();
				GameObject d = PlaceDuplicateObject(ueo2.myName,t.position,t.rotation,t.localScale,ueo2.GetProperties());
				copies.Add(d.GetComponent<UserEditableObject>());
			}
//			FinishedDragParent();

//			dupe.GetComponent<UserEditableObject>().OnObjectWasCreatedAsADuplicate();
			LevelBuilderEventManager.inst.RegisterCreateEvent(copies.ToArray());
			LevelBuilderGroupManager.inst.MakeGroup(copies);
		} else {
			// copy up expects to be returne dthis object
			dupe = PlaceDuplicateObject(ueo.myName,pos,sourceObj.transform.rotation,sourceObj.transform.localScale,ueo.GetProperties());
			LevelBuilderEventManager.inst.RegisterCreateEvent(dupe);
		}

		Vector3 camSkyPos = camSky.WorldToViewportPoint(pos);
		//		// commented Debug.Log("x,y:"+camSkyPos.x+","+camSkyPos.y);
		if (camSkyPos.x > 0.8f || camSkyPos.x < 0.2f || camSkyPos.y > 0.8f || camSkyPos.y < 0.2f){
			SmoothPanTo(pos);
		}

		return dupe; // for "copy up" we need to return the duplicate, otherwise we drop it and move on needing no return val
	}

	GameObject PlaceDuplicateObject(string name, Vector3 pos, Quaternion rot, Vector3 dScale,SimpleJSON.JSONClass props){
		GameObject dupe = LevelBuilderObjectManager.inst.GetPrefabInstanceFromName(name); 

		dupe.transform.position = pos;
		dupe.transform.rotation = rot;
		dupe.transform.localScale = dScale;
		UserEditableObject ueo = dupe.GetComponent<UserEditableObject>();

		ueo.SetProperties(props);
		ueo.OnLevelBuilderObjectCreated();
		ueo.OnLevelBuilderObjectPlaced();
		ShowIndicatorArrow(dupe);
		ueo.SetUuid("duplicated",-1); // set a new uuid.


		return dupe;
		
	}


	public void PlaceObject(){
		
		Vector3 pos = hoveringPiece.transform.position;
//		// commented Debug.Log("placed:"+hoveringPiece);
//		if (TutorialManager.inst) {
//			if (TutorialManager.inst.currentTutorial){
//				TutorialManager.inst.currentTutorial.LevelBuilderObjectPlaced(lastUIButtonPressed); // LevelBuilderObjectPlaced(lastUIButtonPressed); 
//				// Needed to pass this info to UIInstructionHelper/TutorialManager to make sure user pressed and scucessfully dropped the correct item to the map
//			}
//		}
//		else // commented Debug.Log("no tutorial manager");
		dragging = false;
		currentPiece = hoveringPiece; 
//		Debug.Log("Placed obj.curpiece:"+currentPiece);
		hoveringPiece = null;

		currentPiece.transform.position = pos;
		UserEditableObject ueo = currentPiece.GetComponent<UserEditableObject>();
		ueo.OnLevelBuilderObjectPlaced();
//		ueo.TimeCreatedInSeconds = GameManager.inst.unixTime;

		LevelBuilderEventManager.inst.RegisterCreateEvent(currentPiece);
		PlaceContextMenu(currentPiece,true);
		AudioManager.inst.PlayLevelBuilderItemPlace();

//		foreach(IMyMoveInLoop l in currentPiece.GetComponents(typeof(IMyMoveInLoop))){
//			l.StartLooping(); // animate the arrow
//		}

	}

	public void HoverHelpOff(){
//		// commented Debug.Log("hover help off.");
		hoveringButton = null;
		//		float hoverDelay = 8.9f;
//		hoverImage.color = new Color(1,1,0.65f,-LevelBuilder.inst.hoverDelay); // negative alpha value for slower lerp.
//		hoverTitle.GetComponent<Text>().color = new Color(0,0,0,-LevelBuilder.inst.hoverDelay);
//		hoverDescription.GetComponent<Text>().color = new Color(0,0,0,-LevelBuilder.inst.hoverDelay);
	}


	Dictionary<GameObject,Vector3> fxObjects = new Dictionary<GameObject,Vector3>();

	public void AddFXObject(GameObject o, Vector3 p){
		fxObjects.Add(o,p);
	}

	#region clipboard and group selection saving

	public GameObject markerMenuClipBoardGroup; // appears when you make a draggingparent by right click drag over objects
//	public GameObject selectionSaveGroup; // appears when you make a draggingparent by right click drag over objects
	public void SaveDragParentToClipboard(){
		if (ClipboardManager.inst.HaveAvailableClipboardSlot()){
			int newSlot = ClipboardManager.inst.SaveDragParentToClipboard(draggingParent) + 1;
//			LevelBuilderMessager.inst.Display("Clipboard copied to slot: "+newSlot);
		} else {
			LevelBuilderMessager.inst.Display("Your clipboard is full! Hover over a clipboard item at the top of your screen to delete it.");
		}
	}

	public void SaveSelection(){
		
	}

	#endregion



	void ClosePlacedObjectContextMenu(bool preserveDraggingParent = false){
//		Debug.Log("finish close place contx menu. preserve:<color=#f00>"+preserveDraggingParent+"</color>");
		if (currentPiece && mode != LBMode.Duplicating && !currentPiece.GetComponent<UserEditableObject>().destroyedThisFrame){
			LevelBuilderEventManager.inst.RegisterModifyEvent(currentPiece);

		} else {
//			Debug.Log("Tried to register but item gone?");
		}
		if (!preserveDraggingParent && currentPiece){
//			Debug.Log("--CLOSED--");
			currentPiece.GetComponent<UserEditableObject>().OnMarkerMenuClosed();
//			currentPiece.SendMessage("OnMarkerMenuClosed",SendMessageOptions.DontRequireReceiver);
		}
		placedObjectContextMenu.GetComponent<ShrinkAndDisable>().Begin();

	}

//	float movingY = 0;
//	bool maintainCurrentPieceY = false;
//	public List<Transform> additionalDraggingPieces = new List<Transform>(); // because we want to drag additional pieces on top of the big block.
	public void BeginMovingObject(){
		if (!currentPiece){
			WebGLComm.inst.Debug("No current piece while trying to move :/");
			return;
		}
		dragging = true;

		// don't destroy the parent when the menu closes because we are moving this object (it's still relevant)
		// TODO have a better way to track the relevant object, maybe using currentpiece or hoveringpiece, so we dont juggle this boolean
		bool preserveDraggingParent = true; 
		ClosePlacedObjectContextMenu(preserveDraggingParent);

//		// commented Debug.Log("drag!");
		maintainMinimumPosY = currentPiece.transform.position.y;


		fixedYplane.SetActive(true);
		Vector3 p = fixedYplane.transform.position;
		fixedYplane.transform.position = new Vector3(p.x,currentPiece.transform.position.y,p.z);

		SetMode(LBMode.Moving);


		SetHoveringPiece(currentPiece);
		currentPiece.GetComponent<UserEditableObject>().OnLevelBuilderObjectMoveInitiated();

		draggingPieceOriginPosition = currentPiece.transform.position;

	}

	void HandlePlaceContextMenuEdgeOfScreen(){
		// Fuck it let's always move to the object in question. Seems easier.
		SmoothPanTo(currentPiece.transform.position + markerUpOffset);// - camSky.transform.parent.forward * altitude*.707f);
		return;
		mousePositionContextMenu = Input.mousePosition;
		Vector3 camSkyMousePos = camSky.ScreenToViewportPoint(Input.mousePosition);
		if (camSkyMousePos.x > 0.7f || camSkyMousePos.x < 0.3f || camSkyMousePos.y > 0.6f || camSkyMousePos.y < 0.45f){
			if (currentPiece){
//				float altitude = currentPiece.transform.position.y;
//				Vector3 p = currentPiece.transform.position;

//				Vector3 terrainPosAtSelectedPiece = new Vector3(p.x,terrainHeightAtSelectedPiece,p.z);
//				// commented Debug.Log("terrainheight:"+terrainPosAtSelectedPiece);
				SmoothPanTo(currentPiece.transform.position + markerUpOffset);// - camSky.transform.parent.forward * altitude*.707f);
//				SmoothPanTo(terrainPosAtSelectedPiece);
			} else {
				// commented Debug.Log("trying to handle placenotextmenu screen but null curpiece");
			}
		}
	}

	void ShowIndicatorArrow(GameObject obj){
		
		foreach(LoopMoveAtoB looper in FindObjectsOfType<LoopMoveAtoB>()){
			looper.StopLooping();
			//			// commented Debug.Log("stopped looping:"+contextObj);
		}
		if (obj) obj.GetComponent<UserEditableObject>().ShowIndicatorArrows();
	}

	void PlaceContextMenu(GameObject contextObj, bool mute=false){
//		Debug.Log("<color=#0f0>Placed:"+contextObj+"</color>");
		currentPiece = contextObj;
//		SnapPanToCurrentObject();
		SmoothPanToCurrentObject();
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.timesMarkerMenuShown,1);
		// A clumsy way to disable all "UI helper arrows" which are 3D objects that appear on some items to show directionality. 
		// Since we only want the CURRENT object's UI helper arrow to show, we find all arrow objects and disable them
		// Then enable just the one on this object, if it exists.
		ShowIndicatorArrow(contextObj);
		placedObjectContextMenuCoverDark.SetActive(false);
		CloseContextMenuSubMenus();
		DisableContextMenuButtons();
		placedObjectContextMenu.SetActive(true);

//		// commented Debug.Log("curpiece;"+currentPiece);
		placedObjectContextMenu.GetComponent<SinPop>().Begin(mute);


//		PlacedObjectContextMenuSubMenuOpen(); // Close submenus in marker menu

		HandlePlaceContextMenuEdgeOfScreen();
		SetMode(LBMode.Editing);

		// Update fraction limits to "standard" (may be modified below for certain types of objects, e.g. riser does not allow negatives.


		// Which buttons to show on the context menu?
		UserEditableObject ueo = contextObj.GetComponent<UserEditableObject>();
		if (currentPiece) LevelBuilderEventManager.inst.RegisterModifyEvent(currentPiece);

		if (ueo){
//			// commented Debug.Log("set name:"+ueo.GetName);
		}

//		UpdateContextMenuPosition();


		if (ueo) {
			markerObjectContextMenuItemTitle.text = ueo.GetName;
			if (ueo.GetUIElementsToShow() != null){
				foreach(GameObject o in ueo.GetUIElementsToShow()){
					o.SetActive(true);
				}
			}
			// special case because "copy up" is a separate game object but tied to the boolean for "copy regular"
			POCMcopyUpButton.SetActive(POCMcopyButton.activeSelf);
			ueo.OnLevelBuilderObjectSelected();
		}

		if (draggingParent != null) { 
			// Special cases for dragging parents.
			// Dragging parents always get color option
			POCMGroupButton.SetActive(true);
			POCMModifyColorButton.SetActive(true);
			POCMscaleManipulatorMultiple.SetActive(true);
			POCMObjectCyclerButton.SetActive(true);
//			Debug.Log("mod color active.");
			
			// for groups, allow player to save to the clipboard.
			markerMenuClipBoardGroup.SetActive(true);
//			selectionSaveGroup.SetActive(true);
		} else {
			markerMenuClipBoardGroup.SetActive(false);
//			selectionSaveGroup.SetActive(false);
		}

	
		// FindObjectOfType<UIValueCommRotation>().SetRotationTextToCurrentPiece(); // DRY
	}



//	void UpdateContextMenuPosition(){
//		if (currentPiece && placedObjectContextMenu.activeSelf){
//			Vector3 screenPosition = camSky.WorldToScreenPoint(currentPiece.transform.position);
//			placedObjectContextMenu.transform.position = camUI.ScreenToWorldPoint(screenPosition); //camUI.ScreenToWorldPoint(screenPosition);//new Vector3(xPos,yPos,placedObjectContextMenu.transform.position.z);
//		}
//	}


	GameObject lastUIButtonPressed = null; // Needed to pass this info to UIInstructionHelper/TutorialManager to make sure user pressed and scucessfully dropped the correct item to the map
	Vector3 placementUpOffset = Vector3.zero;
	public void UIButtonPressed(LevelBuilderUIButton button){
		// user clicked on an item they want to drag into the scene.
//		// commented Debug.Log("last UI button pressed:"+button.gameObject.name);
		lastUIButtonPressed = button.gameObject;
		ClosePlacedObjectContextMenu();



		SetHoveringPiece((GameObject)Instantiate (button.levelPiecePrefab));
		hoveringPiece.transform.rotation = Quaternion.identity;

		UserEditableObject ueo = hoveringPiece.GetComponent<UserEditableObject>();
//		// commented Debug.Log("hoveringpice:"+hoveringPiece);
		ueo.OnLevelBuilderObjectCreated();
		if (ueo){
			placementUpOffset = Vector3.up * ueo.UpOffset;
//			// commented Debug.Log("upof:"+placementUpOffset);
		} else {
			placementUpOffset = Vector3.zero;
		}

		if (ueo && Utils.IsRootTransform(ueo.transform)) {
//			ueo.transform.localScale = ueo.BiggerFactorForMapView * Vector3.one;

		} else {
//			// commented Debug.Log("no ueo on :"+hoveringPiece.name);
		}
		copyingDialogue.GetComponent<ShrinkAndDisable>().Begin();
		dragging = true;
		SetDragIcon(button.GetComponent<Image>().sprite,button.GetComponent<Image>().color);
		SetMode(LBMode.Placing);


		// Here is where we need to set the animal properties.
		// Or do we do it when the values are changed..?

	}



	float returnToModeNoneTimer = 0;

	public void UserFinishedPlacingObject(){
		

//		Debug.Log("setnone user fin");
		SetMode(LBMode.None);


		ClosePlacedObjectContextMenu();
		CloseContextMenuSubMenus();
		if (!currentPiece) {
			return;
		} else {
			//	currentPiece.name = " at "+Time.realtimeSinceStartup;
		}
		DestroyHoverPiece();
		SetHoveringPiece(null);

		currentPiece = null;
	}

	void DestroyHoverPiece(){
		Utils.RemoveInteractableLevelBuilderComponents(hoveringPiece);
//		Debug.Log("dest hover");
		Destroy(hoveringPiece);
	}

	void SetFractionButtonText(Fraction frac){
		if (frac.denominator == 1){
			fractionButtonInteger.text = frac.numerator.ToString();
			fractionButtonDenominator.text = "";
			fractionButtonNumerator.text = "";
			fractionButtonSlash.text = "";
		} else {
			fractionButtonInteger.text = "";
			fractionButtonSlash.text = "/";
			fractionButtonNumerator.text = frac.numerator.ToString();
			fractionButtonDenominator.text = frac.denominator.ToString();
		}
	}




//
//	void UnhighlightAll(){
//		if (highlightObject) Destroy(highlightObject);
//		if (highlightObjectMultiple) Destroy(highlightObjectMultiple);
//
//	}


	bool needLoadLevelObjects = false;
	float loadLevelObjectsTimer = 0;

	public void ShowLevelBuilder(){
		if (levelBuilderIsShowing) return;
		GameManager.inst.EndGame("level builder opened");
//		SceneManager.inst.ReloadSceneWithCallback(CallbackTarget._LevelBuilder,"ShowLevelBuilderCallback");
//
//	}
//
//	public void ShowLevelBuilderCallback(){
		

		// This error happens (null ref on a foreach(placedobjects) because for a second after level builder is open, its update is running and trying to look at all placedobjects, but 
		// LevelLoader hasn't had a chance to clear and refresh the palcedobjects
		// lol this code debt tho


//		UnhighlightAll();


		BackgroundAudioManager.inst.PauseEnvironmentAudio();
		if (BackgroundAudioManager.inst.editorAudio != null && BackgroundAudioManager.inst.editorAudio.name != "None") BackgroundAudioManager.inst.PlayEditorAudio();
		fixedYplane.SetActive(false);
		levelBuilderIsShowing = true;
		levelBuilderUiParent.SetActive(true);

		eventSystem.SetActive(true); // Now begin detecting UI clicks.
		if (levelBuilderOpenedDelegate != null) levelBuilderOpenedDelegate();
//		ShowTutorialHelp();



		GameManager.inst.SetVisibleObjects(PlayMode.Editor);

		ClosePlacedObjectContextMenu();
		CloseContextMenuSubMenus();
		currentPiece = null; // lose previous reference in case of leftover bs from last edit session.
		placedObjectContextMenu.SetActive(false);

		needLoadLevelObjects = true; 
		loadLevelObjectsTimer = 0.2f;
		JsonLevelLoader.inst.onLevelLoadedDelegate += LevelLoadedOnEditorOpenCallback;
		GameManager.inst.SetPlayerState(PlayMode.Editor);

//		LevelBuilderObjectManager.inst.CleanPlacedObjectsClass(); // awkward way to prevent the manager from looking for objects that may have been created the last time the level builder was opened, but destroyed by player during play mode.
	}

	public void Preview(){
		AudioManager.inst.LevelBuilderPreview();
		// This button is pressed to hide the level builder and begin playing.
		if (LevelBuilderPreviewClicked != null) LevelBuilderPreviewClicked();
		if (levelNameInput.text == "") levelNameInput.text = "Untitled Level";
		if (levelTagsInput.text == "") levelTagsInput.text = "Addition, Subtraction";
		if (levelDescriptionInput.text == "") levelDescriptionInput.text = "No description yet";



		HideLevelBuilder();
		CanvasMouseController.inst.ShowRetrapMouseDialogue();

		// Try a quiet save.
		if (JsonLevelSaver.firstSaveCompleted){
			
			WebGLComm.inst.Debug("<color=#ff0>Levelbuilder:</color>Autosaving from levelbuilder preview");
			JsonLevelSaver.inst.AutosaveNow();
		} else {
			WebGLComm.inst.Debug("<color=#ff0>Levelbuilder:</color>Serialize w/ no save from preview");
			SimpleJSON.JSONClass N = JsonLevelSaver.inst.GetSerializedLevelClassJson();
			JsonLevelLoader.inst.SetTempLevelJsonPlayerPrefs(JsonUtil.GetStringFromJson(N),"autosavenow");
		}

		Player.inst.SlowFadeInBlack();


//		JsonLevelLoader.inst.levelClassFinishedLoading = false; // need to re-load level class once editor is opened. Used for setting/loading templeveljson, only allow save templeveljson after level class load is complete.

	}

	public void HideLevelBuilder(){
		if (!levelBuilderIsShowing)  return;

		// Clear any visual fx objects, no longer relevant for this levelbuilder session 
		foreach(KeyValuePair<GameObject,Vector3> kvp in fxObjects){
			Destroy(kvp.Key);
		}
		fxObjects.Clear();

		Inventory.inst.UpdateBeltPosition();
		BackgroundAudioManager.inst.PauseEditorAudio();
		BackgroundAudioManager.inst.PlayGameBackgroundAudio();
		levelBuilderIsShowing = false;
		fixedYplane.SetActive(false);
		eventSystem.SetActive(false);
//		PlaceRemainingDuplicatingObjects();

		GameManager.inst.SetVisibleObjects(PlayMode.Player); 
		GameManager.inst.SetPlayerState(PlayMode.Player);
//		Inventory.inst.ShowInventory(true);


		CloseContextMenuSubMenus();
		canvasOverlay.SetActive(true);

			
		if (hoveringPiece) Destroy (hoveringPiece);
		currentPiece = null;
		SetMode(LBMode.None);

		GameManager.inst.StartGame("hide levelbuilder");

//		// commented Debug.Log("placed over..");
		levelBuilderUiParent.SetActive(false);


	}




	#region user save

	public void UserOpenedSaveDialogue(){
		
		// User clicked save in the editor and the save dialogue pops up
		// User will select a screenshot, name, tags, description for their level

		CloseContextMenuSubMenus();
		saveDialogueGO.GetComponent<SinPop>().Begin();
		saveDialogueSaveButton.SetActive(true);
		Screenshotter.inst.SwapTreeMaterials();
		Screenshotter.inst.UpdateCameraList();
		// Tree billboards must be 0 so that rendering works for screenshotter save pixels
		MapManager.inst.currentMap.map.GetComponent<Terrain>().treeBillboardDistance = 0; // when screenshot camera is rendering

		// TODO Screenshotter.SetupCameraFromPreloadedValue
		Screenshotter.inst.SetUpCamera();
		Screenshotter.inst.UpdateScreenshot();

		saveDialogueSavecomplete.SetActive(false);
		saveDialogueSaveStarted.SetActive(true);

		//		saveDialogueLevelURL.text = "";

		// User pushes confirm save


	}


	public void UserClickedSave(){
		// User confirmed screenshot and level name

		JsonLevelSaver.inst.PreSaveLevelClass();

	}

	public void SaveLevelToServerCallback(){
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.timesSuccessfullySaved,1); // userData.timesSuccessfullySaved++;
		saveDialoguePleaseWait.SetActive(false);
		saveDialogueSavecomplete.SetActive(true);
		saveDialogueSaveStarted.SetActive(false);
		saveDialogueGO.GetComponent<ShrinkAndDisable>().Begin();
		Screenshotter.inst.RestoreTreeMaterials();
		MapManager.inst.currentMap.map.GetComponent<Terrain>().treeBillboardDistance = 2000;
//		if (userCompletedSaveDelegate != null){
////			// commented Debug.Log("save complete, suer complete save delegate fired");
//			userCompletedSaveDelegate();
//		} else {
//			//			// commented Debug.Log("delegate null.");
//		}
		JsonLevelSaver.firstSaveCompleted = true;
		previewButton.SetActive(true);
		fakePreviewButton.SetActive(false);
		AudioManager.inst.LevelBuilderSave();
		JsonLevelLoader.inst.state = LoadingState.Ready;
		JsonLevelSaver.inst.RecursiveSaveJson();

	}

	#endregion 




	public void MapSmokePuff(Vector3 p){
		float baseScale = FindObjectOfType<LevelBuilderCamSkyManager>().ZoomLevel()/5f;
//		// commented Debug.Log("basescale:"+baseScale);
		for (int i=0;i<8;i++){
			p += Random.insideUnitSphere * baseScale/4f + Vector3.up * 2;
			float scale = Random.Range(baseScale/2f,baseScale*1.5f);
			float energy = Random.Range(0.7f,1.35f);
			Vector3 dir = Vector3.up * 20f;
			EffectsManager.inst.CreateSmokePuffBig(p,dir,scale,energy);
		}

	}

	public void DropdownSelected(GameObject o){
		Dropdown d = o.GetComponent<Dropdown>();
		Dropdown.OptionData selectedValue = d.options[d.value];
		string selectedVal = selectedValue.text;

	}


	public void DisableContextMenuButtons(){
		POCMFractionButton.SetActive(false);
		POCMGroupButton.SetActive(false);
		POCMFractionZeroButton.SetActive(false);
		POCMTextTriggerButton.SetActive(false);
		POCMTagSendMessageButton.SetActive(false);
		POCMModFaucetButton.SetActive(false);
		POCMModifyCharacterButton.SetActive(false);
//		POCMModifyCharacterNameBuptton.SetActive(false);
		POCMResourceDropButton.SetActive(false);
		POCMModifyColorButton.SetActive(false);
		POCMObjectCyclerButton.SetActive(false);
		POCMRandomFractionButton.SetActive(false);
		POCMModifyAnimalRulesButton.SetActive(false);
		POCMcubeSizeManipulator.SetActive(false);
		POCMEditDanMeyerCube.SetActive(false);
		POCMscaleManipulatorMultiple.SetActive(false);
		placedObjectContextMenuNumberWallSizeButton.SetActive(false);
		placedObjectContextMenuNumberWallRoundSizeButton.SetActive(false);
		placedObjectContextMenucharacterSpeechBubbleButton.SetActive(false);
		POCMmatrixFloorSizeButton.SetActive(false);
		POCMheightButton.SetActive(false);
//		POCMsequentialFractionButton.SetActive(false);
		POCMcopyButton.SetActive(false);
		POCMcopyUpButton.SetActive(false);
		POCMModCannonButton.SetActive(false);

		POCMlinkLevelButton.SetActive(false);
		POCMintegerButton.SetActive(false);
//		POCMnumber3dButton.SetActive(false);
		POCMhatButton.SetActive(false);
		POCMmodRiser.SetActive(false);
		POCMmodTowerHeight.SetActive(false);
//		// commented Debug.Log("setting link level button false");
	}

	public void CloseContextMenuSubMenus(){
		foreach(Transform t in submenus){
			t.gameObject.SetActive(false);
//			if (t.GetComponent<ShrinkAndDisable>()){
//				t.GetComponent<ShrinkAndDisable>().Begin();
//			}
		}

//		copyingDialogue.GetComponent<ShrinkAndDisable>().Begin();
	}






	public void LevelLoadedOnEditorOpenCallback(){
		JsonLevelLoader.inst.onLevelLoadedDelegate -= LevelLoadedOnEditorOpenCallback;
		if (firstTimeOpened) {
			// The first time the editor is opened, center map on the player.
			// Subsequent times do not center map it because we probably want to stay focused on the region we're editing.
			ActionCenterOnPlayer();
			firstTimeOpened = false;
		}

	}

	public void ActionCenterMap(){
		Vector3 mapCenter = new Vector3(MapManager.inst.currentMap.map.GetComponent<Terrain>().terrainData.size.x/2f,0,MapManager.inst.currentMap.map.GetComponent<Terrain>().terrainData.size.z/2f);
		SmoothPanTo(mapCenter);
	}

	public void ActionCenterOnPlayer(){
		if (Player.inst) SmoothPanTo(Player.inst.transform.position);
	}

	bool rotatingCamera = false;
	Quaternion targetCameraRotation = Quaternion.identity;
	public void RotateCameraLeft(bool flag){
		if (rotatingCamera) return;
		if (AnySubMenusAreOpen() && mode != LBMode.Duplicating) return;
		if (EditorTesting.inst.ghosting) return;
		AudioManager.inst.PlayWingFlap(Vector3.zero);
		rotatingCamera = true;

		switch(cameraMode){
		case CameraPositionMode.North:
			if (flag) cameraMode = CameraPositionMode.NorthEast;
			else cameraMode = CameraPositionMode.NorthWest;
			break;
		case CameraPositionMode.NorthEast:
			if (flag) cameraMode = CameraPositionMode.East;
			else cameraMode = CameraPositionMode.North;
			break;
		case CameraPositionMode.East:
			if (flag) cameraMode = CameraPositionMode.SouthEast;
			else cameraMode = CameraPositionMode.NorthEast;
			break;
		case CameraPositionMode.SouthEast:
			if (flag) cameraMode = CameraPositionMode.South;
			else cameraMode = CameraPositionMode.East;
			break;
		case CameraPositionMode.South:
			if (flag) cameraMode = CameraPositionMode.SouthWest; 
			else cameraMode = CameraPositionMode.SouthEast; 
			break;
		case CameraPositionMode.SouthWest:
			if (flag) cameraMode = CameraPositionMode.West; 
			else cameraMode = CameraPositionMode.South; 
			break;
		case CameraPositionMode.West:
			if (flag) cameraMode = CameraPositionMode.NorthWest;
			else cameraMode = CameraPositionMode.SouthWest;
			break;
		case CameraPositionMode.NorthWest:
			if (flag) cameraMode = CameraPositionMode.North;
			else cameraMode = CameraPositionMode.West;
			break;
		default:
			break;
		}
//		Debug.Log("rotating cam with flag:"+flag+", campos is:"+cameraMode);
		cameraDirectionText.text = cameraMode.ToString(); //.Split('.')[1];
		int flagint = flag ? 1 : -1;
		Quaternion rot = camSky.transform.parent.rotation;
		rot.eulerAngles += new Vector3(0,45*flagint,0); // shaky. Should be EXACT values for N E S W, not +/- 90 from previous position.
		targetCameraRotation = rot;
	}

	public List<RaycastResult> GetUIObjectsUnderCursor(){
		List<RaycastResult> objectsHit = new List<RaycastResult> ();

		PointerEventData cursor = new PointerEventData(EventSystem.current);                            // This section prepares a list for all objects hit with the raycast
		cursor.position = Input.mousePosition;
		if (EventSystem.current){
			EventSystem.current.RaycastAll(cursor, objectsHit);
		}

		return objectsHit;

	}

	public void PlacedObjectContextMenuSubMenuOpen(SinPop sp = null){
		foreach(Transform t in markerMenus){
			GameObject o = t.gameObject;
			ShrinkAndDisable sad = o.GetComponent<ShrinkAndDisable>();
			if (sad) sad.Begin(); 
			else o.SetActive(false);
		}
		if (sp){
			sp.gameObject.SetActive(true);
			sp.Begin();
			placedObjectContextMenuCoverDark.SetActive(true);
		}
	}

//	public void CloseSubmenu(ShrinkAndDisable sad){
//		placedObjectContextMenuCoverDark.SetActive(false);
//		sad.Begin();
//	}
//

	public bool AnyMenusAreOpen(){
		if (placedObjectContextMenu.activeSelf) {
			return true;
		}
		return AnySubMenusAreOpen();
	}
	public bool AnySubMenusAreOpen(){
//		if (saveDialogueGO.activeSelf) return true;


		foreach (Transform t in submenus){
			if (t.gameObject.activeSelf) {
//				// commented Debug.Log("t was active;"+t.gameObject.name);
				return true;
			}
		}
		return false;
	}

	public void OpenSingleDialogue(SinPop sp){
		if (mode == LBMode.Moving || mode == LBMode.Placing || mode == LBMode.Duplicating) return;
		CloseContextMenuSubMenus();
		sp.Begin();
	}
		


		

	public void SetInstance(){
//		// commented Debug.Log("levelBuilder inst set");
		inst = this;
	}

	public GameObject placementIconTop;
	public GameObject placementIconSide;
	public enum PlacementType {
		Top,
		Side
	}



	public PlacementType placementType = PlacementType.Side;
	public void TogglePlacementType(){
		placementIconTop.SetActive(false);
		placementIconSide.SetActive(false);
		AudioManager.inst.PlayInventoryClose();
		if (placementType == PlacementType.Side){
			placementType = PlacementType.Top;
			placementIconTop.SetActive(true);
		} else {
			placementType = PlacementType.Side;
			placementIconSide.SetActive(true);
		}
	}


	bool gridSnap = false;
	public Text gridSnapText;
	public Transform gridSnapDots;
	public void ToggleGridSnap(){
		if (!gridSnap){
			AudioManager.inst.PlayInventoryOpen();
		} else {
//			Debug.Log("tog");
			AudioManager.inst.PlayInventoryClose();
		}
		gridSnap = !gridSnap;
//		gridSnapImage.color = gridSnap ? new Color(0.976f,0.8f,0,1) : new Color(0.9f,0.9f,0.9f,1);
//		gridSnapText.color = gridSnap ? new Color(0.976f,0.8f,0,1) : new Color(0.9f,0.9f,0.9f,1);
		gridSnapDots.gameObject.SetActive(gridSnap);
		gridSnapText.text = gridSnap ? "SNAP TO GRID: ON" : "SNAP TO GRID: OFF";
	}


//	public GameObject boxOnImage;
//	public Text boxSelectText;
	public GameObject boxSelectObj;
	bool boxing = false;
	bool boxSelect = true;
//	public void ToggleBoxSelect(){
//		if (!boxSelect){
//			AudioManager.inst.PlayInventoryOpen();
//		} else {
//			AudioManager.inst.PlayInventoryClose();
//		}
//		boxSelect = !boxSelect;
//		boxOnImage.gameObject.SetActive(boxSelect);
//		boxSelectText.text = boxSelect ? "BOX SELECT: ON" : "BOX SELECT: OFF";
//	}


//	public void ShowTutorialHelp(){
//		StopCoroutine("ShowSaveHelpE");
//		if (!JsonLevelSaver.inst.firstSaveCompleted) ShowSaveHelpE(120);
//		if (!firstTimeOpened) return;
//		firstTimeOpened = false;
//		StartCoroutine(ShowTutorialHelpE(30.5f));
//
//	}
//
//	IEnumerator ShowTutorialHelpE(float s){
//		yield return new WaitForSeconds(s);
//		tutorialHelpWindow.GetComponent<SinPop>().Begin();
//	}

	IEnumerator ShowSaveHelpE(float s){
		yield return new WaitForSeconds(s);
		if (!JsonLevelSaver.firstSaveCompleted) {
			saveHelpWindow.GetComponent<SinPop>().Begin();
		}
	}



	public void SnapScrollToSelection(RectTransform target){
		Transform p = target.transform.parent;
		ScrollRect sr=null;
		for(int i=0;i<5;i++){
			sr = p.GetComponent<ScrollRect>();
			if (sr) {
				SnapTo(sr,target);
				return;
			}
			else p = p.parent; // recursively look up parents
		}
	}

	ScrollRect currentScrollRect = null;
	float currentScrollRectTargetScrollVal = 0;
	bool needLerpScrollVal = false;
	public void SnapTo(ScrollRect scrollRect, RectTransform target)
	{
		currentScrollRect = scrollRect;
		needLerpScrollVal = true;

//		JsonUtil
		// Unforuntately these values are special cased based on desired vertical scroll bar value for each row in the MAP SELECT window.
		// Should make a general case for snap to selection but meh.
		if (Mathf.Abs(Mathf.Round(target.localPosition.y) - 225) < 5) currentScrollRectTargetScrollVal = 1;
		else if (Mathf.Abs(Mathf.Round(target.localPosition.y) - 35) < 5) currentScrollRectTargetScrollVal = 0.52f;
		else if (Mathf.Abs(Mathf.Round(target.localPosition.y) + 155) < 5) currentScrollRectTargetScrollVal = 0.06f;
		else if (Mathf.Abs(Mathf.Round(target.localPosition.y) + 345) < 5) currentScrollRectTargetScrollVal = 0;
	}


	#region Position Helper
	bool positionHelper = false;
	public Text positionHelperText;
	public Image positionHelperImage;
	public void TogglePositionHelper(){
		positionHelper = !positionHelper;
		positionHelperText.text = positionHelper ? "POSITION HELPER: ON" : "POSITION HELPER: OFF";
		positionHelperImage.color = positionHelper ? new Color(1,0,1) : new Color(1,1,1);
		if (positionHelper) AudioManager.inst.PlayInventoryOpen();
		else AudioManager.inst.PlayInventoryClose();
	}
	void MakePulsateFX(GameObject pulsatePiece){
		if (pulsatePiece && positionHelper){

			// make pulsate fx in 6 directions from object to help with orientation.
			float pulsateInterval = 0.1f;
			float pulsateObjSpeed = 80f;
			GameObject tempParent = new GameObject("temp parent");
			tempParent.transform.parent = pulsatePiece.transform;
			TimedObjectDestructor tod = tempParent.AddComponent<TimedObjectDestructor>();
			tod.DestroyNow(3);

			if (Utils.IntervalElapsed(pulsateInterval)){
				Vector3[] dirs = new Vector3[]{ Vector3.up,Vector3.forward,Vector3.right,-Vector3.up,-Vector3.forward,-Vector3.right };
				foreach(Vector3 dir in dirs){
					GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
					box.transform.localScale = new Vector3(3,3,2f);
					box.transform.forward = dir;
					MoveAlongAxis hi = box.AddComponent<MoveAlongAxis>();
					box.transform.position = pulsatePiece.transform.position;
					box.GetComponent<Renderer>().material = posiitonFxMat;
//					SetFloat("_Mode",2.0f);
//					box.GetComponent<Renderer>().material.SetColor("_Emission", new Color(0,0,0.4f));
//					box.GetComponent<Renderer>().material.color = new Color(0,0,1f,0.2f);
					box.transform.parent = tempParent.transform;
					hi.dir = dir; 
					hi.useLocalDir = false;
					//						Debug.Log("hi dir;"+hi.dir);
					hi.speed = pulsateObjSpeed;

				}
			}
		}
	}
	#endregion // Position helper

	public void EditDanMeyerCube(){
		currentPiece.GetComponent<LevelMachine_DanMeyerCubes>().BeginEditing();
	}
	public GameObject restrictInputPlane;
	public void RestrictInput(bool f){
		restrictInputPlane.SetActive(f);
	}
}
