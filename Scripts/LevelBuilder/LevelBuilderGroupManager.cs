using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LevelBuilderGroupManager : MonoBehaviour {

	public List<List<UserEditableObject>> groups = new List<List<UserEditableObject>>();
	public static string groupManagerKey = "group_manager";
	public static string groupedObjsKey = "groups";
	public static LevelBuilderGroupManager inst;
	public void SetInstance(){
		inst = this;
	}

	void CleanGroups() {
		// in case a ueo was destroyed
		List<List<UserEditableObject>> toRemoveL = new List<List<UserEditableObject>>();
		foreach(List<UserEditableObject> gr in groups){
			//			string els = "";
			List<UserEditableObject> toRemove = new List<UserEditableObject>();
			foreach(UserEditableObject ueo2 in gr){
				if (ueo2 == null){
					toRemove.Add(ueo2);
				}
			}
			foreach(UserEditableObject ueo2 in toRemove){
				gr.Remove(ueo2);
			}
			if (gr.Count == 0){
				toRemoveL.Add(gr);
			}
		}

		foreach(List<UserEditableObject> gr2 in toRemoveL){
			groups.Remove(gr2);
		}

	}


	bool debug = false;
	public List<UserEditableObject> GroupContainingObject(UserEditableObject ueo){
		if (debug) Debug.Log("checking if obj "+ueo.GetUuid()+" is in a group.");
		CleanGroups();
		foreach(List<UserEditableObject> gr in groups){


			if (gr.Contains(ueo)) {
				if (debug)Debug.Log("It was!");
				return gr;
			}
		}
		return new List<UserEditableObject>();
	}

	public void MakeGroup(List<UserEditableObject> ueos){
		Ungroup(ueos);
		groups.Add(ueos);
		if (debug) Debug.Log("made group. groupcount:"+groups.Count);
	}

	public void Ungroup(List<UserEditableObject> ueos){
		List<List<UserEditableObject>> toRemoveL = new List<List<UserEditableObject>>();
		foreach(UserEditableObject ueo in ueos){
			List<UserEditableObject> toRemove = new List<UserEditableObject>();
			foreach(List<UserEditableObject> gr in groups){
				if (gr.Contains(ueo)) {
					toRemove.Add(ueo);
				}
				foreach(UserEditableObject u in toRemove){
					gr.Remove(ueo);
					if (gr.Count == 0){
						toRemoveL.Add(gr);
					}
				}
			}
		}
		foreach(List<UserEditableObject> ll in toRemoveL){
			groups.Remove(ll);
			if (debug) Debug.Log("removed empty group.");
		}
	}

	public SimpleJSON.JSONArray GetProperties(){
		CleanGroups();
		SimpleJSON.JSONArray allGroups = new SimpleJSON.JSONArray();

//		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		foreach(List<UserEditableObject> gr in groups){
			SimpleJSON.JSONArray uuids = new SimpleJSON.JSONArray();
//			List<int> uuids = new List<int>();
			foreach(UserEditableObject ueo in gr){
				SimpleJSON.JSONClass n = new SimpleJSON.JSONClass();
//				Debug.Log("adding;"+ueo.GetUuid());
//				n["uuid"] = new SimpleJSON.JSONClass();
				n["uuid"].AsInt = ueo.GetUuid();
				uuids.Add(n);
			}

//			SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
			allGroups.Add(uuids);

		}

		if (debug) Debug.Log("allgrou:"+allGroups.ToString());
		return allGroups;
	}

	public void ClearGroups(){
		
		groups.Clear();
	}
	public void SetProperties(SimpleJSON.JSONArray allGroups){
		ClearGroups();
		UserEditableObject[] allObjs = FindObjectsOfType<UserEditableObject>();
		foreach(SimpleJSON.JSONArray gr in allGroups.AsArray.Childs){
			List<UserEditableObject> ueoGroup = new List<UserEditableObject>();
			foreach(SimpleJSON.JSONNode j in gr){
				int uuid = -1;
				if (j["uuid"] != null){
					uuid = j["uuid"].AsInt;
				}

				if (debug) Debug.Log("checking for a match of uuid;"+uuid);
				foreach(UserEditableObject u in allObjs){
					if (u.GetUuid() == uuid){
						ueoGroup.Add(u);
						if (debug) Debug.Log("Found match!");
						break;
					} else {
//						Debug.Log("sadly, uuid did not match:"+u.GetUuid()+" and "+uuid);
					}
				}
//				gr.Add(ueoGroup);
			}
			if (debug) Debug.Log("ueo count:"+ueoGroup.Count);
			if (ueoGroup.Count > 0){
				groups.Add(ueoGroup);
			}
		}
		if (debug) Debug.Log("whew! groups:"+groups.Count);
	}
}
