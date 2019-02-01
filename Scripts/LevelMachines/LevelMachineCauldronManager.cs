using UnityEngine;
using System.Collections;

public class LevelMachineCauldronManager : MonoBehaviour {


	public bool puzzleFrozen = false;


	public void ResetPuzzle(){
//		Debug.Log("reset1");
		puzzleFrozen = false;
		if (FindObjectOfType<NumberFaucetSequential>()) {
			FindObjectOfType<NumberFaucetSequential>().Reset();
		}
		foreach(LevelMachineCauldron lc in FindObjectsOfType<LevelMachineCauldron>()){
			lc.Reset();
		}
	}
}
