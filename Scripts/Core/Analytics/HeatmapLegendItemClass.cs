using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeatmapLegendItemClass : MonoBehaviour {

	// This script helps organize HeatmapLegend items (a 2d list) into classes 
	// so you can show/hide the entire class, both on the 2d list (folding) 
	// and show/hide in the 3d game space (show/hide avatar object, which is handled on the HeatmapLegendItem.cs)

	public string className = "";
	public Text classNameText;
	public Image rightArrow;
	public Image downArrow;

	bool showing = true;
	public void Show(){
		downArrow.gameObject.SetActive(true);
		rightArrow.gameObject.SetActive(false);
		if (!showing) showing = true;
		foreach(HeatmapData hd in HeatmapManager.inst.heatmaps){
			if (hd.cls == className){
				hd.legendObject.SetActive(true);

				hd.legendObject.GetComponentInChildren<UIBooleanSlider>().TurnOn();
				hd.legendObject.GetComponent<HeatmapLegendItem>().ShowAvatar();


			}
		}
		UpdateRectHeight();
	}

	public void Hide(){
		downArrow.gameObject.SetActive(false);
		rightArrow.gameObject.SetActive(true);
		if (!showing) showing = false;
		foreach(HeatmapData hd in HeatmapManager.inst.heatmaps){
			if (hd.cls == className){
				hd.legendObject.GetComponent<HeatmapLegendItem>().HideAvatar();
				hd.legendObject.GetComponentInChildren<UIBooleanSlider>().TurnOff(); // should activate or deactive the 3d avatar object
				hd.legendObject.SetActive(false);
			}
		}
		UpdateRectHeight();
	}

	public void UpdateRectHeight(){
		int activeChilds = 0;
		foreach(Transform t in transform){
			if (t.gameObject.activeSelf) activeChilds ++;
		}
		float minHeight = 80; // even empty it has height to show its own self.
		float newHeight = activeChilds * 50 + minHeight; // only active childs
		GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,newHeight);
	}
}
