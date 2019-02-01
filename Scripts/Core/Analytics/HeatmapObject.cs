using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeatmapObject : MonoBehaviour {

	public class HeatmapCollisionPair {
		public HeatmapObject a;
		public HeatmapObject b;
		public HeatmapCollisionPair(HeatmapObject _a, HeatmapObject _b) { a = _a; b = _b; }

	}

	public HeatmapAvatar owner;

	float combineRadius = 3f;
	public void CheckNeighbors(){
		name = "HeatmapObject @ " +Time.time;
		foreach(Collider other in Physics.OverlapSphere(transform.position,combineRadius)){
			
			HeatmapObject hmo = other.GetComponent<HeatmapObject>();
			if (hmo && hmo.owner != this.owner){
				HeatmapObject.collisionPairs.Add(new HeatmapCollisionPair(this,hmo));
//				Debug.Log("add:"+hmo);
			} else {
				
			}
		}
		HeatmapObject.ResolveCollisions();
	}

	public static List<HeatmapCollisionPair> collisionPairs = new List<HeatmapCollisionPair>();
	public static void ResolveCollisions() {
//		Debug.Log("resolving?");
		if(collisionPairs.Count <= 0) return;
		List<HeatmapCollisionPair> existing = new List<HeatmapCollisionPair>();
		List<HeatmapObject> existingHmo = new List<HeatmapObject>();
		//		string existingStr = "";
		foreach(HeatmapCollisionPair p in collisionPairs) {
			if (existingHmo.Contains(p.a) || existingHmo.Contains(p.b)) continue;
			if (p.a == null || p.b == null) continue;
			existingHmo.Add (p.a);
			existingHmo.Add (p.b);
			existing.Add(p);
//			Debug.Log("added:"+p);
			//			existingStr += " " + p.a.name+", "+p.b.name+" ..";
		}


		foreach(HeatmapCollisionPair p in existing) {
			ResolveCollision(p);
		}
		collisionPairs = new List<HeatmapCollisionPair>();
	}

	static void ResolveCollision(HeatmapCollisionPair p){
		p.a.gameObject.AddComponent<SinGrow>();
//		Debug.Log("resolving:"+p.a);
		// get volumes
		float vol = Mathf.Pow(p.b.transform.lossyScale.x,3);
		float otherVol = Mathf.Pow(p.a.transform.lossyScale.x,3);

		// avg colors
		p.b.GetComponent<Renderer>().material.color = (p.a.GetComponent<Renderer>().material.color + p.b.GetComponent<Renderer>().material.color)/2f; // average the color

		// avg positions by weight
		Vector3 centerPoint = (p.a.transform.position + p.b.transform.position)/2f;
		float dist = Vector3.Distance(p.a.transform.position,p.b.transform.position);
		Vector3 dirToHeavier = (otherVol> vol ? p.a.transform.position - p.b.transform.position : p.b.transform.position - p.a.transform.position).normalized;
		float ratioOfHeavier = otherVol > vol ? otherVol/vol : vol/otherVol;
		Vector3 avgPositionByWeight = centerPoint + dirToHeavier * (dist/2f - (1/ratioOfHeavier)*dist/2f);
		p.b.transform.position = avgPositionByWeight;


		// avarege their volumes
		float newVolume = vol + otherVol;
		float cubeRoot = Mathf.Pow(newVolume,0.333f);
		p.b.gameObject.transform.localScale = Vector3.one * cubeRoot;

		// destroy other one
		Destroy(p.a.gameObject);
	}

}
