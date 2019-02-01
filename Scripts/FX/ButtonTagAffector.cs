using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ButtonTagAffector : UserEditableObject {

	// This script attached to a button that lets the player touch it 
	// the button resets any object with a levelbuilderobjecttag matching a tag in its list
	// those objects will be destroyed and recreated
	// if player inside it will dump player out


	// another way
	/*
	 * at beginning of scene, scan for all objects of matching tags and make a dict { tag : [ objs ], tag2 : [objs]} 
	 * also save their properties at this time (jsons)
	 * when "reset", destroy all objects matching that tag, then place all objects (jsons) matching those tags.
	 * 
	 * */

	public static string sendTagsKey = "tagsAffected";
	public bool numbersTrigger = true;

	#region usereditbale
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMTagSendMessageButton);
		els.Add(LevelBuilder.inst.POCMcopyButton);
		els.Add(LevelBuilder.inst.POCMheightButton);
		return els.ToArray();
	}


	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(sendTagsKey)){
			tagsToSend.AddRange(N[sendTagsKey].Value.Split(','));
//			Debug.Log("tag affector set prop tags:"+N[sendTagsKey].Value);
		} else {
//			Debug.Log("ruh roh");
		}
	}

	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N[sendTagsKey] = string.Join(",",tagsToSend.ToArray());
//		Debug.Log("tag affector getprop tags:"+N[sendTagsKey].Value);
		return N;
	}

	#endregion


	public List<string> tagsToSend = new List<string>();

	public void ShowTaggedObjects(GameObject o){
		if (o.GetComponent<Player>() || (o.GetComponent<NumberInfo>() && numbersTrigger)){
			ShowTaggedObjects();
		}
	}

	public void HideTaggedObjects(GameObject o){
		if (o.GetComponent<Player>() || (o.GetComponent<NumberInfo>() && numbersTrigger)){
			HideTaggedObjects();
		}
	}

	public void ResetTaggedObjects(GameObject o){
		if (o.GetComponent<Player>() || (o.GetComponent<NumberInfo>() && numbersTrigger)){
			ResetTaggedObjects();
		}
	}

	public void ShowTaggedObjects(){
		foreach(UserEditableObject ueo in GetMatchingTaggedObjects()){
			ueo.gameObject.SetActive(true);
		}
	}

	public void HideTaggedObjects(){
		foreach(UserEditableObject ueo in GetMatchingTaggedObjects()){
			ueo.gameObject.SetActive(false);
		}
	}

	// buttons get confused because sendmessage() doesn't konw to look for functions that take different arguments
	public void ResetTaggedObjectsB(){		ResetTaggedObjects();	}
	public void HideTaggedObjectsB(){ HideTaggedObjects(); }
	public void ShowTaggedObjectsB(){ ShowTaggedObjects(); }

	public void ResetTaggedObjects(){
		foreach(UserEditableObject ueo in GetMatchingTaggedObjects()){
			SimpleJSON.JSONClass obj = JsonUtil.GetUeoBaseProps(new SimpleJSON.JSONClass(), ueo,SceneSerializationType.Instance);
			UserEditableObject u2 = LevelBuilderObjectManager.inst.PlaceObject(obj,SceneSerializationType.Instance); // object is now fresh and new.
			u2.OnLevelBuilderObjectCreated();
			u2.OnGameStarted();
			Destroy(ueo.gameObject);

		}
	}

	List<UserEditableObject> GetMatchingTaggedObjects(){
		List<UserEditableObject>  ret = new List<UserEditableObject>();

		foreach(UserEditableObject ueo in Utils.FindObjectsOfTypeInScene<UserEditableObject>()){
//		foreach(UserEditableObject ueo in Resources.FindObjectsOfTypeAll<UserEditableObject>()){
			if (ueo.myTags.Intersect(tagsToSend).Any()){
//				Debug.Log("tagsintersect;"+ueo.myTags[0]+" .. ");
				if (Player.inst.transform.root == ueo.transform.root){ // if player was inside, eject player before messing with this object. h4xors write h4xy code
					Player.inst.Unparent();
				}
				ret.Add(ueo);
			} else {
//				Debug.Log("No interesction for;"+ueo.name+" and "+tagsToSend);
			}
		}
		return ret;
				
	}
}
