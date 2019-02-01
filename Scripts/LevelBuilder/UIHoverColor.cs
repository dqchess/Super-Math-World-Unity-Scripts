using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;
using UnityEngine.EventSystems;  
using UnityEngine.UI;

public class UIHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	List<Text> text = new List<Text>();
	List<Image> image = new List<Image>();
	public bool ignoreText = false;
	public bool highlightSelf = false;
	public Color defaultTextColor = Color.white;
	public Color highlightColor = new Color(0.976f,0.8f,0,1);
	Color imageStartColor = new Color(0.7f,0.7f,0.7f,1); // GetComponent<Image>().color;

	void Start(){
		
//		// commented Debug.Log("starT");
//		if (highlightSelf) {
//			
//			// commented Debug.Log("my color wuz:"+imageStartColor+", "+name);
//		}
		foreach(Transform t in transform){
			if (t.GetComponent<Text>() && !ignoreText) text.Add(t.GetComponent<Text>());
			if (t.GetComponent<Image>()) image.Add(t.GetComponent<Image>());
		}
	}

	void OnEnable(){
		SetOrigImageColor();
		HoverOff();
	}
	void OnDisable(){
		HoverOff();
	}

	bool colorset = false;
	void SetOrigImageColor(){
		if (colorset) return;
		colorset =true;
		if (GetComponent<Image>()) {
//			Debug.Log("got");
			imageStartColor = GetComponent<Image>().color;
		}
//		Debug.Log("image star;"+imageStartColor);
	}

	void HoverOff(){
		foreach(Text t in text){
			t.color = defaultTextColor;// Color.white; //Or however you do your color
		}
		foreach(Image i in image){
			if (i) i.color = Color.white;
		}
		if (highlightSelf){
//			Debug.Log("hoveroff, img color:"+imageStartColor);
			GetComponent<Image>().color = imageStartColor;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (highlightSelf) GetComponent<Image>().color = highlightColor;
		foreach(Text t in text){
			t.color = highlightColor; //GameConfig.juneYellow; //Color.white; //Or however you do your color
		}
		foreach(Image i in image){
			if (i) i.color = highlightColor; 
		}

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HoverOff();

	}
}