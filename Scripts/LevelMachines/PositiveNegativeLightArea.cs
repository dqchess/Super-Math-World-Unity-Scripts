using UnityEngine;
using System.Collections;

public enum LightAreaType{
	Positive,
	Negative
}

public class PositiveNegativeLightArea : NumberModifier {

	public NumberModifier.ModifyOperation modifyOp = x => x;
	public LightAreaType lightAreaType = LightAreaType.Positive;
	public GameObject lightGraphics;
	public ParticleEmitter rainEffect;
//	float checkRadius =10;
	bool bNeedsFade = false;
	Color oldColor;
	// Use this for initialization
	public override void Start () {
		base.Start();
		oldColor = lightGraphics.GetComponent<Renderer>().material.color; // save it
		lightGraphics.GetComponent<Renderer>().material.color = new Color(oldColor.r,oldColor.g,oldColor.b,0); // clear it
		DripParticles();
	}
	
	// Update is called once per frame
	float fadeStartTime = 0;
	float fadeDuration = 1;
	float lastFlickerTime = 0;
	float flickerInterval = .1f;
	float triggerTimeout=0;
	void Update () {
		triggerTimeout -= Time.deltaTime;
//		Bounds myBounds = collider.bounds; // check bounds
//		Collider[] colliders = Physics.OverlapSphere(transform.position,checkRadius);
//		foreach (Collider col in colliders){

//		}
		
		if (bNeedsFade){
			float fadeSpeed = 1f;
			
			lightGraphics.GetComponent<Renderer>().material.color = Color.Lerp(lightGraphics.GetComponent<Renderer>().material.color,new Color(oldColor.r,oldColor.g,oldColor.b,0),Time.deltaTime*fadeSpeed);
			if (Time.time > fadeStartTime + fadeDuration){
				bNeedsFade=false;
				DripParticles();
			}
		} else {
			// "flicker" the fade cone
			
			if (Time.time > lastFlickerTime + flickerInterval){
				lastFlickerTime = Time.time;
				float randFloat = Random.Range(0.1f,0.2f);
				lightGraphics.GetComponent<Renderer>().material.color = new Color(oldColor.r,oldColor.g,oldColor.b,randFloat);
				
			}
		}
	}
	
	float triggerTime = 0;
	float triggerWarmUpDelay = .2f;
	
	
//	IEnumerator DelayedSwitch(Collider col) {
//		yield return null;
//		if (Time.time > triggerTime + triggerWarmUpDelay){
//			triggerTime=Time.time;
//			if (!col) yield return false;
//			Transform t = col.transform;
//			yield return new WaitForSeconds(triggerWarmUpDelay); 
//			
//			if (null != t && null != col){
//			// still get nulls for collider..ouch. Use DestroyedThisFrame?
//			
//				NumberInfo ni = col.gameObject.GetComponent<NumberInfo>();
//				if (lightAreaType == LightAreaType.Positive && ni.fraction.numerator < 0 
//				|| lightAreaType == LightAreaType.Negative && ni.fraction.numerator > 0) {
//					ModifyNumber(ni);
//				}
//			}
//		}
//	}
//	
	public override Fraction GetModifiedFraction (Fraction original)
	{
		if(lightAreaType == LightAreaType.Positive) {
			return Fraction.GetAbsoluteValue(original);
		}
		else {
			return Fraction.Multiply(new Fraction(-1,1),Fraction.GetAbsoluteValue(original));
		}
	}
	
	public override string GetEquation (Fraction original)
	{
		if(lightAreaType == LightAreaType.Positive) {
			return "|" + original + "| = " + GetModifiedFraction(original);	
		}
		else {
			return "-|" + original + "| = " + GetModifiedFraction(original);	
		}
	}
	
	
	void OnTriggerEnter(Collider col){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
//		// commented Debug.Log ("trigger entered:"+col.name);
		NumberInfo ni = col.GetComponent<NumberInfo>();
		if (ni){
			ActivateEffects();
			if (lightAreaType == LightAreaType.Negative) ni.SetNumber(Fraction.Multiply(new Fraction(-1,1),Fraction.GetAbsoluteValue(ni.fraction)),false); 
			if (lightAreaType == LightAreaType.Positive) ni.SetNumber(Fraction.Multiply(new Fraction(1,1),Fraction.GetAbsoluteValue(ni.fraction)),false); 


//			NumberModifier.ModifyOperation nmf = (x => x);
//			if (lightAreaType == LightAreaType.Negative && ni.fraction.numerator > 0) nmf = (x => Fraction.Multiply(x, new Fraction(-1, 1)));
//			else if  (lightAreaType == LightAreaType.Negative && ni.fraction.numerator < 0) nmf = (x => Fraction.Multiply(x, new Fraction(1, 1)));
//			else if  (lightAreaType == LightAreaType.Positive && ni.fraction.numerator > 0) nmf = (x => Fraction.Multiply(x, new Fraction(1, 1)));
//			else if  (lightAreaType == LightAreaType.Positive && ni.fraction.numerator < 0) nmf = (x => Fraction.Multiply(x, new Fraction(-1, 1)));
//			modifyOp = nmf;
//			ModifyNumber(ni);

		} else if (col.tag=="Player"){
			ActivateEffects();
//				// commented Debug.Log("Player triggered positive area");
			NumberModifier.ModifyOperation nmf = (x => x);
			if (lightAreaType == LightAreaType.Positive){
				nmf = (x => Fraction.GetAbsoluteValue(x));
			} else if (lightAreaType == LightAreaType.Negative){
				nmf = (x => Fraction.Multiply(new Fraction(-1,1),Fraction.GetAbsoluteValue(x)));
			}
			
			Inventory.inst.ModifyInventoryItems(nmf);
		}
	}
//
//	void ModifyInventory(int posneg){
//		foreach(GameObject o in GlobalVars.inst.inv.spaces){
//			PlayerInventorySpace sp = o.GetComponent<PlayerInventorySpace>();
//			if (sp.heldItem){
//				NumberInfo ni=sp.heldItem.GetComponent<NumberInfo>();
//				if (ni){
//					ni.SetNumber(new Fraction((posneg)*Mathf.Abs (ni.fraction.numerator),ni.fraction.denominator));
//					//					sp.
//				}
//			}
//		}
//		foreach(GameObject o in GlobalVars.inst.inv.beltSpaces){
//			PlayerInventorySpace sp = o.GetComponent<PlayerInventorySpace>();
//			if (sp.heldItem){
//				NumberInfo ni=sp.heldItem.GetComponent<NumberInfo>();
//				if (ni){
//					ni.SetNumber(new Fraction((posneg)*Mathf.Abs (ni.fraction.numerator),ni.fraction.denominator));
//					// commented Debug.Log ("set belt space:" +sp+" to:" + ni.fraction);
//					//					sp.
//				}
//			}
//		}
//		// commented Debug.Log ("?");
//		GlobalVars.inst.inv.UpdateBeltGraphics();
//		GlobalVars.inst.inv.UpdateInventoryGraphics();
//	}
	
	void ActivateEffects(){
		AudioManager.inst.PlayElectricDischarge1(transform.position);
		Color oldColor = lightGraphics.GetComponent<Renderer>().material.color;
		lightGraphics.GetComponent<Renderer>().material.color = new Color(oldColor.r,oldColor.g,oldColor.b,.8f); // set color to non-transparent
		bNeedsFade = true;
		fadeStartTime = Time.time;
		RainParticles();
	}
	
	void DripParticles(){
		rainEffect.minEmission=0.4f;
		rainEffect.maxEmission=0.4f;
		rainEffect.worldVelocity = new Vector3(0,3,0);
		rainEffect.rndVelocity = new Vector3(2,2,2);
	}
	
	void RainParticles(){
		rainEffect.minEmission=20;
		rainEffect.maxEmission=20;
		rainEffect.worldVelocity = new Vector3(0,9,0);
		rainEffect.rndVelocity = new Vector3(5,5,5);
	}
}
