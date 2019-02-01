using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class UserEditableObject : MonoBehaviour {

	//	The root class for any object placeable / editable in the LevelBuilder.
	// When saving the json for level objects, the following methods are called to populate json info,
	// And also to populate item properties based on previously saved json
//	List<LoopMoveAtoB> arrows = new List<LoopMoveAtoB>();
//	void Awake() {
////		Debug.Log("start");
//		arrows.AddRange(GetComponentsInChildren<LoopMoveAtoB>(true));
//		Debug.Log("arrows:"+arrows);
//		LevelBuilder.inst.levelBuilderOpenedDelegate += EnableIndicatorArrows;
//		LevelBuilder.inst.LevelBuilderPreviewClicked += DisableIndicatorArrows;
//	}
//
	ResourceDrop rd;
	UEO_ColorCycler cm;
	UEO_ObjectCycler oc;
	UEO_ScaleManipulator cs;
	void Awake(){
		rd = GetComponentInChildren<ResourceDrop>();
		cm = GetComponentInChildren<UEO_ColorCycler>();
		oc = GetComponentInChildren<UEO_ObjectCycler>();
		cs = GetComponentInChildren<UEO_ScaleManipulator>();
	}
	[System.NonSerialized] public bool ignoreDestroyFlags = false;

	virtual public void OnEnable(){
//		name += "_" + Time.time;
//		if (!LevelBuilderObjectManager.inst.bNeedUpdateCachedObjects){
//			LevelBuilderObjectManager.inst.UpdateCachedObjects();
//		}
//
	}



	virtual public void OnDestroy(){
//		if (isSerializeableForClass && !ignoreDestroyFlags){
//			LevelBuilderObjectManager.inst.RemoveFromPlacedObjects(this,SceneSerializationType.Class);
//		}
//		if (LevelBuilder.inst.levelBuilderIsShowing 
//			&& isSerializeableForClass 
//			&& GameManager.inst.state != GameState.LoadingClass 
//			&& GameManager.inst.state != GameState.LoadingInstance
//			&& GameManager.inst.state != GameState.ChangingMap
//			&& GameManager.inst.state != GameState.Playing
//			&& !ignoreDestroyFlags
//		){
//			LevelBuilderObjectManager.inst.RemoveFromPlacedObjects(this,SceneSerializationType.Class);
//		} else {
//			Debug.Log("game stat:"+GameManager.inst.state);
//		}
//		LevelBuilderObjectManager.inst.UpdateCachedObjects("destroyed:"+myName	);
	}

	public static string uuidKey = "lbo_uuid";

	private int uuid = -1;
	public int GetUuid () {
		if (uuid == -1){
			SetUuid("self");
		}
		return uuid;
	}


//	void EnableIndicatorArrows(){
//		foreach(LoopMoveAtoB arrow in arrows){
//			arrow.gameObject.SetActive(true);
//		}
//	}
//
//	void DisableIndicatorArrows(){
//		foreach(LoopMoveAtoB arrow in arrows){
//			arrow.gameObject.SetActive(false);
//		}
//	}

	public static string tagsKey = "Tags";
	public List<string> myTags = new List<string>();

//	public virtual void AddTag(string tag){
//		myTags.Add(tag);
//	}


	virtual public void ClearTags(){
		myTags.Clear();
	}
	virtual public void AddTags(List<string> ts){
//		Debug.Log("adding tags to;"+ts.Count);
		myTags.AddRange(ts);
		myTags = myTags.Distinct().ToList();
		List<string> toRem = new List<string>();
		foreach(string s in myTags){
			if (s == ""){
				toRem.Add(s);
			}
		}
		foreach(string s in toRem){
			myTags.Remove(s);
		}
	}

	bool bNeedUpdateCachedProperties = true;
	SimpleJSON.JSONClass cachedProperties = new SimpleJSON.JSONClass();
	virtual public void OnMarkerMenuClosed(){
//		SaveCurrentPropertiesToCache();
	}

//	void SaveCurrentPropertiesToCache(){
//		cachedProperties = GetCurrentProperties();
//		bNeedUpdateCachedProperties = false;
//	}

//	public virtual SimpleJSON.JSONClass GetProperties() {
//		if (bNeedUpdateCachedProperties){
//			cachedProperties = GetCurrentProperties();
//		}
//		return cachedProperties;
//	}
//	() { // actually GetCachedProperites but I don't want to refactor all refs to this function
//		return cachedProperties;
//	}

//	public virtual SimpleJSON.JSONClass GetCurrentProperties () {
	public virtual SimpleJSON.JSONClass GetProperties() {
//		Debug.Log("getprop:"+myName);
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N["name"] = myName;
		N[uuidKey].AsInt = GetUuid();
//		N["position"] = 
		if (rd) {
			N = rd.GetProperties(N);
		} 

		if (cm){
			N = cm.GetProperties(N);
		}


		if (oc){
			N = oc.GetProperties(N);
		}


		if (cs){
			// commented Debug.Log("get prop cubezie scale;"+cs.transform.localScale);
			N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyX] = Mathf.RoundToInt(cs.transform.localScale.x/cs.scaleFactor).ToString(); 
			N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyY] = Mathf.RoundToInt(cs.transform.localScale.y/cs.scaleFactor).ToString(); 
			N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyZ] = Mathf.RoundToInt(cs.transform.localScale.z/cs.scaleFactor).ToString(); 
		}

		N[tagsKey] = string.Join(",",myTags.ToArray());
//		Debug.Log("n tagskey;"+N[tagsKey].Value+", tags;"+tags.Count);

//		PlayerNowMessageWithBoxTrigger pmwbt = GetComponentInChildren<PlayerNowMessageWithBoxTrigger>();
//		if (pmbwt){
//			
//		}
//		Debug.Log("Gotprop:"+N.ToString());
		return N;
	}

	public virtual void SetProperties(SimpleJSON.JSONClass N){
//		return;
//		Debug.Log("set prop:"+N.ToString());
		if (N.GetKeys().Contains(uuidKey)){
			SetUuid("setprop",N[uuidKey].AsInt);
//			Debug.Log("just set my uuid key:"+N[uuidKey].AsInt);
		}
		ResourceDrop rd = GetComponentInChildren<ResourceDrop>();
		if (rd && N.GetKeys().Contains(ResourceDrop.key)){
//			Debug.Log("set prop resource:"+N.ToString());
			rd.SetProperties(N);
		}
		if (N.GetKeys().Contains(JsonUtil.scaleKey)){
//			Debug.Log("set scale on: "+myName+", "+N[JsonUtil.scaleKey].AsInt);
//			name += " scaled.";
			transform.localScale = JsonUtil.GetScaleFromInt(N[JsonUtil.scaleKey].AsInt);
			scaleSetFromInstanceLoad = true;
		}
		UEO_ScaleManipulator cs = GetComponentInChildren<UEO_ScaleManipulator>();
		if (cs && N.GetKeys().Contains(UEO_ScaleManipulator.key)){
			
			int x = MathUtils.IntParse(N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyX].Value);
			int y = MathUtils.IntParse(N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyY].Value);
			int z = MathUtils.IntParse(N[UEO_ScaleManipulator.key][UEO_ScaleManipulator.keyZ].Value);
			cs.transform.localScale = new Vector3(x,y,z);
			cs.UpdateSize(x,y,z);
			//			if (cloneObject) cloneObject.transform.localScale = transform.localScale;
		}

		if (N.GetKeys().Contains(UEO_ColorCycler.colorManipulatorKey)){
			UEO_ColorCycler cc = GetComponentInChildren<UEO_ColorCycler>();
			if (cc){
				cc.SetProperties(N);
			}
		}


		if (N.GetKeys().Contains(UEO_ObjectCycler.objectIndexKey)){
			UEO_ObjectCycler cc = GetComponentInChildren<UEO_ObjectCycler>();
			if (cc){
				cc.SetProperties(N);
			}
		}

		if (N.GetKeys().Contains(tagsKey)){
			if (N[tagsKey].Value.Length > 0){
				myTags.AddRange(N[tagsKey].Value.Split(','));
//				Debug.Log("<color=#f0f>addtag:</color>"+N[tagsKey].Value);
			}
//			Debug.Log("tags!:"+tags+", json val:"+N[tagsKey].ToString());
		} else {
//			print("no tagskey on:"+myName);
		}

		PickUppableObject pip = GetComponent<PickUppableObject>();
		if (pip){
			// Vadim todo add Interface IMySetProperties?
			pip.SetProperties(N);
		}
	}
	public virtual GameObject[] GetUIElementsToShow() { 
		List<GameObject> elements = new List<GameObject>();
		if (GetComponentInChildren<ResourceDrop>()) elements.Add(LevelBuilder.inst.POCMResourceDropButton);
		if (GetComponentInChildren<UEO_ScaleManipulator>()) elements.Add(LevelBuilder.inst.POCMcubeSizeManipulator);
		if (GetComponentInChildren<UEO_ColorCycler>()) elements.Add(LevelBuilder.inst.POCMModifyColorButton);
		if (GetComponentInChildren<UEO_ObjectCycler>()) elements.Add(LevelBuilder.inst.POCMObjectCyclerButton);

		return elements.ToArray();
	}
	public bool isSerializeableForSceneInstance = true; // Set to false for objects we don't want to save into scene instances such as Number Walls
	public bool IsSerializeableForSceneInstance(){
		return isSerializeableForSceneInstance && !GetComponentInChildren<TimedObjectDestructor>();
	}
	public bool isSerializeableForClass = true; // not cube prefabs..
//	public bool isEphemeral = true; // Is it a normal (temporary objects) that should be destroyed on loads, or is it a permanent piece/child of a temporary object (like boat numbers) that shouldn't be destroyed alone?
	// This "replacement" is awkward, but basically in the case that this object is "preloaded" with other assets when it's placed, 
	// And those other assets get UNLOADED at the time of placement,
	// We will end up serializing them for the INSTANCE, so if we're serializing levels and we also serialize this "preloaded" as an 
	// Then we'll end up with object duplicates.
	// SO , in the case of serializing a level we replace this preloaded object (if it is one, this should be set to true) with an unloaded "empty" object.

	public bool childrenAreSerializeableForSceneInstance = true;

	bool scaleSetFromInstanceLoad = false; // if true, don't reset scale.
//	public void SetScaleFromLoadLevelInstance(float scale){
//		// TODO: Migrate away from this boolean? Should be in SetProperties
//		transform.localScale = Vector3.one * scale;
//		scaleSetFromInstanceLoad = true;
//	}

	public bool useGravityOnGameStarted = false;
	public virtual void OnGameStarted(){ 
		if (useGravityOnGameStarted){
			Rigidbody rb = GetComponentInChildren<Rigidbody>();
			if (rb){
				rb.isKinematic = false;
				rb.useGravity = true;
			}
		}
		// Todo Vadim: This logic isn't great. Basically some objects are children of parents who get serialized (number bricks in walls) vs numbers who are always children attached to a vehilce (boat number)
		// And their scales need to behave by different rules. HEre are some speical case booleans..
		if (isSerializeableForClass){
			transform.localScale = Vector3.one; 
		}

		// disable arrow indicators
		foreach(LoopMoveAtoB lab in GetComponentsInChildren<LoopMoveAtoB>()){
			Destroy(lab.gameObject);
		}
	}

	public string myName = "no name";
	public virtual string GetName {
		get {
			return myName;
		}
		set {
			myName = value;
		}
	}
	public float footprint = 0;
	public virtual float Footprint { 
		get {
			return footprint;
		}
	} 

	public bool destroyedThisFrame = false; 
	//because sometimes we destroy it, then load a new level with different objects, 
	//then want to iterate over all objects of this type, but the actual destroy doesn't happen
	// until the end of the frame so we flag it as "destroyed" so that it won't be counted in the search after its own destructination

//	public float biggerFactorForMapView = 1;
//	public virtual float BiggerFactorForMapView { 
//		get {
//			return biggerFactorForMapView;
//		}
//	} 

	public float upOffset = 0;
	public virtual float UpOffset { 
		get { 
//			// commented Debug.Log("up:"+upOffset);
			return upOffset;
		}
		set {
			upOffset = value;
		}
	}

	public int rotationSnap = 15;


	public int verticalCopyInterval = 20;

	public bool keepYPosConstant = false; // for big concrete blocks we do keep y pos constant while moving/duplicating so they line up nicely.

	public float gridSnapSpacing = 2.5f;
	public virtual float GridSnapSpacing {
		get {
			return gridSnapSpacing;
		}
		set {
			gridSnapSpacing = value;
		}
	}

	public Sprite icon;
//	public int gridSnapSpacingOffset = 0;
//	public virtual int GridSnapSpacingOffset {
//		get {
//			return gridSnapSpacingOffset;
//		}
//		set {
//			gridSnapSpacingOffset = value;
//		}
//	}


	public virtual void OnLevelBuilderObjectPlaced(){
		if (LevelBuilder.inst.levelBuilderIsShowing){ // should always be, no?
			ShowIndicatorArrows();
//			transform.localScale = Vector3.one * biggerFactorForMapView;
		}
//		// commented Debug.Log("object placed at unixtime:"+GameManager.inst.unixTime+". object saved unix time was:"+TimeCreatedInSeconds);
	}

	public virtual void ShowIndicatorArrows(){
		foreach(LoopMoveAtoB ab in GetComponentsInChildren<LoopMoveAtoB>()){
//			// commented Debug.Log("loop;"+ab.name);
			ab.BeginLooping();
			//			// commented Debug.Log("start looping:"+contextObj);
		}
	}

	public virtual void OnLevelBuilderObjectCreated () {
		if (LevelBuilder.inst.levelBuilderIsShowing){
//			transform.localScale = Vector3.one * biggerFactorForMapView;
		}

	} // this replaces the "start" function on objects

//	public virtual SimpleJSON.JSONClass N ToJson(){
//		return new SimpleJSON.JSONClass();
//	}
	public virtual void OnLevelBuilderObjectSelected() {}
//	float timeCreatedInSeconds = -1;
//	public float TimeCreatedInSeconds { // since unix epoch
//		get {
//			return timeCreatedInSeconds;
//		}
//		set {
//			timeCreatedInSeconds = value;
//		}
//	}

//	public virtual bool Exclude() { return false; }


	virtual public void CleanObjectForSerialization(){
		MonsterAIRevertNumber mairn = GetComponentInChildren<MonsterAIRevertNumber>();
		if (mairn){
			if (mairn.bNeedsRevert){
				mairn.RevertNumber();
			}
		}
	}

	virtual public void OnLevelBuilderObjectMoveFinished(){
		
	}
	virtual public void OnLevelBuilderObjectMoveInitiated(){
		
	}

	virtual public void OnObjectWasCreatedAsADuplicate(){}
	virtual public void OnObjectWasDuplicated(){	}
	virtual public void MoveChildren(Vector3 delta){}

	virtual public void StartMachine(bool levelWasJustLoaded = false) {}

	virtual public void SetTransformProperties(SimpleJSON.JSONClass N){
		transform.position = JsonUtil.GetRealPositionFromTruncatedPosition(N);
		transform.rotation = JsonUtil.GetRealRotationFromJsonRotation(N);
//		transform.localScale
	}

	public void SetUuid(string source="", int _uuid = -1){
		// our own personal way to keep track of uuid.. NOT unity's UUID because that would be a new one for each instance, but if we recreate this obj due to UNDO a delete action Unity's uuid would be new but we want ours to be the same
		// We want our own personal uuid outside of Unity's because if this object is destroyed and recreated in an "undo" action, we want to retain uuid.
		if (_uuid == -1){
			
			uuid = this.gameObject.GetInstanceID();
		} else {
			uuid = _uuid;
		}
//		Debug.Log("set "+this.myName+" uuid:<color=fff>"+uuid+"</color>, source;"+source);
//		name = myName + "__" + uuid;
	}

	
}
