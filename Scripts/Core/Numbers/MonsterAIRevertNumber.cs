using UnityEngine;
using System.Collections;

public class MonsterAIRevertNumber : MonoBehaviour {

	NumberInfo ni;
	private Fraction origFrac;
	public Fraction GetOriginalFraction(){
		return origFrac;
	}
	public bool bNeedsRevert = false;
	float revertCountdownTimer = 3.5f;
	Vector3 origScale;
	Material rainbowMat;



 	void Start () {
		ni = GetComponent<NumberInfo>();
		if (ni.rainbowFX) rainbowMat = ni.rainbowFX.GetComponent<Renderer>().material;
		SetNumber(ni.fraction); //
//		origScale = Vector3.one;
		if (!ni) {
			name += "err";
			// commented Debug.Log ("name: "+name);
		} else if (!ni.childMeshRenderer){
			name += "err2";
			// commented Debug.Log ("name: "+name);
		}
		origScale = ni.childMeshRenderer.transform.localScale;


		if (ni) {
//			if(oldMat == null)
//			{
//				oldMat = ni.childMeshRenderer.sharedMaterial;
//				// commented Debug.Log ("set old mat to :" +oldMat);
//			}
			ni.numberChangedDelegate += ResetTimer;
		}
		else Destroy(this);

	
	}
	
	// Update is called once per frame
	float lastCheckTime = 0;
	float checkInterval = 0.5f;

	float waitTimer=0;
	
//	Material oldMat;
	float coolDownTimer = 0.0f;
	
	void Update () {
		checkTimer -= Time.deltaTime;
		CheckNumber();
		if (lerpRainbowAlpha && rainbowMat){
			rainbowMat.SetFloat("alpha",Mathf.Lerp(rainbowMat.GetFloat("alpha"),targetRainbowAlpha,Time.deltaTime * rainbowAlphaLerpSpeed));
			if (Mathf.Abs(rainbowMat.GetFloat("alpha")-targetRainbowAlpha) < .01f){
				rainbowMat.SetFloat("alpha",targetRainbowAlpha);
				if (targetRainbowAlpha == 0) {
					ni.rainbowFX.SetActive(false);
				}
				lerpRainbowAlpha = false;

			}
		}

		if (bNeedsRevert) {
			if (!ni) Destroy (this);
//			waitTimer += Time.deltaTime;
//			if (waitTimer < 0f) return;

			// float sineSpeed = 5;
			// float totalFlux = .1f;
			// float sineIncreaseSpeed = Mathf.Pow(1/Mathf.Max(0.2f,revertCountdownTimer),1.01f);
			// float sineF = 1 + totalFlux*Mathf.Sin(Time.time*sineIncreaseSpeed*sineSpeed);
			float t = 3.5f - revertCountdownTimer;
//			ni.childMeshRenderer.material.SetColor("_Glow", new Color(0.5f, 0.5f, 0.5f, t/5 + 0.1f));
		
			revertCountdownTimer -= Time.deltaTime;




			if (revertCountdownTimer <= 0){
				//
				RevertNumber();

			}
		}
		else if(coolDownTimer > 0 && ni != null) 
		{
//			ni.childMeshRenderer.material.SetColor("_Glow", new Color(0.5f, 0.5f, 0.5f, coolDownTimer*2));
			coolDownTimer -= Time.deltaTime;
			if(coolDownTimer <= 0) {
//				ni.childMeshRenderer.material = oldMat;

			}
		}
		
	}


	public void ResetTimer(NumberInfo ni){
//		waitTimer = 0;
		revertCountdownTimer = Random.Range(3.5f,4f);
	}

	bool lerpRainbowAlpha = false;
	float rainbowAlphaLerpSpeed = 1;
	float checkTimer = 0;
	float targetRainbowAlpha = 0;


	public void SetNumber(Fraction f){
		origFrac = f;
	}

	void CheckNumber(){
		if (checkTimer > 0) return;
		checkTimer = .3f; // are we srsly? TODO: Move this to a delegate.
//		// commented Debug.Log ("checked.");
		if (Fraction.Equals(ni.fraction,origFrac)) {
			bNeedsRevert=false;
			ni.rainbowFX.SetActive(false);
			return; // no changes needed, number did not change
		} else if (!bNeedsRevert){ // if we weren't already reverting/wobbling, and fraction was NOT EQUAL to what it should be ..
			audioIndexUsed = AudioManager.inst.PlayIceCrackle(transform.position);
			bNeedsRevert = true;
//			AudioManager.inst.PlayWrongAnswer(transform.position);
			ResetTimer(ni);
			if (ni.rainbowFX){
				ni.rainbowFX.SetActive(true);
				rainbowAlphaLerpSpeed = 8;

				int rainbowAlphaDir = ((ni.fraction.numerator > origFrac.numerator) ? -1 : 1); // note this reverses the FX for whether the number is reverting to higher or lower
				rainbowMat.SetInt("direction",rainbowAlphaDir);
				float rainbowAlphaSpeed = Mathf.Max(1.0f,Mathf.Pow(Mathf.Abs(origFrac.numerator - ni.fraction.numerator),0.1f)+0.5f);
//				Debug.Log("sd:"+rainbowAlphaSpeed);
				rainbowMat.SetFloat("speed",rainbowAlphaSpeed);
				rainbowMat.SetFloat("alpha",0);
				targetRainbowAlpha = 0.8f;
				lerpRainbowAlpha = true;
			}

		}
		
	}
	
	public void RevertNumber(){
		if (bNeedsRevert){
	//		ni.childMeshRenderer.material.SetTexture("_CrackTex", null);
			coolDownTimer = 1.0f;
			rainbowAlphaLerpSpeed = 4.5f;
			lerpRainbowAlpha = true;
			targetRainbowAlpha = 0;


	//		ModifyNumber(GetComponent<NumberInfo>());
			ni.SetNumber(origFrac); // should reset material too?
			AudioManager.inst.PlayCrystalThump1(transform.position);
			bNeedsRevert = false;
			GetComponent<NumberInfo>().childMeshRenderer.transform.localScale = origScale; // otherwise it might get stuck haflway during a bulge.
			EffectsManager.inst.RevertSparks(transform.position,transform.localScale.x);
	//		// commented Debug.Log("setting color:"+ni.name+", val="+ni.fraction);
	//		ni.SetColor();
		}

	}
	


	int audioIndexUsed=-1;
	void ReturningToPool() {
		if (audioIndexUsed != -1) AudioManager.inst.StopAudioAt(audioIndexUsed); 
		Destroy (this);
	}
	void OnDestroy(){
//		if (ni)
		if (audioIndexUsed != -1) AudioManager.inst.StopAudioAt(audioIndexUsed); 

	}
}
