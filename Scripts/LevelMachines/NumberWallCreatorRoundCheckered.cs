using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberWallCreatorRoundCheckered : NumberWallCreatorRound {




	// so not DRY
	// o wells
	
	public Fraction frac2 = new Fraction(-1,1);
	public bool regrow = true;
	int maxWallSize=0;
//		Dictionary<int,GameObject> wall = new Dictionary<int,GameObject>();
	
//	public override GameObject MakeSingleBrick (Vector3 pos, Fraction f, GameObject wallBrick = null ) {
////		Fraction fractionToUse = frac;
////		if (i%2==0) {
////			fractionToUse=frac2;
////		}
//		bool pickup=false;
//		Quaternion rot = Quaternion.identity;
//		NumberShape shape = NumberShape.Cube;		
//		// index -> xyz
//		
//
//		bool needSetNum = false;
//		if (wallBrick != null){
//			wallBrick.transform.position = pos;
//			needSetNum = true;
//		} else {
//			wallBrick = NumberManager.inst.CreateNumber(
//			f,
//			pos,
//			shape);
//		}
//		wallBrick.transform.rotation = rot;
//		createdNumbers.Add(pos,wallBrick);
//
//		if (createdNumbers.Count>maxWallSize) maxWallSize=createdNumbers.Count;
//
//		if(autoRevert){
//			wallBrick.AddComponent<MonsterAIRevertNumber>();
//		}
//		
//		wallBrick.transform.parent=transform;
//		wallBrick.transform.localScale = new Vector3(brickScale,brickScale,brickScale) + fixZFighting;
//		wallBrick.transform.localRotation =GetRotationFromIndex(i);
//		NumberInfo ni = wallBrick.GetComponent<NumberInfo>();
//		if (needSetNum) ni.SetNumber(frac);
////		
////		NumberInfoDefaults nid = new NumberInfoDefaults(ni, ni.fraction, i);
////		if(numbers.Count <= i) {
////			numbers.Add(nid);
////		}
////		else {
////			numbers[i] = nid;
////		}
////		
//		
////		if (!wall.ContainsKey(i)) {
////
////
////			wall.Add(i,wallBrick);
////		}
//
//		return wallBrick;
//	}

	public override Fraction GetFractionFromIndex(int i){
		if (i%2==0) return frac2;
		else return frac;
	}

		
	float regrowTimer=.5f;
	public override void Update(){
		base.Update();
//		if(Network.isClient) { return; }
		if (regrow){
			if (createdNumbers.Count >= maxWallSize) { return; }
			
			regrowTimer-=Time.deltaTime;
//			if (transform.childCount < wallX*wallY*wallZ){ // after all bricks created,
				//			if (true){
				if (regrowTimer < 0){
					regrowTimer=1f;
					
					StartCoroutine(RebuildWall());
					
				}
//			}
			
		}
	}
		
		
	IEnumerator RebuildWall(){
		List<int> replacementList = new List<int>();

//		// commented Debug.Log("attempting rebuild on "+name);
		yield return new WaitForSeconds(2.5f); // should be the same time as the "revert number" is set to for maximum effect


		// Check for any missing bricks
		// if a brick is missing, check its neighbors; 
			// if >1 neighbor is not missing, or its at the edge, rebuild this brick.
		foreach(KeyValuePair<int, GameObject> w in wall){

			// check above is +1, check below is -1, -height, +height
			if (w.Value==null){ // this brick was missing!
				int neighbors = 0;

				// Check below.
				if (w.Key % height==0){ // Was this brick on bottom?
//					// commented Debug.Log("bottom brick detected as missing");
					neighbors++; // bottom bricks automatically act as if floor is a "neighbor"
				} else { // not on bottom; check one directly below it
					if ( wall[(w.Key-1)] != null){
						neighbors++; // brick directly below this one was still there.
					}
				}



				// Check above.
				if ((w.Key+1)%height==0){ // Was this brick on top?
//					// commented Debug.Log("top brick detected as missing");
					neighbors++; // top bricks act as if they have a neighbor above them.
				} else {
					if (wall[(w.Key+1)%maxWallSize] != null){
						neighbors++; // brick above was there
					}
				}

				// check left.
				if (wall[(w.Key+height)%maxWallSize] != null){
					neighbors++;
				}

				// check right. 
				// Edge case for n < height.
				if (w.Key<height){
					int index=w.Key-height+maxWallSize-1;
//					// commented Debug.Log("indeX: "+index+", w.key:"+w.Key);
					if (wall.Count > index && wall[index] != null){
						neighbors++;
					}
				}else if (wall[(w.Key-height)%maxWallSize] != null){ 
					neighbors++;
				}

				if (neighbors>3){
					replacementList.Add(w.Key);
				}
			}
		}

		int j=0;
		foreach(int i in replacementList){
			yield return new WaitForSeconds(.1f);
			GameObject replacement = MakeSingleBrick(i);
			wall[i] = replacement;

			j++; // for sound. DO NOT USE
		}
	

	}





	public override void Start(){
		base.Start();
	}



	override public int NumberFromIndex(int i) {
//		return numberToCreate;
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
		Vector3 pos = transform.position + transform.TransformDirection(new Vector3(xPos,j*brickScale,yPos));
		return pos;
	}
	
	public override Quaternion GetRotationFromIndex (int i)
	{
		int count = (int)(degreesToComplete * radius / brickScale / 60);
		float arcLength = degreesToComplete / (float)count;
		
		int t = (i / height) % count;
		return Quaternion.AngleAxis(t*arcLength, Vector3.up);
	}


	

}
