using UnityEngine;
using System.Collections;

public class SceneLayerMasks : MonoBehaviour {

	public LayerMask terrainOnly;
//	public LayerMask canPlaceLevelObjectsOnTheseLayers;
	public LayerMask canPlaceLevelObjectsOnTheseLayers;
	public LayerMask terrainAndWaterForObjectPlacement;
	public LayerMask terrainAndWaterAndPlayer;
	public LayerMask terrainAndWaterSurface;
	public LayerMask uiMapTarget;
	public LayerMask skyCamVisible;

	public static SceneLayerMasks inst;

	public void SetInstance(){
		inst = this;
	}
}
