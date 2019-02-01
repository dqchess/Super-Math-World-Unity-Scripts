using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIValueCommSelectTutorial : MonoBehaviour {

	public Dropdown tutorial;
	// Use this for initialization


	List<string> tutorialOptions = new List<string>();
	public void OnEnable(){
		tutorial.ClearOptions();
		tutorialOptions.Clear();
		for(int i=0; i <TutorialManager.inst.tutorial_create.Length; i++){
			GameObject o = TutorialManager.inst.tutorial_create[i];
			tutorialOptions.Add(o.name);
		}
		tutorial.AddOptions(tutorialOptions);
	}

	public void BeginTutorial(){
		TutorialManager.inst.ActivateTutorial(TutorialManager.inst.tutorial_create[tutorial.value]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
