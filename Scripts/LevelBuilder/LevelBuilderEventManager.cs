using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelBuilderEventType {
	Create,
	Delete,
	Modified
}

public class LevelBuilderEventObject {
	public UserEditableObject ueo;
	public int uuid;
	public SimpleJSON.JSONClass N;
	public LevelBuilderEventObject(UserEditableObject _ueo, int _uuid, SimpleJSON.JSONClass _N){
		ueo = _ueo;
		uuid = _uuid;
		N = _N;
	}
}

public class LevelBuilderEvent {
	public List<LevelBuilderEventObject> lbeos = new List<LevelBuilderEventObject>();
	public LevelBuilderEventType type;

	public LevelBuilderEvent (List<LevelBuilderEventObject> _lbeos, LevelBuilderEventType _type){
		lbeos = _lbeos;
		type = _type;
	}
}

public class LevelBuilderEventManager : MonoBehaviour {

	// todo 
	// keep a simple history log
	// deleted object X at position,rot,scale (Y,R,S) at timestep T
	// Created object Y at position, rotation, scale ABC
	// moved object A to new pos
	// rotated
	// scaled
	// etc..



	// This script handles actions taken in LevelBuilder allowing you to step forwards and backwards
	// problems: begin moving and end moving need to be able to reset to position before it was moved
	// problems: need to address when user has selected multiple objects in a drag parent and either a) register an undo event for each item or b) register an undo event for all items in a group
	// this is not done by default since we are registering a single "obj" as the undo event target, such as the "draggingParent" which we cannot recover later by simply trying to instantiate
	// "draggingparent" because "draggingparent" is an ephemeral undefined item -- we would need to make a list of all items inside it
	public static LevelBuilderEventManager inst;
	public int currentEventIndex = 0;
	List<LevelBuilderEvent> pastEvents = new List<LevelBuilderEvent>();
	List<LevelBuilderEvent> futureEvents = new List<LevelBuilderEvent>(); // happens when you use "undo"
	bool debug = false;
	public void SetInstance(){
		inst = this;
	}

	public bool TypeIsRegisterable(UserEditableObject ueo){
		if (ueo.GetType() == typeof(UEO_DraggingParent)) return false;
		if (ueo.GetType() == typeof(PlayerStart)) return false;
		return true;

	}

	public void RegisterLevelBuilderEvent(UserEditableObject[] ueos, LevelBuilderEventType type){
		if (debug) Debug.Log("<color=fff>Register</color> "+type);
		List<LevelBuilderEventObject> lbeos = new List<LevelBuilderEventObject>();
		foreach(UserEditableObject ueo in ueos){
			if (!TypeIsRegisterable(ueo)) continue;
			if (!ueo.isSerializeableForClass) continue;
			lbeos.Add(new LevelBuilderEventObject(ueo,ueo.GetUuid(),JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(),ueo,SceneSerializationType.Class)));
			if (debug) Debug.Log("....<color=fff>uuid</color>:"+ueo.GetUuid());
		}
		if (lbeos.Count > 0){
			LevelBuilderEvent ev = new LevelBuilderEvent(lbeos,type);
			pastEvents.Add(ev);
		}

	}



	public void Undo(){
		if (pastEvents.Count > 0) {
			LevelBuilderEvent ue = pastEvents[pastEvents.Count - 1];
			if (debug) Debug.Log("<color=#9f9>Undo: "+ue.type+"</color> ueos:");
			switch(ue.type){
			case LevelBuilderEventType.Create:
				foreach(LevelBuilderEventObject lbeo in ue.lbeos){
					if (lbeo.ueo) Destroy(lbeo.ueo.gameObject);
				}
				break;
			case LevelBuilderEventType.Delete:
				List<LevelBuilderEventObject> replacementGroup = new List<LevelBuilderEventObject>();
				
				foreach(LevelBuilderEventObject lbeo in ue.lbeos){
					if (debug) Debug.Log("<color=5f5>Creating (undo)</color>: "+lbeo.uuid);
					UserEditableObject ueo = LevelBuilderObjectManager.inst.PlaceObject(lbeo.N,SceneSerializationType.Class,lbeo.uuid);
					replacementGroup.Add(new LevelBuilderEventObject(ueo,lbeo.uuid,JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(),ueo,SceneSerializationType.Class)));
					ReconnectBrokenUuidsForUndeletedObjects(ueo,lbeo.uuid);
				}
				ue.lbeos = replacementGroup;

				break;
			case LevelBuilderEventType.Modified:
				foreach(LevelBuilderEventObject lbeo in ue.lbeos){
//					Debug.Log("undo mod;"+kvp.Key.myName);
					lbeo.ueo.SetProperties(lbeo.N);
					lbeo.ueo.SetTransformProperties(lbeo.N);
					
//					JsonUtil.SetUeoTransformProps(kvp.key,	kvp.Value);
				}
				break;
			default: break;
				
			}
			futureEvents.Add(ue);
			pastEvents.Remove(ue);
		}
	}

	public void Redo(){
		if (futureEvents.Count > 0){
			LevelBuilderEvent ue = futureEvents[futureEvents.Count-1];
			if (debug) Debug.Log("<color=#9f9>Redo: "+ue.type+"</color> ueos:");
			switch(ue.type){
			case LevelBuilderEventType.Create:
				List<LevelBuilderEventObject> replacementGroup = new List<LevelBuilderEventObject>();
				foreach(LevelBuilderEventObject lbeo in ue.lbeos){
					if (debug) Debug.Log("<color=5f5>Creating (</color><color=44f>do</color>): "+lbeo.uuid);
					UserEditableObject ueo = LevelBuilderObjectManager.inst.PlaceObject(lbeo.N,SceneSerializationType.Class,lbeo.uuid);
					replacementGroup.Add(new LevelBuilderEventObject(ueo,ueo.GetUuid(),JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(),ueo,SceneSerializationType.Class)));
					ReconnectBrokenUuidsForUndeletedObjects(ueo,ueo.GetUuid());
				}
				ue.lbeos = replacementGroup;
				break;
				case LevelBuilderEventType.Delete:
				foreach(LevelBuilderEventObject lbeo in ue.lbeos){

					if (lbeo.ueo) Destroy(lbeo.ueo.gameObject);
				}
				break;
				case LevelBuilderEventType.Modified:
				foreach(LevelBuilderEventObject lbeo in ue.lbeos){
					
					lbeo.ueo.SetProperties(lbeo.N);
					lbeo.ueo.SetTransformProperties(lbeo.N);
				}
				break;
				default:break;
//				Debug.Log("redoing properties of:"+ue.obj.name+", set prop to:"+ue.prop.ToString());
//				ue.obj.GetComponent<UserEditableObject>().SetProperties(ue.prop);
			}
			futureEvents.Remove(ue);
			pastEvents.Add(ue);
		}
	}

	void Update(){
		
	}

//	public List<LevelBuilderEventObject> GetUeoGroupFromObject(UserEditableObject ueo){ 
//		// We need to re-assign the newly undone (redone?) creation (destruction?) of 
//		//an object because whenever obejcts are RE created they lose their original ueo reference in the events list (pastEvents or futureEvents)
//		List<LevelBuilderEventObject> lbeos = new List<LevelBuilderEventObject>();
//		lbeos.Add(new LevelBuilderEventObject(ueo,ueo.GetUuid(),JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(),ueo,SceneSerializationType.Class)));
//		return lbeo;
//		
//	}
//
	public void RegisterModifyEvent(GameObject p){
		if (pastEvents.Count > 0){
//			Debug.Log("past ev > 0");
			UserEditableObject ueo = p.GetComponent<UserEditableObject>();
			if (ueo 
				&& ueo == pastEvents[pastEvents.Count-1].lbeos[0].ueo){

				if ( pastEvents[pastEvents.Count-1].lbeos[0].N.ToString() == JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(), ueo,SceneSerializationType.Class).ToString()
					&& pastEvents[pastEvents.Count-1].type == LevelBuilderEventType.Modified

				){
					if (debug) Debug.Log("<color=fff>Register</color> Could not modify..dupe;"+pastEvents.Count);
					return;
				} else {
//					Debug.Log("s1;"+pastEvents[pastEvents.Count-1].lbeos[0].N.ToString());
//					Debug.Log("s2;"+JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(), ueo,SceneSerializationType.Class).ToString());
				}
			}
			
		} else {
		}

		RegisterLevelBuilderEvent(p.GetComponentsInChildren<UserEditableObject>(),LevelBuilderEventType.Modified);
//		Debug.Log("<color=fff>Register</color> Could not modify..pastcount;"+pastEvents.Count);

	}

	public void RegisterDeleteEvent(GameObject p){
		
		RegisterLevelBuilderEvent(p.GetComponentsInChildren<UserEditableObject>(),LevelBuilderEventType.Delete);
	}

	public void RegisterCreateEvent(UserEditableObject[] ueos){
		
		RegisterLevelBuilderEvent(ueos,LevelBuilderEventType.Create);
	}

	public void RegisterCreateEvent(GameObject p){
//		p.name += Time.time;

		RegisterLevelBuilderEvent(p.GetComponentsInChildren<UserEditableObject>(),LevelBuilderEventType.Create);
	}

	public void ResetEvents(){
		pastEvents.Clear();
		futureEvents.Clear();
	}

	int eventStackLimit = 40;
	public void PruneEvents(){
		while (pastEvents.Count > eventStackLimit){
			pastEvents.RemoveAt(0);
			if (debug) Debug.Log("removed 1 past");
		}
		while (futureEvents.Count > eventStackLimit){
			futureEvents.RemoveAt(0);
			if (debug) Debug.Log("removed 1 fut");
		}
	}

	public void ReconnectBrokenUuidsForUndeletedObjects(UserEditableObject ueo, int uuid){
//		Debug.Log("Trying to reconnect uuid :"+uuid);
		List<LevelBuilderEvent> allEvents = new List<LevelBuilderEvent>();
		allEvents.AddRange(pastEvents);
		allEvents.AddRange(futureEvents);
		foreach(LevelBuilderEvent ev in allEvents){
			foreach(LevelBuilderEventObject lbeo in ev.lbeos){
				if (lbeo.uuid == uuid){

					lbeo.ueo = ueo;
//					Debug.Log("reconnecting:"+lbeo.ueo.name+" to id;"+uuid);
				} else {
//					Debug.Log(lbeo.ueo.GetUuid()+"!="+uuid);
				}
			}
		}
	}
}
