using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NumberWallCreatorCheckered : NumberWallCreatorSquare {

	public Fraction frac2 = new Fraction(-1,1);
	public bool regrow = true;
	Dictionary<int,GameObject> wallPositions = new Dictionary<int,GameObject>();
	public bool debug=false;
	
//	public override GameObject MakeSingleBrick (int i, GameObject wallBrick = null)
//	{
//		
//		bool pickup=false;
//		Quaternion rot = Quaternion.identity;
//		NumberShape shape = NumberShape.Cube;
//
//		// index -> xyz
//		
////		Vector3 pos = PositionFromIndex(i);
//
//
//		if (wallBrick != null){
//			wallBrick.transform.position = pos;
//
//		} else wallBrick = NumberManager.inst.CreateNumber(
//			f,
//			pos);
//
//
//		createdNumbers.Add(pos,wallBrick);
//		wallBrick.name="hi";
////		// commented Debug.Log("hi");
//		if(autoRevert){
//			wallBrick.AddComponent<MonsterAIRevertNumber>();
//		}
//		
//		wallBrick.transform.parent=transform;
//		wallBrick.transform.localScale = new Vector3(brickScale,brickScale,brickScale) + fixZFighting;
//		wallBrick.transform.localRotation =GetRotationFromIndex(i);
//		NumberInfo ni = wallBrick.GetComponent<NumberInfo>();
//		ni.SetNumber(f);
////		ni.comb = Combineability.allOverride; // hardcoded
////		ni.isStable = true;
////		if (true)
//		
//		NumberInfoDefaults nid = new NumberInfoDefaults(ni, ni.fraction, i);
//		if(numbers.Count <= i) {
//			numbers.Add(nid);
//		}
//		else {
//			numbers[i] = nid;
//		}
//
//
//		if (!wallPositions.ContainsKey(i)){
////			// commented Debug.Log ("making :" +i);
//			wallPositions.Add(i,wallBrick);
//		}
//
//		if (wallPositions[i] == null){
//			wallPositions[i] = wallBrick;
//		}
//		
//		return wallBrick;
//	}
	public override Fraction GetFractionFromIndex(int i){
		if (i%2==0) return frac2;
		else return frac;
	}

	float regrowTimer=.5f;
	void Update(){
		if (regrow){
			if (transform.childCount >= createdNumbers.Count) {
//				if (debug) // commented Debug.Log("t chc: "+transform.childCount+"; creatnumcount: "+createdNumbers.Count);
				return;
			}

			regrowTimer-=Time.deltaTime;
			if (transform.childCount < wallX*wallY*wallZ){ // after all bricks created,
				if (regrowTimer < 0){
					regrowTimer=1f;
					StopCoroutine("RebuildWall");
					StartCoroutine(RebuildWall());
				}
			} 

        }
    }

	List<int> GetMissingBricks (){
		List<int> wposes = new List<int>();
		foreach(KeyValuePair<int,GameObject> kvp in wallPositions) {
//			yield return new WaitForSeconds(.01f);
			int wpos = kvp.Key;
			int neighbors=0;
			
			if (kvp.Value == null){
				//				// commented Debug.Log("one "+wpos+"was null");
				// check bottom
				if (wpos%wallY==0) neighbors++; // bottom piece uses ground as neighbor
				else if (wallPositions[wpos-1] != null) neighbors++;
				
				// check top
				if ((wpos+1)%wallY == 0) neighbors++; // top wall piece uses air as neighbor
				else if (wallPositions[wpos+1] != null) neighbors++;
				
				// check left
				if (wpos-wallY < 0) neighbors++; // edge
				else if (wallPositions[wpos-wallY] != null) neighbors++;
				
				// check right..
				if (wpos+wallY > (wallY*wallX -1)) neighbors++; // edge
				else if (wallPositions[wpos+wallY] != null) neighbors++;
				
				if (neighbors>3) {
					wposes.Add(wpos);
					
				} else {
					//					// commented Debug.Log("not 3 neighbos");
				}
			}
			
		}
		return wposes;
	}


	IEnumerator RebuildWall(){
		yield return new WaitForSeconds(3.5f);
		List<int> wposes = GetMissingBricks();
		foreach (int i in wposes){
			yield return new WaitForSeconds(.1f);
			MakeSingleBrick(i);
			float pitch = 0.5f + (float)i/wposes.Count/2f;
			AudioManager.inst.PlayBubblePop(wallPositions[i].transform.position,pitch);
		}
//		// commented Debug.Log ("and here");
	}


}
