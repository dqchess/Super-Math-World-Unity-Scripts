using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//public enum ButtonBackground {
//	Transparent,
////	WhiteOn
//
//}

public class LevelBuilderUIButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

	public GameObject levelPiecePrefab;

	public Text extraDescriptionText;


	void Start(){
//		startImageColor = GetComponent<Image>().color;
	}

	public void OnPointerDown (PointerEventData eventData) {
		AnalyticsManager.inst.RecordButtonPressEvent(this.name); // userData.levelBuilderButtonsPressed.Add(this.name);
		LevelBuilder.inst.UIButtonPressed(GetComponent<LevelBuilderUIButton>());
	}

	public void OnPointerEnter(PointerEventData eventData){
//		GetComponent<Image>().color = Color.white;
	}



	public void OnPointerExit(PointerEventData eventData){
		
//		HoverColorOff();
	}

	void HoverColorOff(){
//		GetComponent<Image>().color = startImageColor;
	}

	void OnDisable(){
//		HoverColorOff();
	}




}
