
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Combineability{
	allOverride,
	all,
	inverseOnly,
	none
}

//[System.Serializable]
//public class NumberTextUI {
//	public Text integer;
//	public Text numerator;
//	public Text line;
//	public Text denominator;
//	public void SetText(Fraction f){
//		if (Mathf.Abs(f.denominator) == 1){
//			integer.gameObject.SetActive(true); 
//			integer.text = f.numerator.ToString();
//			numerator.gameObject.SetActive(false);// numerator.text = "";
//			line.gameObject.SetActive(false); //line.text = "";
//			denominator.gameObject.SetActive(false); //denominator.text = "";
//		} else {
//			integer.gameObject.SetActive(false); //integer.text = "";
//			numerator.gameObject.SetActive(true); 
//			line.gameObject.SetActive(true);
//			denominator.gameObject.SetActive(true);
//			numerator.text = f.numerator.ToString();
//			line.text ="—";
//			denominator.text = f.denominator.ToString();
//		}
//	}
//}

[System.Serializable]
public class NumberText {
	public CCText integer;
	public CCText numerator;
	public CCText line;
	public CCText denominator;
//	public void SetText(Fraction f){
//		if (Mathf.Abs(f.denominator) == 1){
//			integer.gameObject.SetActive(true); 
//			integer.Text = f.numerator.ToString();
//			numerator.gameObject.SetActive(false);// numerator.Text = "";
//			line.gameObject.SetActive(false); //line.Text = "";
//			denominator.gameObject.SetActive(false); //denominator.Text = "";
//		} else {
//			integer.gameObject.SetActive(false); //integer.Text = "";
//			numerator.gameObject.SetActive(true); 
//			line.gameObject.SetActive(true);
//			denominator.gameObject.SetActive(true);
//			numerator.Text = f.numerator.ToString();
//			line.Text ="-";
//			denominator.Text = f.denominator.ToString();
//		}
//	}
}

public enum CombineTest {
	Greater,
	GreaterOrEqual,
	Equal,
	NotEqual,
	LessOrEqual,
	Less,
	Always,
	None
}

public enum NumberShape {
	Cube,
	Sphere,
	Tetrahedron,
	Schur,// a sphere, but transparent (cauldron puzzle number)
	Ghost, // a sphere ish , but transparent (ghost)
	Face, // a sphere number with eyes
	SimpleHat, // sphereical with pointy hat
	Hat, // a sphere number with eyes and hat
	Arrow, // ammo for the bow gadget.
	WavePercentAmmo, // ammo for the percent wave
}


public class NumberInfo : NumberModifier {


	public bool isPercent = false;
	#region UserEditable 
//	public override bool Exclude () {
//		return true;
//	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMFractionButton);
		return els.ToArray();
	}


	[System.NonSerialized] public bool usedThisFrame = false; // a stop-gap measure to help sheep and machines not eat the same number twice in the same frame.
	public override SimpleJSON.JSONClass GetProperties(){ 
		
		// TODO: could have an inspector that only looks at what properties are not shared class properites
		SimpleJSON.JSONClass N = base.GetProperties(); //new SimpleJSON.JSONClass();
		Fraction fr = fraction;
		MonsterAIRevertNumber mairn = GetComponent<MonsterAIRevertNumber>();
		if (mairn.bNeedsRevert) fr = mairn.GetOriginalFraction();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,fr);
		N[JsonUtil.scaleKey].AsInt = JsonUtil.GetScaleAsInt(transform);

		return N;

	}



	public override void OnGameStarted(){
		if (turnOffKinematicOnStart){
			GetComponent<Rigidbody>().isKinematic = false;
		}
		base.OnGameStarted();

	}


	public override void SetProperties(SimpleJSON.JSONClass N){

		base.SetProperties(N);
		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
		f = Fraction.ReduceFully(f);

		SetNumber(f);

		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */
	}

//	public override void OnLevelBuilderObjectPlaced(){

//		base.OnLevelBuilderObjectPlaced();
//		NumberManager.inst.AddNumberToScene(this);
//	}
	#endregion

	public delegate void EnergyNumberDestroyedDelegate(GameObject source);
	public EnergyNumberDestroyedDelegate energyNumberDestroyedDelegate;
	public delegate void NumberDestroyedDelegate(NumberInfo ni);
	public NumberDestroyedDelegate numberDestroyedDelegate;


	[Header("NumberInfo")] // GUI layout
	public bool turnOffKinematicOnStart = false; // for loose numbers only, because during eidt mode we need them to be kinematic so as to not fall through ground etc while moving them/editing
	public bool neverEats = false;
	public bool destroyIfZero = true;
	public CombineTest combineTest = CombineTest.Always;
	public int combineLayer = 1;
	public int greaterThanCombine = 0;
	public int lessThanCombine = 2;

	public delegate void NumberChanged(Fraction newNum);
	public NumberChanged numberChanged;
	public delegate void NumberChangedDelegate(NumberInfo ni);
	public NumberChangedDelegate numberChangedDelegate;


	public Fraction fraction = new Fraction(1,1);
	public NumberShape myShape = NumberShape.Sphere;


	[System.NonSerialized] public bool growing = false;

	//TODO: Make these class variables so they aren't serialized during JSON save
	[System.NonSerialized] public float growTimer = 0;
	[System.NonSerialized] public float growSpeed = 2.8f;
	[System.NonSerialized] public float growScale = 0.5f;
	public float baseScale = 0.5f;
	[System.NonSerialized] public float origScale=1.1f;

	public bool modifyable = true; // ugh, because some things like AIMonsterBase.NumberTower inherits from NumberInfo and must have a number which can be set by things like Rocket explosions, and if set to 0 they die -- but in reality these should not be numbers at all! They only are numbers because I wanted to share functionality between AIMonsterBase and the way a NumbeRTower chases you and stays in its zone, similar to Spikey. But a Spikey IS a number whereas the number tower isn't. Perhaps I should have muiltiple inheritance or something here but this bool will fix the problem for now.
	public Renderer childMeshRenderer;
	public Renderer extraMesh;
	public Renderer outlineMesh;
	public GameObject rainbowFX;
	public GameObject soapFx;
	public Transform digits;
//	public GameObject blockParent;
	[SerializeField] public NumberText[] texts;
	[System.NonSerialized] public bool destroyedThisFrame = false;
	[System.NonSerialized] public float playerTouchedTime = float.NegativeInfinity;

	public override void Start () {

		base.Start();

		if (childMeshRenderer) baseScale = childMeshRenderer.transform.localScale.x;
//		Debug.Log("basescale ste:"+baseScale);
		InitSinGrowAttributes();

//		NumberManager.inst.AddNumberToScene(this);

	}

	public void OnPlayerCollect(){

	}

	// TODO static class initializer
	// http://csharpindepth.com/Articles/General/Beforefieldinit.aspx

	// static class initializer
	//	public void InitClassAttributes(){
	//		childMeshRenderer = 
	//	}

	bool initiatedGrowAttr = false;
	public bool useLocalScale = true;
	public void InitSinGrowAttributes(float modFactor  = 0.5f, bool force=false){ // modfactor - for when we create an ammo we don't want it to sin grow as much ..
		

		if (initiatedGrowAttr && !force) return; // we are initiating twice, and want one to override. bad code!
//		if (myShape == NumberShape.Cube && modFactor == 0.5f) {
//			modFactor = 1;
//		}
		initiatedGrowAttr = true;
		if (useLocalScale) origScale = transform.localScale.x;
		else origScale = transform.lossyScale.x;

		growScale = 1.5f / origScale * modFactor;


	}

	public void OnPlayerThrow() {

		PlayerTouched();
	}
	public void PlayerTouched(){
		playerTouchedTime = Time.time;
	}

//	bool textsDisabled = false;
//	public void DisableTexts(){
//		textsDisabled = true;
//		digits.gameObject.SetActive(false);
//
//	}
//
	virtual public void UpdateNumberText(){
		if (texts == null) {
			
			return;
		}
		if (isPercent){
			GetTextScale(5);
			string p = (Mathf.RoundToInt(fraction.GetAsPercent()).ToString() + "%");
			foreach(NumberText t in texts){
				t.integer.Text = p;
			}
			return;
		}


		if (fraction.denominator == 1) SetTextInteger();
		else SetTextFraction();
	}

	void SetTextFraction(){
		int sign = fraction.numerator > 0 ? 1 : -1;
		int mixedNumberCoefficient = Mathf.FloorToInt(Mathf.Abs (fraction.GetAsFloat()));

		int numerator = fraction.numerator;
		float leftRightOffset = 0;
		float scale = GetIntegerScale(mixedNumberCoefficient);
		if (mixedNumberCoefficient != 0){
			mixedNumberCoefficient *= sign;
			numerator -= mixedNumberCoefficient * fraction.denominator;
			numerator = Mathf.Abs(numerator);
			scale *= .8f;
			leftRightOffset = -0.2f;
		} else {
//			numerator *= sign;

		}
		if (myShape == NumberShape.Cube) scale *= 3f;
		else scale *= 2.5f;
//		switch(myShape){ 
//		case NumberShape.Cube: scale *= 3f; break;
//		case NumberShape.Sphere:
//			case NumberShape.Face:
//			case NumberShape.Ghost:
//		case NumberShape.Hat:
//			case NumberShape.SimpleHat:
//			case NumberShape.Tetrahedron
//			
//		}
//		if (myShape == NumberShape.Cube) 
//		if (myShape == NumberShape.Sphere) scale *= 2.5f;
//
		foreach(NumberText t in texts){
			if (mixedNumberCoefficient == 0) t.integer.Text = "";
			else t.integer.Text = mixedNumberCoefficient.ToString();

			t.integer.transform.localScale = Vector3.one * scale;

			t.numerator.Text = numerator.ToString();
			t.denominator.Text = fraction.denominator.ToString();
			t.line.Text = "_";

			t.integer.transform.localScale = Vector3.one * scale * .35f;
			t.numerator.transform.localScale = Vector3.one * scale * .2f;
			t.denominator.transform.localScale = Vector3.one * scale * .2f;

			t.integer.transform.localPosition = new Vector3(-leftRightOffset,t.integer.transform.localPosition.y,t.integer.transform.localPosition.z); // asuming parent is positioned correctyl in cubes and spheres. If not, absolute localpos will be wrong
			t.numerator.transform.localPosition = new Vector3(leftRightOffset,t.numerator.transform.localPosition.y,t.numerator.transform.localPosition.z);
			t.line.transform.localPosition =  new Vector3(leftRightOffset,t.line.transform.localPosition.y,t.line.transform.localPosition.z);
			t.denominator.transform.localPosition = new Vector3(leftRightOffset,t.denominator.transform.localPosition.y,t.denominator.transform.localPosition.z);

			t.numerator.gameObject.SetActive(true);
			t.denominator.gameObject.SetActive(true);
			t.line.gameObject.SetActive(true);
		}

	}

	float GetIntegerScale(int integer){
		int digitsLength = integer.ToString().Replace("-","").Length;
		return GetTextScale(digitsLength);
	}

	float GetTextScale(int digitsLength){
		//		if (integer < 0) digitsLength ++;
		float newScale = 1f;
		switch(digitsLength){
		case 0: newScale = 1; break;
		case 1: newScale = 1; break;
		case 2: newScale = 0.8f; break; // special hardcoded sizes based on integer length.
		case 3: newScale = 0.5f; break;
		case 4: newScale = 0.45f; break;
		case 5: newScale = 0.37f; break;
		case 6: newScale = 0.31f; break;
		case 7: newScale = 0.25f; break;
		default: newScale = 0.23f; break;
		}
//		newScale *= .83f;


		return newScale;

	}

	void SetTextInteger(){

		float newScale = GetIntegerScale(fraction.numerator);
		foreach(NumberText t in texts){
			t.integer.Text = fraction.numerator.ToString();

			t.integer.transform.localScale = Vector3.one * newScale;
			t.integer.transform.localPosition = new Vector3(0,t.integer.transform.localPosition.y,t.integer.transform.localPosition.z); 
			if (!t.numerator) {

				name += "no at time ";
			}
			t.numerator.Text = "";
			t.denominator.Text = "";
			t.line.Text = "";
			t.numerator.gameObject.SetActive(false);
			t.denominator.gameObject.SetActive(false);
			t.line.gameObject.SetActive(false);
		}
	}

	public void NotifyDestroyer(GameObject o){
		// Who killed this object?
		destroyer = o;
	}
	GameObject destroyer = null;
	public override void OnDestroy(){
		base.OnDestroy();
		if (SceneManager.inst.sceneState == SceneState.Ready){
	//		Debug.Log("ondest, p:"+transform.position);
			if (energyNumberDestroyedDelegate != null) energyNumberDestroyedDelegate(destroyer);
			if (numberChanged != null) numberChanged(new Fraction(0,1));
			if (numberDestroyedDelegate != null) numberDestroyedDelegate(this);
			foreach(NumberFaucet nf in faucetRels){
				nf.SetOneballRel(null);
			}
		}
		// was this destroy event as a result of player action (not editor delete) ?
		if (GameManager.inst.state == GameState.Playing){
			if (myShape == NumberShape.Cube){
				AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.blocksDestroyed,1);
			} else if (GetComponent<MonsterAIBase>()){
				AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.monstersDestroyed,1);
			} else if (GetComponent<Animal>()){
				AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.animalsDestroyed,1);
			}
		}

	}

	public void DestroyedFX(){ // shrink and disappear

		GameObject dyingNumber = (GameObject)Instantiate(gameObject,transform.position,transform.rotation);
		CleanComponentsFromDyingNumber(dyingNumber);
		dyingNumber.AddComponent<ShrinkAndDisappear>();
	}

	public void ZeroFX(Vector3 pos){ // break and disappear

		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.zerosCreated,1);
		EffectsManager.inst.CreateShards(pos);
		AudioManager.inst.PlayNumberShatter(pos);
		SMW_FX.CreateTextEffect(pos,"zero");
	}
	
	float timer=1;
	

	public void PostGrowFX(){

		growing = false;
		growTimer = 0;
		childMeshRenderer.transform.localScale = new Vector3(baseScale,baseScale,baseScale);
		if (myShape == NumberShape.Cube) digits.transform.localScale = new Vector3(baseScale,baseScale,baseScale);


	}
	
	public virtual void OnCollisionEnter(Collision collision){
		if (forbidCombinations) return;
		if (collision.gameObject.GetInstanceID() == gameObject.GetInstanceID()) return;
		OnTouchedSomething(collision.collider);
	}

//	public Transform excludeRootFromRocketExplosion;
	public void OnTouchedSomething(Collider other){
		NumberInfo ni = other.gameObject.GetComponent<NumberInfo>();
		if (ni && ni.enabled && enabled){
			if (!GetComponent<DoesExplodeOnImpact>()  && !ni.GetComponent<DoesExplodeOnImpact>()){
				AttemptCombine(ni);
			}
		}

	}

	public void SetNumber(Fraction frac, bool sinGrow=false, bool allowRevert=true) {
		frac = Fraction.ReduceFully(frac);

		if (numberChanged != null) {
			numberChanged(frac);
		}

		if (numberChangedDelegate != null){
			numberChangedDelegate(this);
		}
		if (frac == null) {
			
			return;
		}
		if (frac.numerator == 0 && destroyIfZero){

//			DestroyedFX();
			ZeroFX(transform.position);
//			if (myShape == NumberShape.Cube){
			Destroy(gameObject);
			
			GemFX();

			return;
		}
		if (sinGrow) {

			SinGrowOnce();
		}

//		if (!transform) return; //? WTF?
		fraction = frac;
		UpdateNumberText();

		SetColor();

		if (!allowRevert){
			MonsterAIRevertNumber mairn = GetComponent<MonsterAIRevertNumber>();
			if (mairn){
				mairn.SetNumber(frac);
			} else {
				
			}
		} else {
		}
	}

	public void GemFX(){
		if (!LevelBuilder.inst.levelBuilderIsShowing && !JsonLevelLoader.inst.levelIsBeingLoaded){
			//			Debug.Log("tried");
			// Chance to drop gem is higher as number between 1 - 10,000. Probability range from .01 - 1.
			if (Random.Range(0f,1f) < .09f + Mathf.Abs(fraction.GetAsFloat())/10000f){
				// Number of gems dropped is random between 1 and 1 at lowend, and 10 and 100 at upend
				int lower = Mathf.CeilToInt(Random.Range(0f,10f) * Mathf.Abs(fraction.GetAsFloat())/100000f);
				int upper = Mathf.CeilToInt(Random.Range(10f,200f) * Mathf.Abs(fraction.GetAsFloat())/100000f);
				int singles = Random.Range(lower,upper);
//				Debug.Log("low, up, singles:"+lower+","+upper+","+singles);


				EffectsManager.inst.DropGemCombo(singles,transform.position);
			}
		} 
	}
	
	public override string GetEquation (Fraction original)
	{
		return fraction + " + " + original + " = " + GetModifiedFraction(original);
	}
	
	public override Fraction GetModifiedFraction (Fraction original)
	{
		return Fraction.Add(fraction, original);
	}

	public void SinGrowOnce(){
		if (!GetComponent<SinGrowNumber>()) {

			SinGrowNumber sgn = gameObject.AddComponent<SinGrowNumber>();
			sgn.Init(this);


		}
	}

	public void Eat(NumberInfo ni) {
		if (!ni) return;
		if (ni.faucetRels.Count > 0 && faucetRels.Count > 0){ // Two oneballs with both have a faucet rel. e.g. two oneballs were combined.
			faucetRels.AddRange(ni.faucetRels);
			foreach(NumberFaucet nf in faucetRels){
				nf.SetOneballRel(this);
			}
			ni.faucetRels.Clear(); // the number we're eating shouldn't have a faucet rel any more.
		} else { // at most one of the two balls was a "oneball" so lose all oneball-ness and faucet rels.
			if (faucetRels.Count > 0) {
				foreach(NumberFaucet nf in faucetRels){
					nf.SetOneballRel(null);
				}	
			} else if (ni.faucetRels.Count > 0) {
				foreach(NumberFaucet nf in ni.faucetRels){
					nf.SetOneballRel(null);
				}
			}
		}

		if (GetComponent<Rigidbody>()) { 
			if (!GetComponent<Rigidbody>().isKinematic) GetComponent<Rigidbody>().velocity/=5f;
		}

		NumberManager.inst.DestroyOrPool(ni);
//		if (ni.myShape == NumberShape.Cube){
//			NumberManager.inst.DestroyOrPool(ni);
//		} else {	
//			NumberManager.inst.DestroyOrPool(ni);
//		}

		Fraction result = Fraction.Add (ni.fraction,fraction);
		if (result.numerator == 0){
			ZeroFX(transform.position);
			GemFX();
		} else {
			ni.DestroyedFX();
			AudioManager.inst.PlayNumberEat(transform.position);
		}
		SetNumber(result,true);
//		GetComponent<SometimesFacePlayer>().FacePlayerOnce();

	}
	
	class CollisionPair {
		public NumberInfo a;
		public NumberInfo b;
		public CollisionPair(NumberInfo _a, NumberInfo _b) { a = _a; b = _b; }
		public CollisionPair Swap(CollisionPair s){
			return new CollisionPair(s.b,s.a);
		}
	}
	static List<CollisionPair> collisionPairs = new List<CollisionPair>();
	public void AttemptCombine(NumberInfo ni){
		if (!CombineTestEat(ni,this) && !CombineTestEat(this,ni)) {

			return;
		}
		foreach(CollisionPair p in collisionPairs) {
			if((p.a == this && p.b == ni) || (p.a == ni && p.b == this)) {
				return;
			}
		}

		collisionPairs.Add(new CollisionPair(this, ni));
	}
	

	public static void ResolveCollisions() {

		if(collisionPairs.Count <= 0) return;
		List<CollisionPair> existing = new List<CollisionPair>();
		List<NumberInfo> existingNi = new List<NumberInfo>();
//		string existingStr = "";
		foreach(CollisionPair p in collisionPairs) {
			if (existingNi.Contains(p.a) || existingNi.Contains(p.b)) continue;
			if (p.a == null || p.b == null) continue;
			existingNi.Add (p.a);
			existingNi.Add (p.b);
			existing.Add(p);
//			existingStr += " " + p.a.name+", "+p.b.name+" ..";
		}


		foreach(CollisionPair p in existing) {
			ResolveCollision(p);
		}
		collisionPairs = new List<CollisionPair>();
	}

	bool CombineTestEat(NumberInfo a, NumberInfo b){

//		if ((a.GetComponent<EnergyBall>() && !b.GetComponent<EnergyBall>()) || (!a.GetComponent<EnergyBall>() && b.GetComponent<EnergyBall>())) {
//			return false;
//		}
		if (a.GetComponent<EnergyBall>() || b.GetComponent<EnergyBall>()){
			if (!(a.GetComponent<EnergyBall>() && b.GetComponent<EnergyBall>())){
				return false;
			}
		}
		if (a.GetComponent<AlgebraInfo>() || b.GetComponent<AlgebraInfo>()){
			if (!(a.GetComponent<AlgebraInfo>() && b.GetComponent<AlgebraInfo>())){
				return false;
			}
		}
		if (b.combineLayer > a.greaterThanCombine
		    && b.combineLayer < a.lessThanCombine
		    && a.combineLayer > b.greaterThanCombine
		    && a.combineLayer < b.lessThanCombine){
			return true;
		}
		return false;

//		List<CombineTest> greaterCheck = new List<CombineTest> { CombineTest.Always,CombineTest.Greater,CombineTest.GreaterOrEqual,CombineTest.NotEqual };
//		List<CombineTest> lesserCheck = new List<CombineTest> { CombineTest.Always,CombineTest.Less,CombineTest.LessOrEqual,CombineTest.NotEqual };
//		List<CombineTest> equalCheck = new List<CombineTest> { CombineTest.Always,CombineTest.LessOrEqual,CombineTest.GreaterOrEqual,CombineTest.Equal };
//		if (a.combineLayer > b.combineLayer) {
//			if (lesserCheck.Contains(a.combineTest) && greaterCheck.Contains(b.combineTest)) {
//				return true;
////			} else {

//			}
//		}
//		if (a.combineLayer < b.combineLayer){
//			if (greaterCheck.Contains(a.combineTest) && lesserCheck.Contains(b.combineTest)) {
//				return true;
//			} else {

//			}
//		}
//		
//		if (a.combineLayer == b.combineLayer){
//			if (equalCheck.Contains(a.combineTest) && greaterCheck.Contains(b.combineTest)) {
//				return true;
//			} else {

//			}
//		}
//		return false;
	}

	static void ResolveCollision(CollisionPair pair) {

		if(pair.a == null || pair.b == null || pair.a.gameObject == null || pair.b.gameObject == null){
			
			return;
		}

		NumberInfo a = pair.a;
		NumberInfo b = pair.b;
		if (a.transform.parent == b.transform.parent && b.transform.parent != null) { return; } // don't combine from same generator
//		if (a.transform.parent.GetComponent<AISpikeyGenerator>() && b.tran
		if (a.destroyedThisFrame || b.destroyedThisFrame) { return; }
		if (a.GetComponent<SoapedNumber>() && b.GetComponent<SoapedNumber>()) { return; } // soaped number don't combine with each other.
		if (a.GetComponent<TemporaryPreventCombine>() || b.GetComponent<TemporaryPreventCombine>()) { return; } // numbers chopped with sword won't recombine. This behavior is destoyed on player pickup.
		if (a.GetComponent<BlobNumber>() || b.GetComponent<BlobNumber>()) {
//			if (a.GetComponent<MonsterSnail>() || b.GetComponent<MonsterSnail>()) { return; } // blob monsters
			if (a.GetComponent<BlobNumber>() && b.GetComponent<Rigidbody>() && !b.GetComponent<Rigidbody>().isKinematic){
				a.Eat(b);
			} else if (b.GetComponent<BlobNumber>() && a.GetComponent<Rigidbody>() && !a.GetComponent<Rigidbody>().isKinematic){
				b.Eat(a);
			}
		} else if (a.GetComponent<MonsterAISpikey1>() || b.GetComponent<MonsterAISpikey1>()) { 
			if (a.GetComponent<MonsterAISpikey1>() && b.GetComponent<MonsterAISpikey1>()) { return; } // spikey' dont eat each other
			else if (!a.GetComponent<Rigidbody>() || !b.GetComponent<Rigidbody>()) { return; } // spikeys dont interact with non rigidbodies
			else if (a.GetComponent<Rigidbody>() && a.GetComponent<Rigidbody>().isKinematic) { return; } // spikeys dont eat kinematics.
			else if (b.GetComponent<Rigidbody>() && b.GetComponent<Rigidbody>().isKinematic) { return; } // spikeys dont eat kinematics.
			else if (a.GetComponent<MonsterAISpikey1>() && !b.GetComponent<MonsterAISpikey1>()) { a.Eat(b); } // spikeys eat regular num
			else if (!a.GetComponent<MonsterAISpikey1>() && b.GetComponent<MonsterAISpikey1>()) { b.Eat(a); }
		} else if(a.fraction.numerator == -b.fraction.numerator && a.fraction.denominator == b.fraction.denominator) {

//			DestroyedFX(a.transform,b.tran);
//			b.DestroyedFX();
			a.ZeroFX(a.transform.position);
			a.GemFX();
			b.ZeroFX(b.transform.position);
			b.GemFX();
			Destroy(a.gameObject); //if (a.myShape == NumberShape.Cube) NumberPool.inst.DestroyOrPool (a);
//			else Destroy(a.gameObject);
			Destroy(b.gameObject); //if (b.myShape == NumberShape.Cube) NumberPool.inst.DestroyOrPool (b);
//			else Destroy(b.gameObject);

			SMW_FX.CreateSmallPurpleExplosion(a.transform.position);//,1.5f,.5f);
			SMW_FX.CreateWhiteFlash(a.transform.position);//,20f,.3f);
			SMW_FX.CreateShards(a.transform.position);


			SMW_FX.CreateTextEffect(a.transform.position,"zero");
			a.destroyedThisFrame = true;
			b.destroyedThisFrame = true;
//			a.DestroyedFX();
//			AudioManager.inst.PlayNumberShatter(a.transform.position);
			return;
		}
		else if (a.myShape == NumberShape.Cube && b.myShape == NumberShape.Sphere) { a.Eat(b); }
		else if (a.myShape == NumberShape.Sphere && b.myShape == NumberShape.Cube) { b.Eat(a); }
		else if (a.myShape == NumberShape.Cube && b.myShape == NumberShape.Cube){
			if(a.gameObject.GetComponent<Rigidbody>() && !b.gameObject.GetComponent<Rigidbody>()) { b.Eat(a); }
			else if(b.gameObject.GetComponent<Rigidbody>() && !a.gameObject.GetComponent<Rigidbody>()) { a.Eat(b); }
		}
		else if(a.gameObject.GetComponent<Rigidbody>() && a.gameObject.GetComponent<Rigidbody>().isKinematic) { a.Eat(b); }
		else if(b.gameObject.GetComponent<Rigidbody>() && b.gameObject.GetComponent<Rigidbody>().isKinematic) { b.Eat(a); }
		else if (a.neverEats) { b.Eat (a); }
		else if (b.neverEats) { a.Eat (b); }
		else if (a.combineLayer > b.combineLayer) { a.Eat (b); }
		else if (a.combineLayer < b.combineLayer) { b.Eat (a); }
		else if(a.gameObject == PlayerGadgetController.inst.thrownNumber) { b.Eat(a); }
		else if(b.gameObject == PlayerGadgetController.inst.thrownNumber) { a.Eat(b); }
		else if (a.GetComponent<SoapedNumber>() && !b.GetComponent<SoapedNumber>()) { a.Eat(b); }
		else if (b.GetComponent<SoapedNumber>() && !a.GetComponent<SoapedNumber>()) { b.Eat(a); }
		else if(a.gameObject.GetComponent<Rigidbody>() && b.gameObject.GetComponent<Rigidbody>()) {
			float velA = a.gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude;
			float velB = b.gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude;
			if(velA < velB) {
				a.Eat(b);
			}
			else {
				b.Eat(a);
			}
		}
		else if(Fraction.AbsGreater(a.fraction, b.fraction)) { a.Eat(b); }
		else if(Fraction.AbsGreater(b.fraction, a.fraction)) { b.Eat(a); }

		
	}

	public virtual void SetColor(bool transparent = false){
		SetColor (fraction,transparent);
	}
	public virtual void SetColor(Fraction f, bool transparent = false){
		if (!childMeshRenderer) return;
		if (myShape == NumberShape.Tetrahedron || myShape == NumberShape.Schur || myShape == NumberShape.Ghost || myShape == NumberShape.WavePercentAmmo) return;
		Material posm = NumberManager.inst.positiveNumberMaterial;
		Material negm = NumberManager.inst.negativeNumberMaterial;
		if (transparent){
			posm = NumberManager.inst.positiveNumberMaterialTransparent;
			negm = NumberManager.inst.negativeNumberMaterialTransparent;
			if (outlineMesh) outlineMesh.enabled = false;
		} else {
			if (outlineMesh) outlineMesh.enabled = true;
		}

		if (f.numerator > 0) {
			if (Application.isPlaying) {

				childMeshRenderer.material = posm;
				if (extraMesh) extraMesh.material = posm;

			}
			foreach(NumberText t in texts){
				t.integer.Color = Color.black;
				t.numerator.Color = Color.black;
				t.line.Color = Color.black;
				t.denominator.Color = Color.black;
			}
			if (GetComponent<EnergyBall>()){
				GetComponent<EnergyBall>().energySprite.GetComponent<SpriteRenderer>().color = Color.white;
			}
		}
		else {
			childMeshRenderer.material = negm; 
			if (extraMesh) extraMesh.material = negm; 
			foreach(NumberText t in texts){
				t.integer.Color = Color.white;
				t.numerator.Color = Color.white;
				t.line.Color = Color.white;
				t.denominator.Color = Color.white;
			}

			if (GetComponent<EnergyBall>()){
				GetComponent<EnergyBall>().energySprite.GetComponent<SpriteRenderer>().color = new Color(0.5f,0.5f,0.5f,1);
			}
		}
	}

	public void DisableAllRenderers(){
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			r.enabled = false;
		}

	}

	public void OnCutInHalf(){
		PostGrowFX();
	}

	bool forbidCombinations = false;
	float forbidCombinationsForSeconds = 0;
	public void ForbidCombinationsForSeconds(float s){
		forbidCombinationsForSeconds = s;
		forbidCombinations = true;
	}

//	public virtual void SetScaleBasedOnMyFraction(){
//		float fl = Mathf.Abs(fraction.GetAsFloat());

//		transform.localScale = Vector3.one * (Mathf.Max(0,Mathf.Log(fl)) + 1);
//	}

	List<NumberFaucet> faucetRels = new List<NumberFaucet>();

	public void SetFaucetRel(NumberFaucet nf){
		faucetRels.Add(nf);
	}

	void CleanFaucetRels(){
		List<NumberFaucet> toDel = new List<NumberFaucet>();
		foreach(NumberFaucet nf in faucetRels){
			if (nf == null){
				toDel.Add(nf);
			}
		}
		foreach(NumberFaucet nf in toDel){
			faucetRels.Remove(nf);
		}
		
	}

	#region Soapable
	// "Soaped" numbers do not combine.
	public bool IsSoapable(){
		return soapFx != null;
	}

	public void SoapNumber(){
		soapFx.SetActive(true);
		SoapedNumber sn = gameObject.GetComponent<SoapedNumber>();
		if (!sn) gameObject.AddComponent<SoapedNumber>();
//		ExpireOverTime 
	}

	public void UnSoapNumber (){
		soapFx.SetActive(false);
		SoapedNumber sn = gameObject.GetComponent<SoapedNumber>();

		if (sn) Destroy(sn);
	}
	public void SoapScale(float f){
		ParticleSystem ps = soapFx.GetComponentInChildren<ParticleSystem>();
		ps.startSize = f/4f;
		if (myShape == NumberShape.Cube){
			ParticleSystem.ShapeModule pss = ps.shape;
//			pss.scale = new Vector3(f,f,f);
			//					ps.sh
		}

	}

	#endregion

//*/
//	public override static GameObject FromJson(SimpleJSON.JSONClass N){
//		 Instantiate the prefab

//		 Gameobject newNumberCube = Instantiate(LevelBuilderObjectManager.cubeprefab)


//		foreach(SimpleJSON.JSONClass n in N["properties"]){
//			 prfab.nnumberinfo.setprop(prop)

//		}
//		SetProperties(N);
//	}




// */
	

}
