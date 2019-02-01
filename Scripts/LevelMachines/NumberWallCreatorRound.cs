using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NumberWallCreatorRound : NumberStructureCreator {

	#region UserEditable 



	public static string wallDegreesKey = "WallDegrees";
	public static string wallRadiusKey = "WallRadius";
	public static string wallHeightKey = "WallHeight";
	public static string wallCreatorRoundKey = "wallCreatorRound";

	public override SimpleJSON.JSONClass GetProperties(){ 

		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		//		if (degreesToComplete == 0) degreesToComplete = 360;
		N[wallCreatorRoundKey] = new SimpleJSON.JSONClass();
		N[wallCreatorRoundKey][wallDegreesKey] = degreesToComplete.ToString();
		N[wallCreatorRoundKey][wallRadiusKey] = radius.ToString();
		N[wallCreatorRoundKey][wallHeightKey] = height.ToString();
		//		// commented Debug.Log("getting prop:"+N.ToString());
		return N;
	}

	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] {
			LevelBuilder.inst.POCMFractionButton,
			LevelBuilder.inst.placedObjectContextMenuNumberWallRoundSizeButton,
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMheightButton
		};
	}



	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(wallCreatorRoundKey)) SetPropertiesSize(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)) SetFraction(JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N));

	}



	void SetPropertiesSize(SimpleJSON.JSONNode N){
		radius = N[wallCreatorRoundKey][wallRadiusKey].AsInt;
		degreesToComplete = N[wallCreatorRoundKey][wallDegreesKey].AsInt;
		//		if (degreesToComplete == 0) degreesToComplete = 360;
		height = N[wallCreatorRoundKey][wallHeightKey].AsInt;



		CreateWall();
//		DestroyRemainingChilds();
		//		// commented Debug.Log("setting rad ,deg ,height, prop:"+radius+","+degreesToComplete+","+height);
	}

//	public override void SetFraction(Fraction f){
//		base.SetFraction(f);
//		if (skyCamText) {
//			skyCamText.Text = Fraction.ReduceFully(frac).ToString();
//			skyCamText.Color = f.numerator > 0 ? Color.black : Color.white;
//			skyCamText.transform.localPosition = new Vector3(0,height*brickScale+1,0);
//		}
//
//	}





	/* footpring was: (){
		return 1;
	 */
	#endregion

	public int degreesToComplete = 360;
	public int radius = 10;
	public int height = 4;
	public int thickness = 1;
	//	public Combineability numberCombineability;

	public bool isRollableWheel = false;

	bool isFinalPiece; //this feels wrong and awkward. Let's never speak of it, or to each other, again.
	public bool yieldOnStart=false;
	public float yieldTime = 5;

	public override void Start(){

		//		CreateWall();
	}

	virtual public int NumberFromIndex(int i) {
		return i;
	}

	public override Vector3 GetPositionFromIndex (int i)
	{
		int count = (int)(degreesToComplete * radius / brickScale / 60);
		float arcLength = degreesToComplete / (float)count;

		int j = i % height;
		int t = (i / height) % count;
		int h = i / (height * count);

		float xPos = Mathf.Sin(Mathf.Deg2Rad*t*arcLength)*(radius+h*brickScale);
		float yPos = Mathf.Cos(Mathf.Deg2Rad*t*arcLength)*(radius+h*brickScale);		
		Vector3 pos = transform.position + transform.TransformDirection(new Vector3(xPos,j*brickScale,yPos)) + Vector3.up * upoffset;
		return pos;
	}

	public override Quaternion GetRotationFromIndex (int i)
	{
		int count = (int)(degreesToComplete * radius / brickScale / 60);
		float arcLength = degreesToComplete / (float)count;

		int t = (i / height) % count;
		return Quaternion.AngleAxis(t*arcLength, Vector3.up);
	}

	public override void CreateWall() {
//		Debug.Log("wall");
		List<int> targetPositions = new List<int>();
		if (isRollableWheel) {
			NumberWallCreatorRound.CreateRollableWall(
				transform.position,
				radius,
				degreesToComplete,
				brickScale,
				thickness,
				height, 
				frac,
				transform);
			return;
		}
//		else {
			
		int count = (int)(degreesToComplete * radius / brickScale / 60);


		for (int i=0;i<thickness * count * height;i++){
//			#if UNITY_EDITOR
//			Debug.Log("ed");
//			MakeSingleBrick(i);
//			#else
//			Debug.Log("target:"+i);
			targetPositions.Add(i);
//			#endif
			

		}

		SetCreatedNumbersToTarget(targetPositions);
			
//		}
	}

	public override void OnWallCompleted(){
		base.OnWallCompleted();
		foreach(KeyValuePair<int,GameObject> kvp in createdNumbers){
			GameObject b = kvp.Value;
			if (nonKinematic) b.GetComponent<Rigidbody>().isKinematic = false;
			else {
				BoxCollider bc = b.GetComponent<BoxCollider>() as BoxCollider;
				bc.size = Vector3.one * 1.1f;
			}
		}
	}

	public override int GetStartingBlocksCount(){
		int count = (int)(degreesToComplete * radius / brickScale / 60);
		int ret = thickness * count * height;
		// commented Debug.Log("start blocksc count on:"+name+"  was:"+ret);

		return ret;
	}

	public static void CreateRollableWall(
		Vector3 startPos,
		float radius, 
		int degreesToComplete, 
		float brickScale,
		int thickness, 
		float height, 
		Fraction frac,
		Transform brickParent
	){


		int count = 36;

		GameObject previousPiece=null;
		GameObject finalPiece=null;
		GameObject firstPiece=null;


		bool isFinalPiece;


		count = (int)(degreesToComplete * radius / brickScale / 60);

		bool pickup=false;
		Quaternion rot = Quaternion.identity;
		NumberShape shape = NumberShape.Cube;

		float arcLength = degreesToComplete / (float)count;
		GameObject wallBrick;
		for (int h=0; h<thickness; h++){
			isFinalPiece=false;

			for (int i=0;i<count;i++){

				float xPos = Mathf.Sin(Mathf.Deg2Rad*i*arcLength)*(radius+h*brickScale); // x and y calculated with "trigonometry"
				float yPos = Mathf.Cos(Mathf.Deg2Rad*i*arcLength)*(radius+h*brickScale);
				for (int j=0;j<height;j++){

					Vector3 pos = startPos + brickParent.TransformDirection(new Vector3(xPos,j*brickScale,yPos));
					wallBrick = NumberManager.inst.CreateNumber(
						frac,
						pos,
						shape);
					wallBrick.transform.rotation = rot;
//					createdNumbers.Add(pos,wallBrick);

					wallBrick.AddComponent<MonsterAIRevertNumber>();

					wallBrick.transform.localScale = new Vector3(brickScale,brickScale,brickScale);
					wallBrick.transform.parent=brickParent.transform;
					Quaternion newRot = new Quaternion();
					newRot.eulerAngles += new Vector3(0,i*arcLength,0);
					wallBrick.transform.localRotation = newRot;

					NumberInfo ni = wallBrick.GetComponent<NumberInfo>();
					//					ni.comb = numberCombineability;
					//					ni.isStable = true;


					wallBrick.AddComponent<FixedJoint>();
					wallBrick.GetComponent<Rigidbody>().isKinematic=false;
					if (previousPiece != null){
						wallBrick.GetComponent<FixedJoint>().connectedBody = previousPiece.GetComponent<Rigidbody>();
					} else firstPiece = wallBrick;

					previousPiece = wallBrick;

					// Was it final piece? (need to connect first piece with final piece for continuity, to "close the circle".)
					if (j==height-1 && i==count-1){
						finalPiece = wallBrick;
						isFinalPiece=true;
					}
				}

				if (isFinalPiece){
					firstPiece.GetComponent<FixedJoint>().connectedBody = finalPiece.GetComponent<Rigidbody>();
				}
			}

		}

//		return parent; // the entire ring should be returned as children of this single gameobject.

	}
}
