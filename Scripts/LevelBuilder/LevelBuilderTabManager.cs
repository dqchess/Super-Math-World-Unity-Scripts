using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelBuilderTabManager : MonoBehaviour {

	public Transform tabButtonParent;
	public Transform tabParent;
	public Button defaultTabButton;
	public GameObject defaultTab;
	public static LevelBuilderTabManager inst;

	public void SetInstance(){
		inst = this;
	}

	public void SwapTabButton(Button b){
		foreach(Image im in tabButtonParent.GetComponentsInChildren<Image>()){
			im.color = new Color(0,0,0,0);
			im.GetComponentInChildren<Text>().color = Color.white;
		}
		b.GetComponent<Image>().color = Color.white;
		b.GetComponentInChildren<Text>().color = Color.black;
	}

	GameObject activeTab;
	public void SwapTab(GameObject o){
		activeTab = o;
		foreach(Transform t in tabParent){
			t.gameObject.SetActive(false);
		}
		AudioManager.inst.PlayClick2();
		o.SetActive(true);
		ScrollCurrentTabToTop();
	}

	public void SelectPrimeTab(){
		SwapTabButton(defaultTabButton);
		SwapTab(defaultTab);
	}

	public void ScrollCurrentTabToTop(){
		activeTab.GetComponentInChildren<Scrollbar>().value = 1;
	}

//	public void SwapNextTab(){
//		for (int i=0; i<tabButtonParent.childCount; i++){
//			if (tabButtonParent.GetChild(i).gameObject.activeSelf){
//				tabButtonParent.GetChild(i).gameObject.SetActive(false);
//
//				tabButtonParent.GetChild((i+1)%tabButtonParent.childCount).gameObject.SetActive(true);
//				continue;
//			}
//
//		}
//	}
}
