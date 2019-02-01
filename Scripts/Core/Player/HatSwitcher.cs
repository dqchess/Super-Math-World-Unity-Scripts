using UnityEngine;
using System.Collections;

public class HatSwitcher : MonoBehaviour {

	public GameObject[] hats;
	public GameObject glasses;
	int hatIndex=0;


	public void SetHatActive(int i){
		DeactivateHats();
		if (i != -1) hats[i].SetActive(true);
	}

	public void DeactivateHats(){
		foreach(GameObject h in hats){
			h.SetActive(false);
		}
	}
	public void ShowHats(bool f){
		foreach(GameObject h in hats){
			if (h.GetComponent<Renderer>()) h.GetComponent<Renderer>().enabled = f;
		}
	}
}
