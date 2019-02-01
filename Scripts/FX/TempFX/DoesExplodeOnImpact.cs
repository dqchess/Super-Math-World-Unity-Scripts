using UnityEngine;
using System.Collections;

public class DoesExplodeOnImpact : MonoBehaviour {

	GameObject smokeTrail;
	public Transform excludeRootFromRocketExplosion;
	void Start(){
		name += " at time "+Time.time;
		smokeTrail = (GameObject)Instantiate(EffectsManager.inst.smokeTrail,transform.position,Quaternion.identity); // add smoke trail
		smokeTrail.name = "smokeTrail";
		smokeTrail.transform.parent = transform;
	}

	bool used = false;
	void OnCollisionEnter(Collision hit){
//		// commented Debug.Log("hit and exp:"+hit.collider.name);
		if (hit.collider.transform.root == excludeRootFromRocketExplosion) {
//			// commented Debug.Log("don't collide with this");
			return;
		}
		if (used) return;
		used = true;
		Collider other = hit.collider;
		gameObject.SendMessage("OnExplode",SendMessageOptions.DontRequireReceiver);

//		// commented Debug.Log("collision! from "+name+", col time:"+Time.time);
		RocketExplosionEffect.MakeRocketExplosion(GetComponent<NumberInfo>(), transform.position, excludeRootFromRocketExplosion);
		smokeTrail.transform.parent=null;
		smokeTrail.GetComponent<ParticleSystem>().emissionRate = 0;
		Destroy (gameObject);
	}

	void OnDestroy(){
		if (smokeTrail){
			TimedObjectDestructor tod = smokeTrail.AddComponent<TimedObjectDestructor>();
		}
//		tod.DestroyNow(2.5f);
//		Destroy (smokeTrail);
	}
}
