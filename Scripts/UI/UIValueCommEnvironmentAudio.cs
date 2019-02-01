using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIValueCommEnvironmentAudio : MonoBehaviour {


	// For selecting an audio background with your level from the SAVE LEVEL inspetor.

	List<string> optsListWithSpaces = new List<string>();
	public Dropdown audioSelection;

	void OnEnable(){
		SetUpSelectableAudios();
	}

	void SetUpSelectableAudios(){
		// Based on Environment Audio FX manager, set up available spaces.
		audioSelection.ClearOptions();
		optsListWithSpaces.Clear();
		int i = 0;
		int selectedIndex = 0;
		foreach(BackgroundAudio env in BackgroundAudioManager.inst.environmentAudios){
			string val = env.name.ToString();
			optsListWithSpaces.Add(val);
			if (env == BackgroundAudioManager.inst.gameAudio){
				selectedIndex = i;
			}
			i++;
		}
		audioSelection.AddOptions(optsListWithSpaces);
		audioSelection.value = selectedIndex; // updates the currently visible selection with the existing audio that was set before (probably from json levelloader.)
	}

	public void UpdateGameBackgroundAudioValue(){
		BackgroundAudioManager.inst.SetGameBackgroundAudio( BackgroundAudioManager.inst.environmentAudios[audioSelection.value].name);
	}

	//		// commented Debug.Log("Finished optslits for:"+button.levelPiecePrefab.name+": list: "+debug_optslist);

}
