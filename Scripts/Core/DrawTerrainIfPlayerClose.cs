using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTerrainIfPlayerClose : MonoBehaviour {

	public Terrain blockPlayerFallTerrain;
	public float distToCare = 40f;
	// Use this for initialization
	void Start () {
		LevelBuilder.inst.levelBuilderOpenedDelegate += LevelBuilderOpened;
	}

	void LevelBuilderOpened(){
		Hide();
	}

	// Update is called once per frame
	void Update () {
		if (Player.inst.transform.position.y < distToCare){
			Show();

		} else {
			Hide();
		}
	}

	void Show(){
		if (Vector3.Distance(transform.position,Utils.FlattenVector(Player.inst.transform.position)) > 200) 
			transform.position = Utils.FlattenVector(Player.inst.transform.position); // stay centered under player.
		if (!blockPlayerFallTerrain.gameObject.activeSelf){
			blockPlayerFallTerrain.gameObject.SetActive(true);
		}
	}

	void Hide() {
		if (blockPlayerFallTerrain.gameObject.activeSelf){
			blockPlayerFallTerrain.gameObject.SetActive(false);
		}
	}
}
