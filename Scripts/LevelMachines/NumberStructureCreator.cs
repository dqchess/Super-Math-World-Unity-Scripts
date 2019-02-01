using UnityEngine;
using System.Collections;
using System.Collections.Generic;




#if UNITY_EDITOR_OSX || UNITY_EDITOR
//[ExecuteInEditMode]
#endif

public class NumberStructureCreator : UserEditableObject
{


	#region UserEditable 
	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();
		return N;
	}

	/* footpring was: (){
		return 1;
	 */

	public override void OnLevelBuilderObjectCreated(){ // from editor only. Before placement happens.
		base.OnLevelBuilderObjectCreated();
//		// commented Debug.Log("object created!");
		CreateWall();
		SetFraction(frac);
	}

	public override void OnGameStarted(){
		base.OnGameStarted();
//		SetStartingBlocksCount();
	}

	// upoffset 	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		// this parent class doesn't implement, pas through to children
	}
	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[0];
	}

	virtual public void SetFraction(Fraction f){
		frac = f;
		frac = Fraction.ReduceFully(f);
		foreach(KeyValuePair<int,GameObject> kvp in createdNumbers){
			if (kvp.Value){
				Fraction ff = GetFractionFromIndex(kvp.Key);
				MonsterAIRevertNumber mairn = kvp.Value.GetComponent<MonsterAIRevertNumber>();
				if (mairn) mairn.SetNumber(ff);
				kvp.Value.GetComponent<NumberInfo>().SetNumber(ff);
			}
		}
		SetSkyCamText();
	}

	virtual public void SetSkyCamText(){
		if (skyCamText && skyCamTextParent) {
			if (Fraction.ReduceFully(frac).denominator == 1){
				skyCamText.Text = Fraction.ReduceFully(frac).numerator.ToString();	
			} else {
				skyCamText.Text = Fraction.ReduceFully(frac).ToString();
			}
			skyCamText.Color = frac.numerator > 0 ? Color.black : Color.white;
			skyCamTextParent.GetComponent<Renderer>().material.color = frac.numerator > 0 ? new Color(1,1,1,0.2f) : new Color(0,0,0,0.2f);


			// reposition height of skycam indicator text for round and square number walls, so bricks do not overlap the indicator text.
			// for other walls such a spheres this is not needed.
			if (this.GetType() == typeof(NumberWallCreatorSquare)) {
				NumberWallCreatorSquare nwcs = GetComponent<NumberWallCreatorSquare>();
				skyCamTextParent.localPosition = new Vector3(0,nwcs.wallY*brickScale+1,0);
			} else if (this.GetType() == typeof(NumberWallCreatorRound)){
				NumberWallCreatorRound nwr = GetComponent<NumberWallCreatorRound>();
				skyCamTextParent.localPosition = new Vector3(0,nwr.height*brickScale+1,0);
//				// commented Debug.Log("sky cam text parent:"+skyCamTextParent.localPosition.y);
			}

		}
	}

	#endregion
	public CCText skyCamText;
	public Transform skyCamTextParent;
	public float upoffset = 1.5f; // TODO: This hardcoded value should actually be HALF of whatever number scale these bricks have.
	public float brickInterval = .01f;
	public bool nonKinematic = false;
	public bool hideGizmos=false;	 
//	protected bool destroyChildren=true;
	protected bool createOnStart=true;	
	public Fraction frac = new Fraction(1,1);

	Fraction fracSeq = new Fraction(1,1);
	public Fraction fracSeqStepA = new Fraction(0,1);
	public Fraction fracSeqStepB = new Fraction(0,1);
	public int maxFracSeqSteps = 0;
	bool growInSeconds=false;
	
	public Dictionary<int,GameObject> createdNumbers = new Dictionary<int,GameObject>();
	public Dictionary<int,GameObject> ephemeralNumbers = new Dictionary<int,GameObject>();
	public List<int> createdNumbersTarget = new List<int>();
	public float brickScale = 3;

//	float autoResetTimer = 5.0f;
	
	public bool leaveZeroes = false;
	public bool autoRevert = true;

	[System.NonSerialized] public Vector3 fixZFighting = new Vector3(-0.001f, -0.001f, -0.001f);
	
	protected List<NumberInfoDefaults> numbers = new List<NumberInfoDefaults>();
	protected List<NumberInfoDefaults> numbersToReset = new List<NumberInfoDefaults>();
	[System.NonSerialized] public bool percents = false;
	[System.NonSerialized] public Dictionary<int,GameObject> wall = new Dictionary<int,GameObject>();

	public virtual void Reset(){}
	
	public struct NumberInfoDefaults {
		public NumberInfo ni;
		public Fraction defaultFraction;
		public int index;
		public NumberInfoDefaults(NumberInfo _ni, Fraction _df, int i) {
			ni = _ni;
			defaultFraction = _df;
			index = i;
		}
	}
	virtual public void Start(){
//		DestroyChilds();
		fracSeq = frac;

	}

	public int textSkipMod = 0;
	virtual public void CreateWall() {

		SetTextSkipMod();

	}

	void SetTextSkipMod(){
		textSkipMod = 0; // fuck it
		return;
		if (this.GetType() == typeof(NumberWallCreatorSquare)){
			NumberWallCreatorSquare sq = GetComponent<NumberWallCreatorSquare>();
			if (sq.wallX * sq.wallY * sq.wallZ > 20){ // only skipmod if more than 20 bricks
				if (sq.wallZ % 5 == 0 || sq.wallX % 5 == 0 || sq.wallY % 5 == 0) textSkipMod = 4; // 
				else textSkipMod = 5;
//				else  textSkipMod = 4;
				//				if (sq.wallZ % 3 == 0) textSkipMod = 4;
				// if z is 9 or 6, skip every 4 blocks
				// if z is 
			}
		} else if (this.GetType() == typeof(NumberWallCreatorRound)){
			NumberWallCreatorRound ro = GetComponent<NumberWallCreatorRound>();
			int count = (int)(ro.degreesToComplete * ro.radius / brickScale / 60);
			if (count * ro.height > 20) {
				if (count % 5 != 0) textSkipMod = 5;
				else textSkipMod = 4;
			}
//			if (sq.wallX * sq.wallY * sq.wallZ > 100){ // only skipmod if more than 100 bricks
//				if (sq.wallZ % 2 == 0) textSkipMod = 3; // 
//				else  textSkipMod = 4;
				//				if (sq.wallZ % 3 == 0) textSkipMod = 4;
				// if z is 9 or 6, skip every 4 blocks
				// if z is 
//			}
		} else { 
			textSkipMod = 4;
		}
	}





	public virtual void DestroyAndClearEphemeralNumbers(){
		
		List<GameObject> toDel = new List<GameObject>();
		string adds = "";
		foreach(KeyValuePair<int,GameObject> kvp in ephemeralNumbers){
//			// commented Debug.Log("Destroyed:"+kvp.Value);
			Destroy(kvp.Value);
			adds += kvp.Value.name;
		}
		ephemeralNumbers.Clear();
//		// commented Debug.Log("nums destroyed:"+adds);


	}

	virtual public void OnDrawGizmos(){
		if (hideGizmos) return;
		Gizmos.color=Color.grey;
		Gizmos.DrawSphere(transform.position,1);
		Gizmos.DrawSphere(transform.position+new Vector3(0,2,0),1);
	}
		

	
	public virtual IEnumerator RespawnBricks(){
		yield return new WaitForSeconds(2.5f);
		List<int> bricksToRespawn=new List<int>();
		foreach(KeyValuePair<int,GameObject> w in wall){
			if (w.Value==null){
				bricksToRespawn.Add(w.Key);
			}
		}

		int j=0; // for sound! Do NOT use!
		foreach(int i in bricksToRespawn){
			wall[i] = MakeSingleBrick(i);
			AudioManager.inst.PlayBubblePop(wall[i].transform.position,0.5f+j/20f);
			j++;
		}
	}
	
	


	virtual public Vector3 GetPositionFromIndex(int i) {
		return Vector3.zero;
	}
	
	virtual public Quaternion GetRotationFromIndex(int i) {
		return Quaternion.identity;
	}


	void MoveCreatedNumbersToEphemerals(){
//		string adds = "";
		foreach(KeyValuePair<int,GameObject> kvp in createdNumbers){
			if (ephemeralNumbers.ContainsValue(kvp.Value)) {
				// commented Debug.LogError("ERROR, epheemral and craeted contained same val:"+kvp.Value);
			} 
//			// commented Debug.Log("moved:"+kvp.Value);
			ephemeralNumbers.Add(kvp.Key,kvp.Value);
//			adds += kvp.Key+","; 
		}
		createdNumbers.Clear();
//		// commented Debug.Log("nums added to eph:"+adds);
	}

	bool needTransition = false;
	public void SetCreatedNumbersToTarget(List<int> targetPositions){
		MoveCreatedNumbersToEphemerals(); // created number list should be clear after this
		needTransition = true;
		createdNumbersTarget = targetPositions;
		createdNumberIndex = 0;
	}

	int createdNumberIndex = 0;
	int bricksPerTransition = 10;
	int bricksThisTransition = 0;
	float t = 0;
	virtual public void Update(){
		if (needTransition){
			t -= Time.deltaTime;
			if (t < 0){
				for (int i=0; i<bricksPerTransition; i++){
					if (createdNumberIndex < createdNumbersTarget.Count){
						int curIndex = createdNumbersTarget[createdNumberIndex]; 
						// creatednumber index goes direct from 0 to max and counts the total numbers. 
						// curIndex may skip some as PositionFromIndex(i) is where this will determine the vector3 based on an index i, but because of hollow/other features of walls this does not go from 0 - some number
						// but curindex may skip some instead
						if (ephemeralNumbers.ContainsKey(curIndex)){
	//						// commented Debug.Log("making from eph w index:"+createdNumberIndex+","+curIndex);
							MakeSingleBrick(curIndex,ephemeralNumbers[curIndex]);
							ephemeralNumbers.Remove(curIndex);
						} else {
//							// commented Debug.Log("making from scratch w index:"+createdNumberIndex+","+curIndex);
							MakeSingleBrick(curIndex);
						}
						createdNumberIndex ++;
//						bricksThisTransition ++;
						if (bricksThisTransition == bricksPerTransition){
							bricksThisTransition = 0;
						}
					}
					if (createdNumberIndex == createdNumbersTarget.Count){
						DestroyAndClearEphemeralNumbers();
						DisableSomeOfTheWallBricksTexts();
						OnWallCompleted();
						needTransition = false;
						return;
					}
				}
				t = brickInterval;
			}

		}
	}

	[System.NonSerialized] public bool wallCompleted = false;
	virtual public void OnWallCompleted(){
		wallCompleted = true;
//		// commented Debug.Log("wall complete:"+name);
		SetSkyCamText();
		SetStartingBlocksCount();
	}

	void DisableSomeOfTheWallBricksTexts(){
		foreach(KeyValuePair<int,GameObject> kvp in createdNumbers){
			if (textSkipMod != 0 && kvp.Key % textSkipMod != 0){
//				kvp.Value.GetComponent<NumberInfo>().DisableTexts();
			}
		}
	}

	override public void OnLevelBuilderObjectSelected(){}

	virtual public Fraction GetFractionFromIndex(int i){
//		if (fracSeqStepA.numerator != 0 || fracSeqStepB.numerator != 0) {
//			if (i % 2 == 0) return Fraction.Add(frac,new Fraction(fracSeqStepA.numerator * (i % maxFracSeqSteps),fracSeqStepA.denominator));
//			else if (i % 2 != 0) return Fraction.Add(frac,new Fraction(fracSeqStepB.numerator * (i % maxFracSeqSteps),fracSeqStepB.denominator));
//		}
		return frac;
	}

	virtual public GameObject MakeSingleBrick(int i, GameObject wallBrick = null) { 
		Fraction f = GetFractionFromIndex(i);
		Quaternion rot = GetRotationFromIndex(i);
		Vector3 pos = GetPositionFromIndex(i);
		bool pickup=false;

		NumberShape shape = NumberShape.Cube;		
		// index -> xyz

//		Vector3 pos = PositionFromIndex(i);

		NumberManager nm;


		// // commented Debug.Log("making single brick:"+frac);
		bool numNeedsSet = false;
		if (wallBrick != null) {
			numNeedsSet = true;
			wallBrick.transform.position = pos;

		} else {
			wallBrick = NumberManager.inst.CreateNumber(
			f,
			pos,
			shape);
		}
//		// commented Debug.Log("single brick made:"+wallBrick.name+", frac;"+wallBrick.GetComponent<NumberInfo>().fraction);
		if (!createdNumbers.ContainsKey(i)){
			createdNumbers.Add(i,wallBrick);
//			// commented Debug.Log("added to craeted key:"+pos+", craeted ct;"+createdNumbers.Count);
		} else {
//			// commented Debug.Log("dict key exists!:"+pos+" on :"+name+" for i:"+i);
		}
		NumberInfo ni = wallBrick.GetComponent<NumberInfo>();
		if (numNeedsSet) ni.SetNumber(f);
		if (autoRevert) {
			MonsterAIRevertNumber mairn = wallBrick.GetComponent<MonsterAIRevertNumber>();
			if (!mairn) mairn = wallBrick.AddComponent<MonsterAIRevertNumber>();
		} else {
			MonsterAIRevertNumber mairn = wallBrick.GetComponent<MonsterAIRevertNumber>();
			if (mairn) mairn.enabled = false;
		}

		wallBrick.transform.parent=transform;
		wallBrick.transform.localRotation = rot;
		wallBrick.transform.localScale = new Vector3(brickScale,brickScale,brickScale) + fixZFighting;



		NumberInfoDefaults nid = new NumberInfoDefaults(ni, ni.fraction, i);
		if(numbers.Count <= i) {
			numbers.Add(nid);
		}
		else {
			numbers[i] = nid;
		}

//		// commented Debug.Log("made:"+wallBrick);
		return wallBrick;
	}
	


	





	public void SetStartingBlocksCount(){
		startingBlocksCount = GetComponentsInChildren<NumberInfo>().Length;
//		// commented Debug.Log("starting blox:"+startingBlocksCount);
	}

	int startingBlocksCount;
	virtual public int GetStartingBlocksCount(){
		return startingBlocksCount;
	}

	virtual public int GetBlocksCount(){
		int len = GetComponentsInChildren<NumberInfo>().Length;
//		Debug.Log("count:"+len);
		return len;
	}

	public void DestroyNumbers(){
		// destroy all my pieces.
//		List<GameObject> toDestroy = new List<GameObject>();
		foreach(NumberInfo ni in GetComponentsInChildren<NumberInfo>()){
			NumberManager.inst.DestroyOrPool(ni);
		}
//		foreach(
	}


	
}

