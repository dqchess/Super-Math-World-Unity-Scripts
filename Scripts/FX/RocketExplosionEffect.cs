using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//public struct RocketHitInfo{
//
//	public float dist;
//	public NumberModifier.ModifyOperation nmf;
//}

public class RocketExplosionEffect : NumberModifier {



	public Transform excludeRootFromRocketExplosion = null;
	public Fraction rocketValue;
	public float radius = 1;
	public float maxRadius = 45;
	public Dictionary<float,GameObject> touched = new Dictionary<float,GameObject>();
	public List<NumberInfo> touchedNi = new List<NumberInfo>();
	public Transform extraparticles;
	NumberInfo myNi;
	public override Fraction GetModifiedFraction (Fraction original)
	{
//		Debug.Log ("adding:"+original+" plus "+myNi.fraction+"... resultl"+Fraction.Add(myNi.fraction,original).ToString());
		return Fraction.Add(myNi.fraction, original);
	}
	
	public override string GetEquation (Fraction original)
	{
		return original + " + " + myNi.fraction + " = " + GetModifiedFraction(original);
	}
	
		//		// commented Debug.Log ("yep, excluding:" +excludeRootFromRocketExplosion);
	public static void MakeRocketExplosion(NumberInfo ni, Vector3 pos, Transform excludeRootFromRocketExplosion = null) {

//		AudioManager.inst.PlayExplosion1(pos);
		GameObject rocketAOE = (GameObject)GameObject.Instantiate(EffectsManager.inst.rocketAOE, pos, Quaternion.identity);	
		RocketExplosionEffect aoe = rocketAOE.GetComponent<RocketExplosionEffect>();
		aoe.myNi = ni; // a bit roundabout
		if (excludeRootFromRocketExplosion != null) aoe.excludeRootFromRocketExplosion = excludeRootFromRocketExplosion;
//		aoe.rocketValue = num;

		
	}
	
	List<float> orderedObjectsDiff = new List<float>();
	Fraction result = new Fraction(-100,1);
	List<NumberStructureCreator> structuresHit = new List<NumberStructureCreator>();
	public float explosionForce = 90;
	public override void Start() {
		name = "AOE " + Random.Range(0,1000).ToString();
		base.Start();
//		Debug.Log("start on rocket aoe");
//		// commented Debug.Log ("Starting.."+Time.time);
		if(extraparticles)
			extraparticles.parent=null;
		
//		if(Network.isClient) { return; }
		
		// CVN moved to "Start" to avoid overlapsphere every frame
//		maxRadius*=3;
		Collider[] cols = Physics.OverlapSphere(transform.position, maxRadius);
//		Debug.Log("cols;"+cols.Length);
//		// commented Debug.Log("cols len:"+cols.Length);

		foreach(Collider c in cols){

//			continue;

			NumberInfo ni = c.GetComponent<NumberInfo>();
			if (!ni) continue;
			if (ni == myNi) continue;
			if (!ni.modifyable) continue;
			if (c.GetComponent<Animal>()) continue;
			if (c.transform.parent && c.transform.parent.GetComponentInChildren<MonsterSnail>()) continue;
			bool brk = false;
			foreach(RaycastHit hit in Physics.SphereCastAll(transform.position,0.5f,ni.transform.position-transform.position,maxRadius)){
				if (hit.collider.GetComponent<NumberDestroyer>()){
					EffectsManager.inst.EmitLightningBall(hit.point);
					brk = true; // there was a nopass wall between the explosion and the number in question.
				} else {
//					Debug.Log("safe;"+hit.collider.name);
				}
			}
			if (brk) continue;
			float diff = Vector3.SqrMagnitude(c.transform.position - transform.position) + Random.Range (-1f,1f); // how far was this from the rocket? Plus a random offset in case it's equadistant (happens sometimes.)
			if (touched.ContainsKey(diff)) continue; 
			if (touched.ContainsValue(c.gameObject)) continue;

			touched.Add (diff,c.gameObject);
//			Debug.Log(name + " added;"+c.gameObject.name);
			NumberStructureCreator nsc = c.transform.root.gameObject.GetComponent<NumberStructureCreator>();
			if (nsc && !structuresHit.Contains(nsc)){
				structuresHit.Add(nsc);
			}
			Rigidbody rb = c.GetComponent<Rigidbody>();
			if (rb){
				Vector3 force = Vector3.Normalize(c.transform.position - transform.position);

				rb.AddForce(force * explosionForce);
			}
//			// commented Debug.Log ("added;"+c.gameObject.name);


		}
		orderedObjectsDiff = touched.Keys.ToList();
		orderedObjectsDiff.Sort();

//		// commented Debug.Log ("ordered obj diff len:"+orderedObjectsDiff.Count+"; touched len:"+touched.Count);
	}
	
	float interval=.005f;
	float t=0f;
	float growSpeed = 40;

	void Update() {
		t -= Time.deltaTime;
		if (t<0){
//			// commented Debug.Log ("intervalling!");
			t=interval;
			int iMax = Random.Range (5,9);
			for (int i=0;i<iMax;i++){
				if (orderedObjectsDiff.Count > i) {
					if (touched[orderedObjectsDiff[i]] != null) {
//						// commented Debug.Log ("doing i:"+i);
						NumberInfo ni = touched[orderedObjectsDiff[i]].GetComponent<NumberInfo>();
//						// commented Debug.Log ("Adding from rocket. Ni hit;"+ni.fraction+", my frac;"+myNi.fraction+"; mod fracl"+GetModifiedFraction(ni.fraction));
//						// commented Debug.Log("ni set number from rocket:"+ni.fraction);
//						Debug.Log("mod;"+ni.fraction+" with:"+this.GetModifiedFraction(ni.fraction));
						ni.SetNumber(GetModifiedFraction(ni.fraction),true);

//						if (GetModifiedFraction(ni.fraction).numerator == 0){
////							// commented Debug.Log ("ZERO FX ");
//							ni.ZeroFX(ni.transform.position);
//						}
					}
					touched.Remove(orderedObjectsDiff[i]);
					orderedObjectsDiff.RemoveAt(i);
					if (orderedObjectsDiff.Count == 0){
						// After the explosion is finished, check the strucutre (numberwall?) we hit to see if it's missing too many blocks. If it is, "crumble" it!
						CheckMissingBlocksOnHitStructures();
					}
				}
			}
		}

		if (radius < maxRadius) radius += Time.deltaTime *growSpeed;
		if(radius >= maxRadius){
			GetComponent<Renderer>().enabled=false; 
//			if (orderedObjectsDiff.Count==0) { Destroy(gameObject); }
		}
		
		
		transform.localScale =  Vector3.one * ((maxRadius - radius) * 0.4f + radius * 1.2f);

		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(radius / 2, radius / 4);

		
	}

	void CheckMissingBlocksOnHitStructures(){
		foreach(NumberStructureCreator nsc in structuresHit){
			int startingBlocks = nsc.GetStartingBlocksCount();
			if (startingBlocks == 0) return; // some do not care, and the superclass returns 0 by default.. ugly
			int blocksNow = nsc.GetBlocksCount(); // GetComponentsInChildren<NumberInfo>().Length;
//			// commented Debug.Log("blocks:"+blocksNow+", starT:"+startingBlocks);
			#if UNITY_EDITOR
			if ((float)blocksNow / (float)startingBlocks < 0.7f){ // higher chance for explosions in editor when we may be taking video.
			#else
			if ((float)blocksNow / (float)startingBlocks < 0.5f){
			#endif
				DestroyStructure(nsc);
			}
		}
	}

	void DestroyStructure(NumberStructureCreator nsc){
		foreach(NumberInfo ni in nsc.GetComponentsInChildren<NumberInfo>()){
//			ni.transform.parent = null;
			Rigidbody rb = ni.GetComponent<Rigidbody>();
			if (!rb) rb = ni.gameObject.AddComponent<Rigidbody>();
			rb.mass = 20;
			ni.ForbidCombinationsForSeconds(5);
			rb.isKinematic = false;
			rb.useGravity = true;
			Vector3 force = Vector3.Normalize(ni.transform.position - transform.position);
			ni.GetComponent<Rigidbody>().AddForce(force * explosionForce);
//			PickUppableObject pip = ni.gameObject.GetComponent<PickUppableObject>();
//			if (pip) Destroy(pip);pip = ni.gameObject.AddComponent<PickUppableObject>();
//			pip.heldScale = 0.2f;
		}
//		Destroy(nsc);
	}
	
}
