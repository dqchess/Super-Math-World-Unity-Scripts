// *********** //

// LATTICE
// Behavior:
// It has a bunch of numbers. If you throw a number near it (check every x frames) it tries to dissect that number.
//  	e.g. if lattice is made of 2s and you throw a 2, a single lattice number will laser the 2 and destroy it.
// 		e.g. if the lattice is made of 2s and you throw a 18 at it, 9 of the lattice 2s will zap the 18 and split it up.
// 		e.g. if you throw a 3 at a 2 lattice ,it cannot be split. The 3 glows red and if it touches the lattice on the other side or destroys it somehow.


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LatticeNum {
	public NumberInfo ni;
	public Vector3 pos;
	public List<Transform> neighbors = new List<Transform>();
	public Color col;
	public bool acting = false;
	public float actingTimer = 0f;
}

public class LevelMachine_Lattice : MonoBehaviour {

	float secondsToFreezePerAct = 4f;

	[SerializeField] public LatticeNum[,] latticeNums = new LatticeNum[0,0]; //List<LatticeNum>();
	Dictionary<NumberInfo,Vector3> splitDirDict = new Dictionary<NumberInfo,Vector3>();
	public Fraction frac;
	public Renderer boundsRenderer;
	public Transform origin; // the corner;
//	List<NumberInfo> affectedNumbers = new List<NumberInfo>();
//	List<NumberInfo> latticeNumsList = new List<NumberInfo>();// gah
	// Use this for initialization
	void Start () {

//		CreateLattice();

//		PopulateLatticeNeighbors();

	}

//	void PopulateLatticeNeighbors(){
//		for (int i=0; i<latticeNums.Count; i++) {
//
//		}
//	}
	
	// Update is called once per frame
	int frameSkip = 4;
	int frame = 0;
	void Update () {
		return;
//		DrawFX();
		UpdateActing();
		frame ++;
		if (frame < frameSkip) return;
		frame = 0;
//		CheckChildren();
//		foreach(KeyValuePair<NumberInfo,Vector3> kvp in latticeNums){
//
//		}

	}

	void UpdateActing(){
		// Release acting number so it can ACT AGAIN

		for(int i=0;i<latticeNums.GetLength (0);i++){ // Go each row
			// Each col go by 2s, always start from 2nd column in.
			for (int j=0;j<latticeNums.GetLength (1); j++){
				int oddRow = 0;
				if (i % 2 == 1) {
					oddRow = 1;
					if (j == latticeNums.GetLength(1) - 1) continue; // skip because of staggering fx
				}
				if (latticeNums[i,j].acting){
					latticeNums[i,j].actingTimer -= Time.deltaTime;
					if (latticeNums[i,j].actingTimer < 0){
						latticeNums[i,j].acting = false;
					}
				}
				
				
			}
		}
	}

	void CheckChildren(){
		Collider[] cols = Physics.OverlapSphere(transform.position,boundsRenderer.transform.localScale.x);
		foreach(Collider col in cols){
			NumberInfo ni = col.GetComponent<NumberInfo>();
			OnTriggerEnter(col);
//			if (ni && ){
//				CheckNumber(ni);
//		    }
		}
	}

	void OnRenderObject(){
//		DrawFX();
	}


	void CheckFactors(NumberInfo ni){
		return;
//		Fraction total = new Fraction(0,1);


		// We want to randomize WHICH lattice number is checked first, so first put them all in a list.

		for(int i=0;i<latticeNums.GetLength (0);i++){ // Go each row
			// Each col go by 2s, always start from 2nd column in.
			for (int j=0;j<latticeNums.GetLength (1); j++){
				int oddRow = 0;
				if (i % 2 == 1) {
					oddRow = 1;
					if (j == latticeNums.GetLength(1) - 1) continue; // skip because of staggering fx
				}
				NumberInfo ni2 = latticeNums[i,j].ni;
				if (Fraction.Equals(Fraction.GetAbsoluteValue(ni2.fraction),Fraction.GetAbsoluteValue(ni.fraction)) && latticeNums[i,j].acting == false && ni.GetComponent<Rigidbody>().useGravity == true){
//					affectedNumbers.Add (ni);
					SMW_GF.inst.DrawLineFromTo(ni2.transform,ni.transform,Color.blue);
					FreezeAndDestroy(ni);
					latticeNums[i,j].acting = true;
					latticeNums[i,j].actingTimer = secondsToFreezePerAct;
					break;
				}
				if (Fraction.AIsFactorOfB(ni2.fraction,ni.fraction) && latticeNums[i,j].acting == false && ni.GetComponent<Rigidbody>().useGravity == true){
//					affectedNumbers.Add (ni);
					// Freeze, split, and draw lines to number
					SMW_GF.inst.DrawLineFromTo(ni2.transform,ni.transform,Color.blue);
					SplitNumber(ni,ni2);
					latticeNums[i,j].acting = true;
					latticeNums[i,j].actingTimer = secondsToFreezePerAct;
					break;
				}
			}
		}




	}

	void FreezeAndDestroy(NumberInfo ni){
		ni.GetComponent<Rigidbody>().useGravity = false;
//		ni.GetComponent<Rigidbody>().isKinematic = true;
		ni.GetComponent<Collider>().enabled = false;
		ni.GetComponent<Rigidbody>().drag = 10.5f;

		StartCoroutine (NumberGlowRedAndDie(ni));
	}

	IEnumerator NumberGlowRedAndDie(NumberInfo ni){

		float glowTime = Random.Range (1.5f,2f);
		float t = 0;
		float redSpeed = 1;
		List<Renderer> rs = new List<Renderer>();
		foreach(Renderer rr in ni.GetComponentsInChildren<Renderer>()){
			if (rr.material.HasProperty("_Color")) rs.Add (rr);
		}
		while (t < glowTime){
			t += Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
			foreach(Renderer r in rs){
				if (r) r.material.color = Color.Lerp(r.material.color,Color.red,Time.deltaTime * redSpeed);
			}
		}
//		if (ni) ni.gameObject.AddComponent<ShrinkAndDisappear>();
//		Destroy (ni.gameObject);
	}
	void SplitNumber(NumberInfo ni, NumberInfo fin){
		StartCoroutine(SplitNumberE(ni,fin));
	}

	IEnumerator SplitNumberE(NumberInfo ni, NumberInfo fin){
		ni.GetComponent<Rigidbody>().useGravity = false;
//		ni.GetComponent<Rigidbody>().isKinematic = true;

		ni.GetComponent<Rigidbody>().drag = 1.5f;
		float t=0;
		float floatTime = 0.5f;
		float floatSpeed = 4f;
		float minDistToGround = 10f;
		while (t < floatTime){
			t += Time.deltaTime;
			if (!ni) yield break;
			if (Physics.Raycast(ni.transform.position,Vector3.down,minDistToGround,~LayerMask.NameToLayer("Terrain"))){
				ni.transform.position += Vector3.up * Time.deltaTime * floatSpeed;
//				// commented Debug.Log ("up!");
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}
		ni.GetComponent<Collider>().enabled = false;

		Destroy (ni.gameObject);
		t = 0;
		float splitTime = 1.2f;
		float splitSpeed = 1.3f;
//		float splitForce = 50;
		int splitQty = fin.fraction.numerator; //Fraction.Divide(ni.fraction,f).numerator;
		Dictionary<GameObject,Vector3> splitNums = new Dictionary<GameObject,Vector3>();
		Vector3 newSplitDir = transform.right;
		if (splitDirDict.ContainsKey(ni) && splitDirDict[ni] == transform.right){
			newSplitDir = transform.up;
		}
		for(int i=0;i<splitQty;i++){
			if (!ni) yield break;
			GameObject newNum  = NumberManager.inst.CreateNumber(Fraction.Divide(ni.fraction,fin.fraction),ni.transform.position);
			SMW_GF.inst.DrawLineFromTo(fin.transform,ni.transform,Color.yellow);
			newNum.transform.localScale = Vector3.one * 3;
			newNum.GetComponent<Rigidbody>().useGravity = false;
//			newNum.GetComponent<Rigidbody>().isKinematic = true;
			newNum.GetComponent<Rigidbody>().drag = 2.5f;
			newNum.GetComponent<Collider>().enabled = false;
			newNum.transform.parent = transform;
			Destroy (newNum.GetComponent<PickUppableObject>());
			splitNums.Add (newNum,Quaternion.AngleAxis(360f/splitQty * i,transform.forward) * newSplitDir);
			splitDirDict.Add (newNum.GetComponent<NumberInfo>(),newSplitDir);
		}
		bool colsFlipped = false;
		while (t < splitTime){
			t += Time.deltaTime;
			foreach(KeyValuePair<GameObject,Vector3> kvp in splitNums){
				if (kvp.Key != null){
					if (Physics.Raycast(kvp.Key.transform.position,Vector3.down,minDistToGround,LayerMask.NameToLayer("Terrain"))){
						kvp.Key.transform.position += Vector3.up * Time.deltaTime * floatSpeed;
//						// commented Debug.Log ("up2!");
					}
					kvp.Key.transform.position += kvp.Value * Time.deltaTime * splitSpeed;
					if (t > splitTime / 3f && !colsFlipped) {
						colsFlipped = true;
						kvp.Key.GetComponent<Collider>().enabled = true;

					}
	
//					kvp.Key.GetComponent<Rigidbody>().velocity = kvp.Value * Time.deltaTime * splitSpeed;
				}
			}
			yield return new WaitForSeconds(Time.deltaTime);
		}



		foreach(KeyValuePair<GameObject,Vector3> kvp in splitNums){
			if (kvp.Key){
				kvp.Key.GetComponent<Rigidbody>().useGravity = true;
//				kvp.Key.GetComponent<Rigidbody>().isKinematic = false;
				kvp.Key.GetComponent<Collider>().enabled = true;
				kvp.Key.AddComponent<PickUppableObject>();
//				StartCoroutine(ReduceDragAfterSeconds(kvp.Key.GetComponent<Rigidbody>(),2f));
			}
		}



		ClearDeadSplitDirDict();
//		ClearAffectedNumbers();

	}

//	IEnumerator ReduceDragAfterSeconds(Rigidbody r, float s){
//		yield return new WaitForSeconds(s);
//		if (r) r.drag = 0.3f;
//	}

	void ClearDeadSplitDirDict(){
		Dictionary<NumberInfo,Vector3> newDict = new  Dictionary<NumberInfo,Vector3>();
		foreach(KeyValuePair<NumberInfo,Vector3> kvp in splitDirDict){
			if (kvp.Key != null){
				newDict.Add(kvp.Key,kvp.Value);
			}
		}
		splitDirDict = newDict;
	}

//	void ClearAffectedNumbers(){ // TODO: MathUtils or ListUtils to clear null from list.
//		List<NumberInfo> newList = new List<NumberInfo>();
//		foreach(NumberInfo ni in affectedNumbers){
//			if (ni != null){
//				newList.Add(ni);
//			}
//		}
//		affectedNumbers = newList;
//	}

	void DrawFX(){
//		// commented Debug.Log ("lattice num lenght o:"+latticeNums.GetLength (0));
		for(int i=0;i<latticeNums.GetLength (0);i++){ // Go each row
			// Each col go by 2s, always start from 2nd column in.
			for (int j=1;j<latticeNums.GetLength (1); j++){
				int oddRow = 0;
				if (i % 2 == 1) {
					oddRow = 1;
					if (j == latticeNums.GetLength(1) - 1) continue; // skip because of staggering fx
				}
				// We were not on the top row. Draw a tri between every 2 in this row and the 1 right above it.
				int offsetCount = 1;
				if (i < latticeNums.GetLength(0) - 1){
					Vector3[] pos = new Vector3[] { 
						latticeNums[i,j-1].pos, 
						latticeNums[i,j].pos, 
						latticeNums[i+1,j-1+oddRow].pos
					};
					GLTriangles.DrawTriangle(pos,latticeNums[i,j].col);
				}

				// We were not on the bottom row. Draw a tri between every 2 in this row and the 1 right below it.
				if (i > 0){
					Vector3[] pos = new Vector3[] { 
						latticeNums[i,j-1].pos, 
						latticeNums[i,j].pos, 
						latticeNums[i-1,j-1+oddRow].pos
					};
					GLTriangles.DrawTriangle(pos,latticeNums[i,j-1].col);
				}

			}
		}
	}

//	void DrawLaser(){
//		foreach(LatticeNum
//	}

	public void CreateLattice(){
		while(GetComponentsInChildren<NumberInfo>().Length > 0){

#if UNITY_EDITOR
			DestroyImmediate(GetComponentInChildren<NumberInfo>().gameObject);
#else
			Destroy(GetComponentInChildren<NumberInfo>().gameObject);
#endif
		}



		// OK, we want to "fill" the bounds with numbers spaced out accordingly, AND we want to connect numbers that are close to each other within these bounds so we can make linerenderer FX between them. 
		// We also want to have "wobbly/shimmer" behavior, a wave that ripples through the wall because it will look super cool.
		float spacing = 10;
		float boundsY = boundsRenderer.transform.localScale.x;
		float boundsX = boundsRenderer.transform.localScale.y; // Why are these x y reversed..
		int xMax = Mathf.FloorToInt(boundsX/spacing);
		int yMax = Mathf.FloorToInt(boundsY/spacing);
		latticeNums = new LatticeNum[xMax,yMax];
//		// commented Debug.Log ("init lattice nums with; "+xMax+" , " +yMax);

		// The rows are staggered, but the bottom row should fill the most (so second row would be N-1, bottom row would be N numbers)
		for(int i=0; i<xMax; i++){
			for (int j=0; j<yMax; j++){
				float rightOffset = 0;
				if (i % 2 == 1){
					rightOffset = spacing / 2f; // for staggering every other row
					if ( (j+1) * spacing + rightOffset > boundsY) continue; // skip last one on staggered rows
//					else // commented Debug.Log ("j * spacing + rightOffset > boundsy:"+j+" * "+spacing+" + "+rightOffset+" > "+boundsY);
				}
				Vector3 pos = origin.position + transform.right * (j * spacing + rightOffset + spacing/2f) + transform.up * (i * spacing + spacing/2f);
				GameObject ln = NumberManager.inst.CreateNumber(frac,pos);
				ln.GetComponent<Rigidbody>().useGravity = false;
				ln.GetComponent<Rigidbody>().isKinematic = true;
//				Destroy(ln.GetComponent<Collider>()); //.enabled = false;
				ln.GetComponent<Collider>().enabled = false;
//				// commented Debug.Log ("ln col:"+ln.GetComponent<Collider>());
				ln.transform.parent = transform;
				ln.transform.localScale = Vector3.one * 3;
				ln.name = "Lattice num:" + i + " , " + j;
				LatticeNum latNum = new LatticeNum();
				latNum.pos = pos;
				latNum.ni = ln.GetComponent<NumberInfo>();
				latNum.col = new Color(Random.Range (.0f,.1f),Random.Range (.0f,.1f),Random.Range (.5f,.8f),Random.Range (.4f,.6f));
//				latticeNumsList.Add (latNum.ni);
//				// Since neigbors are unidirectional we don't need to do neighbors for old lattice nums, only the fresh ones (e..g nowtime);
//				if (j > 0) {
//					foreach(LatticeNum otherLat in latticeNums){
//						if (otherLat.xy.x == i) latNum.neighbors.Add (otherLat); // Same row
//						else if (otherLat.xy.x == i - 1){ // One row down
//							if(otherLat.xy.y == j) latNum.neighbors.Add (otherLat); // same column
//							else if(otherLat.xy.y == j - 1) latNum.neighbors.Add (otherLat); // same column one over
//						}
//					}
//				}
//				// commented Debug.Log ("lattice nums i,j: "+i+","+j);
				latticeNums[i,j] = latNum;

			}
		}


	}

	void CheckNumber(NumberInfo ni){
//		// commented Debug.Log ("checking number:"+ni);
		if (ni && ni.fraction.denominator == 1) {
//			// commented Debug.Log ("checking factor:"+ni);
			CheckFactors(ni);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Rigidbody>() && other.GetComponent<Rigidbody>().useGravity == false) return;
		NumberInfo ni = other.GetComponent<NumberInfo>();
		CheckNumber(ni);

	}
}
