using UnityEngine;
using System.Collections;

public class FlowerManager : MonoBehaviour {

	public GameObject[] flowerPrefabs;
	public static FlowerManager inst;


	public void SetInstance(){
		inst = this;
	}

}
