using UnityEngine;
using System.Collections;

public class TeacherLevelRestrictionManager : MonoBehaviour {

	private int teacherLevel = 1;
	public static TeacherLevelRestrictionManager inst;
	public GameObject lockIconPrefab;

	public TeacherLevelRestrictItem[] items;
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		WebGLComm.inst.GetTeacherLevel();
		items = Resources.FindObjectsOfTypeAll<TeacherLevelRestrictItem>();
		LevelBuilder.inst.levelBuilderOpenedDelegate += LevelBuilderOpened;
	}


	void LevelBuilderOpened(){
		WebGLComm.inst.GetTeacherLevel();
		UpdateLockStates();
	}
	void UpdateLockStates(){
		foreach (TeacherLevelRestrictItem item in items){
			item.UpdateLockedStatus(teacherLevel);
		}
	}

	public void SetTeacherLevel(int level){
		teacherLevel = level;
		UpdateLockStates();
	}
	
}
