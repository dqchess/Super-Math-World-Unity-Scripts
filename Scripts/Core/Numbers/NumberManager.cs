using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
// handles all number prefabs and functions concerning numbers.
// Can also approximate fractions.


// Also keeps track of all the numbers in the scene so we don't have to findobjectsoftype or physicsoverlapsphere in order to find numbers.

[System.Serializable]
public class NumberStack {
	// used for managing the FX for number stacks which is used to visually represent multiplication. 
	// when implemented, the number is "frozen" and cannot interact
	// the number stack animates to show the user the multiplication / addition relation
	// the stack collapses and gives a result
	// number is reanimated
	public int stackHeight = 0;
	public bool kinematic = false;
	public bool useGravity = true;
	public int sign = 1;
	public NumberInfo stackingNumber;
	public float stackTime = 0;
	public List<GameObject> stackedGhosts = new List<GameObject>(); // the visual FX of the stack, these are not "real" numbers, we'll use number ammo, so we call them ghosts;
	public List<Renderer> hiddenRends = new List<Renderer>(); // we will hide all enabled renderers, but some renderers werent enabled on the object, so we list ones to re-enable here.
}

public class NumberManager : MonoBehaviour {
	float numberStackTimeThreshhold = 2.5f; // the number of seconds before the stack begins collapsing
	float numberStackFXDuration = 6f; // the total length of time the stack lasts (regardless of how close the "ghost "numbers are together in the end)

	public void CreateNumberStack(NumberInfo ni, int stackHeight){
		NumberStack newStack = new NumberStack();
		ni.enabled = false;
		PickUppableObject pip = ni.GetComponent<PickUppableObject>();
		if (pip){
			pip.enabled = false;
		}
		MonsterAIBase maib = ni.GetComponent<MonsterAIBase>();
		if (maib){
			maib.enabled = false;
		}
		// the stack of fx numbers will be positive or negative depending on the result.
		int resultSign = ni.fraction.numerator * stackHeight > 0 ? 1 : -1;
		Fraction resultFrac = Fraction.Multiply(Fraction.GetAbsoluteValue(ni.fraction),new Fraction(resultSign,1));
//		Debug.Log("stack:"+stackHeight+", restul;"+resultFrac+", ni frac;"+ni.fraction+", resultsign:"+resultSign);
		for(int i=0; i<Mathf.Abs(stackHeight);i++){
			GameObject numberAmmo = (GameObject)Instantiate(ni.gameObject);
			numberAmmo.GetComponentInChildren<NumberInfo>().SetNumber(resultFrac); // destroys if zero.
			Rigidbody rb = numberAmmo.GetComponent<Rigidbody>();
			if (rb) Destroy(rb);
			Collider c = numberAmmo.GetComponent<Collider>();
			if (c) Destroy(c);
			Vector3 fixzfighting =UnityEngine.Random.insideUnitSphere*.01f;
			numberAmmo.transform.position = ni.transform.position + Vector3.up * i * Utils.RealHeight(ni.transform) * 1.14f + fixzfighting;// ni.transform.localScale.x;
			newStack.stackedGhosts.Add(numberAmmo);
		}
		foreach(Renderer r in ni.GetComponentsInChildren<Renderer>()){
			if (r.enabled){
				r.enabled =false;
				newStack.hiddenRends.Add(r);
			}
		}
		newStack.stackTime = numberStackFXDuration;
		newStack.stackingNumber = ni;
		newStack.stackHeight = stackHeight;
		newStack.sign = stackHeight > 0 ? 1 : -1;
		if (ni.GetComponent<Rigidbody>()){
			newStack.kinematic = ni.GetComponent<Rigidbody>().isKinematic;
			newStack.useGravity = ni.GetComponent<Rigidbody>().useGravity;
			ni.GetComponent<Rigidbody>().useGravity = false;
			ni.GetComponent<Rigidbody>().isKinematic = true;
		}
//		if (ni.GetComponent<Rigidbody>()) ni.GetComponent<Rigidbody>().isKinematic = true;
//		ni.GetComponent<Collider>().enabled = false;

//		Debug.Log("added newstack. ghosts;"+newStack.stackedGhosts[0]);
		numberStacks.Add(newStack);
	}

	void FinishStack(NumberStack ns){
		NumberInfo ni = ns.stackingNumber;
		MonsterAIBase maib = ni.GetComponent<MonsterAIBase>();
		if (maib){
			maib.enabled = true;
		}
		foreach(Renderer r in ns.hiddenRends){
			r.enabled = true;
		}
		if (ni.GetComponent<Rigidbody>()){
			ni.GetComponent<Rigidbody>().isKinematic = ns.kinematic;
			ni.GetComponent<Rigidbody>().useGravity = ns.useGravity;
		}
		PickUppableObject pip = ni.GetComponent<PickUppableObject>();
		if (pip){
			pip.enabled = true;
		}
		ns.stackingNumber.SetNumber(Fraction.Multiply(ns.stackingNumber.fraction,new Fraction(ns.stackHeight,1)),true);
		EffectsManager.inst.RevertSparks(ns.stackingNumber.transform.position,2);
		AudioManager.inst.PlayCrystalThump1(ns.stackingNumber.transform.position);
//		if (ni.GetComponent<Rigidbody>()) ni.GetComponent<Rigidbody>().isKinematic = false;
//		ni.GetComponent<Collider>().enabled = true;
		ni.enabled = true;	
		foreach(GameObject o in ns.stackedGhosts) {
			foreach(IMuteDestroySound mute in o.GetComponents(typeof(IMuteDestroySound))){
				mute.MuteDestroy();
				
			}
			Destroy(o);
		}
		// note the stack is not destroyed here beacuse this method is called in an iterator blcok over the list 
		// can't take one out int he middle of iteration, so it's taken care of after the loop finishes in update()
	}


	[SerializeField] public List<NumberStack> numberStacks = new List<NumberStack>();

	public static NumberManager inst;

	public Texture positiveSphereIcon;
	public Texture negativeSphereIcon;
	public Material rainbowMaterial;
	public Material inventoryDigitMaterial;
	public Material positiveNumberMaterial;
	public Material negativeNumberMaterial;
	public Material positiveNumberMaterialTransparent;
	public Material negativeNumberMaterialTransparent;
	public GameObject energyBallSpritePrefab;
	public GameObject energyBallParticlesPrefab;
	public GameObject tetrahedronModel;
	public GameObject schurNumberPrefab;
	public float numberScale = 3;
	public float numberScale2 = 1.6f;

	public Material pos_block;
	public Material neg_block;

	public GameObject digitPrefab;
	public GameObject numberSpherePrefab;
	public GameObject numberCubeWaveAmmoGraphics;
	public GameObject numberSpherePrefabFace;
	public GameObject numberSpherePrefabSimpleHat;
	public GameObject numberSpherePrefabFaceHat;
	public GameObject numberArrowPrefab;
	public GameObject numberAmmoPrefabSphere;
	public GameObject numberAmmoPrefabCube;
	public GameObject numberCubePrefab;
	public GameObject resourceNumberPrefab;
	public GameObject resourceNumberAmmoPrefab;
	public GameObject schurNumberAmmoPrefab;
	
	[System.NonSerialized] public Vector3 fixZFighting = new Vector3(-.01f,-.01f,-.01f);
	
	

	public void SetInstance(){
		inst = this;
		
	}
	void Start () {
		GameManager.inst.onLevelWasRestartedDelegate += LevelRestarted;
	}

	void LevelRestarted(){
		foreach(NumberStack ns in numberStacks){
			if (ns.stackedGhosts.Count > 0){
				foreach(GameObject o in ns.stackedGhosts){
					if (o){
						Destroy(o);
					}
				}
			}
		}
		numberStacks.Clear();
	}



	List<NumberInfo> numbersInScene = new List<NumberInfo>();
//	public void AddNumberToScene(NumberInfo ni){
//		if (numbersInScene.Contains(ni)) return;
//		numbersInScene.Add(ni);
//	}

//	public NumberInfo[] GetAllActiveNumbersInScene(){
//		List<NumberInfo> ret = new List<NumberInfo>();
//		foreach(NumberInfo ni in GetAllNumbersInScene()){
//			if (ni.gameObject.activeSelf){
//				ret.Add(ni);
//			}
//		}
//		return (NumberInfo[])ret.ToArray();
//	}

//	List<NumberInfo> previousNumbers = new List<NumberInfo>();

	float getAllNumbersTimer = 0f;
	NumberInfo[] allNumbersCached;
	public NumberInfo[] GetAllNumbersInScene(bool activeOnly = false){
		if (getAllNumbersTimer < 0){
			getAllNumbersTimer = 0.3f;
			allNumbersCached = Utils.FindObjectsOfTypeInScene<NumberInfo>().ToArray();
		}
		return allNumbersCached;
//		return numbersInScene.ToArray();
//		if (previousNumbers.Count == 0 || requestTimer < 0){
//			requestTimer = 4f; // don't need this all the time.
//			previousNumbers.Clear();
//			List<NumberInfo> toDel = new List<NumberInfo>();
//			List<NumberInfo> activeNumbers = new List<NumberInfo>();
//			foreach(NumberInfo ni in numbersInScene){
//				if (ni == null){
//					toDel.Add(ni);
//				} else if (ni.gameObject.activeSelf){
//					activeNumbers.Add(ni);
//				}
//			}
//			foreach(NumberInfo ni in toDel){
//				numbersInScene.Remove(ni);
//			}
//			previousNumbers = activeNumbers;
//			if (!activeOnly) return (NumberInfo[])numbersInScene.ToArray();
//			else return (NumberInfo[])activeNumbers.ToArray();
//		} else return previousNumbers.ToArray();
	}
	
	public GameObject MakeIntoRocket(GameObject rocket){
		TimedObjectDestructor tod = rocket.AddComponent<TimedObjectDestructor>();
		tod.autoDestruct=false;
		tod.DestroyNow(10); // rockets die out after 10 secodns.
		
		rocket.transform.parent = null;
		rocket.GetComponent<Collider>().enabled=true;
		// Create the rocket.

		
		rocket.GetComponent<Rigidbody>().isKinematic=false; // Why is this true by default?!
		rocket.GetComponent<Rigidbody>().useGravity=false;
		rocket.GetComponent<Rigidbody>().drag=0.01f;
		rocket.GetComponent<Rigidbody>().angularDrag=0.01f;
		rocket.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		
		
		NumberInfo ni = rocket.GetComponent<NumberInfo>();
		if (!ni.gameObject.GetComponent<DoesExplodeOnImpact>()) {
			ni.gameObject.AddComponent<DoesExplodeOnImpact>();
		}
		//		// commented Debug.Log ("rocket created, value;"+rocket.GetComponent<NumberInfo>().fraction);
		return rocket;
	}
	

	// TODO: This is old and doesn't use CCText.. INVENTORY still uses this system. Move Inventory to CCText? .. obviously..
//	public GameObject CreateDigits(GameObject blockParent, Fraction frac){
//		if(frac.denominator == 1) {
//			string numStr = frac.numerator.ToString();
//			AddNumberString(numStr, blockParent, Vector2.zero, Vector2.one * .8f, frac.numerator > 0);
//		}
//		if (frac.denominator != 1){
//			string fracString = frac.ToString();
//			string[] mixed = fracString.Split(' ');
//			string whole = "";
//			if(mixed.Length > 1) {
//				whole = mixed[0];
//				int numerator = int.Parse(mixed[1].Split('/')[0]);
//				int denominator = int.Parse(mixed[1].Split('/')[1]);
//				Fraction fracPart = new Fraction(numerator, denominator);
//				AddNumberString(whole, blockParent, new Vector2(0.18f + (0.06f * (whole.Length/Mathf.Pow (whole.Length,.5f) - 1)), 0f), new Vector2(0.75f, 0.75f), frac.numerator >= 0);
//				AddImproperFraction(fracPart, blockParent, new Vector3(-0.20f - 0.05f * (whole.Length/Mathf.Pow (whole.Length,.5f)-2), 0f), new Vector2(0.3f, 0.3f), frac.numerator >= 0);
//			}
//			else {
//				AddImproperFraction(frac, blockParent, new Vector3(0f, 0f), new Vector2(0.4f,0.4f), frac.numerator >= 0);				
//			}
//
//		}
//		return blockParent;
//	}

	GameObject CreateBlock(GameObject digitToCreate, float xScale){
		GameObject newObj = (GameObject)Instantiate(digitToCreate,transform.position,transform.rotation);
		if (xScale != 1) newObj.transform.localScale = new Vector3(xScale*1.5f,newObj.transform.localScale.y,newObj.transform.localScale.z);
		return newObj;
	}
	

	
	public GameObject CreateNumberAmmo(Fraction frac, NumberShape shape = NumberShape.Sphere){
		GameObject newNumber = null;
		if (shape == NumberShape.Sphere) newNumber = (GameObject)Instantiate (numberAmmoPrefabSphere);
		else if (shape == NumberShape.Cube) newNumber = (GameObject)Instantiate (numberAmmoPrefabCube);
		else if (shape == NumberShape.Tetrahedron) newNumber = (GameObject)Instantiate(resourceNumberAmmoPrefab);
		else if (shape == NumberShape.Arrow) newNumber = (GameObject)Instantiate(numberArrowPrefab);
		else if (shape == NumberShape.WavePercentAmmo) newNumber = (GameObject)Instantiate(numberCubeWaveAmmoGraphics);
		else if (shape == NumberShape.Schur) {
//			Debug.Log("newnum schr");
			newNumber = (GameObject)Instantiate(schurNumberAmmoPrefab);
		}
//		Debug.Log("shape:"+shape);
		NumberInfo ni = newNumber.GetComponent<NumberInfo>();
		ni.destroyIfZero = false;
		ni.SetNumber(frac);
		ni.isSerializeableForSceneInstance = false;

		return newNumber;
	}




	public GameObject CreateNumber (Fraction frac, Vector3 pos, NumberShape numberShape=NumberShape.Sphere, bool destroyIfZero = true){
		
		if (frac == null) {
			// commented Debug.Log("Who is caling create number?! it was null.");
			return null;
		}
		frac = Fraction.ReduceFully(frac);
		if (frac.numerator == frac.denominator && frac.numerator != 1){
			frac.numerator=1; frac.denominator=1;
		}
		NumberInfo ni = null;
		if (numberShape == NumberShape.Cube){
			GameObject n = NumberPool.inst.GetFromPool();
			if (n) ni = n.GetComponent<NumberInfo>();
			else ni = (NumberInfo)Instantiate(numberCubePrefab).GetComponent<NumberInfo>();
			ni.name = "Number id: "+id.ToString();
//				// commented Debug.Log("no:"+newNumber);
//			}
		} else if (numberShape == NumberShape.Sphere) {
			ni = (NumberInfo)Instantiate(numberSpherePrefab).GetComponent<NumberInfo>();
		} else if (numberShape == NumberShape.Tetrahedron) {
			ni = (NumberInfo)Instantiate(resourceNumberPrefab).GetComponent<NumberInfo>();
		} else if (numberShape == NumberShape.Schur) {
			ni = (NumberInfo)Instantiate(schurNumberPrefab).GetComponent<NumberInfo>();

			// commented Debug.LogError("Invalid:"+numberShape);
		} else if (numberShape == NumberShape.Face) {
			ni = (NumberInfo)Instantiate(numberSpherePrefabFace).GetComponent<NumberInfo>();
		} else if (numberShape == NumberShape.SimpleHat) {
			ni = (NumberInfo)Instantiate(numberSpherePrefabSimpleHat).GetComponent<NumberInfo>();
		} else if (numberShape == NumberShape.Hat) {
			ni = (NumberInfo)Instantiate(numberSpherePrefabFaceHat).GetComponent<NumberInfo>();
		} else if (numberShape == NumberShape.Arrow) {
			ni = (NumberInfo)Instantiate(numberArrowPrefab).GetComponent<NumberInfo>();
		}
//		NumberInfo ni = newNumber.GetComponent<NumberInfo>();
		ni.destroyIfZero = destroyIfZero;
//		// commented Debug.Log("frac:"+frac);
		ni.SetNumber(frac);
//		// commented Debug.Log("set frac:"+frac);
		ni.myShape = numberShape;
		
		ni.transform.position = pos;
//		// commented Debug.Log("renaming:"+newNumber.name+" to:"+id.ToString());

		id++;
//		AddNumberToScene(ni);
//		// commented Debug.Log("Added number to scene!");
		return ni.gameObject;
	}
	int id = 0;
	
	int maxNumbersAllowed = 600;
	public bool MaxNumbersReached(){
		int numberOfNumbersInScene = GameObject.FindGameObjectsWithTag("Number").Length;
		if (numberOfNumbersInScene >= maxNumbersAllowed) return true;
		else return false;
	}
	
	public bool IsTauFraction(Fraction frac)
	{
		float num = frac.GetAsFloat();
		float factorOf = (Mathf.PI / 8);
		
		int times = (int)Mathf.Round((float)num / factorOf);
		float off = Mathf.Abs(num - factorOf * times);
		if(off < 0.01f)
		{
			return true;
		}		
		return false;
	}
	
	public Fraction GetTauFraction(Fraction frac) 
	{
		float num = frac.GetAsFloat() / (Mathf.PI * 2);
		num *= 16;
		return Fraction.ReduceFully(new Fraction(Mathf.RoundToInt(num), 16));
	}
	
	public void ModifyDigits(GameObject blockParent, Fraction frac){
		
	}
	


	// Smart math stuff. DO NOT TOUCH.
	// todo: Move this to MathUtils.cs
	string ReduceFraction(string fraction) {
		
		int Div=1;
		string[] frac = fraction.Split("/"[0]);
		int frac0=int.Parse(frac[0]);
		int frac1=int.Parse(frac[1]);
		for (var zxc0=1;zxc0<100;zxc0++){
			if (frac0%zxc0==0&&frac1%zxc0==0){
				Div=zxc0;
			}
		}
		var fractionString = (frac0/Div).ToString()+"/"+(frac1/Div).ToString();
		//// commented Debug.Log("Reduced "+fraction+" :"+fractionString);
		return fractionString;
	}
	
	// Smart math stuff. DO NOT TOUCH
	// approximate fractions from decimalNumbers here: view-source:http://www.mindspring.com/~alanh/fracs.html
	Vector2 approximateFractions ( float decimalNumber  ){
		if (decimalNumber%1==0) {
			return new Vector2(decimalNumber,1);
		}
		float d= decimalNumber;
		int numerator=0;
		int denominator=0;
		
		float[] numerators = new float[100];
		numerators[0]=0;
		numerators[1]=1;
		float[] denominators = new float[100];
		denominators[0]=1;
		denominators[1]=0;
		
		int maxNumerator= getMaxNumerator(d);
		float d2= d;
		float calcD;
		float prevCalcD=.11f;
		
		//for (FIXME_VAR_TYPE i= 2; i < 1000; i++)  {
		
		for (var i= 2; i < 100; i++)  {
			var L2= Mathf.Floor(d2);
			numerators[i] = L2 * numerators[i-1] + numerators[i-2];
			if (Mathf.Abs(numerators[i]) > maxNumerator) {	
				// commented Debug.Log("oops: exceeded max numerator"); // breaks on -1/2
				return new Vector2(1,1);
			}
			
			denominators[i] = L2 * denominators[i-1] + denominators[i-2];
			// // commented Debug.Log("numerators["+i+"] set to "+numerators[i]+"/"+denominators[i]);
			
			calcD = numerators[i] / denominators[i];
			// // commented Debug.Log(numerators[i]+"/"+denominators[i]+", calcd:"+calcD+";prev:"+prevCalcD);
			// appendFractionsOutput(numerators[i], denominators[i]);
			//	    // commented Debug.Log("max numerator: "+maxNumerator+"; i:"+i);
			//		// commented Debug.Log("approx fraction of "+decimalNumber+" is "+numerators[i]+"/"+ denominators[i]);
			if (calcD == prevCalcD) {
				
				string numString="";
				string denString="";
				for (var j=0;j<i;j++){
					numString+=numerators[j]+",";
					denString+=denominators[j]+",";
				}
				// commented Debug.Log("oops--no incr. diff.. nums: "+numString+"_____denoms:"+denString);
				return new Vector2(-11,1);
			}
			
			if (calcD == d) {
				// // commented Debug.Log("Sweet. This fraction is exactly the same as the decimalNumber you requested. (within 3 digits)");
				numerator=(int)numerators[i];
				denominator=(int)denominators[i];
				return new Vector2(numerator,denominator);
			}
			
			prevCalcD = calcD;
			
			d2 = 1/(d2-L2);
		}
		
		// commented Debug.Log("returning :"+numerator+"/"+denominator);
		
		return new Vector2(numerator,denominator);
		// }
		
	}
	
	// Smart math stuff. DO NOT TOUCH
	// DO NOT PASS INTEGERS INTO THIS FUNCTION. EACH TIME YOU DO, AN ELF RABBIT DIES. ALSO YOU WILL GET BS RESULTS.
	int getMaxNumerator ( float f  ){ 
		// clever genius stuff 
		
		int numDigits = f.ToString().Length-1;
		var numIntDigits= Mathf.Floor(f).ToString().Length;
		if (Mathf.Floor(f)==0) numIntDigits=0; 
		var numDigitsPastdecimalNumber= numDigits - numIntDigits;
		
		//	// commented Debug.Log("for f="+f+";int:"+numIntDigits+";float:"+numDigitsPastdecimalNumber);
		// Get the list of all integers in the 12345.678f by removing the decimalNumber
		var digits= int.Parse(f.ToString().Replace(".",""));
		//// commented Debug.Log("digits:"+digits);
		int L=digits;
		
		
		for (var i=numDigitsPastdecimalNumber; i>0 && L%2==0; i--) L/=2; // so clever
		for (var i=numDigitsPastdecimalNumber; i>0 && L%5==0; i--) L/=5;
		
		return L;
	}


//	float requestTimer = 0f;
	void LateUpdate(){
		NumberInfo.ResolveCollisions();
//		requestTimer -= Time.deltaTime;
	}

	public void DestroyAllNumbers(){
		foreach(NumberInfo n in numbersInScene){
			if (n == null) continue;
			n.energyNumberDestroyedDelegate = null; // otherwise it wil lsend "energy lost so play "stop" sound on vehicles that lose their number this frame"
			Destroy(n.gameObject);
		}
		numbersInScene.Clear();
		foreach(NumberInfo n in FindObjectsOfType<NumberInfo>()){ // extras?
			// awkward delegate clearing, must be cleaner way
			n.energyNumberDestroyedDelegate = null; // otherwise it wil lsend "energy lost so play "stop" sound on vehicles that lose their number this frame"
		}
		int j=0;
		foreach(NumberInfo ni in FindObjectsOfType<NumberInfo>()){
			NumberManager.inst.DestroyOrPool(ni);
			j++;
		}
		foreach(NumberInfoAmmo nim in FindObjectsOfType<NumberInfoAmmo>()){
			Destroy(nim.gameObject);
		}
//		WebGLComm.inst.Debug("Destroyed "+j+" numbers");
	}

	public static bool IsCombineable(NumberInfo ni){
		// A special case check for resource numbers and Schur puzzle numbers, which still use NumberInfos but are not modifyable or combineable (to each other or gadgets..).
		if (ni == null) return false;
		if (ni.GetComponent<ResourceNumber>()) return false;
		if (ni.GetComponent<CauldronNumber>()) return false;
		return true;
	}


	void Update(){
		getAllNumbersTimer -= Time.deltaTime;
		List<NumberStack> toDel = new List<NumberStack>();
		foreach(NumberStack ns in numberStacks){
			
			ns.stackTime -= Time.deltaTime;
			float stackCollapseSpeed = 0.4f;
			if (ns.stackingNumber){
				if (ns.stackTime < numberStackTimeThreshhold){
					foreach(GameObject o in ns.stackedGhosts){
						if (o){
							o.transform.position = Vector3.Lerp(o.transform.position,ns.stackingNumber.transform.position,Time.deltaTime * stackCollapseSpeed);
						}
					}
				}
				if (ns.stackTime < 0){
					FinishStack(ns);
					toDel.Add(ns);
				}
			} else {
				toDel.Add(ns);
			}
		}
		foreach (NumberStack ns in toDel){
			numberStacks.Remove(ns);
		}
	}

	public void DestroyOrPool(NumberInfo ni){
		Destroy(ni.gameObject);
//		NumberManager.DestroyOrPool(ni.gameObject);
	}
}
