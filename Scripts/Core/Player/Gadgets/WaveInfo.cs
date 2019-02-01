using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveInfo : MonoBehaviour {
	
	public Fraction waveFrac1;
	public CCText waveText;
	
	List<NumberInfo> touched;
	
	// Use this for initialization
	void Start () {

		string s = "";
		s += "x ";
		s += waveFrac1.ToString() + " ";
		waveText.Text = s;
		touched = new List<NumberInfo>();
	}


	public void OnTriggerEnterO(GameObject o){
		OnTriggerEnter(o.GetComponent<Collider>());
	}
	List<MonsterAIBase> maibs = new List<MonsterAIBase>();
	public void OnTriggerEnter(Collider other){
//		if(Network.isClient) { return; }
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni && ni.enabled){ 
			if(touched.Contains(ni)) { return; }
			touched.Add(ni);
			AudioManager.inst.PlayCrystalThump1(other.transform.position);

			int stackHeight = waveFrac1.numerator;
//			Debug.Log ("a starts:" +a+ " and " +ni.fraction);

//			ni.fraction = Fraction.Multiply(a,ni.fraction);

//			Debug.Log ("ni.fraction at this stage :" + ni.fraction);

//			Debug.Log ("a at this stage 2:" + a);
			MonsterAIBase maib = ni.transform.root.GetComponentInChildren<MonsterAIBase>();
			if (stackHeight == 1) {
				// nothing happens to target num when you multiply by 1

			} else if (stackHeight == -1){
				// we multiplied by -1
				// simply invert the number immediately (no stack)
				ni.SetNumber(Fraction.Multiply(new Fraction(stackHeight,1),ni.fraction));
			}else if (ni.fraction.numerator == -1 && ni.fraction.denominator == 1 ){
				// target num was -1
				ni.SetNumber(Fraction.Multiply(new Fraction(stackHeight,1),ni.fraction));
			}else if (ni.fraction.numerator == 1 && ni.fraction.denominator ==1 ){
				// target num was a 1, but the source num was not, so set the number immediate (no stack)
				// dry
				ni.SetNumber(Fraction.Multiply(new Fraction(stackHeight,1),ni.fraction));
			} else if (maib) {
//				Debug.Log("was maib");
				if (waveFrac1.numerator == 0){
					EffectsManager.inst.CreateShards(maib.transform.position);
					AudioManager.inst.PlayNumberShatter(maib.transform.position);
					Destroy(maib.gameObject);

				} else {
					AudioManager.inst.PlayWrongAnswerError(maib.transform.position,1,1);
				}
				return;
				if (!maibs.Contains(maib)){
					maibs.Add(maib);
					for(int i=0;i<stackHeight; i++){
						GameObject copy = (GameObject)Instantiate(maib.gameObject,maib.transform.right * i * 4f, maib.transform.rotation);
//						Debug.Log("copy");
					}
//					Debug.Log("amib:"+maib);
				}
			} else {
				// the regular case
				// perform an x by y operation on the number hit by the wave
				// create stack fx 
				NumberManager.inst.CreateNumberStack(ni,stackHeight);
			}

//			ni.SetNumber(ni.fraction);
			EffectsManager.inst.CreateSmallPurpleExplosion(ni.transform.position,1,1);
		}
	}
	

}
