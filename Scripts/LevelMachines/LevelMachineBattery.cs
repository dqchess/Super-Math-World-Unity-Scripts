using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachineBattery : MonoBehaviour {

	public static string currentChargeKey = "CurrentChargeKey"; // was: "EnergyDoor_CurrentChargeKey", will break existing instances with charge.

	public Transform indicatorArrow;
	public Material altBeakerMaterial;
	Material origBeakerMaterial;
	public CCText indicatorTextWhole;
	public CCText indicatorTextFractionNumerator;
	public CCText indicatorTextFractionLine;
	public CCText indicatorTextFractionDenominator;
	public Fraction totalChargeCollected = new Fraction(0,1);
	public Fraction maxCharge = new Fraction(1,1);
	public Transform inputTransform;
	public Transform liquidLevel;
	List<GameObject> charge = new List<GameObject>();
	float newY=0;
	public Transform top;
	public Transform bottom;
	public Transform emptyPositionT;
	public GameObject[] machinesToStart;
	public bool finished=false;
	public CCText maxNumberText;
	public CCText overflowText;
	public GameObject resetLever; // will be destroyed when battery finished.

	void Start(){
		overflowText.gameObject.SetActive(false);
		origBeakerMaterial = liquidLevel.GetComponent<Renderer>().material;
//		SetMaxCharge(maxCharge);
//		SetIndicatorText();

	}

	void OnTriggerEnter(Collider other){

		OnReceivedNumber(other);

	}

	public void SetMaxCharge(Fraction max){
//		Debug.Log("setting max charge:"+max);
		max = Fraction.ReduceFully(max);//  Fraction.Add (max,new Fraction(0,1)); // reduce
		maxCharge = max;
		if (max.denominator > 1){
			maxNumberText.Text = max.numerator + "/" + max.denominator;
		} else {
			maxNumberText.Text = max.numerator.ToString();
		}
//		Debug.Log("max set:"+maxCharge);
	}


	float recTimer=0;
	public void OnReceivedNumber(Collider other){

		if (finished) {
			// commented Debug.Log ("finisheD");
			return;
		}
		if (recTimer > 0) {
//			// commented Debug.Log ("rectimer");
			return;
		}

		ResourceNumber rn = other.GetComponent<ResourceNumber>();
		if (rn){
			recTimer = 1f;

			NumberInfo ni = other.GetComponent<NumberInfo>();
			totalChargeCollected = Fraction.Add(ni.fraction,totalChargeCollected);

			charge.Add(other.gameObject);
			GrabNewNumber(other.gameObject,ni);
			if (Fraction.AbsGreater(totalChargeCollected,maxCharge)){
				StartCoroutine(OverfillAndEmpty());
//				EmptyBattery();
			} else{

			}
		}
	}

	void SetIndicatorText(){
		float whole = Mathf.Floor (totalChargeCollected.numerator/totalChargeCollected.denominator);
		if (whole == 0){
			indicatorTextWhole.Text = "";
		} else {
			indicatorTextWhole.Text = whole.ToString();

		}

		Fraction part = Fraction.Subtract(totalChargeCollected,new Fraction(whole,1));

		if (part.numerator==0){
			indicatorTextFractionLine.Text = "";
			indicatorTextFractionNumerator.Text = "";
			indicatorTextFractionDenominator.Text = "";
		} else {
			indicatorTextFractionLine.Text = "_";
			indicatorTextFractionNumerator.Text = part.numerator.ToString();
			indicatorTextFractionDenominator.Text = part.denominator.ToString();
		}
	}

	void Update(){
		recTimer-=Time.deltaTime;
		float heightDiff = indicatorArrow.position.y - bottom.position.y;
		if (Mathf.Abs(heightDiff -newY)>.01f){
//			// commented Debug.Log ("newy moving: "+heightDiff+","+newY);
			float lerpSpeed=4;
			float lerpY = Mathf.Lerp(indicatorArrow.position.y,bottom.position.y+newY,Time.deltaTime*lerpSpeed);
			indicatorArrow.position = new Vector3(indicatorArrow.position.x,lerpY,indicatorArrow.position.z);
			float indicatorDiff = indicatorArrow.transform.position.y - bottom.position.y;
			float h = top.position.y - bottom.position.y;
			liquidLevel.transform.localScale = new Vector3(1,1,indicatorDiff/h + .06f);
		}
	}

	IEnumerator OverfillAndEmpty(){
		yield return new WaitForSeconds(1.5f);
		AudioManager.inst.PlayWrongAnswerError(transform.position,1,1);
		float t=0;
		bool swap=true;
		overflowText.gameObject.SetActive(true);
		while (t < 3.5f){
			yield return new WaitForSeconds(.2f);
			t+=.2f;
			swap = !swap;
			if (swap) {
				liquidLevel.GetComponent<Renderer>().material = origBeakerMaterial;
				overflowText.Color = Color.black;
			} else {
				liquidLevel.GetComponent<Renderer>().material = altBeakerMaterial;
				overflowText.Color = Color.white;
			}
		}
		overflowText.gameObject.SetActive(false);
		EmptyBattery();



	}

	public void EmptyBatteryResetLever(){
		if (!finished) {
			EmptyBattery();
		}
	}

	void EmptyBattery(){ 
		// can be called when battery is over-filled (player puts in too much energy, more than the max)
		// can be called by a lever with SendMessage() (player put in some energy, but wants the energy back)

//		indicatorTextWhole.Text = "0";

		AudioManager.inst.PlayStaticDischarge(transform.position,1,1);
		AudioManager.inst.PlayWrongAnswerError(transform.position,1,1);
//		AudioManager.inst.PlayWrongAnswer(transform.position);
		LightningFX(liquidLevel,4);
		Fraction ensureMaxChargePossible = new Fraction(0,0);
		Fraction extraBit = new Fraction(0,0);
		for (int i=0;i<charge.Count;i++){
			Vector3 pos = emptyPositionT.position + emptyPositionT.forward * (i*5);
			Fraction f = charge[i].GetComponent<NumberInfo>().fraction;
			MakeReplacementPiece(f,pos,i);

//			GameObject repsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//			repsphere.transform.position = pos;
//			repsphere.name = "repsphere:"+i;
//			repsphere.collider.isTrigger=true;
//			if (Fraction.Greater(Fraction.Add (f,ensureMaxChargePossible),maxCharge)){
//				extraBit = Fraction.Subtract(maxCharge,ensureMaxChargePossible);
//			}
//			if (Fraction.Greater(ensureMaxChargePossible,maxCharge)){
//				Fraction overAmount = Fraction.Subtract(ensureMaxChargePossible,maxCharge);
//				Fraction underAmount = Fraction.Subtract(f,overAmount);
//
//				extraBit = underAmount;
//			}
//			ensureMaxChargePossible = Fraction.Add (ensureMaxChargePossible,f);

		}
//		Vector3 pos2 = transform.position - transform.right*22 + transform.up*5 + transform.forward * (charge.Count*5);
//		int index2 = charge.Count;
//		MakeReplacementPiece(extraBit,pos2,index2);
		totalChargeCollected = new Fraction(0,1);
		SetIndicatorText();
//		charge = new List<GameObject>();
		newY = 0;
//		// commented Debug.Log("newY: zero:");
		charge.Clear();
	}

	void MakeReplacementPiece(Fraction f, Vector3 pos, int i){
		GameObject rep = NumberManager.inst.CreateNumber(f,pos,NumberShape.Tetrahedron);
//		rep.transform.localScale = Vector3.one;
//		rep.AddComponent<ResourceNumber>();
		rep.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(charge[i].transform.position-transform.position)*100);
	}

	void GrabNewNumber(GameObject other,NumberInfo ni){

//		ni.SetStability(true);
//		ni.dying=false;
//		ni.bNeedsWobble=false;

		other.transform.localScale = Vector3.one;
		// Make it non-numbery
		Destroy(other.GetComponent<Collider>());
		Destroy(other.GetComponent<Rigidbody>());
		other.layer = 0;
		EnemyBrick eb = other.GetComponent<EnemyBrick>();
		if (eb) Destroy(eb);
		
		
		// Move new number
		other.transform.position = inputTransform.position;
		other.transform.rotation = inputTransform.rotation;
		SMW_GF.inst.MoveObjTo(other,inputTransform.position+inputTransform.forward*2f,1,false,.9f);

		StartCoroutine(PostGrabNumber(other));

		AudioManager.inst.PlayCartoonEat(transform.position,1);
		
	}

	public IEnumerator PostGrabNumber(GameObject other){
		yield return new WaitForSeconds(1);

		AudioManager.inst.PlayPlungerSuck(transform.position,1);
		other.gameObject.SetActive(false);
		SetChargeLevel(totalChargeCollected);

	}

	public void SetChargeLevel(Fraction f, bool levelWasJustLoaded=false){
		totalChargeCollected = f;
		SetIndicatorText();
		float h = top.position.y - bottom.position.y;
		float max = (float)maxCharge.numerator/(float)maxCharge.denominator;
		float cur = (float)totalChargeCollected.numerator/(float)totalChargeCollected.denominator;
		newY = cur / max * h;

		//		// commented Debug.Log ("newy: "+newY);
		if (cur == max){
			WebGLComm.inst.Debug("battery charge set:"+f+", lev just loaded:"+levelWasJustLoaded);
			MachineSolved(levelWasJustLoaded);
		}
	}

	void MachineSolved(bool levelWasJustLoaded =false){
		foreach(GameObject o in machinesToStart){
			UserEditableObject ueo = o.GetComponent<UserEditableObject>();
			ueo.StartMachine(levelWasJustLoaded);

		}
		finished=true;
		Destroy(resetLever);
		AudioManager.inst.PlayElectricArc(transform.position,1,1);
		StartCoroutine(LightningFX(liquidLevel,10,5));
		StartCoroutine(LightningFX(indicatorArrow,15,3));
		StartCoroutine(LightningFX(machinesToStart[0].transform,10,7));
		SinTransparency st = liquidLevel.gameObject.AddComponent<SinTransparency>();
		st.amplitude=1.1f;
		st.frequency = 7;
//		indicatorArrow.gameObject.SetActive(false);

	}

	IEnumerator LightningFX(Transform origin, int maxStrikes=10, float maxTime=.5f){
		Collider[] cols = Physics.OverlapSphere(origin.position,25); // should be raycasts
		int i=0;
		yield return new WaitForSeconds(.1f);
		foreach(Collider col in cols){
			if (Random.Range(0,10)>5){
				yield return new WaitForSeconds(Random.Range(.1f,.6f));
				if (col) SMW_GF.inst.CreateLightning(origin,col.transform,Random.Range(.3f,maxTime));

				i++;
           	}
			if (i>maxStrikes) yield return false;
		
		}
	}
}
