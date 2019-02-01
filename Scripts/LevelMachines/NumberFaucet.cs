using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NumberFaucet : UserEditableObject {


	public GameObject debugText;
	bool debugging = false;
	#region UserEditable 


	public static string offsetKey = "offset";
	public static string intervalKey = "interval";
	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> els = new List<GameObject>();
		els.AddRange(base.GetUIElementsToShow());
		els.Add(LevelBuilder.inst.POCMFractionButton);
		els.Add(LevelBuilder.inst.POCMcopyButton);
		els.Add(LevelBuilder.inst.POCMheightButton);
		els.Add(LevelBuilder.inst.POCMModFaucetButton);
		return els.ToArray();

	}

	public override SimpleJSON.JSONClass GetProperties(){ 

		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		N[offsetKey].AsFloat = offset;
		N[intervalKey].AsFloat = interval;
		return N;
	}



	public void SetOffset(float o){
		offset = o;
		genTimer = o;
	}


	/* footpring was:  (){
		return 2f;
	 */ 

	// upoffset 	}
	public override void OnGameStarted() {
		
	}



	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)) SetPropertiesFraction(N);
		if (N.GetKeys().Contains(offsetKey)){
			SetOffset(N[offsetKey].AsFloat);
		}
		if (N.GetKeys().Contains(intervalKey)){
			interval = N[intervalKey].AsFloat;
		}

	}


	void SetPropertiesFraction(SimpleJSON.JSONClass N){
		frac = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
		SetSkyCamText();
		foreach(NumberInfo ni  in GetComponentsInChildren<NumberInfo>()){ // this shouldn't exist, it basically is saying if you modify this object's properties while in edit mode, its children (which shoulnd't have been produced yet) should also change..
			ni.SetNumber(frac,false,false);
		}
	
	}

	#endregion

	[Header("Faucet Properties")]
//	List<GameObject> createdNumbers = new List<GameObject>(); // keep track of numbers we created. Don't let there be too many.
//	int maxCreatedNumbersAllowed = 20;

	public float dripTime=0;
	public bool soaped = false;
	
	[System.NonSerialized] public float maxDripTime=.5f;
	public GameObject dripNumber;
	public Fraction frac;
	public Transform dripStartT;
	float bubblestimer = 0;
	float randTime = 3;
	public bool oneball = false; // When set to true, the number faucet will not drip a new ball until the original ball has disappeared somehow.

	public GameObject generatedNumber;
	public float genTimer = 1; 
	public float offset = 0;
	public float interval = 4;
	// Use this for initialization
	void Start () {
		// Create the 3D prefab inside the faucet that "slowly faces playeR"

		name = frac + " " + name;
	}
	
	// Update is called once per frame
	float childCareTimer = 2f;
	virtual public void Update () {
		if (SMW_CHEATS.inst.cheatsEnabled){
			if (Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.X)){
				if (debugText){
					debugText.SetActive(true);
					debugging = true;
				}
			}
		}
		if (debugging){
			string gn = generatedNumber == null ? "null" : generatedNumber.name;

			debugText.GetComponentInChildren<CCText>().Text = "cangen:"+CanGenerateNumber()+"\ngentmr;"+(Mathf.RoundToInt(genTimer*100)/100f).ToString()+"\nNonumareclsby()"+NoNumbersAreCloseby()+"\ngen numbr:"+gn;
		}
		if (GameManager.inst.GameFrozen) return;
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (Time.timeScale == 0) return;
		childCareTimer -= Time.deltaTime;
		if (childCareTimer < 0){
			childCareTimer = Random.Range(3.1f,4.2f);
			if (generatedNumber){
				SMW_GF.inst.CreateLightning(dripStartT,generatedNumber.transform,.3f);
			}
		}


		if (destroyGeneratedNumbertimer < 0){
			destroyGeneratedNumbertimer = 30;
			Destroy(generatedNumber);
			generatedNumber = null;
		}
		BubblesFX();
		if (dripNumber != null) DripFX();

		if (CanGenerateNumber()) { // can we generate a number in these conditions?
			GenerateNumber(frac);
		}
	}

	float distToDestroyGeneratedOneball = 30f;
	float destroyGeneratedNumbertimer = 30f;
	bool CanGenerateNumber(){
		
		if (!dripNumber) {
			if (oneball){
				if (generatedNumber && generatedNumber.activeSelf){
					if (Vector3.Distance(transform.position,generatedNumber.transform.position) > distToDestroyGeneratedOneball){
						destroyGeneratedNumbertimer -= Time.deltaTime;
					}
				}
				if (generatedNumber == null){
					genTimer -= Time.deltaTime;
//					// commented Debug.Log("oneball, gentimer:"+genTimer);
				}
			} else if (!HaveChild()) {
//				// commented Debug.Log("gentimre:"+genTimer);
				genTimer -= Time.deltaTime;
			}
		}
		if (SMW_CHEATS.inst.PlayerDebug(this.transform,Color.yellow)){
			PlayerNowMessage.inst.DisplayInstant( "oneball:"+oneball+", generatednumber:"+generatedNumber+",havechild:"+HaveChild()+", gentimer;"+genTimer+",nonum are close:"+NoNumbersAreCloseby());
		}
		return ((( oneball && generatedNumber == null) || (oneball == false && HaveChild() == false)) && genTimer < 0 && NoNumbersAreCloseby());
	}

	public bool NoNumbersAreCloseby(){
		float detectDist = 5f;
		foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene(true)){
			if (ni && ni.gameObject.activeSelf){
				if (Vector3.Distance(ni.transform.position,transform.position) < detectDist && ni.myShape == NumberShape.Sphere){
					return false;
				}
			}
		}
		return true;
	}

	// Handles the number generated during its trip down as it leaks out of the faucet
	public virtual void DripFX(){
		// Let NumberGenerator operate as normal. When a number is generated let it "drip" for dripTime, then fall onto the ground. Don't combine usually.

		if (dripNumber.transform.parent == null){
			dripNumber = null;
			return;
		}

		dripNumber.transform.position = dripStartT.position + (Vector3.up * -dripTime);
		dripTime += Time.deltaTime;
		if (dripTime > maxDripTime){
			gameObject.SendMessage ("OnNumberDrip",dripNumber,SendMessageOptions.DontRequireReceiver);
			dripNumber.transform.position += Random.insideUnitSphere * .4f;

			dripNumber=null;
			AudioManager.inst.PlayDrip(transform.position,Random.Range (.4f,.6f),Random.Range (.4f,.5f));
		}
	}

	void BubblesFX(){
		bubblestimer += Time.deltaTime;
		if (bubblestimer > randTime){
			bubblestimer =  0;
			randTime = Random.Range (0,9);
			AudioManager.inst.PlayBubbles(transform.position,Random.Range (.8f,1),Random.Range (.8f,1));
		}

	}

	// Actually creates a number, which will be dripped.
	virtual public void GenerateNumber(Fraction f){
		
		genTimer = interval;
		dripNumber = NumberManager.inst.CreateNumber(f,dripStartT.position);
//		createdNumbers.Add(dripNumber);
//		dripNumber.AddComponent<ExpireOverTime>();
		if (soaped) dripNumber.GetComponent<NumberInfo>().SoapNumber();
		if (oneball) {
			dripNumber.GetComponent<NumberInfo>().SetFaucetRel(this);
			dripNumber.GetComponent<UserEditableObject>().isSerializeableForSceneInstance = false;
//			dripNumber.GetComponent<UserEditableObject>().isEphemeral = false;
		}
		generatedNumber = dripNumber;
		dripNumber.transform.localScale = Vector3.one * GameConfig.inst.numberScale;
		dripTime = 0;
		dripNumber.transform.parent = transform;
	}

	public bool HaveChild(){
		TryLoseChild();
		if (GetComponentInChildren<NumberInfo>() != null) return true;
		else return false;
	}

	void TryLoseChild(){
		float loseChildDistance = 10f;
		foreach(NumberInfo ni in GetComponentsInChildren<NumberInfo>()){
			if (Vector3.SqrMagnitude(ni.transform.position - transform.position) > loseChildDistance * loseChildDistance){
				ni.transform.SetParent(null);
			}
		}
	}

	public void SetFraction(Fraction f){
		frac = f;
		SetSkyCamText();
	}


	//This rel is used to track when oneballs combine with each other
	// e.g. if there are two oneball faucets side by side, dropping 1 and 2 respectively, when they combine to make 3
	// neither faucet should want to generate a new number
	// so each fauct considers the new "3" ball as an original child of its own
	public void SetOneballRel(NumberInfo ni){
		if (ni) generatedNumber = ni.gameObject;
		else generatedNumber = null;
//		Debug.Log(name+" received oneball rel from ni:"+ni);
	}

	public CCText skyCamText;
	public Transform skyCamTextParent;
	virtual public void SetSkyCamText(){
		if (skyCamText && skyCamTextParent) {
			if (Fraction.ReduceFully(frac).denominator == 1){
				skyCamText.Text = Fraction.ReduceFully(frac).numerator.ToString();	
			} else {
				skyCamText.Text = Fraction.ReduceFully(frac).ToString();
			}

			skyCamText.Color = frac.numerator > 0 ? Color.white : Color.black;
			skyCamTextParent.GetComponent<Renderer>().material.color = frac.numerator > 0 ? new Color(0,0,0,0.2f) : new Color(1,1,1,0.2f);
		}
	}

}
