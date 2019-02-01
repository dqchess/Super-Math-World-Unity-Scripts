using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class TutorialManager : MonoBehaviour {

	public static TutorialManager inst;
	public GameObject[] tutorial_play;
	public GameObject[] tutorial_create;
	public GameObject restrictInputBig;
	public GameObject restrictInputSmall;

	public GameObject openTutorialButton;
	public UIInstructionsHelper currentTutorial;

	public void SetInstance(){
		inst =this;
	}

	public bool tutorialActive = false;

	void Start(){
		
		foreach(GameObject o in tutorial_play){
			o.SetActive(false);
		}
		foreach(GameObject o in tutorial_create){
			o.SetActive(false);
		}
	}



	public void ActivateTutorial(GameObject o){
//		GameManager.inst.SetScrollTop();
		tutorialActive = true;
		openTutorialButton.SetActive(false);
		LevelBuilder.inst.ShowLevelBuilder();
		o.SetActive(true);
		currentTutorial = o.GetComponent<UIInstructionsHelper>();
		currentTutorial.Init();
	}

	public void EndInstructionMode(){
		tutorialActive = false;
		foreach(GameObject o in tutorial_create){
			o.SetActive(false);
		}
		openTutorialButton.SetActive(true);
		currentTutorial = null;
		HoverHelperManager.inst.HideHoverHelp();
		restrictInputBig.SetActive(false);
		restrictInputSmall.SetActive(false);
//		hoverParent.SetActive(false);
//		gameObject.SetActive(false);
		foreach(DetectMouseClickUI dui in FindObjectsOfType<DetectMouseClickUI>()){
			Destroy(dui);
		}
	}



}
