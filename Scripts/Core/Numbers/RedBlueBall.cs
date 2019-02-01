using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RedBlueColor {
	Red,
	Blue
}

public class RedBlueBall : MonoBehaviour {
	class CollisionPair2 {
		public RedBlueBall a;
		public RedBlueBall b;
		public CollisionPair2(RedBlueBall _a, RedBlueBall _b) { a = _a; b = _b; }
		public CollisionPair2 Swap(CollisionPair2 s){
			return new CollisionPair2(s.b,s.a);
		}
	}

	List<CollisionPair2> existing = new List<CollisionPair2>();
	static List<CollisionPair2> collisionPairs = new List<CollisionPair2>();

	public static void ResolveCollisions() {

		if(collisionPairs.Count <= 0) return;
		List<CollisionPair2> existing = new List<CollisionPair2>();

		//		string existingStr = "";
		foreach(CollisionPair2 p in collisionPairs) {
			if (existing.Contains(p)) continue;
			if (p.a == null || p.b == null) continue;
			existing.Add(p);
			//			existingStr += " " + p.a.name+", "+p.b.name+" ..";
		}

		//		// commented Debug.Log("resolve col. existing;"+existingStr);
		foreach(CollisionPair2 p in existing) {
			ResolveCollision(p);
		}
		collisionPairs = new List<CollisionPair2>();
	}


	public RedBlueColor color = RedBlueColor.Blue;

	void OnCollisionEnter(Collision hit){
		Collider other = hit.collider;
		RedBlueBall rbb = other.GetComponent<RedBlueBall>();
		if (rbb && rbb.color != color){
			// two balls of opposite kind hit.

		}
	}

	static void ResolveCollision(CollisionPair2 pair) {

		if(pair.a == null || pair.b == null || pair.a.gameObject == null || pair.b.gameObject == null){
			// commented Debug.Log("null!");
			return;
		}

		RedBlueBall a = pair.a;
		RedBlueBall b = pair.b;
		if (a.color != b.color){
			EffectsManager.inst.CreateSmallPurpleExplosion(a.transform.position,2,2);
			EffectsManager.inst.CreateSmallPurpleExplosion(b.transform.position,2,2);
			AudioManager.inst.PlayWrongAnswerError(a.transform.position,1,1);
		}
	}
}
