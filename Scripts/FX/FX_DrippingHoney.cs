using UnityEngine;
using System.Collections;

public class FX_DrippingHoney : MonoBehaviour {

	public GameObject dropletPrefab;
	public GameObject fallingDropletPrefab;
	public GameObject splashParticlesPrefab;
	GameObject currentDroplet;



	// Use this for initialization
	void Start () {
	
	}

	void OnDrawGizmos(){
//		Gizmos.color = Color.cyan;
//		Gizmos.DrawSphere (transform.position,1);
	}

	float timer = 0;
	float totalDropTime = 10;
	float maxDropletSize = 0.5f;
	float dropletIncrease = 0.003f;
	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		timer -= Time.deltaTime;
		if (timer < 0){
			totalDropTime = Random.Range (15,35f);
			timer = totalDropTime;
//			// commented Debug.Log ("timer under zero, total drop time now:"+totalDropTime);
			currentDroplet = (GameObject)Instantiate(dropletPrefab,transform.position,transform.rotation);
//			// commented Debug.Log("made droplet");
			currentDroplet.transform.localScale = Vector3.one * 0.1f;
		} else {
			if (currentDroplet){
				
				currentDroplet.transform.localScale += Vector3.one * dropletIncrease;
//				// commented Debug.Log("growing current droplet.."+currentDroplet.transform.localScale);
				if (currentDroplet.transform.localScale.x > maxDropletSize){
//					// commented Debug.Log("droplet dropped.");
					Destroy (currentDroplet);
					currentDroplet = null;
//					timer = -1f;
					GameObject fallingDroplet = (GameObject)Instantiate(fallingDropletPrefab,transform.position + Vector3.up * -maxDropletSize * 3f,Quaternion.identity);
					CollisionEnterBroadcaster cob = fallingDroplet.AddComponent<CollisionEnterBroadcaster>();
					cob.onCollisionEnterDelegate += OnFallingDropletCollisionEnter;
				}
			}
		}
	}

	public void OnFallingDropletCollisionEnter(GameObject o,Collision hit){
		CollisionEnterBroadcaster cob = o.GetComponent<CollisionEnterBroadcaster>();
		cob.onCollisionEnterDelegate -= OnFallingDropletCollisionEnter; // needed?
		Quaternion rot = new Quaternion();
		rot.eulerAngles = new Vector3(-90,0,0);
		Instantiate (splashParticlesPrefab,o.transform.position,rot);
		Destroy(o);
	}
}
