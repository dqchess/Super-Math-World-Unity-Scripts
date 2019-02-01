using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ObjectInfo {
	public string name = "default";
	public GameObject prefab;
	public ObjectInfo(string _name, GameObject _prefab){
		name = _name;
		prefab = _prefab;
	}
}


public class LevelBuilderObjectManager : MonoBehaviour {

	[SerializeField] public List<ObjectInfo> objectInfoList = new List<ObjectInfo>();
	Dictionary<string,GameObject> objectLibrary = new Dictionary<string, GameObject>();

	public static LevelBuilderObjectManager inst;
	public void SetInstance(){
		inst = this;
	}


	void Awake() {
		// We serialize the object info list into a dictionary
		// REASON 1: Dictionaries are faster to look up by key e.g. objectLibarary['thisobjectname']
		// REASON 2: Dictionary is not serializable at compile time so we need to store the values in the ordered list instead.
		foreach(ObjectInfo oi in objectInfoList){
			objectLibrary.Add(oi.name,oi.prefab);
		}
	}

	public GameObject GetPrefabInstanceFromName(string n){
//		Debug.Log("getting:"+n+" from a dict of len:"+objectLibrary.Count);
		GameObject ret = null;
		if (objectLibrary.ContainsKey(n)){
			ret = (GameObject)Instantiate(objectLibrary[n]);
		} 
		return ret;
	}

	List<LevelBuilderSelectableObject> levelBuilderSelectableObjects = new List<LevelBuilderSelectableObject>();

//	List<UserEditableObject> cachedObjectsClass = new List<UserEditableObject>();
	public List<LevelBuilderSelectableObject> GetCachedObjects(string source = "default"){
//		WebGLComm.inst.Debug("<color=#a50>Got cached for:</color>"+source+", bneeds;"+bNeedUpdateCachedObjects);
		string c = "";
//		if (SceneManager.inst.sceneWasReloadedThisFrame) return new List<UserEditableObject>(); // if scene was just loaded, OnDestroy() hasn't finished for old destroyed objects.
		if (bNeedUpdateCachedObjects){
//			Debug.Log("<color=#f94>Getting fresh cached:</color>"+source);
			// in case this is requested before we had a chance to execute the most recent UpdateCachedObjs which will only happen in LateUpdate
			// for example reloading a scene, some LevelBuilder.Update loop will request GetCacheDObjects before LateUpdate has a chance to run and clear them.
			UpdateCachedObjectsImmediate();
		} else {
//			Debug.Log("<color=#a50>Getting stale cached:</color>"+source);
		}

		return levelBuilderSelectableObjects;
	}

	[System.NonSerialized] public bool bNeedUpdateCachedObjects = false;
	public void UpdateCachedObjects(){
//		WebGLComm.inst.Debug("<color=#f70>Will update cache for: </color>"+source);
		bNeedUpdateCachedObjects = true;
		// If called multiple times per frame, call only once at the end of the frame?
	}


	void UpdateCachedObjectsImmediate(){
//		WebGLComm.inst.Debug("<color=#fa0>Update cache now</color>");
//		int oldLen = cachedObjectsClass.Count;
		List<LevelBuilderSelectableObject> lbsos = new List<LevelBuilderSelectableObject>();
		foreach(LevelBuilderSelectableObject lbso in FindObjectsOfType<LevelBuilderSelectableObject>()){
			lbsos.Add(lbso);
		}
//		LevelBuilderSelectableObject.Clear();
		levelBuilderSelectableObjects = lbsos;
//		string nc = "";
//		foreach(UserEditableObject ueo in LevelBuilderSelectableObject){
//			nc += ","+ueo.name;
//		}
////		Debug.Log("newcache;"+nc);
		bNeedUpdateCachedObjects = false;
	}

	void LateUpdate(){
//		if (bNeedUpdateCachedObjects){
//			UpdateCachedObjectsImmediate();
////			Debug.Log("updating cached:<color=#88f>"+oldLen+"</color> to <color=#88f> "+cachedObjectsClass.Count+"</color>");
//		}
//		bNeedUpdateCachedObjects = false;
	}

	// Freshly populate the placed objects
//	public List<LevelBuilderSelectableObject> GetPlacedObjects(SceneSerializationType type){
//		LevelBuilderSelectableObject
//		return ueos;
//	}

	public UserEditableObject PlaceObject(SimpleJSON.JSONClass N, SceneSerializationType type, int uuid = -1){
//		Debug.Log("placing:"+N["name"]);
		GameObject objToPlace = GetPrefabInstanceFromName(N["name"].Value);
		bool objActiveState = true;
		if (N.GetKeys().Contains("active")) {
//			Debug.Log("obj active;"+N["active"].ToString());
			objActiveState = N["active"].AsBool; // if the object has this information, it might have been inactive when it was serialized.
		}
		if (objToPlace == null){
			Debug.Log("<color=red>obj null</color>:"+N["name"]);
			return null;
		}
		objToPlace.transform.position = JsonUtil.GetRealPositionFromTruncatedPosition(N);
//		objToPlace.name += Random.Range(0,100000);
		objToPlace.transform.rotation = JsonUtil.GetRealRotationFromJsonRotation(N);


		SimpleJSON.JSONClass props = (SimpleJSON.JSONClass)N["properties"];
		UserEditableObject ueo = objToPlace.GetComponent<UserEditableObject>();

		if (uuid != -1) props[UserEditableObject.uuidKey].AsInt = uuid;
		else if (N.GetKeys().Contains(UserEditableObject.uuidKey)){
			props[UserEditableObject.uuidKey].AsInt = N[UserEditableObject.uuidKey].AsInt;
		}

		ueo.OnLevelBuilderObjectCreated(); 
		ueo.SetProperties(props);

		if (LevelBuilder.inst.levelBuilderIsShowing) ueo.OnLevelBuilderObjectPlaced();
		if (!objActiveState) objToPlace.SetActive(objActiveState);
//		Debug.Log("Built;"+ueo.name+"at :"+ueo.transform.position);
		return ueo;
	}

//	public void DestroyRemainingObjects(){
////		Debug.Log("destr rem:"+FindObjectsOfType<UserEditableObject>().Length);
//		foreach(UserEditableObject ueo in FindObjectsOfType<UserEditableObject>()){
////			Debug.Log("Found uel"+ueo.myName);
//			if (ueo.isSerializeableForSceneInstance){
////				DestroyObject(ueo,SceneSerializationType.None);
//				Destroy(ueo.gameObject);
////				Debug.Log("Destryoing remaining ueo:"+ueo.myName);
//			}
//		}
//	}


//	public void DestroyPlacedObjects(SceneSerializationType type){
////		Debug.Log("<color=#f0f>Destroy placed:</color>"+type);
//		int j=0;
//		List<UserEditableObject> ueos = new List<UserEditableObject>();
//		if (type == SceneSerializationType.Class){
//			ueos = placedObjectsClass;
////			Debug.Log("Clas count:"+ueos.Count);
//		} else if (type == SceneSerializationType.Instance){
//			ueos = placedObjectsInstance;
//		}
//		foreach(UserEditableObject ueo in ueos){
////			Debug.Log("dest:"+type+",:"+ueo.name);
////			DestroyObject(ueo,type,false);
//			if (ueo) Destroy(ueo.gameObject);
//		}
////		WebGLComm.inst.Debug("Destroyed "+j+" objs of type "+type.ToString());
//		if (type == SceneSerializationType.Class){
////			Debug.Log("cleared");
//			placedObjectsClass.Clear();
//		} else if (type == SceneSerializationType.Instance){
//			placedObjectsInstance.Clear();
//		}
//	}

//	public void DestroyObject(UserEditableObject ueo, SceneSerializationType type, bool clearFromList=true){
//		// disable colliders for this frame beacuse a new object may also be instantiated this frame, and the destroy gameobject won't happen until the end of the frame, so e.g. the car will go flying
//		// because its collider hits the collider of the previous car who is about to be destroyed at the end of the frame.
//
//		if (ueo.GetType() == typeof(UEO_DraggingParent)){
////			Debug.Log("type match dragparent");
//			// we deleted the dragging parent
//			foreach(Transform t in ueo.transform){
//				DestroyObject(t.GetComponent<UserEditableObject>(), SceneSerializationType.Class);
//			}
//			Destroy(ueo.gameObject);
//			return;
//		}
//
//		if (ueo && ueo.gameObject){
//			foreach(Collider c in ueo.GetComponentsInChildren<Collider>()){
//				c.enabled = false;
//			}
//		}
//		if (type == SceneSerializationType.Class){
//			
//			if (clearFromList){
//				if (placedObjectsClass.Contains(ueo)){
//					placedObjectsClass.Remove(ueo);
//				} else {
//					Debug.LogError("Tried to destroy:"+ueo.myName+", but didn't exist in our "+type.ToString()+" list");
//				}
//			}
//		} else if (type == SceneSerializationType.Instance){
//			if (clearFromList){
//				if (placedObjectsInstance.Contains(ueo)){
//					placedObjectsInstance.Remove(ueo);
//				} else {
//					Debug.LogError("Tried to destroy:"+ueo.myName+", but didn't exist in our "+type.ToString()+" list");
//				}
//			}
//		}
//		if (ueo){
//			NumberInfo ni = ueo.GetComponent<NumberInfo>();
//			if (ni){
//				ni.energyNumberDestroyedDelegate = null;
//			}
//			Destroy(ueo.gameObject);
//			ueo.destroyedThisFrame = true;
//		}
//
//	}

//	public List<UserEditableObject> GetPlacedObjects(SceneSerializationType type){
////		LevelBuilderObjectManager.inst.CleanPlacedObjects(type);
//		if (type == SceneSerializationType.Class) return placedObjectsClass;
//		if (type == SceneSerializationType.Instance) return placedObjectsInstance;
//		return null;
//	}
//
//	public void RemoveFromPlacedObjects(UserEditableObject ueo, SceneSerializationType type){
//		
//		if (type == SceneSerializationType.Class){
//			if (placedObjectsClass.Contains(ueo)){
//				placedObjectsClass.Remove(ueo);
////				Debug.Log("<color=#822>Removed:</color>"+ueo.myName);
//			} else {
////				Debug.Log("<color=#f55>Not exist:</color>"+ueo.myName);
////				PrintClassObjs();
//			}
//		} 
//
//	}



}
