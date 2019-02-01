using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class NumberWallCreatorSquare : NumberStructureCreator {

	public bool box = false;
	public bool soaped = false;
	public bool regenerative = false;

	#region UserEditable 


	public static string wallXkey = "wallSizeX";
	public static string wallYkey = "wallSizeY";
	public static string wallZkey = "wallSizeZ";
	public static string wallCreatorSquareKey = "wallCreatorSquare";


	public override void OnLevelBuilderObjectSelected(){
		// hella hax, because I want to indicate a special property to the user by highlighting or not the sequential fraction button. It should be green if this object is already a sequential frac object
//		LevelBuilder.inst.SetSequentialFractionBackboardColor(fracSeqStepA.numerator,fracSeqStepB.numerator);


	}

	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] {
			LevelBuilder.inst.POCMFractionButton,
			LevelBuilder.inst.placedObjectContextMenuNumberWallSizeButton,
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMheightButton
//			LevelBuilder.inst.POCMsequentialFractionButton
		};
	}

	public override SimpleJSON.JSONClass GetProperties(){ 

		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		N[wallCreatorSquareKey][wallXkey] = wallX.ToString();
		N[wallCreatorSquareKey][wallYkey] = wallY.ToString();
		N[wallCreatorSquareKey][wallZkey] = wallZ.ToString();

//		// commented Debug.Log("getting prop:"+N.ToString());
		return N;
	}


	//	public override void SetFraction(string fraction){
	//		// nothing this doesn't have fraction.
	//	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
//		// commented Debug.Log("set prop on obj:"+name+", prop:"+N.ToString());
		if (N.GetKeys().Contains(wallCreatorSquareKey)) SetPropertiesSize(N);

		// move following to base.setprop?
		if (N.GetKeys().Contains(Fraction.fractionKey)) SetFraction(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N));

	}

	void SetPropertiesFraction(SimpleJSON.JSONClass N){
		frac = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
//		// commented Debug.Log("setting frac prop:"+frac+", N tr;"+N.ToString());
		foreach(NumberInfo ni  in GetComponentsInChildren<NumberInfo>()){
			ni.SetNumber(frac,false,false);
		}
//		// commented Debug.Log("setting FRAC prop:"+frac);
	}

	void SetPropertiesSize(SimpleJSON.JSONNode N){
		wallX = N[wallCreatorSquareKey][wallXkey].AsInt;
		wallY = N[wallCreatorSquareKey][wallYkey].AsInt;
		wallZ = N[wallCreatorSquareKey][wallZkey].AsInt;
//		DestroyChilds();
		CreateWall();


	}



	#endregion


	public NumberShape ns = NumberShape.Cube;
	public bool replaceableBricks = false;
	public int wallX = 7;
	public int wallY = 3;
	public int wallZ = 1;

	public bool checkered=false;
	public int checkeredIndex=0;
	public bool bricksDoCombine=true;
	public bool skipEveryOtherBlock=false;
	
//	float checkInterval = 0.25f;
	//int radius=15;
	
	// Use this for initialization

//	NumberWallCreatorRound nwr;
//	public override void Start(){
////		CreateWall();
//	}






	
	public override Vector3 GetPositionFromIndex (int i)
	{
		int x = i / (wallY * wallZ);
		int y = (i / wallZ) % wallY;
		int z = i % wallZ;
		return transform.position + transform.TransformDirection(new Vector3(x*brickScale,y*brickScale,z*brickScale)) + Vector3.up * upoffset;
	}

	public override GameObject MakeSingleBrick(int i, GameObject wallBrick = null){
		GameObject singleBrick = base.MakeSingleBrick(i,wallBrick);
		if (soaped){
			singleBrick.GetComponent<NumberInfo>().SoapNumber();
			singleBrick.GetComponent<NumberInfo>().SoapScale(brickScale * 2f * 0.86f); // correction factors ..
		}

		return singleBrick;
	}
//	public Vector3 KeyFromIndex (int i){
//		int x = i / (wallY * wallZ);
//		int y = (i / wallZ) % wallY;
//		int z = i % wallZ;
//		return new Vector3(x,y,z);
//	}



//	public override GameObject MakeSingleBrick (Vector3 pos,GameObject wallBrick = null)
//	{
//
////		if (skipEveryOtherBlock && i%2==1) return null;
////		if (checkered && (i+checkeredIndex)%2==0) return null;
//
//
////		return base.MakeSingleBrick(pos,wallBrick);
//	}
//

	public override void CreateWall(){
		base.CreateWall();
		List<int> targetPositions = new List<int>();

		for (var x=0;x<wallX;x++){
			for (var y=0;y<wallY;y++){
				for (var z=0;z<wallZ;z++){
//					if (brickInterval > 0) yield return new WaitForSeconds(brickInterval);
					
					// xyz -> index
					// xyz -> index
					int build = 0; // hollow, must be at the edge in at least 2 of the 3 axis
					if (x == 0 || x == wallX - 1) build ++;
					if (y == 0 || y == wallY - 1) build ++;
					if (z == 0 || z == wallZ - 1) build ++;
					int minReq = box ? 1 : 0;
					if (build > minReq) {
						int i = x * wallY * wallZ + y * wallZ + z;
						targetPositions.Add(i);
					} else {
//						// commented Debug.Log("skip brick on xyz:"+x+","+y+","+z+"; wallx wally wallz:"+wallX+","+wallY+","+wallZ+" ... build:"+build);
					}
//					int i = x * wallY * wallZ + y * wallZ + z;
				}
			}
		}

//		// commented Debug.Log("set created numbers to target:"+targetPositions.Count);
		SetCreatedNumbersToTarget(targetPositions);
	}

	float regenTime = 8f;
	float regenTimer = 8f;
	void Update(){
		if (regenerative && wallCompleted && GetStartingBlocksCount() != GetBlocksCount()){
			// the wall lost a brick due to player interaction or other game interaction, and needs to be regenerated. Start the countdown.
			regenTimer -= Time.deltaTime;
			if (regenTimer < 0){
				ResetWall();
			}
		}

		base.Update();

	}

	void ResetWall(){
		wallCompleted = false;
		regenTimer = regenTime;
		DestroyNumbers();
		StartCoroutine(CreateWallAfterSeconds(0.5f)); // give it time to destroy the old ones at the end of the frame.
	}

	IEnumerator CreateWallAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		CreateWall();
	}

//	public override int GetStartingBlocksCount(){
//		int ret = box ? wallX * 4 + wallY * 4 + wallZ * 4 - 8 : wallX * wallY * wallZ;
//		// commented Debug.Log("start blocksc count on:"+name+" with box:"+box+" was:"+ret);
//		return ret;
//	}
}
