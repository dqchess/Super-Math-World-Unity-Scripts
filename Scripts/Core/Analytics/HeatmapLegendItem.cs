using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeatmapLegendItem : MonoBehaviour {

	public HeatmapAvatar avatar;
	public Image hair;
	public Image head;
	public Image body;
	public UIBooleanSlider slider;
	public Text totalTime;
	public Text avatarName;
	public Text avatarClass;

	public void SetHeatmapLegendProperties(HeatmapAvatar ha,string nm,string cls,string t,Color bodyColor,Color headColor,Color hairColor){
		avatar = ha;
		avatarName.text = nm;
//		avatarName.color = bodyColor;
		avatarClass.text = cls;
		totalTime.text = t;
		hair.color = hairColor;
		body.color = bodyColor;
		head.color = headColor;
	}

	public void CenterCameraOnThisAvatar(){
		if (avatar) LevelBuilder.inst.SnapPanToPosition(avatar.transform.position);
	}
	void OnEnable(){
		if (avatar) SliderToggleCallback(); // when enabled, if slider was "off" go ahead and hide avatar if we did so already
	}
	public void HideAvatar(){
		avatar.gameObject.SetActive(false);
//		foreach(GameObject o in avatar.droppedObjects){
//			o.SetActive(false);
//		}
	}
	public void ShowAvatar(){
		avatar.gameObject.SetActive(true);
	}
	public void SliderToggleCallback(){
		if (slider.GetSliderValue() == true){
			ShowAvatar();
		} else {
			HideAvatar();
		}
	}
}
