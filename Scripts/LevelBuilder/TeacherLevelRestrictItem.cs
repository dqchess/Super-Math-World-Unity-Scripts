using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeacherLevelRestrictItem : MonoBehaviour {

	public int itemLevel = 1;
	public LevelBuilderUIButton uib;
	public Button b;
	public UIHoverHelp hh;
	string hhOriginalTitle;
	string hhOriginalDescription;
	GameObject lockIcon;

	bool initiated=false;
	void Init(){
		if (initiated) return;
		initiated = true;
		uib = GetComponent<LevelBuilderUIButton>();
		b = GetComponent<Button>();
		hh = GetComponent<UIHoverHelp>();
		hhOriginalTitle = hh.title;
		hhOriginalDescription = hh.description;
		if (!hh) {
			hh = gameObject.AddComponent<UIHoverHelp>();
			hh.title = "";
			hh.description ="";
		}
		
		if (itemLevel > 1) {
			LockItem();
		}
	}

	void Start(){
		Init();
	}


	public void UpdateLockedStatus(int teacherLevel){
		Init();
//		WebGLComm.inst.Debug("update teacher level to:"+teacherLevel+" on "+name);
		if (teacherLevel >= itemLevel){
			UnlockItem();
		}
	}

	void UnlockItem(){
//		Debug.Log("unlcoked!"+
		if (hh){
			hh.title = hhOriginalTitle;
			hh.description = hhOriginalDescription;
			if (hh.title == "") Destroy(hh);
			if (uib) uib.enabled = true;
			if (b) b.enabled = true;
		}
		if (lockIcon) Destroy(lockIcon);
	}

	void LockItem(){
		lockIcon = (GameObject)Instantiate(TeacherLevelRestrictionManager.inst.lockIconPrefab,transform.position,Quaternion.identity);
		lockIcon.transform.parent = transform;
		lockIcon.GetComponent<RectTransform>().localPosition = new Vector3(-16,-20,0);
		if (uib) uib.enabled = false;
		if (b) b.enabled = false;
		hh.title += " (LOCKED)";
		hh.description = "(Required class level: "+itemLevel.ToString()+") "+hh.description;
	}
}
