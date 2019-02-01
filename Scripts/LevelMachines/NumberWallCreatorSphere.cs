using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberWallCreatorSphere : NumberStructureCreator {


	
	public bool halfdome=false;
	public int timesToSubdivide=1;
	float sphereSpacing=0;
	public float scale = 6;

	#region usereditable
	public override void SetProperties(SimpleJSON.JSONClass N){
		SetFraction(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N));
//		CreateWall();
	}
	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();
		N =  JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		return N;
	}
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMFractionButton);
		els.Add(LevelBuilder.inst.POCMheightButton);
		return els.ToArray();
//		return new GameObject[] { LevelBuilder.inst.POCMFractionButton };
	}

	public override void OnLevelBuilderObjectPlaced(){
		CreateWall();
	}

	public override void OnGameStarted(){
		
	}

	#endregion

	List<Vector3> vectors = new List<Vector3>();
	List<int> indices = new List<int>();
	
	public override void Start(){
		if (vectors.Count==0){ SetVectors(); } 

	}

	void SetVectors(){
		GeometryProvider.Icosahedron(vectors, indices);
		sphereSpacing = scale*(float)timesToSubdivide;
		for (var i = 0; i < timesToSubdivide; i++) {
			GeometryProvider.Subdivide(vectors, indices, true);
		}
//		// commented Debug.Log("start, vectorsc:"+vectors.Count);	
	}


	public override Quaternion GetRotationFromIndex(int i){
		return Quaternion.LookRotation(transform.position-GetPositionFromIndex(i),Vector3.up);
	}



	// CreateIcosphere();




	public override Vector3 GetPositionFromIndex(int i){

//		vectors[i]=Vector3.Normalize(vectors[i]);
//			//			// commented Debug.Log("vectors i:"+i+" :"+vectors[i]);
		return transform.position + Vector3.Normalize(vectors[i])*sphereSpacing;

	}

	public override void CreateWall(){
		
		base.CreateWall();
		if (vectors.Count==0){ SetVectors(); } 
		List<int> targetNumbers = new List<int>();
//		// commented Debug.Log("createwall1, vecc:"+vectors.Count);
		for (var i = 0; i < vectors.Count; i++) {
			if (halfdome){
				if (GetPositionFromIndex(i).y > transform.position.y - 1) {
					
					targetNumbers.Add(i);
				}
			} else {
				targetNumbers.Add(i);
			}
		}
		SetCreatedNumbersToTarget(targetNumbers);

	}

	public override int GetStartingBlocksCount(){
		int ret = 0;
		for (int i=0;i<vectors.Count;i++){
			if (halfdome){
				if (GetPositionFromIndex(i).y > transform.position.y - 1) {

					ret++;
				}
			} else {
				ret++;
			}
		}
		return ret;
	}
	
}
