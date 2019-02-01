using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIValueCommHeight : UIValueComm {

//	public InputField height;
	public Text height;
	LevelBuilderUIInputText restrict;

	float distToTerrain = 0;
	int userVisibleDistToTerrain = 0;
	int heightDistUnitScale = 1;
	public int min = 0;
	public int max = 200;

	void Start(){
//		restrict = GetComponentInChildren<LevelBuilderUIInputText>();
//		min = MathUtils.IntParse(restrict.min);
//		max = MathUtils.IntParse(restrict.max);
	}

	void OnEnable(){
		UpdateHeightText();
//		UpdateDistToTerrain();

	}

	void UpdateHeightText(){
//		// commented Debug.Log("updating height text. real height:"+LevelBuilder.inst.currentPiece.transform.position.y);
		if (LevelBuilder.inst.currentPiece) height.text = Utils.ToString(LevelBuilder.inst.currentPiece.transform.position.y,1); //).ToString();
	}



	public void PointerDownHeight(float amount){
		copying = true;
		copyAmount = amount;
		AddValueToHeight(copyAmount);
		repeatInterval = 0.25f; // initially have to wait .2 seconds before repeat kicks in
	}

	public void PointerUp(){
		copying = false;
	}

	float repeatInterval = 0;
	bool copying = false;
	float copyAmount = 0;
	void Update(){
		repeatInterval -= Time.deltaTime;
		if (repeatInterval < 0 && copying){
			repeatInterval = 0.01f;
			AddValueToHeight(copyAmount);
		}
	}


	public void AddValueToHeight(float amount){
//		int spacing = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().VerticalSnapSpacing;
//		float spacing = LevelBuilder.inst.verticalSnapSpacing;
		Vector3 p = LevelBuilder.inst.currentPiece.transform.position;
//		float newY = Mathf.RoundToInt(p.y / spacing) * spacing + spacing * amount;
//		float deltaHeight = newY - p.y;
//		// commented Debug.Log("deltah:"+deltaHeight);
		LevelBuilder.inst.currentPiece.transform.position = new Vector3(p.x,p.y+amount,p.z); // * int.Parse(height.text) * heightDistUnitScale;
		UpdateHeightText();
		LevelBuilder.inst.SnapPanToCurrentObject();
	}


	//	RaycastHit hit = new RaycastHit();
	//	void UpdateDistToTerrain(){
	////		// commented Debug.Log("pos:"+LevelBuilder.inst.currentPiece.transform.position);
	//		int upoffset = 10;
	//		Ray ray = new Ray(LevelBuilder.inst.currentPiece.transform.position + Vector3.up * upoffset,Vector3.down);
	//
	//		RaycastHit[] hits = Physics.RaycastAll(ray,500,SceneLayerMasks.inst.terrainOnly); //,out hit, Mathf.Infinity,~LayerMask.NameToLayer("Terrain"))){
	//		foreach(RaycastHit hitt in hits){
	////			// commented Debug.Log("hit:"+hitt.collider.name);
	//			if (hitt.collider.gameObject.layer == LayerMask.NameToLayer("Terrain") && hitt.collider.transform.root.gameObject != LevelBuilder.inst.currentPiece){
	////				// commented Debug.Log("success");
	//				hit = hitt;
	//				distToTerrain = hitt.distance - upoffset;
	////				// commented Debug.Log("hitdist;"+distToTerrain);
	//				userVisibleDistToTerrain = Mathf.RoundToInt(distToTerrain * heightDistUnitScale);
	//			}
	//		}
	//		height.text = userVisibleDistToTerrain.ToString();
	////		// commented Debug.Log("updated:"+distToTerrain);
	//		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().UpOffset = distToTerrain;
	//	}
	

//	public void UpdateHeight(){
//		height.text = Mathf.Clamp(MathUtils.IntParse(height.text),min,max).ToString();
////		// commented Debug.Log("pos:"+LevelBuilder.inst.currentPiece.transform.position);
//		UpdateAbsoluteHeight();
////		UpdateDistToTerrain();
//	}

}
