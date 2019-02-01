using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilderMoreOptions : MonoBehaviour {

	public Transform openPosition;
	public Transform closedPosition;
	public Transform optionsMenu;
	public GameObject backboard;
	Transform targetPos;
	bool moving = false;



	void Start(){
		targetPos = closedPosition;
	}

	void Update(){
		if (moving){
			float moveSpeed = 8f;
			optionsMenu.position = Vector3.Lerp(optionsMenu.position,targetPos.position,moveSpeed * Time.deltaTime);
			if (Vector3.Distance(targetPos.position,optionsMenu.position) < .1f){
				optionsMenu.position = targetPos.position;
				moving = false;
			}
		}
	}

	public void Show(){

		targetPos = openPosition;
		moving = true;
		backboard.SetActive(true);
	
	}

	public void Hide(){
		
		backboard.SetActive(false);
		targetPos = closedPosition;
		moving = true;

	}

	public void Toggle(){
		if (targetPos == closedPosition){
			Show();
		} else {
			Hide();
		}
	}

	public GameObject mouseScrollReverseToggleCheckImage;
	public void ToggleMouseReversed(){
		GameManager.inst.ToggleMouseScrollReversed();
		mouseScrollReverseToggleCheckImage.SetActive(GameManager.inst.mouseScrollReversed);
	}
}
