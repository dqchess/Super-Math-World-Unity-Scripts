using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveInfoPercentShield : MonoBehaviour {
	
	public Fraction waveFrac1;
	public CCText waveText;
	
	List<NumberInfo> touched;
	public void SetWaveInfo(Fraction f){
		waveFrac1 = f;
		waveText.Text = Mathf.RoundToInt(waveFrac1.GetAsPercent())+"%";
	}
	// Use this for initialization
	void Start () {

//		string s = "";
//		s += "x ";
//		s += waveFrac1.ToString() + " ";
//		waveText.Text = s;
		touched = new List<NumberInfo>();
	}


	public void OnTriggerEnterO(GameObject o){
		OnTriggerEnter(o.GetComponent<Collider>());
	}
	List<MonsterAIBase> maibs = new List<MonsterAIBase>();
	public void OnTriggerEnter(Collider other){
//		if(Network.isClient) { return; }
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni && ni.enabled) {

//			if(touched.Contains(ni)) { return; }
//			touched.Add(ni);

			if (Fraction.Subtract(ni.fraction,waveFrac1).numerator == 0){
				Rigidbody rb = ni.GetComponent<Rigidbody>();
				if (ni.myShape == NumberShape.Sphere && !rb) rb = ni.gameObject.AddComponent<Rigidbody>();
				if (rb){
					float bounceForce = 10000f;
					rb.AddForce(transform.forward * bounceForce);
					EffectsManager.inst.CreateSmallPurpleExplosion(ni.transform.position,1,1);
					AudioManager.inst.PlayCrystalThump1(other.transform.position);
				}
//				Destroy(ni.gameObject);
			} else {
//				Debug.Log("hit frac:"+ni.fraction+" with wave:"+waveFrac1);
			}



//			ni.SetNumber(ni.fraction);
		}
	}
	

}
