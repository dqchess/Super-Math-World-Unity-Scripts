using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NumberWallCreatorPyramid : NumberStructureCreator {

	public NumberShape ns = NumberShape.Cube;
	
	public int pyramidHeight=4;
//	public float brickInterval = .01f;

	public bool bricksDoCombine=true;
	public int totalBricks=0;
	
//	protected Dictionary<int,GameObject> wall = new Dictionary<int,GameObject>();
	
//	float checkInterval = 0.25f;
	//int radius=15;
	
	// Use this for initialization

	NumberWallCreatorRound nwr;

	public override void Start(){
		
//		CreateWall();

	}


	
	
//	public override IEnumerator RespawnBricks(){ yield return 0; }
	


	
	public override Vector3 GetPositionFromIndex (int i)
	{
		// which layer is it on?
		// how many total bricks? height=4 = 4*4 + 3*3 + 2*2 + 1
		int x=0;
		int y=0;
		int z=0;
		int j=0;
		float xOffset=0;
		float zOffset=0;
		int lastVal=0;
		int sqrVal=0;
		for (j=0; j<pyramidHeight; j++){
			sqrVal= (pyramidHeight-j)*(pyramidHeight-j);
			if (i< sqrVal+lastVal) {
				xOffset=(pyramidHeight-j)*brickScale/2f;
				x = i % (pyramidHeight - j);
				break;
			} else {
				lastVal += sqrVal; 
			}
		}
		
		lastVal=0;
		for (j=0; j<pyramidHeight; j++){
			sqrVal= (pyramidHeight-j)*(pyramidHeight-j);
			if (i < sqrVal+lastVal) {
				zOffset=(pyramidHeight-j)*brickScale/2f;
				z = i / (pyramidHeight - j) % (pyramidHeight - j);
				break;
			} else {
				lastVal += sqrVal;
			}
		}
		
		lastVal=0;
		for (j=0; j<pyramidHeight; j++){
			sqrVal = (pyramidHeight-j)*(pyramidHeight-j);
			if (i < sqrVal+lastVal) {
				y=j;
				break;
			} else {

				lastVal+=sqrVal;
			}
		}
		
		return transform.position + transform.TransformDirection(new Vector3(x*brickScale-xOffset,y*brickScale+brickScale/2f,z*brickScale-zOffset));
	}
//	
//	public override GameObject MakeSingleBrick (Vector3 pos, Fraction f, GameObject wallBrick = null)
//	{
//		bool pickup=false;
//		Quaternion rot = Quaternion.identity;
//		NumberShape shape = NumberShape.Cube;		
//		// index -> xyz
//		
//		bool numNeedsSet
//		if (wallBrick != null){
//			wallBrick.transform.position = pos;
//		} else {
//			wallBrick = NumberManager.inst.CreateNumber(
//			frac,
//			pos,
//			shape);
//		}
//		wallBrick.transform.rotation = rot;
//		createdNumbers.Add(pos,wallBrick);
//		wallBrick.name=i.ToString();
////		// commented Debug.Log("hi");
//		if(autoRevert){
//			wallBrick.AddComponent<MonsterAIRevertNumber>();
//		}
//		
//		wallBrick.transform.parent=transform;
//		wallBrick.transform.localScale = new Vector3(brickScale,brickScale,brickScale) + fixZFighting;
//		wallBrick.transform.localRotation =GetRotationFromIndex(i);
//		NumberInfo ni = wallBrick.GetComponent<NumberInfo>();
//
//		
//		NumberInfoDefaults nid = new NumberInfoDefaults(ni, ni.fraction, i);
//		if(numbers.Count <= i) {
//			numbers.Add(nid);
//		}
//		else {
//			numbers[i] = nid;
//		}
//
//		if (!wall.ContainsKey(i)){
//			wall.Add(i,wallBrick);
//		}
//
//		return wallBrick;
//	}


}
